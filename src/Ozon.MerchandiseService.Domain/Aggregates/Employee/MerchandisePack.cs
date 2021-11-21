using Ozon.MerchandiseService.Domain.Models;
using Ozon.MerchandiseService.Domain.ValueObjects;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee
{
    public class MerchandisePack: Entity
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        
        public MerchandisePack(MerchandisePackType merchandisePackType)
        {
            TypeId  = merchandisePackType.Id;
            Name = merchandisePackType.Name;
        }
    }
}