using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class LanguageServiceEvent : IEvent
    {
        public LanguageServiceEvent(string data)
        {
            Data = data;
        }

        public string Name => nameof(LanguageServiceEvent);

        public string Data { get; }
    }
}