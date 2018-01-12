using Khooversoft.Actor;
using Khooversoft.EventFlow;
using Khooversoft.Net;
using Khooversoft.Observers;
using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultApi.Application
{
    public static class ApiApplication
    {
        static ApiApplication()
        {
            EventSubject = new EventListenerSubject()
                .EnableEvents(ToolboxEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(ActorEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(NetEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(ObserversEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(SecurityEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(ServicesEventSource.Log, EventLevel.LogAlways);
        }

        public static EventListenerSubject EventSubject { get; private set; }

        public static EventDataBufferObserver EventDataBuffer { get; } = new EventDataBufferObserver();

    }
}
