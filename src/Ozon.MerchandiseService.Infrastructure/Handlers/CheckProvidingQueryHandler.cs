using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Infrastructure.Queries;

namespace Ozon.MerchandiseService.Infrastructure.Handlers
{
    public class CheckProvidingQueryHandler: IRequestHandler<CheckProvidingQuery, CheckProvidingQueryResponse>
    {
        private readonly IMerchandiseProvidingRequestRepository _merchandiseProvidingRequestRepository;

        public CheckProvidingQueryHandler(IMerchandiseProvidingRequestRepository merchandiseProvidingRequestRepository)
        {
            _merchandiseProvidingRequestRepository = merchandiseProvidingRequestRepository ?? throw new ArgumentNullException(nameof(merchandiseProvidingRequestRepository));
        }

        public async Task<CheckProvidingQueryResponse> Handle(CheckProvidingQuery request, CancellationToken cancellationToken)
        {
            var merchProvidingRequests = await _merchandiseProvidingRequestRepository.FindAllByEmployeeIdAsync(request.EmployeeId, cancellationToken);
            return new CheckProvidingQueryResponse
            {
                MerchandiseProvidingRequests = merchProvidingRequests.Select(r => new MerchandiseProvidingRequestDto
                {
                    MerchProvidingRequestId = r.Id,
                    MerchPackId = r.MerchandisePack.Id,
                    EmployeeId = r.EmployeeId,
                    Status = r.CurrentStatus.Name,
                    CreatedDate = r.CreatedAt.ToString(),
                    CompletedDate = r.CompletedAt?.ToString()
                })  
            };
        }
    }
}