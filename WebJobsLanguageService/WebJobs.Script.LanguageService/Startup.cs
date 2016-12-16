using System;
using System.Collections.Generic;
using System.Linq;
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
            builder.RegisterType<LanguageServiceConnection>();

            IContainer container = builder.Build();

            var connectionConfig = new ConnectionConfiguration();
            connectionConfig.Resolver = new AutofacDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.MapSignalR<LanguageServiceConnection>("/ls", connectionConfig);
        }
    }
}
