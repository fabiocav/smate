using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebJobs.Script.LanguageService.Startup))]

namespace WebJobs.Script.LanguageService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR<LanguageServiceConnection>("/ls");
        }
    }
}
