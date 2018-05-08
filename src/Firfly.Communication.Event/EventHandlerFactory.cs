using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firfly.Communication.Event
{

    public class EventHandlerFactory
    {
        private readonly IEnumerable<INamedEventHandler> handlers;
        public EventHandlerFactory(IEnumerable<INamedEventHandler> handlers)
        {
            this.handlers = handlers;
        }

        public INamedEventHandler GetHandler(string eventType)
        {
            return handlers.First(h=>h.EventType == eventType);
        }
    }

}
