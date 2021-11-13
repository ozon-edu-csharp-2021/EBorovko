using Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects;
using Ozon.MerchandiseService.Domain.Exceptions.Employee;
using Xunit;

namespace Ozon.Merchandise.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_WithValidEmail_ReturnEmail()
        {
            var email = Email.Create("ivanov@gmail.com");
            Assert.NotNull(email);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ivanovgmail.com")]
        public void Create_WithInValidEmail_ThrowEmailInvalidException(string email)
        {
            Assert.Throws<EmailInvalidException>(() => Email.Create(email));
        }
        
    }
}