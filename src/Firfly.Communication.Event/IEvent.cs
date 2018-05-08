using Bond.IO.Unsafe;
using Bond.Protocols;
using System;
using Deserializer = Bond.Deserializer<Bond.Protocols.SimpleBinaryReader<Bond.IO.Unsafe.InputBuffer>>;
using Serializer = Bond.Serializer<Bond.Protocols.SimpleBinaryWriter<Bond.IO.Unsafe.OutputBuffer>>;

namespace Firfly.Communication.Event
{
    public interface IEvent
    {
        string Type { get; }
        string Scope { get; }
    }

    public interface IDomainEvent : IEvent
    {
        byte[] AggregateId { get; }
    }

    public abstract class BondEvent<T> where T: IEvent
    {
        public static readonly Deserializer Deserializer = new Deserializer(typeof(T));
        public static readonly Serializer serializer = new Serializer(typeof(T));

        public static T Deserialize(byte[] data)
        {
            var input = new InputBuffer(data);
            var bondReader = new SimpleBinaryReader<InputBuffer>(input);
            return Deserializer.Deserialize<T>(bondReader);
        }

        public static ArraySegment<byte> Serialize(T @event)
        {
            var output = new OutputBuffer();
            var bondWriter = new SimpleBinaryWriter<OutputBuffer>(output);
            serializer.Serialize(@event, bondWriter);
            return output.Data;
        }

        public static byte[] SerializeToBytes(T @event)
        {
            return Serialize(@event).Array;
        }
    }


}
