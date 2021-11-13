using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ozon.MerchandiseService.Domain.Exceptions.Employee;
using Ozon.MerchandiseService.Domain.Models;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; }

        private Email(string email)
        {
            Value = email;
        }
        
        public static Email Create(string email)
        {
            if (String.IsNullOrEmpty(email) || !IsValidEmail(email))
                throw new EmailInvalidException(email);
            
            return new Email(email);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static bool IsValidEmail(string emailString)
            => Regex.IsMatch(emailString, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
    }
}