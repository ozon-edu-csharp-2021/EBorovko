using System.Collections.Generic;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects
{
    public sealed class Sku : ValueObject
    {
        public long Value { get; }
        public string Name { get; }

        public Sku(long value, string name)
        {
            Value = value;
            Name = name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}