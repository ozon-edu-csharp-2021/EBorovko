using System.Threading;
using System.Threading.Tasks;
using Ozon.MerchandiseService.Domain.Contracts;

namespace Ozon.MerchandiseService.Domain.Aggregates.Employee
{
    public interface IEmployeeRepository: IRepository<Employee>
    {
        /// <summary>
        /// Найти сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(long employeeId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Получить все мерчпаки
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Employee> GetWithAllMerchPacksAsync(long employeeId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Получить все мерчпаки
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Employee> GetWithAllMerchPacksAsync(string email, CancellationToken cancellationToken);
        
        
        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateAsync(Employee employee, CancellationToken cancellationToken);
        
        /// <summary>
        /// Обновить данные по сотруднику
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken);
    }
}