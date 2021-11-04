using System.Collections.Generic;

namespace Ozon.MerchandiseService.Infrastructure.Queries
{
    public class CheckProvidingQueryResponse
    {
        public IEnumerable<MerchandiseProvidingRequestDto> MerchandiseProvidingRequests { get; init; }
    }

    public class MerchandiseProvidingRequestDto
    {
        public long MerchProvidingRequestId { get; init; }
        public long EmployeeId { get; init; } 
        public int MerchPackId { get; init; }
        public string Status { get; init; }
        public string CreatedDate { get; init; }
        public string CompletedDate { get; init; }
    }
}