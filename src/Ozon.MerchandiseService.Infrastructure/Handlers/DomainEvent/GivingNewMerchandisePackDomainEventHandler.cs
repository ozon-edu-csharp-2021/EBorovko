using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Events;

namespace Ozon.MerchandiseService.Infrastructure.Handlers.DomainEvent
{
    public class GivingNewMerchandisePackDomainEventHandler: INotificationHandler<GivingNewMerchandisePackDomainEvent>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GivingNewMerchandisePackDomainEventHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }
        
        
        public async Task Handle(GivingNewMerchandisePackDomainEvent notification, CancellationToken cancellationToken)
        {
            await _employeeRepository.UpdateAsync(notification.Employee, cancellationToken);
        }
    }
}