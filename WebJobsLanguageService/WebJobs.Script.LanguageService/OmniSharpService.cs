using System;
using System.IO;
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
        private bool _started;

        public OmniSharpService(IEventManager eventManager)
        {
            _eventManager = eventManager;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                "..\\..\\", @"WebJobsLanguageService\ConsoleTester.Echo\bin\Debug\ConsoleTester.Echo.exe");

            _processManager = new ProcessManager(path);

            _outputSubscription = _processManager.Output
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => ProcessEvent(s))
                .Subscribe(OnOutput);

            _inputSubscription = _eventManager.Events.OfType<LanguageServiceEvent>()
                .Where(e => string.Equals(e.Type, LanguageServiceConstants.EventTypeRequest, StringComparison.OrdinalIgnoreCase))
                .Subscribe(async e => await SendEvent(e));
        }

        private LanguageServiceEvent ProcessEvent(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<LanguageServiceEvent>(s);
            }
            catch (JsonSerializationException exc)
            {
                return new LanguageServiceEvent(JObject.FromObject(exc), "2", LanguageServiceConstants.EventTypeResponse, "error") { EventId = 3 };            
            }
        }

        private void OnOutput(LanguageServiceEvent outputEvent)
        {
            _eventManager.Publish(outputEvent);
        }

        public async Task SendEvent(LanguageServiceEvent message)
        {
            await _processManager.Write(JsonConvert.SerializeObject(message));
        }

        public void Start()
        {
            if (!_started)
            {
                _started = true;
                _processManager.Start();
            }
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
