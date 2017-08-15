using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class LanguageServiceEvent : IEvent
    {
        public LanguageServiceEvent(JToken data, string clientId, string type, string name)
        {
            Data = data;
            ClientId = clientId;
            Type = type;
            Name = name;
        }

        public string Name { get; set; }

        public string ClientId { get; set; }

        public string Type { get; set; }

        public int EventId { get; set; }

        public JToken Data { get; }
    }
}