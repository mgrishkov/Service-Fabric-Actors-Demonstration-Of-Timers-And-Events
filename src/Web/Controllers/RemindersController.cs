using System;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using MyActor.Interfaces;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class RemindersController : Controller
    {
        private readonly StatelessServiceContext context;

        public RemindersController(StatelessServiceContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid? actorID, string message, int postponeReminderSeconds, int remindEverySeconds)
        {
            var actorId = actorID ?? Guid.NewGuid();
            var proxy = ActorProxy.Create<IMyActor>(new ActorId(actorId));

            await proxy.CreateWakeupCallAsync(message, TimeSpan.FromSeconds(postponeReminderSeconds), TimeSpan.FromSeconds(remindEverySeconds));

            await proxy.SubscribeAsync<IWakeupCallEvents>(new WakeupCallEventsHandler());

            ServiceEventSource.Current.ServiceMessage(context, $"Reminder {message} created for actor {actorId}");

            return Ok(actorId);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid actorId, string message)
        {
            var proxy = ActorProxy.Create<IMyActor>(new ActorId(actorId));

            await Task.CompletedTask;

            proxy.DismissWakeupCallAsync(message);

            ServiceEventSource.Current.ServiceMessage(context, $"Reminder {message} will be deleted for actor {actorId}");

            return Accepted();
        }
    }
}
