using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Firfly.Communication.Event
{
    public interface IEventBus
    {
        Task PublishBondEvent<TEvent>(TEvent @event) where TEvent : BondEvent<TEvent>, IEvent;

    }
}
