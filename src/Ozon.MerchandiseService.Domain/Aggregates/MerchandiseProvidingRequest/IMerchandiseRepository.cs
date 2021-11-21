using System.Threading;
using System.Threading.Tasks;

namespace Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest
{
    public interface IMerchandiseProvidingRequestRepository
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
        Task<long> UpdateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken);
    }
}