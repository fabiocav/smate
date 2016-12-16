using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Web;

namespace WebJobs.Script.LanguageService.Eventing
{
    public class EventManager : IEventManager
    {
        private static readonly Subject<IEvent> _subject = new Subject<IEvent>();

        public IObservable<IEvent> Events => _subject.AsObservable();

        public void Publish(IEvent scriptEvent)
        {
            _subject.OnNext(scriptEvent);
        }
    }
}