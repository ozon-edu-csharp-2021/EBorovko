using System;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects
{
    public class ClothingSize : Enumeration
    {
        public static ClothingSize XS = new(1, nameof(XS));
        public static ClothingSize S = new(2, nameof(S));
        public static ClothingSize M = new(3, nameof(M));
        public static ClothingSize L = new(4, nameof(L));
        public static ClothingSize XL = new(5, nameof(XL));
        public static ClothingSize XXL = new(6, nameof(XXL));

        public ClothingSize(int id, string name) : base(id, name)
        {
        }
        
        public static ClothingSize Parse(int clothingSize)
        {
            return clothingSize switch
            {
                1 => XS,
                2 => S,
                3 => M,
                4 => L,
                5 => XL,
                6 => XXL,
                _ => throw new ArgumentOutOfRangeException(nameof(clothingSize), clothingSize, null)
            };
        }
        
        
    }
}