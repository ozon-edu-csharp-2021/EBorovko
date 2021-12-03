using Microsoft.Extensions.Configuration;
using System;
using Confluent.Kafka;

namespace Ozon.MerchandiseService.Infrastructure.MessageBrocker
{
    public class ProducerBuilderWrapper : IProducerBuilderWrapper
    {
        /// <inheritdoc cref="Producer"/>
        public IProducer<string, string> Producer { get; set; }

        /// <inheritdoc cref="EmailNotificationTopic"/>
        public string EmailNotificationTopic { get; set; }

        public ProducerBuilderWrapper(IConfiguration configuration)
        {
            try
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = configuration["KafkaOptions:BootstrapServers"]
                };

                Producer = new ProducerBuilder<string, string>(producerConfig).Build();
                EmailNotificationTopic = configuration["KafkaOptions:EmailNotificationTopic"];
            }
            catch (Exception e)
            {
                throw new ApplicationException("Configuration for kafka server was not specified", e);
            }
        }
    }
}