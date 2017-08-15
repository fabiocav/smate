using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WebJobs.Script.LanguageService.Eventing;
using ConsoleTester.Echo.Properties;
using System.Diagnostics;
using WebJobs.Script.LanguageService;

namespace ConsoleTester.Echo
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                LanguageServiceEvent languageEvent = JsonConvert.DeserializeObject<LanguageServiceEvent>(input);

                LanguageServiceEvent response = null;
                switch (languageEvent.Command)
                {
                    case "/autocomplete":
                        JToken data = JToken.Parse(Resources.AutoCompleteResponse);
                        response = new LanguageServiceEvent(data, languageEvent.ClientId, LanguageServiceConstants.EventTypeResponse, languageEvent.Command);
                        response.Sequence = languageEvent.Sequence;
                        break;
                    case "/updatebuffer":
                        response = new LanguageServiceEvent(JObject.FromObject(new { result = true }), languageEvent.ClientId, LanguageServiceConstants.EventTypeResponse, languageEvent.Command);
                        response.Sequence = languageEvent.Sequence;
                        break;
                    default:
                        break;
                }

                if (response != null)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(response));
                }

                input = Console.ReadLine();
            }
        }
    }
}
