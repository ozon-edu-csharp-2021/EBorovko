using Confluent.Kafka;

namespace Ozon.MerchandiseService.Infrastructure.MessageBrocker
{
    public interface IProducerBuilderWrapper
    {
        /// <summary>
        /// Producer instance
        /// </summary>
        IProducer<string, string> Producer { get; set; }

        /// <summary>
        /// Топик для отправки сообщения что мерчпак выдан
        /// </summary>
        string EmailNotificationTopic { get; set; }
    }
}