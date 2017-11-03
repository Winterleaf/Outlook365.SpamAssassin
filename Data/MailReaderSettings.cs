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

namespace Outlook365.SpamAssassin.Data
{
    /// <summary>
    ///     Class holds the settings for how to treat each message
    /// </summary>
    public class MailReaderSettings
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="user">Username to use to authenticate with, not EMAIL ADDRESS (USER@DOMAIN.XXX)</param>
        /// <param name="pass">Password for Account</param>
        /// <param name="spamFlag"></param>
        /// <param name="serviceApi">URI For exchange service</param>
        /// <param name="maxBodyLenght">Max Body length to check in KB</param>
        /// <param name="spamDetect"></param>
        /// <param name="webServerHostName"></param>
        /// <param name="webServerPort"></param>
        /// <param name="webServerProtocol"></param>
        public MailReaderSettings(string user, string pass, double spamDetect, double spamFlag, Uri serviceApi, int maxBodyLenght, string workingFolder,
            string webServerHostName, int webServerPort, string webServerProtocol)
        {
            User = user;
            Pass = pass;
            SpamDetect = spamDetect;
            SpamFlag = spamFlag;
            ServiceApi = serviceApi;
            SpamAssassinWorkingFolder = workingFolder;
            MaxBodyLength = maxBodyLenght * 1024;
            WebServerHostName = webServerHostName;
            WebServerPort = webServerPort;
            WebServerProtocol = webServerProtocol;
        }

        /// <summary>
        ///     Max Body length in bytes
        /// </summary>
        public int MaxBodyLength { get; }

        /// <summary>
        ///     Password to connect with.
        /// </summary>
        public string Pass { get; }

        /// <summary>
        ///     ExchangeService URI
        /// </summary>
        public Uri ServiceApi { get; }

        public string SpamAssassinWorkingFolder { get; }

        public double SpamDetect { get; }

        public double SpamFlag { get; }

        /// <summary>
        ///     Username to connect with
        /// </summary>
        public string User { get; }

        public string WebServerHostName { get; }
        public int WebServerPort { get; }
        public string WebServerProtocol { get; }
    }
}