using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class LanguageServiceEvent : IEvent
    {
        public LanguageServiceEvent(string clientId, string type, string name)
        {
            ClientId = clientId;
            Type = type;
            Command = name;
        }

        public string Command { get; set; }

        public string ClientId { get; set; }

        public string Type { get; set; }

        public int EventId { get; set; }

        [JsonProperty("Seq")]
        public int Sequence { get; set; }
    }
}