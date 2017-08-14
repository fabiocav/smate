using System.Web.Hosting;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using WebJobs.Script.LanguageService.Eventing;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(WebJobs.Script.LanguageService.Startup))]

namespace WebJobs.Script.LanguageService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            var builder = new ContainerBuilder();

            builder.RegisterType<EventManager>()
                .As<IEventManager>()
                .SingleInstance();
            builder.RegisterType<OmniSharpService>().As<IOmniSharpService>();
            builder.RegisterType<LanguageServiceHub>();

            IContainer container = builder.Build();

            var connectionConfig = new HubConfiguration();
            connectionConfig.Resolver = new AutofacDependencyResolver(container);
            connectionConfig.EnableJavaScriptProxies = true;

            app.UseAutofacMiddleware(container);
            app.MapSignalR("/ls", connectionConfig);

            container.Resolve<IOmniSharpService>().Start();

            HostingEnvironment.QueueBackgroundWorkItem((ct) =>
            {
                ct.WaitHandle.WaitOne();
            });
        }
    }
}
