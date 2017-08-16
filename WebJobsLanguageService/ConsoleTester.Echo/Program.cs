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
                        response = new LanguageServiceResponse(languageEvent.ClientId, languageEvent.Command) { Body = data };
                        response.Sequence = languageEvent.Sequence;
                        break;
                    case "/updatebuffer":
                        response = new LanguageServiceResponse(languageEvent.ClientId, languageEvent.Command) { Body = JObject.FromObject(new { result = true }) };
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
