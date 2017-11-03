using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using Outlook365.SpamAssassin.Data;
using Outlook365.SpamAssassin.Email;
using Outlook365.SpamAssassin.WebServer.Modules.Root.ViewModel;

namespace Outlook365.SpamAssassin.WebServer.Modules.Root
{
    public class AuthenticatedUsers
    {
        public string SessionKey { get; set; }
        public string Value { get; set; }
    }

    public sealed class RootModule : NancyModule
    {
        public static List<AuthenticatedUsers> AuthenticatedUser = new List<AuthenticatedUsers>();

        public RootModule()
        {
            // Define a single route that returns our index.html view
            //
            //Get["/"] = _ => View["Login"];

            Get("/", args =>
            {
                ViewBag.Message = "Please Log in with your Office365 Account Credentials";
                return View["Index.html"];
            });

            Post("/Authenticate", args => ProcessLogin(this.Bind<LoginCredentials>()));

            Post("/EditBlackListDomain", args => EditBlackListDomain(this.Bind<AuthenticatedUsers>()));
            Post("/AddBlackListDomain", args => AddBlackListDomain(this.Bind<AuthenticatedUsers>()));
            Post("/RemoveBlackListDomain", args => RemoveBlackListDomain(this.Bind<AuthenticatedUsers>()));

            Post("/EditBlackListEmail", args => EditBlackListEmail(this.Bind<AuthenticatedUsers>()));
            Post("/AddBlackListEmail", args => AddBlackListEmail(this.Bind<AuthenticatedUsers>()));
            Post("/RemoveBlackListEmail", args => RemoveBlackListEmail(this.Bind<AuthenticatedUsers>()));

            Post("/EditWhiteListDomain", args => EditWhiteListDomain(this.Bind<AuthenticatedUsers>()));
            Post("/AddWhiteListDomain", args => AddWhiteListDomain(this.Bind<AuthenticatedUsers>()));
            Post("/RemoveWhiteListDomain", args => RemoveWhiteListDomain(this.Bind<AuthenticatedUsers>()));

            Post("/EditWhiteListEmail", args => EditWhiteListEmail(this.Bind<AuthenticatedUsers>()));
            Post("/AddWhiteListEmail", args => AddWhiteListEmail(this.Bind<AuthenticatedUsers>()));
            Post("/RemoveWhiteListEmail", args => RemoveWhiteListEmail(this.Bind<AuthenticatedUsers>()));

            Post("/Configuration", args => Configuration(this.Bind<AuthenticatedUsers>()));
        }

        public Negotiator AddBlackListDomain(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.RemovePostUrl = "RemoveBlackListDomain";
            ViewBag.ListType = "Domain Black List";
            ViewBag.AddPostUrl = "AddBlackListDomain";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.BlackListDomain).Distinct().OrderBy(x => x).ToList();
            options.Add(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.BlackListDomain);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator AddBlackListEmail(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Email Black List";
            ViewBag.AddPostUrl = "AddBlackListEmail";
            ViewBag.RemovePostUrl = "RemoveBlackListEmail";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.BlackListEmailAddress).Distinct().OrderBy(x => x).ToList();
            options.Add(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.BlackListDomain);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator AddWhiteListDomain(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Domain White List";
            ViewBag.AddPostUrl = "AddWhiteListDomain";
            ViewBag.RemovePostUrl = "RemoveWhiteListDomain";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.WhiteListDomain).Distinct().OrderBy(x => x).ToList();
            options.Add(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.BlackListDomain);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator AddWhiteListEmail(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Email White List";
            ViewBag.AddPostUrl = "AddWhiteListEmail";
            ViewBag.RemovePostUrl = "RemoveWhiteListEmail";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.WhiteListEmailAddress).Distinct().OrderBy(x => x).ToList();
            options.Add(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.BlackListDomain);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator Configuration(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];

            var c = Config.ReadConfig();
            ViewBag.Username = c.User;
            ViewBag.WebServiceApi = c.ServiceApi;
            ViewBag.SpamAssassin_WorkingFolder = c.SpamAssassinWorkingFolder;
            ViewBag.MaxMessageBodyLengthK = c.MaxBodyLength;
            ViewBag.SpamFlag = c.SpamFlag;
            ViewBag.SpamDetect = c.SpamDetect;
            ViewBag.WebServerHostName = c.WebServerHostName;
            ViewBag.WebServerPort = c.WebServerPort;
            ViewBag.WebServerProtocol = c.WebServerProtocol;
            ViewBag.SessionKey = sessionKey.SessionKey;

            return View["_Configuration"];
        }

        public Negotiator EditBlackListDomain(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Domain Black List";
            ViewBag.Options = GenerateSelectOptions(Config.ReadList(Config.ListType.BlackListDomain));
            ViewBag.AddPostUrl = "AddBlackListDomain";
            ViewBag.RemovePostUrl = "RemoveBlackListDomain";
            ViewBag.SessionKey = sessionKey.SessionKey;
            return View["_ListEdit"];
        }

        public Negotiator EditBlackListEmail(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Email Black List";
            ViewBag.AddPostUrl = "AddBlackListEmail";
            ViewBag.RemovePostUrl = "RemoveBlackListEmail";
            ViewBag.SessionKey = sessionKey.SessionKey;
            ViewBag.Options = GenerateSelectOptions(Config.ReadList(Config.ListType.BlackListEmailAddress));
            return View["_ListEdit"];
        }

        public Negotiator EditWhiteListDomain(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Domain White List";
            ViewBag.AddPostUrl = "AddWhiteListDomain";
            ViewBag.RemovePostUrl = "RemoveWhiteListDomain";
            ViewBag.SessionKey = sessionKey.SessionKey;
            ViewBag.Options = GenerateSelectOptions(Config.ReadList(Config.ListType.WhiteListDomain));
            return View["_ListEdit"];
        }

        public Negotiator EditWhiteListEmail(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Email White List";
            ViewBag.AddPostUrl = "AddWhiteListEmail";
            ViewBag.RemovePostUrl = "RemoveWhiteListEmail";
            ViewBag.SessionKey = sessionKey.SessionKey;
            ViewBag.Options = GenerateSelectOptions(Config.ReadList(Config.ListType.WhiteListEmailAddress));
            return View["_ListEdit"];
        }

        private static string GenerateSelectOptions(IEnumerable<string> options)
        {
            return options.Aggregate("", (current, s) => current + $"<option value='{s}'>{s}</option>\r\n");
        }

        public bool IsAuthorized(AuthenticatedUsers auth)
        {
            return AuthenticatedUser.Any(x => x.SessionKey == auth.SessionKey);
        }

        public Negotiator ProcessLogin(LoginCredentials creds)
        {
            if (creds.EmailAddress != Config.ReadConfig().User)
            {
                ViewBag.Message =
                    "<span style='color: gold;font-size:16px;'><span class='glyphicon glyphicon-warning-sign'></span>Username does not match Config.txt</span>";
                return View["_Login"];
            }
            if (EmailReader.AuthenticateEmailAccount(creds.EmailAddress, creds.Password))
            {
                ViewBag.SessionKey = Guid.NewGuid().ToString();
                AuthenticatedUser.Add(new AuthenticatedUsers {SessionKey = ViewBag.SessionKey});
                return View["_MainMenu"];
            }
            ViewBag.Message = "<span style='color: gold;font-size:16px;'><span class='glyphicon glyphicon-warning-sign'></span>Bad Username or Password</span>";
            return View["_Login"];
        }

        public Negotiator RemoveBlackListDomain(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Domain Black List";
            ViewBag.AddPostUrl = "AddBlackListDomain";
            ViewBag.RemovePostUrl = "RemoveBlackListDomain";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.BlackListDomain).Distinct().OrderBy(x => x).ToList();
            options.Remove(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.BlackListDomain);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator RemoveBlackListEmail(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Email Black List";
            ViewBag.AddPostUrl = "AddBlackListEmail";
            ViewBag.RemovePostUrl = "RemoveBlackListEmail";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.BlackListEmailAddress).Distinct().OrderBy(x => x).ToList();
            options.Remove(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.BlackListEmailAddress);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator RemoveWhiteListDomain(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.RemovePostUrl = "RemoveWhiteListDomain";
            ViewBag.ListType = "Domain White List";
            ViewBag.AddPostUrl = "AddWhiteListDomain";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.WhiteListDomain).Distinct().OrderBy(x => x).ToList();
            options.Remove(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.WhiteListDomain);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }

        public Negotiator RemoveWhiteListEmail(AuthenticatedUsers sessionKey)
        {
            if (!IsAuthorized(sessionKey))
                return View["_NoAuth"];
            ViewBag.ListType = "Email White List";
            ViewBag.AddPostUrl = "AddWhiteListEmail";
            ViewBag.RemovePostUrl = "RemoveWhiteListEmail";
            ViewBag.SessionKey = sessionKey.SessionKey;
            var options = Config.ReadList(Config.ListType.WhiteListEmailAddress).Distinct().OrderBy(x => x).ToList();
            options.Remove(sessionKey.Value);
            options = options.Where(x => x.Trim() != "").OrderBy(x => x).ToList();
            Config.WriteList(options, Config.ListType.WhiteListEmailAddress);
            ViewBag.Options = GenerateSelectOptions(options);
            return View["_ListEdit"];
        }
    }
}