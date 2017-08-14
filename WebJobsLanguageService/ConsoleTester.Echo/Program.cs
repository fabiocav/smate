using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WebJobs.Script.LanguageService.Eventing;
using ConsoleTester.Echo.Properties;

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
                switch (languageEvent.Type)
                {
                    case "/autocomplete":
                        response = new LanguageServiceEvent(Resources.AutoCompleteResponse, languageEvent.ClientId, languageEvent.Type);
                        break;
                    case "/updatebuffer":
                        response = new LanguageServiceEvent("true", languageEvent.ClientId, languageEvent.Type);
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
