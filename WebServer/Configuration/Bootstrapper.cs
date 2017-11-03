using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Outlook365.SpamAssassin.WebServer.Configuration
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // Add a view location convention that looks for views in a folder
            //  named "views" next to the module class
            //
            Conventions.ViewLocationConventions.Add((viewName, model, context) => $"WebServer/Modules/{context.ModuleName}/Views/{viewName}");
        }
    }
}