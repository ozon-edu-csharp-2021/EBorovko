using System;

namespace Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest
{
    public class CompleteAtException: Exception
    {
        public CompleteAtException(string createAt, string completeAt)
            : base($"CompleteAt must be later then createAt. CompleteAt = {completeAt}, createAt = {createAt}")
        { }

    }
}