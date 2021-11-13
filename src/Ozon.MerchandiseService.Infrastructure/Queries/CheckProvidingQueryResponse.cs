using System.Collections.Generic;

namespace Ozon.MerchandiseService.Infrastructure.Queries
{
    public class CheckProvidingQueryResponse
    {
        public IEnumerable<MerchandisePackDto> MerchandisePacks { get; init; }
    }

    public class MerchandisePackDto
    {
        public long Id { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
    }
}