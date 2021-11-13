using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;

namespace Ozon.MerchandiseService.Domain.Events
{
    public class GivingNewMerchandisePackDomainEvent: INotification
    {
        public Employee Employee { get; }

        public GivingNewMerchandisePackDomainEvent(Employee employee)
        {
            Employee = employee;
        }
        
    }
}