/* Copyright 2017 Fairfield Tek, L.L.C.
* Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
* to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,
* sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
* OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
* OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.WebServices.Data;
using Outlook365.SpamAssassin.Data;
using Outlook365.SpamAssassin.Spamassassin;

namespace Outlook365.SpamAssassin.Email
{
    internal static class EmailReader
    {
        private static readonly object Locker = new object();

        public static bool AuthenticateEmailAccount(string username, string password)
        {
            try
            {
                var service = new ExchangeService
                {
                    Credentials = new WebCredentials(username, password),
                    Url = Config.ReadConfig().ServiceApi
                };
                var myInbox = Folder.Bind(service, WellKnownFolderName.Inbox);
                var view = new ItemView(1, 0, OffsetBasePoint.Beginning)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived)
                };
                var findResults = myInbox.FindItems(view);
                var item = findResults.FirstOrDefault();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Process One email from the inbox
        /// </summary>
        /// <param name="oep">MailReaderSettings settings</param>
        public static void ProcessEmail(object oep)
        {
            try
            {
                var ep = (MailReaderSettings)oep;

                //Connect to the Outlook365 Web Service
                var service = new ExchangeService { Credentials = new WebCredentials(ep.User, ep.Pass), Url = ep.ServiceApi };

                //Create our default ItemView
                var view = new ItemView(1, 0, OffsetBasePoint.Beginning)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived)
                };

                //Set Order
                view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Ascending);

                //Map Inbox
                var myInbox = Folder.Bind(service, WellKnownFolderName.Inbox);

                //Set FolderView
                var folderView = new FolderView(1)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly) { FolderSchema.DisplayName },
                    Traversal = FolderTraversal.Deep
                };

                //Folder used for holding while we evaluate the message
                //If it doesn't exist, create it.
                SearchFilter spamassassinFolderSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "SpamAssassin");
                var spamassassinFolder = service.FindFolders(WellKnownFolderName.MsgFolderRoot, spamassassinFolderSearchFilter, folderView).FirstOrDefault();
                var spamassassinFolderId = spamassassinFolder?.Id;
                if (spamassassinFolderId == null)
                {
                    spamassassinFolder = new Folder(service) { DisplayName = "SpamAssassin" };
                    spamassassinFolder.Save(WellKnownFolderName.MsgFolderRoot);
                    spamassassinFolderId = spamassassinFolder.Id;
                }

                //Folder where we move junk email to
                //If it doesn't exist, create it.
                SearchFilter junkFolderSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Junk Email");
                var junkFolder = service.FindFolders(WellKnownFolderName.MsgFolderRoot, junkFolderSearchFilter, folderView).FirstOrDefault();
                var junkFolderId = junkFolder?.Id;
                if (junkFolderId == null)
                {
                    junkFolder = new Folder(service) { DisplayName = "Junk Email" };
                    junkFolder.Save(WellKnownFolderName.MsgFolderRoot);
                    junkFolderId = junkFolder.Id;
                }

                //Folder used for the new Inbox,
                //If it doesn't exist create it.
                // ReSharper disable once InconsistentNaming
                SearchFilter IInboxFolderSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "IInbox");
                // ReSharper disable once InconsistentNaming
                var IInboxFolder = service.FindFolders(WellKnownFolderName.MsgFolderRoot, IInboxFolderSearchFilter, folderView).FirstOrDefault();
                // ReSharper disable once InconsistentNaming
                var IInboxFolderId = IInboxFolder?.Id;
                if (IInboxFolderId == null)
                {
                    IInboxFolder = new Folder(service) { DisplayName = "IInbox" };
                    IInboxFolder.Save(WellKnownFolderName.MsgFolderRoot);
                    IInboxFolderId = IInboxFolder.Id;
                }

                Item item;

                //Lock the mutex, we only want one thread at a time reading an email from the inbox.
                lock (Locker)
                {
                    //Search
                    var findResults = myInbox.FindItems(view);
                    //Get First
                    item = findResults.FirstOrDefault();
                    //Check for Null
                    if (item == null)
                        return;
                    //Move to Processing
                    item = item.Move(spamassassinFolderId);
#if DEBUG
                    Console.WriteLine("Moving Item To SpamAssassin Folder");
#endif
                }

                //Define properties we want to fetch when contacting the exchange service
                var props = new PropertySet(ItemSchema.MimeContent, ItemSchema.Subject, EmailMessageSchema.From, EmailMessageSchema.Sender, ItemSchema.Body);

                //Get the message and bind it to an emailmessage
                var em = EmailMessage.Bind(service, item.Id, props);

                //Load the properties we want
                em.Load(props);

                //Get the mimeContent for the message, basically, headers, etc.
                var mimeContent = em.MimeContent;

                //Set it as not read.
                em.IsRead = false;

                string msg;
                //Copy to MimeContent to a string.
                using (var fs = new MemoryStream())
                {
                    fs.Write(mimeContent.Content, 0, mimeContent.Content.Length);
                    fs.Position = 0;
                    var sr = new StreamReader(fs);
                    msg = sr.ReadToEnd();
                }

                //Get the domain to the sender
                string domain = em.From.Address.Substring(em.From.Address.IndexOf('@') + 1).ToLower();

                bool doProcess = false;
                if (!Config.ReadList(Config.ListType.WhiteListEmailAddress).Any(x => x.Equals(em.From.Address, StringComparison.InvariantCultureIgnoreCase)))
                    if (!Config.ReadList(Config.ListType.WhiteListDomain).Any(x => x.ToLower().EndsWith(domain)))
                        if (!em.From.Address.ToLower().StartsWith("/o"))
                        {
                            if (Config.ReadList(Config.ListType.BlackListDomain).Any(x => x.EndsWith(domain)))
                            {
#if DEBUG
                                Console.WriteLine($"Moving To Junk: BlackList Domain");
#endif
                                em.Subject = "{BlackList Domain} " + em.Subject;
                                em.Update(ConflictResolutionMode.AlwaysOverwrite);
                                item.Move(junkFolderId);
                                return;
                            }
                            if (Config.ReadList(Config.ListType.BlackListEmailAddress).Contains(em.From.Address))
                            {
#if DEBUG
                                Console.WriteLine($"Moving To Junk: BlackList Email Address");
#endif
                                em.Subject = "{BlackList Email Address} " + em.Subject;
                                em.Update(ConflictResolutionMode.AlwaysOverwrite);
                                item.Move(junkFolderId);
                                return;
                            }
                            if (em.Body.Text != null)
                            {
                                //If the lengh < max length checked
                                if (em.Body.Text.Length < ep.MaxBodyLength)
                                {
                                    doProcess = true;
                                }
                                else
                                {
#if DEBUG
                                    Console.WriteLine("Skipping Check Size to Big.");
#endif
                                }
                            }
                        }
                
                //We need to check this email
                if (doProcess)
                {
                    //Get the Spam Report
                    var spamReport = SimpleSpamAssassin.GetReport("127.0.0.1", msg);
#if DEBUG
                    Console.WriteLine($"Checkedg Message, Subject: '{em.Subject}' Score: {spamReport.Score} out of {ep.SpamFlag}");
#endif
                    //Is the score reported > threshold?
                    if (spamReport.Score >= ep.SpamFlag)
                    {
                        //Adjust the subject line
                        em.Subject = $"{{Spam}} {{{spamReport.Score} / {ep.SpamFlag}}} " + em.Subject;
                        //Update the message
                        em.Update(ConflictResolutionMode.AlwaysOverwrite);
                        //Move to Junk
                        item.Move(junkFolderId);
#if DEBUG
                        Console.WriteLine($"Moving To Junk: SPAM {spamReport.Score} out of {spamReport.SpamScore}");
#endif
                    }
                    else if (spamReport.Score >= ep.SpamDetect)
                    {
                        //Adjust the subject line
                        em.Subject = $"{{Possible Spam}} {{{spamReport.Score} / {ep.SpamDetect}}} " + em.Subject;
                        //Update the message
                        em.Update(ConflictResolutionMode.AlwaysOverwrite);
                        //Move to Junk
                        item.Move(IInboxFolderId);
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine("Moving To IInbox");
#endif
                        //Move it to our IInbox
                        item.Move(IInboxFolderId);
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("Moving To IInbox");
#endif
                    //Move it to our IInbox
                    item.Move(IInboxFolderId);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                //Just Dump Out, we just want to try it again if it fails and not crash the service.
            }
        }
    }
}