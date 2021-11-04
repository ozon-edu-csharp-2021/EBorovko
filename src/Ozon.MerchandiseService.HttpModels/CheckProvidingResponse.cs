using System.Collections.Generic;

namespace Ozon.MerchandiseService.HttpModels
{
    public class CheckProvidingResponse
    {
        public IEnumerable<MerchandiseProvidingRequestDto> MerchandiseProvidingRequests { get; set; }
    }

    public class MerchandiseProvidingRequestDto
    {
        public long MerchProvidingRequestId { get; set; }
        public long EmployeeId { get; set; } 
        public int MerchPackId { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string CompletedDate { get; set; }
    }
}