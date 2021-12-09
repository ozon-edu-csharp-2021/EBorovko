using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Infrastructure.Queries;

namespace Ozon.MerchandiseService.Infrastructure.Handlers
{
    public class CheckProvidingQueryHandler: IRequestHandler<CheckProvidingQuery, CheckProvidingQueryResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public CheckProvidingQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        public async Task<CheckProvidingQueryResponse> Handle(CheckProvidingQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetWithAllMerchPacksAsync(request.EmployeeEmail, cancellationToken);
            if (employee == null)
                throw new Exception("Employee not found");
            
            return new CheckProvidingQueryResponse
            {
                MerchandisePacks = employee.MerchandisePacks.Select(pack => new MerchandisePackDto()
                {
                    TypeId = pack.TypeId,
                    Name = pack.Name
                })  
            };
        }
    }
}