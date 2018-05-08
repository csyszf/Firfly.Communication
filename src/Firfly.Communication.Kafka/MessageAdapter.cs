using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Firfly.Communication.Kafka
{
    public class MessageAdapter: IMessageAdapter
    {
        private readonly ILogger logger;
        private readonly Producer producer;
        private readonly KafkaOptions options;
        public MessageAdapter(
            ILogger<MessageAdapter> logger,
            IOptions<KafkaOptions> options)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.options = options.Value;

            var kafkaConfig = this.options.ToKafkaProducerConfig();
            producer = new Producer(kafkaConfig);
            producer.GetMetadata(false, null, TimeSpan.FromSeconds(3));
            producer.OnError += Producer_OnError;
            producer.OnLog += Producer_OnLog;
            producer.OnStatistics += Producer_OnStatistics;
        }

        private void Producer_OnStatistics(object sender, string e)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug(e);
        }

        private void Producer_OnLog(object sender, LogMessage e)
        {
            logger.LogInformation($"error: [{e.Name}]{e.Message}");
        }

        private void Producer_OnError(object sender, Error e)
        {
            logger.LogWarning($"error: [{e.Code}]{e.Reason}");
        }

        public async Task ProduceAsync(string topic, byte[] key, byte[] val)
        {
            var msg = await producer.ProduceAsync(topic, key, val);
            if (!msg.Error.HasError)
                return;
            logger.LogWarning($"failed to produce kafka message: [{msg.Error.Code}]{msg.Error.Reason}");
        }

    }
}
