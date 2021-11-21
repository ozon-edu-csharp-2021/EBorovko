using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;

namespace Ozon.MerchandiseService.Domain.Events
{
    public record RequestStatusChangedOnDoneDomainEvent: INotification
    {
        public Employee Employee { get; set; }
        public RequestStatusChangedOnDoneDomainEvent(Employee employee)
        {
            Employee = employee;
        }
        
    }
}