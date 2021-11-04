using System;

namespace Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest
{
    public class UnknownMerchPackId: Exception
    {
        public UnknownMerchPackId(int merchPackId) : base($"Unknown merchPackId {merchPackId}")
        {
            
        }
    }
}