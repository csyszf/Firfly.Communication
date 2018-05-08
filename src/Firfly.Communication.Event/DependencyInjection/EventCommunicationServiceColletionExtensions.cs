using Firfly.Communication.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Serializer = Bond.Serializer<Bond.Protocols.SimpleBinaryWriter<Bond.IO.Unsafe.OutputBuffer>>;

namespace Firfly.Communication.Event.DependencyInjection
{
    public static class EventCommunicationServiceColletionExtensions
    {

        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<KafkaOptions> options)
        {
            if (options != null)
                services.Configure<KafkaOptions>(options);

            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IMessageAdapter, MessageAdapter>();

            return services;
        }

        public static IServiceCollection AddEventReciever(this IServiceCollection services,
            Action<KafkaOptions> kafkaConfigure,
            Action<KafkaConsumerOptions> consumerConfigure,
            Action<EventRecieverOptions> eventConfigure,
            params Type[] handlers )
        {

            if (kafkaConfigure != null) services.Configure(kafkaConfigure);
            if (consumerConfigure != null) services.Configure(consumerConfigure);
            if (eventConfigure != null) services.Configure(eventConfigure);

            services.AddSingleton<EventReciever>();
            services.AddSingleton<EventHandlerFactory>();

            foreach (var handlerType in handlers)
            {
                services.AddSingleton(typeof(INamedEventHandler), handlerType);
            }
            return services;
        }

    }
}
