using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;


namespace Ozon.MerchandiseService.Infrastructure.Repositories
{
    public class StubMerchandiseProvidingRequestRepository: IMerchandiseProvidingRequestRepository
    {
        private readonly List<MerchandiseProvidingRequest> _requests;
        private long _idCounter; 
        
        public StubMerchandiseProvidingRequestRepository()
        {
            _requests = new List<MerchandiseProvidingRequest>
            {
                new(1, "ivanov@ozon.com", 10, DateTimeOffset.Now.AddDays(-1)),
                new(2, "petrov@ozon.com", 20, DateTimeOffset.Now.AddDays(-10)),
                new(3, "sidorov@ozon.com", 30, DateTimeOffset.Now.AddDays(-100))
            };
        }
        
        public Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeIdAsync(int merchPackId, long employeeId, CancellationToken cancellationToken)
        {
            var request = _requests
                .Where(r => r.MerchandisePackType.Id == merchPackId && r.EmployeeId == employeeId)
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