using Bond;
using Bond.IO.Unsafe;
using Bond.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deserializer = Bond.Deserializer<Bond.Protocols.SimpleBinaryReader<Bond.IO.Unsafe.InputBuffer>>;
using Serializer = Bond.Serializer<Bond.Protocols.SimpleBinaryWriter<Bond.IO.Unsafe.OutputBuffer>>;

namespace Firfly.Communication.Event
{

    [Schema]
    public class EventMessage
    {
        [Id(0)]
        public string EventType { get; set; }
        [Id(1), Type(typeof(Bond.Tag.nullable<System.ArraySegment<byte>>))]
        public ArraySegment<byte> Payload { get; set; }
    }

    public static class EventMessageSerializer
    {
        private static readonly Serializer serializer = new Serializer(typeof(EventMessage));
        private static readonly Deserializer deserializer = new Deserializer(typeof(EventMessage));

        public static byte[] Serialize(this EventMessage @event)
        {
            var output = new OutputBuffer();
            var bondWriter = new SimpleBinaryWriter<OutputBuffer>(output);
            serializer.Serialize(@event, bondWriter);
            return output.Data.ToArray();
        }

        public static EventMessage Deserialize(byte[] data)
        {
            var input = new InputBuffer(data);
            var bondReader = new SimpleBinaryReader<InputBuffer>(input);
            return deserializer.Deserialize<EventMessage>(bondReader);
        }
    }

}
