﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebJobs.Script.LanguageService.Eventing;

namespace WebJobs.Script.LanguageService
{
    public class OmniSharpService : IOmniSharpService, IDisposable
    {
        private ProcessManager _processManager;
        private readonly IObservable<LanguageServiceEvent> _eventStream;
        private readonly IEventManager _eventManager;
        private readonly IDisposable _outputSubscription;
        private readonly IDisposable _inputSubscription;
        private bool _disposed = false;

        public OmniSharpService(IEventManager eventManager)
        {
            _eventManager = eventManager;

            _processManager = new ProcessManager(@"D:\src\gh.fabiocav\smate\WebJobsLanguageService\ConsoleTester.Echo\bin\Debug\ConsoleTester.Echo.exe");

            _outputSubscription = _processManager.Output
                .Select(s => ProcessEvent(s))
                .Subscribe(OnOutput);

            _inputSubscription = _eventManager.Events.OfType<LanguageServiceEvent>()
                .Where(e => string.Equals(e.Type, LanguageServiceConstants.EventTypeRequest, StringComparison.OrdinalIgnoreCase))
                .Subscribe(async e => await SendEvent(e));
        }

        private LanguageServiceEvent ProcessEvent(string s)
        {
            var jsonObject = JObject.Parse(s);
            return new LanguageServiceEvent(s, string.Empty, LanguageServiceConstants.EventTypeResponse)
            {
                EventId = jsonObject.Value<int>("Request_seq")
            };
        }

        private void OnOutput(LanguageServiceEvent outputEvent)
        {
            _eventManager.Publish(outputEvent);
        }

        public async Task SendEvent(LanguageServiceEvent message)
        {
            await _processManager.Write(message.Data.ToString());
        }

        public void Start()
        {
            _processManager.Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _processManager?.Dispose();
                    _inputSubscription?.Dispose();
                    _outputSubscription?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
