using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;

namespace Ozon.MerchandiseService.Infrastructure.Repositories.Stubs
{
    public class StubMerchandiseProvidingRequestRepository: IMerchandiseProvidingRequestRepository
    {
        private readonly List<MerchandiseProvidingRequest> _requests;
        private long _idCounter; 
        
        public StubMerchandiseProvidingRequestRepository()
        {
            _requests = new List<MerchandiseProvidingRequest>
            {
                new(new Employee(1, Email.Create("ivanov@ozon.com")), 10, DateTimeOffset.Now.AddDays(-1)),
                new(new Employee(2, Email.Create("petrov@ozon.com")), 20, DateTimeOffset.Now.AddDays(-10)),
                new(new Employee(3, Email.Create("sidorov@ozon.com")), 30, DateTimeOffset.Now.AddDays(-100))
            };
        }
        
        public Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeIdAsync(int merchPackId, long employeeId, CancellationToken cancellationToken)
        {
            var request = _requests
                .Where(r => r.MerchandisePackType.Id == merchPackId && r.Employee.Id == employeeId)
                .OrderByDescending(r => r.CompletedAt)
                .FirstOrDefault();
            
            return Task.FromResult(request ?? new MerchandiseProvidingRequest());
        }

        public Task<long> CreateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            merchandiseRequest.Id = ++_idCounter;
            _requests.Add(merchandiseRequest);
            return Task.FromResult(merchandiseRequest.Id);
        }

        public Task<long> UpdateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}