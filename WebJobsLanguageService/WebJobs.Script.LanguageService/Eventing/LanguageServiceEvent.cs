using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class LanguageServiceEvent : IEvent
    {
        public LanguageServiceEvent(string data, string clientId, string type)
        {
            Data = data;
            ClientId = clientId;
            Type = type;
        }

        public string Name => nameof(LanguageServiceEvent);

        public string ClientId { get; set; }

        public string Type { get; set; }

        public int EventId { get; set; }

        public string Data { get; }
    }
}