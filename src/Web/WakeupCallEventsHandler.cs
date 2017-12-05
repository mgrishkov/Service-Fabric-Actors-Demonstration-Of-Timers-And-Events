using System;
using MyActor.Interfaces;

namespace Web
{
    public class WakeupCallEventsHandler : IWakeupCallEvents
    {
        public void WakeupCall(string message, Guid actorId)
        {
            ServiceEventSource.Current.Message($"{message} from {actorId}");
        }
    }
}