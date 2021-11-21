using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Events;

namespace Ozon.MerchandiseService.Infrastructure.Handlers.DomainEvents
{
    public class MerchandiseRequestStatusDoneEventHandler: INotificationHandler<RequestStatusChangedOnDoneDomainEvent>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public MerchandiseRequestStatusDoneEventHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task Handle(RequestStatusChangedOnDoneDomainEvent notification, CancellationToken cancellationToken)
        {
            bool isExists = await _employeeRepository.ExistsAsync(notification.Employee.Id, cancellationToken);
            if (!isExists)
                await _employeeRepository.CreateAsync(notification.Employee, cancellationToken);
        }
    }
}