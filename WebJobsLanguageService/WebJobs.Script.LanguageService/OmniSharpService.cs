using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using WebJobs.Script.LanguageService.Eventing;

namespace WebJobs.Script.LanguageService
{
    public class OmniSharpService : IOmniSharpService
    {
        private ProcessManager _processManager;
        private readonly IObservable<LanguageServiceEvent> _eventStream;
        private readonly IEventManager _eventManager;
        private readonly IDisposable _outputSubscription;
        private readonly IDisposable _inputSubscription;

        public OmniSharpService(IEventManager eventManager)
        {
            _eventManager = eventManager;

            _processManager = new ProcessManager(@"D:\src\gh.fabiocav\smate\WebJobsLanguageService\ConsoleTester.Echo\bin\Debug\ConsoleTester.Echo.exe");

            _outputSubscription = _processManager.Output
                .Select(s => new LanguageServiceEvent(s, string.Empty, LanguageServiceConstants.EventTypeResponse))
                .Subscribe(OnOutput);

            _inputSubscription = _eventManager.Events.OfType<LanguageServiceEvent>()
                .Where(e => string.Equals(e.Type, LanguageServiceConstants.EventTypeRequest, StringComparison.OrdinalIgnoreCase))
                .Subscribe(async e => await SendEvent(e));
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
    }
}
