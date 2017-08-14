using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJobs.Script.LanguageService.Eventing
{
    public interface IEvent
    {
        string Name { get; }
    }
}