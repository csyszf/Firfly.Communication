using Bond;
using Firfly.Communication.Event.DependencyInjection;
using Firfly.Communication.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Firfly.Communication.Event.Test
{
    public class DITests
    {
        [Fact]
        public void EventBusDI()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddEventBus(op=> { });

            services.Replace(new ServiceDescriptor(typeof(IMessageAdapter), Mock.Of<IMessageAdapter>()));
            var sp = services.BuildServiceProvider();

            sp.GetRequiredService<IEventBus>();

        }

        [Fact]
        public void EventHandlerFactoryDI()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddEventReciever(o => { }, o => { }, o => { }, typeof(EventAHandler), typeof(EventBHandler));
            var sp = services.BuildServiceProvider();

            var factory = sp.GetRequiredService<EventHandlerFactory>();

            Assert.IsType<EventAHandler>(factory.GetHandler(EventA.TYPE));
            Assert.IsType<EventBHandler>(factory.GetHandler(EventB.TYPE));
        }

        [Fact]
        public void EventRecieverDI()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddEventReciever(o => { }, o => { }, o => { });
            var sp = services.BuildServiceProvider();

            sp.GetRequiredService<EventHandlerFactory>();

        }

        [Schema]
        internal class EventA : IEvent
        {
            public const string TYPE = "EventA";
            public const string SCOPE = "ScopeA";

            public string Type => TYPE;

            public string Scope => SCOPE;
        }

        [Schema]
        internal class EventB : IEvent
        {
            public const string TYPE = "EventB";
            public const string SCOPE = "ScopeB";

            public string Type => TYPE;

            public string Scope => SCOPE;
        }

        internal class EventAHandler: INamedEventHandler
        {
            public string EventType => EventA.TYPE;

            public Task Handle(byte[] payload, byte[] aid = null)
            {
                throw new NotImplementedException();
            }

            public Task Handle(ArraySegment<byte> payload, byte[] aid = null)
            {
                throw new NotImplementedException();
            }
        }

        internal class EventBHandler : INamedEventHandler
        {
            public string EventType => EventB.TYPE;

            public Task Handle(byte[] payload, byte[] aid = null)
            {
                throw new NotImplementedException();
            }

            public Task Handle(ArraySegment<byte> payload, byte[] aid = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
