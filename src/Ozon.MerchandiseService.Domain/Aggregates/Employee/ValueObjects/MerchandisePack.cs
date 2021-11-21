using System.Collections.Generic;
using Ozon.MerchandiseService.Domain.Models;
using Ozon.MerchandiseService.Domain.ValueObjects;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects
{
    public class MerchandisePack: ValueObject
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        
        public MerchandisePack(MerchandisePackType merchandisePackType)
        {
            TypeId  = merchandisePackType.Id;
            Name = merchandisePackType.Name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TypeId;
            yield return Name;
        }
    }
}