using System;

namespace Ozon.MerchandiseService.Domain.Exceptions.MerchandiseProvidingRequest
{
    public class StatusException: Exception
    {
        public StatusException(string currentStatus, string correctStatus) : base($"Incorrect request status {currentStatus}. Status must be {correctStatus}")
        {
            
        }
    }
}