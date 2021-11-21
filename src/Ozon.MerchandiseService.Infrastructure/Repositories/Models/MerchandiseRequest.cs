using System;

namespace Ozon.MerchandiseService.Infrastructure.Repositories.Models
{
    public class MerchandiseProvidingRequest
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public int MerchPackTypeId { get; set; }
        public int Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
    }
}