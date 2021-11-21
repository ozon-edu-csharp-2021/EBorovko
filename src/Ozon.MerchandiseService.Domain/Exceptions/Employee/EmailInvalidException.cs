using System;

namespace Ozon.MerchandiseService.Domain.Exceptions.Employee
{
    public class EmailInvalidException: Exception
    {
        public EmailInvalidException(string email) : base($"Email is invalid: '{email}'")
        {
            
        }
    }
}