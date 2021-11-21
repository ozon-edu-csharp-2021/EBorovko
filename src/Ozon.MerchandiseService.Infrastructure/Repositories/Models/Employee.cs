using System.Collections.Generic;

namespace Ozon.MerchandiseService.Infrastructure.Repositories.Models
{
    public class Employee
    {
        public long Id { get; set; }
        public string Email { get; set; }

        public List<MerchandiseProvidingRequest> MerchandiseProvidingRequests { get; set; }
    }
}