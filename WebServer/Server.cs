using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Outlook365.SpamAssassin.Data;
using Owin;

namespace Outlook365.SpamAssassin.WebServer
{
    public class Server
    {
        public void Run()
        {
            //  WebApp.Start()

            // s = new OwinService();

            //    s.ConstructUsing(() => new OwinService());
            //    s.WhenStarted(service => service.Start());
            //    s.WhenStopped(service => service.Stop());

            //HostFactory.Run(x =>
            //{
            //    x.Service<OwinService>(s =>
            //    {
            //        s.ConstructUsing(() => new OwinService());
            //        s.WhenStarted(service => service.Start());
            //        s.WhenStopped(service => service.Stop());
            //    });
            //});
        }

        public class OwinService
        {
            private IDisposable _webApp;

            public OwinService()
            {
                Console.WriteLine("ServiceHost constructed");
            }

            public void ShutDown()
            {
                Console.WriteLine("ServiceHost shutting down");
            }

            public void Start()
            {
                Console.WriteLine("ServiceHost started");
                var config = Config.ReadConfig();
                _webApp = WebApp.Start<StartOwin>($"{config.WebServerProtocol}://{config.WebServerHostName}:{config.WebServerPort}");
            }

            public void Stop()
            {
                Console.WriteLine("Server shutting down");

                _webApp.Dispose();

                Console.WriteLine("ServiceHost stopped");
            }
        }

        public class StartOwin
        {
            public void Configuration(IAppBuilder appBuilder)
            {
                appBuilder.Map("/api", api =>
                {
                    var config = new HttpConfiguration();
                    config.MapHttpAttributeRoutes();
                    api.UseWebApi(config);
                });
                appBuilder.UseNancy();
            }
        }
    }
}