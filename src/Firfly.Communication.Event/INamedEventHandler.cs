using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Firfly.Communication.Event
{
    public interface INamedEventHandler
    {
        string EventType { get; }
        Task Handle(ArraySegment<byte> payload, byte[] aid = null);
    }

    //public interface ITypedEventHandler<TEvent> where TEvent: IEvent
    //{
    //    Task Handle(ArraySegment<byte> payload, byte[] aid = null);
    //}
}
