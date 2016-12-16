using System;

namespace WebJobs.Script.LanguageService.Eventing
{
    public interface IEventManager
    {
        IObservable<IEvent> Events { get; }

        void Publish(IEvent scriptEvent);
    }
}