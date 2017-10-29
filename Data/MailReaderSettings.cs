using System;

namespace Outlook365.SpamAssassin.Data
{
    /// <summary>
    /// Class holds the settings for how to treat each message
    /// </summary>
    public class MailReaderSettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">Username to use to authenticate with, not EMAIL ADDRESS (USER@DOMAIN.XXX)</param>
        /// <param name="pass">Password for Account</param>
        /// <param name="spamFlag"></param>
        /// <param name="serviceApi">URI For exchange service</param>
        /// <param name="maxBodyLenght">Max Body length to check in KB</param>
        /// <param name="spamDetect"></param>
        /// 
        public MailReaderSettings(string user, string pass,double spamDetect,double spamFlag,Uri serviceApi,int maxBodyLenght,string workingFolder)
        {
            User = user;
            Pass = pass;
            SpamDetect = spamDetect;
            SpamFlag = spamFlag;
            ServiceApi = serviceApi;
            SpamAssassinWorkingFolder = workingFolder;
            MaxBodyLength = maxBodyLenght * 1024;
        }

        /// <summary>
        /// Max Body length in bytes
        /// </summary>
        public int MaxBodyLength { get; }
        public string SpamAssassinWorkingFolder { get; }

        /// <summary>
        /// ExchangeService URI
        /// </summary>
        public Uri ServiceApi { get; }

        /// <summary>
        /// Username to connect with
        /// </summary>
        public string User { get; }

        /// <summary>
        /// Password to connect with.
        /// </summary>
        public string Pass { get; }

        public double SpamDetect { get;  }

        public double SpamFlag { get; }
    }
}