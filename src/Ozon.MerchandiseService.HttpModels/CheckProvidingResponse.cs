using System.Collections.Generic;

namespace Ozon.MerchandiseService.HttpModels
{
    public class CheckProvidingResponse
    {
        public IEnumerable<MerchandisePackDto> MerchandisePacks { get; set; }
    }

    public class MerchandisePackDto
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
    }
}