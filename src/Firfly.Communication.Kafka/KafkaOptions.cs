using System;
using System.Collections.Generic;

namespace Firfly.Communication.Kafka
{
    public class KafkaOptions
    {
        public string BrokerList { get; set; }
        public TimeSpan SocketTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan StatisticsInterval { get; set; } = TimeSpan.Zero;

        public long StatisticsIntervalMs { get; set; } = 0;
    }

    public class KafkaConsumerOptions
    {
        public List<string> TopicList { get; set; }
        public string ConsumerGroup { get; set; }
    }

    public static class KafkaConfigurationExtensions
    {

        public static Dictionary<string, object> ToKafkaProducerConfig(this KafkaOptions kafkaOptions)
        {
            return new Dictionary<string, object>
            {
                { "bootstrap.servers", kafkaOptions.BrokerList },
                { "socket.timeout.ms", kafkaOptions.SocketTimeout.TotalMilliseconds },
                { "statistics.interval.ms", kafkaOptions.StatisticsInterval.TotalMilliseconds },
            };
        }

        public static Dictionary<string, object> ToKafkaConsumerConfig(this KafkaConsumerOptions receiverOptions, KafkaOptions kafkaOptions)
        {
            return new Dictionary<string, object>
            {
                { "group.id", receiverOptions.ConsumerGroup },
                { "enable.auto.commit", true },
                { "auto.commit.interval.ms", 5000 },
                { "bootstrap.servers", kafkaOptions.BrokerList },
                { "socket.timeout.ms", kafkaOptions.SocketTimeout.TotalMilliseconds },
                { "statistics.interval.ms",  kafkaOptions.StatisticsInterval.TotalMilliseconds },
                { "default.topic.config", new Dictionary<string, object>()
                    {
                        { "auto.offset.reset", "smallest" }
                    }
                }
            };
        }

    }
}
