using System.Linq;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.ValueObjects;
using Xunit;

namespace Ozon.Merchandise.Domain.Tests.Aggregates
{
    public class EmployeeTests
    {
        [Fact]
        public void InitEmployee()
        {
            long id = 1;
            string email = "ivanov@ozon.com";
            var merchItems = new[] {new MerchandisePack(MerchandisePackType.WelcomePack), new MerchandisePack(MerchandisePackType.StarterPack)};
            var employee = new Employee(id, email, merchItems );
            
            Assert.Equal(id, employee.Id);
            Assert.Equal(email, employee.Email);
            Assert.True(merchItems.SequenceEqual(employee.MerchandisePacks));
        }

        [Fact]
        public void Give_AddToMerchandisePacks()
        {
            long id = 1;
            string email = "ivanov@ozon.com";
            var employee = new Employee(id, email);
            var pack = new MerchandisePack(MerchandisePackType.WelcomePack);
            
            employee.Give(pack);
            
            Assert.True(employee.MerchandisePacks.FirstOrDefault(p => p == pack) != null);
        }
        
    }
}