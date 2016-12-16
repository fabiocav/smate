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
    public class LanguageServiceConnection : PersistentConnection
    {
        private readonly IEventManager _eventManager;

        public LanguageServiceConnection(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return Connection.Send(connectionId, "connected");
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            _eventManager.Publish(new LanguageServiceEvent(data));

            return Task.CompletedTask;
        }
    }
}