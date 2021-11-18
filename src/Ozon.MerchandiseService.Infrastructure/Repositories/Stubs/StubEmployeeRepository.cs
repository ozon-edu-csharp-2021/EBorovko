using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects;
using Ozon.MerchandiseService.Domain.ValueObjects;

namespace Ozon.MerchandiseService.Infrastructure.Repositories.Stubs
{
    public class StubEmployeeRepository: IEmployeeRepository
    {
        private readonly List<Employee> _employees;
        
        public StubEmployeeRepository()
        {
            _employees = new List<Employee>
            {
                new(1, Email.Create("ivanov@ozon.com"), new []{ new MerchandisePack(MerchandisePackType.StarterPack),new MerchandisePack(MerchandisePackType.ConferenceListenerPack)}),
                new(2, Email.Create( "petrov@ozon.com"), new []{ new MerchandisePack(MerchandisePackType.WelcomePack)}),
                new(3, Email.Create( "sidorov@ozon.com"), Enumerable.Empty<MerchandisePack>())
            };
        }
        
        public Task<Employee> FindAsync(long employeeId, CancellationToken cancellationToken)
        {
            var employee =  _employees.FirstOrDefault(e => e.Id == employeeId);
            return Task.FromResult(employee);
        }

        public Task<bool> ExistsAsync(long employeeId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Employee> GetWithAllMerchPacksAsync(long employeeId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(Employee employee, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken)
        {
            var finded = _employees.FirstOrDefault(e => e.Id == employee.Id);
            _employees.Remove(finded);
            _employees.Add(employee);
            return Task.FromResult(employee);
        }
    }
}