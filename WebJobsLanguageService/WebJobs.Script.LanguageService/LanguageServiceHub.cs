using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using WebJobs.Script.LanguageService.Eventing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WebJobs.Script.LanguageService
{
    public class LanguageServiceHub : Hub
    {
        private readonly IEventManager _eventManager;
        private readonly IDisposable _clientEvents;
        private string _connectionId;

        public LanguageServiceHub(IEventManager eventManager , IOmniSharpService service)
        {
            service.Start();
            _eventManager = eventManager;
        }

        public async Task<string> LanguageServiceRequest(string message)
        {
            var result = await Task.Run(async () =>
            {
                var inputEvent = JsonConvert.DeserializeObject<LanguageServiceRequest>(message);
                inputEvent.Arguments["fileName"] = Path.Combine(@"c:\func", inputEvent.Arguments["fileName"].Value<string>());
                // Create an awaiter watching for a response.
                var resultAwaiter = _eventManager.Events
                .OfType<LanguageServiceResponse>()
                .FirstAsync(e => e.RequestSequence == inputEvent.Sequence && e.Type == LanguageServiceConstants.EventTypeResponse)
                .Timeout(TimeSpan.FromSeconds(10), MissingResponse(inputEvent.ClientId, inputEvent.EventId));

                _eventManager.Publish(inputEvent);

                return await resultAwaiter;
            }).ConfigureAwait(false);

            string eventData = JsonConvert.SerializeObject(result);
            this.Clients.Caller.languageServiceEvent(eventData);
            return eventData;
        }

        private IObservable<LanguageServiceEvent> MissingResponse(string clientId, int eventId) =>
            new[] { new LanguageServiceResponse(clientId, "missingresposne") { EventId = eventId, Body = JObject.FromObject(new { error = "missing response" }) } }.ToObservable();

    }
}