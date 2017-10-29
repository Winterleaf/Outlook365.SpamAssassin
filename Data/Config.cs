using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using Outlook365.SpamAssassin.Email;

namespace Outlook365.SpamAssassin.Data
{


    public static class Config
    {

        public  static void CreateConfig()
        {
            const string filedata = @"#Outlook365 
Username                   : username@domain.com
Password                   : password
WebServiceApi              : https://outlook.office365.com/EWS/Exchange.asmx

#SpamAssassin
SpamAssassin_WorkingFolder : C:\SpamAssassin\

#Settings
MaxMessageBodyLengthK      : 4
SpamFlag                   : 2.0
SpamDetect                 : 4.0
";

            const string listDomanData = @"Somewhere.com;something.com;";
            const string listEmailData = @"someone@somewhere.com;me@joe.com;";

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Config.txt")))
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Config.txt"),filedata);

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Config.WhiteList.Domain.txt")))
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Config.WhiteList.Domain.txt"), listDomanData);

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Config.BlackList.Domain.txt")))
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Config.BlackList.Domain.txt"), listDomanData);


            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Config.BlackList.EmailAddress.txt")))
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Config.BlackList.EmailAddress.txt"), listEmailData);


            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Config.WhiteList.EmailAddress.txt")))
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Config.WhiteList.EmailAddress.txt"), listEmailData);

        }

        public static MailReaderSettings ReadConfig()
        {
            string username = null;
            string password = null;
            string spamDetect = null;
            string spamFlag = null;
            string webServiceApi = null;
            string spamAssassinWorkingFolder = null;
            string maxMessageBodyLength = null;
            string filedata;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Config.txt");
            try
            {
                filedata = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + path);
                throw;
            }
            foreach (string line in filedata.Split('\r'))
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;
                var lineParts = line.Split(':');

                if (lineParts.Length < 2)
                    continue;
                if (lineParts[0].Trim().Equals("Username", StringComparison.InvariantCultureIgnoreCase))
                    username = lineParts[1].Trim().Replace("\r", "").Replace("\n", "");

                if (lineParts[0].Trim().Equals("Password", StringComparison.InvariantCultureIgnoreCase))
                    password = lineParts[1].Trim().Replace("\r", "").Replace("\n", "");

                if (lineParts[0].Trim().Equals("SpamDetect", StringComparison.InvariantCultureIgnoreCase))
                    spamDetect = lineParts[1].Trim().Replace("\r", "").Replace("\n", "");

                if (lineParts[0].Trim().Equals("SpamFlag", StringComparison.InvariantCultureIgnoreCase))
                    spamFlag = lineParts[1].Trim().Replace("\r", "").Replace("\n", "");

                if (lineParts[0].Trim().Equals("WebServiceApi", StringComparison.InvariantCultureIgnoreCase))
                    webServiceApi = line.Substring(line.IndexOf(":")+1).Trim().Replace("\r", "").Replace("\n", "");
                
                if (lineParts[0].Trim().Equals("SpamAssassin_WorkingFolder", StringComparison.InvariantCultureIgnoreCase))
                    spamAssassinWorkingFolder = line.Substring(line.IndexOf(":") + 1).Trim().Replace("\r", "").Replace("\n", "");

                if (lineParts[0].Trim().Equals("MaxMessageBodyLengthK", StringComparison.InvariantCultureIgnoreCase))
                    maxMessageBodyLength = lineParts[1].Trim().Replace("\r", "").Replace("\n", "");
            }

            if (
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(spamDetect) ||
                string.IsNullOrEmpty(spamFlag) ||
                string.IsNullOrEmpty(webServiceApi) ||
                string.IsNullOrEmpty(spamAssassinWorkingFolder) ||
                string.IsNullOrEmpty(maxMessageBodyLength)
                )
                throw new Exception("Error reading Config.txt");

            if (username.Equals("NOTSET") || password.Equals("NOTSET"))
                throw new Exception("Account Username/Password wrong!");

            double sf, sd, ml;

            if (double.TryParse(spamDetect, out double chk))
                sd = chk;
            else
                throw new Exception("SpamFlag must be a number");

            if (double.TryParse(spamFlag, out chk))
                sf = chk;
            else
                throw new Exception("SpamFlag must be numeric.");

            if (double.TryParse(maxMessageBodyLength, out chk))
                ml = chk;
            else
                throw new Exception("Max length must be numeric.");

            Uri webAddr;
            try
            {
                webAddr = new Uri(webServiceApi);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message + " : Web Address URI is wrong format");
            }

            return new MailReaderSettings(username, password, sd, sf, webAddr, (int)(ml * 1024), spamAssassinWorkingFolder);



        }

        public enum ListType
        {
            WhiteListDomain,
            WhiteListEmailAddress,
            BlackListDomain,
            BlackListEmailAddress
        }


        public static List<string> ReadList(ListType listType)
        {
            string filedata;

            string path;
            switch (listType)
            {
                case ListType.WhiteListDomain:
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Config.WhiteList.Domain.txt");
                    break;
                case ListType.WhiteListEmailAddress:
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Config.WhiteList.EmailAddress.txt");
                    break;
                case ListType.BlackListDomain:
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Config.BlackList.Domain.txt");
                    break;
                case ListType.BlackListEmailAddress:
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Config.BlackList.EmailAddress.txt");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(listType), listType, null);
            }


            try
            {
                filedata = File.ReadAllText(path);
            }
            catch (Exception)
            {
                Console.WriteLine(path);
                throw;
            }

            return filedata.Split(';').Select(line => line.ToLower().Trim()).ToList();
        }

    }
}
