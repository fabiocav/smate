using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using WebJobs.Script.LanguageService.Eventing;

[assembly: OwinStartup(typeof(WebJobs.Script.LanguageService.Startup))]

namespace WebJobs.Script.LanguageService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
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

            HostingEnvironment.QueueBackgroundWorkItem((ct) =>
            {
                container.Resolve<IOmniSharpService>().Start();
                ct.WaitHandle.WaitOne();
            });
        }
    }
}
