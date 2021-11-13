using System.Threading;
using System.Threading.Tasks;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Найти сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Employee> FindAsync(long employeeId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Обновить данные по сотруднику
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken);
    }
}