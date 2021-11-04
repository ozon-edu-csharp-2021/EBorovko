using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects
{
    public class SkuList: ValueObject, IEnumerable<Sku>
    {
        private List<Sku> Items { get; }

        public SkuList(IEnumerable<Sku> items)
        {
            Items = new List<Sku>(items);   
        }
        
        public IEnumerator<Sku> GetEnumerator() => Items.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        protected override IEnumerable<object> GetEqualityComponents() => Items.OrderBy(x => x.Value);
    }
}