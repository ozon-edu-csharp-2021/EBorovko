using System.Collections.Generic;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects
{
    public class Email:ValueObject
    {
        private string Value { get; }
        
        public Email(string value)
        {
            Value = value;
        }

        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return  Value;
        }
    }
}