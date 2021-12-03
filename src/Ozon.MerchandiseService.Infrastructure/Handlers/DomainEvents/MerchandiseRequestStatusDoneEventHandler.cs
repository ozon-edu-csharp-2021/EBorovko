using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Ozon.MerchandiseService.Domain.Events;
using Ozon.MerchandiseService.Infrastructure.MessageBrocker;

namespace Ozon.MerchandiseService.Infrastructure.Handlers.DomainEvents
{
    public class MerchandiseRequestStatusDoneEventHandler: INotificationHandler<RequestStatusChangedOnDoneDomainEvent>
    {
        private readonly IProducerBuilderWrapper _producerBuilderWrapper;

        public MerchandiseRequestStatusDoneEventHandler(IProducerBuilderWrapper producerBuilderWrapper)
        {
            _producerBuilderWrapper = producerBuilderWrapper ?? throw new ArgumentNullException(nameof(producerBuilderWrapper));
        }

        public Task Handle(RequestStatusChangedOnDoneDomainEvent notification, CancellationToken cancellationToken)
        {
            var message = new Message<string, string>()
            {
                Key = notification.RequestId.ToString(),
                Value = JsonSerializer.Serialize(new NotificationEvent()
                {
                    EmployeeEmail = notification.Employee.Email.Value,
                    Payload = new MerchDeliveryEventPayload
                    {
                        MerchType = (MerchType) notification.MerchandisePackType.Id
                    }
                })
            };
            _producerBuilderWrapper.Producer.Produce(_producerBuilderWrapper.EmailNotificationTopic, message);
            
            return Task.CompletedTask;
        }
    }
}