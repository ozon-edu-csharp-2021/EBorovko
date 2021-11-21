using System.Threading.Tasks;

namespace Ozon.MerchandiseService.Infrastructure.Services.Interfaces
{
    public interface IStockApiService
    {
        Task<int> GetAvailableQuantityBySkuId(long skuId);
        Task ReserveBySkuId(long skuId);
    }
}