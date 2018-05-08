using Confluent.Kafka;
using Firfly.Communication.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Firfly.Communication.Event
{
    public class EventReciever
    {
        private readonly ILogger logger;
        private readonly Consumer consumer;
        private readonly KafkaOptions kafkaOptions;
        private readonly KafkaConsumerOptions kafkaConsumerOptions;
        private readonly EventRecieverOptions eventRecieverOptions;
        private readonly EventHandlerFactory eventHandlerFactory;
        private readonly List<ActionBlock<Message>> queues;

        public EventReciever(
            ILogger<EventReciever> logger,
            IOptions<KafkaOptions> kafkaOptions,
            IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
            IOptions<EventRecieverOptions> eventRecieverOptions,
            EventHandlerFactory eventHandlerFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.kafkaOptions = kafkaOptions.Value;
            this.kafkaConsumerOptions = kafkaConsumerOptions.Value;
            this.eventRecieverOptions = eventRecieverOptions.Value;
            this.eventHandlerFactory = eventHandlerFactory;

            var kafkaConfig = this.kafkaConsumerOptions.ToKafkaConsumerConfig(this.kafkaOptions);
            consumer = new Consumer(kafkaConfig);

            consumer.OnError += _consumer_OnError;
            consumer.OnLog += Consumer_OnLog;
            consumer.OnStatistics += Consumer_OnStatistics;
            consumer.OnMessage += _consumer_OnMessage;

            queues = new List<ActionBlock<Message>>();
            var blockOptions = new ExecutionDataflowBlockOptions();
            blockOptions.BoundedCapacity = 1;
            blockOptions.MaxDegreeOfParallelism = 1;
            for (int i = 0; i < this.eventRecieverOptions.TotalQueueCount; i++)
            {
                this.queues.Add(new ActionBlock<Message>(HandleMessage, blockOptions));
            }

        }

        private void Consumer_OnStatistics(object sender, string e)
        {
            if(logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug(e);
        }

        private void Consumer_OnLog(object sender, LogMessage e)
        {
            logger.LogInformation($"error: [{e.Name}]{e.Message}");
        }

        private void _consumer_OnError(object sender, Error e)
        {
            logger.LogWarning($"error: [{e.Code}]{e.Reason}");
        }

        private void _consumer_OnMessage(object sender, Message e)
        {
            var total = this.eventRecieverOptions.TotalQueueCount;
            int blockId;
            if (e.Key != null)
            {
                blockId = e.Key[e.Key.Length - 1] % total;
                var block = this.queues[blockId];
                block.SendAsync(e).Wait();
            }
            else
            {
                /**
                 * alternative solutino:
                 * A. LimitedConcurrencyLevelTaskScheduler
                 * B. ActionBlock with MaxDegreeOfParallelism
                 */
                Task.Factory.StartNew(() => HandleMessage(e), TaskCreationOptions.LongRunning);
            }

        }

        private Task HandleMessage(Message message)
        {
            var @event = EventMessageSerializer.Deserialize(message.Value);
            var handler = eventHandlerFactory.GetHandler(@event.EventType);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"handle msg: [{message.Topic}]{@event.EventType}");
            }
            return handler.Handle(@event.Payload, message.Key);
        }


        public void Poll()
        {
            consumer.Subscribe(kafkaConsumerOptions.TopicList);
            logger.LogInformation("Start poll events");
            while (true)
            {
                consumer.Poll(100);
            }
        }
    }
}
