using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;

namespace Ozon.MerchandiseService.Infrastructure.Services
{
    public class MockStockApiService: IStockApiService
    {
        private readonly List<StockItem> _stockItems;

        public MockStockApiService()
        {
            _stockItems = new List<StockItem>
            {
                new() {SkuId = 1, Quantity = 10},
                new() {SkuId = 2, Quantity = 10},
                new() {SkuId = 3, Quantity = 10},
                new() {SkuId = 4, Quantity = 10},
                new() {SkuId = 5, Quantity = 10},
                new() {SkuId = 6, Quantity = 0}
            };
        }

        public Task<int> GetAvailableQuantityBySkuId(long skuId)
        { 
            int quantity = _stockItems.First(si => si.SkuId == skuId).Quantity;
            return Task.FromResult(quantity);
        }

        public Task ReserveBySkuId(long skuId)
        {
            var stockItem = _stockItems.First(si => si.SkuId == skuId);
            stockItem.Quantity -= 1;
            return Task.CompletedTask;
        }
    }

    public class StockItem
    {
        public long SkuId { get; init; }
        public int Quantity { get; set; }
    }
    
}