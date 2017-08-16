using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class LanguageServiceRequest : LanguageServiceEvent
    {
        public LanguageServiceRequest(JToken arguments, string clientId, string name) 
            : base(clientId, LanguageServiceConstants.EventTypeRequest, name)
        {
            Arguments = arguments;
        }

        public JToken Arguments { get; }
    }
}
