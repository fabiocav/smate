using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class LanguageServiceResponse : LanguageServiceEvent
    {
        public LanguageServiceResponse(string clientId, string name)
            : base(clientId, LanguageServiceConstants.EventTypeResponse, name)
        {
        }

        public JToken Body { get; set; }

        public bool Success { get; set; }

        public bool Running { get; set; }

        public string Message { get; set; }

        [JsonProperty("Request_seq")]
        public int RequestSequence { get; set; }
    }
}
