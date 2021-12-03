using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ozon.MerchandiseService.Infrastructure.Services.Interfaces
{
    public interface IStockApiService
    {
        /*Task<int> GetAvailableQuantityBySkuId(long skuId);
        Task ReserveBySkuId(long skuId);*/
        
        Task <GetByItemTypeDto> GetByItemTypeAsync(int itemType, int size);
        Task<bool> GiveOutItemsAsync(IEnumerable<long> skuIds);
        
    }
    public class GetByItemTypeDto
    {
        public long Id { get; set; }
        public long ItemType { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public long? SizeId { get; set; }
    }
}