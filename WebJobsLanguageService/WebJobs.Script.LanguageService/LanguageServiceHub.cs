using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using WebJobs.Script.LanguageService.Eventing;

namespace WebJobs.Script.LanguageService
{
    public class LanguageServiceHub : Hub
    {
        private readonly IEventManager _eventManager;
        private readonly IDisposable _clientEvents;
        private string _connectionId;

        public LanguageServiceHub(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public async Task<string> LanguageServiceRequest(string name, string message)
        {
            var result = await Task.Run(async () =>
            {
                var inputEvent = new LanguageServiceEvent(message, string.Empty, LanguageServiceConstants.EventTypeRequest);

                // Create an awaiter watching for a response.
                var resultAwaiter = _eventManager.Events
                .OfType<LanguageServiceEvent>()
                .FirstAsync(e => e.EventId == inputEvent.EventId && e.Type == LanguageServiceConstants.EventTypeResponse)
                .Timeout(TimeSpan.FromSeconds(10), MissingResponse(inputEvent.ClientId, inputEvent.EventId));

                _eventManager.Publish(inputEvent);

                return await resultAwaiter;
            }).ConfigureAwait(false);

            return result.Data;
        }

        private IObservable<LanguageServiceEvent> MissingResponse(string clientId, int eventId) =>
            new[] { new LanguageServiceEvent("error", clientId, LanguageServiceConstants.EventTypeResponse) { EventId = eventId } }.ToObservable();

    }
}