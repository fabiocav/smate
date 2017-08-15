﻿using System;
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
                var inputEvent = JsonConvert.DeserializeObject<LanguageServiceEvent>(message);
                inputEvent.Type = LanguageServiceConstants.EventTypeRequest;

                // Create an awaiter watching for a response.
                var resultAwaiter = _eventManager.Events
                .OfType<LanguageServiceEvent>()
                .FirstAsync(e => e.EventId == inputEvent.EventId && e.Type == LanguageServiceConstants.EventTypeResponse)
                .Timeout(TimeSpan.FromSeconds(10), MissingResponse(inputEvent.ClientId, inputEvent.EventId));

                _eventManager.Publish(inputEvent);

                return await resultAwaiter;
            }).ConfigureAwait(false);

            return JsonConvert.SerializeObject(result);
        }

        private IObservable<LanguageServiceEvent> MissingResponse(string clientId, int eventId) =>
            new[] { new LanguageServiceEvent(JObject.FromObject(new { error = "missing response"}), clientId, LanguageServiceConstants.EventTypeResponse, "missingresposne") { EventId = eventId } }.ToObservable();

    }
}