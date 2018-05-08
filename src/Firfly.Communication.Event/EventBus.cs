using Firfly.Communication.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
namespace Firfly.Communication.Event
{
    public class EventBus : IEventBus
    {
        private readonly ILogger logger;
        private readonly IMessageAdapter adapter;
        public EventBus(
            ILogger<EventBus> logger,
            IMessageAdapter adapter
            )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public async Task PublishBondEvent<TEvent>(TEvent @event) where TEvent : BondEvent<TEvent>, IEvent
        {
            var scope = @event.Scope;
            var type = @event.Type;
            var payload = BondEvent<TEvent>.Serialize(@event);
            var aid = @event is IDomainEvent devent ? devent.AggregateId : null;
            var msg = new EventMessage { EventType = type, Payload = payload };
            await adapter.ProduceAsync(scope, aid, msg.Serialize());

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"produced event message: [{scope}]{type}");
            }
        }

    }
}
