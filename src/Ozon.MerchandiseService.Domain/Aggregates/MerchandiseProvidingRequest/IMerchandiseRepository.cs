using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ozon.MerchandiseService.Domain.Contracts;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest
{
    public interface IMerchandiseProvidingRequestRepository: IRepository<MerchandiseProvidingRequest>
    {
        /// <summary>
        /// Найти последний завершенный запрос по идентификатору мерчпака и идентификатору сотрудника
        /// </summary>
        /// <param name="merchPackId"></param>
        /// <param name="employeeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeIdAsync(int merchPackId, long employeeId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Найти последний завершенный запрос по идентификатору мерчпака и email сотрудника
        /// </summary>
        /// <param name="merchPackId"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeEmailAsync(int merchPackId, string email, CancellationToken cancellationToken);
        
        /// <summary>
        /// Создать запрос
        /// </summary>
        /// <param name="merchandiseRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> CreateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken);
        
        /// <summary>
        /// Обновить запрос
        /// </summary>
        /// <param name="merchandiseRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Получить все заявки с определенным статусом
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MerchandiseProvidingRequest>> GetAllWithStatus(int status, CancellationToken cancellationToken);
    }
}