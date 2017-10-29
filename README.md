# Outlook365.Spamassassin

<h2>Description</h2>

Outlook365.Spamassassin is a windows service you can run on any windows 10 computer and it will monitor your email and remove spam based on the spamassassin threshold you set.  The computer running this service <b>DOES NOT</b> need outlook installed.

<h2>License</h2>
Copyright 2017 Fairfield Tek, L.L.C.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


<h2>History</h2>

<b>October 28,2017</b> 

Due to the amount of spam I get on my work Outlook365 email account I decided to see if I could use <b>"Microsoft.Exchange.Webservices"</b> and <a href="https://www.jam-software.com/spamassassin/">SpamAssassin for Windows</a> to provide a bit more spam blocking.  After a couple days of work I ended up with this program.  This program is a windows service that you can install on any computer to monitor your email and use SpamAssassin to filter out junk email.  I've been testing it for a few days and it has been working great.  Any Feedback or bugs are welcome.

<h2>Requirements</h2>

1. Visual Studio 2017
2. Dot.Net 4.7.1 (It should work on 4.6.1 and higher)
3. <a href="https://www.jam-software.com/spamassassin/">SpamAssassin for Windows</a>
4. Outlook365 Mail Account

<h2>What does it do?</h2>

This program will create 2 new folders in your mail account:
1. <b>"IInbox"</b> your new filtered Inbox
2. <b>"SpamAssassin"</b> a working folder used during the process

Outlook365.Spamassassin will monitor your inbox, when a new message is recieved it will use spam assassin to determine if it is spam.  If it is determined to be junk or blacklisted, it will be moved to your junk folder, if not it will be moved to the "IInbox" folder.
It will run as a windows service.

<b>Note:</b> You can edit the Black List and White List files at any point and it will used the new file without having to restart the application.

<H2>Setup</h2>

1. Install SpamAssassin For Windows
2. Compile Project
3. Copy the compiled program folder to a different directory

4. Edit the <b>Config.Template.Txt</b> File, change the following fields
5. <b>Username</b> -> Your outlook365 Username
6. <b>Password</b> -> Your outlook365 Password
7. <b>SpamAssassin_WorkingFolder</b> -> Where you installed "SpamAssassin for Windows"
8. <b>MaxMessageBodyLengthK</b> -> the max message size to be run through SpamAssign in Kilobytes
9. <b>SpamFlag</b> -> Any score lower than this will cause the program to alter the message subject with a warning.
10. <b>SpamDetect</b> -> Any score lower than this will cause the program to move the message to the junk folder.
11. Rename <b>Config.Template.Txt</b> to <b>Config.txt</b>

12. Edit the <b>Config.BlackList.Domain.Template.txt</b> file
13. Add any domains you want to blacklist ending with a semi-colon (i.e. somewhere.com;joestraders.com;
14.  Rename <b>Config.BlackList.Domain.Template.txt</b> to <b>Config.BlackList.Domain.txt</b>

15. Edit the <b>Config.WhiteList.Domain.Template.txt</b> file
16. Add any domains you want to Whitelist ending with a semi-colon (i.e. somewhere.com;joestraders.com;
17.  Rename <b>Config.WhiteList.Domain.Template.txt</b> to <b>Config.WhiteList.Domain.txt</b>
  
18. Edit the <b>Config.BlackList.EmailAddress.Template.txt</b> file
19. Add any email address you want to Blacklist ending with a semi-colon (i.e. joe@somewhere.com;joe@joestraders.com;
20.  Rename <b>Config.BlackList.EmailAddress.Template.txt</b> to <b>Config.BlackList.EmailAddress.txt</b>

21. Edit the <b>Config.WhiteList.EmailAddress.Template.txt</b> file
22. Add any email address you want to Whitelist ending with a semi-colon (i.e. joe@somewhere.com;joe@joestraders.com;
23.  Rename <b>Config.WhiteList.EmailAddress.Template.txt</b> to <b>Config.WhiteList.EmailAddress.txt</b>

24. From the command prompt, go to the directory you copied the project output to.
25. Type in <b>"Outlook365.SpamAssassin.exe"</b>
26. A console window should display, if everything is configured correctly, it will start showing message header information
27. Hit <b>"Q" + Enter</b> to exit console application.

28. To install it as a system service type in <b>"Outlook365.SpamAssassin.exe -install"</b> into the console
29. Open your services and set it to autostart
30. Start the service.

Note: To uninstall service type <b>Outlook365.SpamAssassin.exe -uninstall</b>


