using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.ValueObjects;

namespace Ozon.MerchandiseService.Domain.Events
{
    public record RequestStatusChangedOnDoneDomainEvent: INotification
    {
        public long RequestId { get; init; }
        public Employee Employee { get; init; }
        public MerchandisePackType MerchandisePackType { get; init; }

        public RequestStatusChangedOnDoneDomainEvent(long requestId, Employee employee, MerchandisePackType merchandisePackType)
        {
            RequestId = requestId;
            Employee = employee;
            MerchandisePackType = merchandisePackType;
        }
        
    }
}