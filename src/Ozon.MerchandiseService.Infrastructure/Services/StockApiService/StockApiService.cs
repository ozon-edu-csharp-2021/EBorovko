using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;
using OzonEdu.StockApi.Grpc;

namespace Ozon.MerchandiseService.Infrastructure.Services.StockApiService
{
    public class StockApiService: IStockApiService
    {
        private readonly StockApiGrpc.StockApiGrpcClient _stockApiGrpcClient;
        private readonly ILogger<StockApiService> _logger;
        public StockApiService(StockApiGrpc.StockApiGrpcClient stockApiGrpcClient, ILogger<StockApiService> logger)
        {
            _stockApiGrpcClient = stockApiGrpcClient ?? throw new ArgumentException(nameof(stockApiGrpcClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> GiveOutItemsAsync(IEnumerable<long> skuIds)
        {
            try
            {
                var skuItems = skuIds.Select(skuId => new SkuQuantityItem() {Sku =  skuId, Quantity = 1});
                var result = await _stockApiGrpcClient.GiveOutItemsAsync(new GiveOutItemsRequest() { Items =  { skuItems } });
                return result.Result == GiveOutItemsResponse.Types.Result.Successful;
            }
            catch (RpcException e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<GetByItemTypeDto> GetByItemTypeAsync(int itemType, int size)
        {
            try
            {
                var result = await _stockApiGrpcClient.GetByItemTypeAsync(new IntIdModel() {Id = itemType});
                var item = result.Items.Select(itemUnit => new GetByItemTypeDto()
                {
                    Id = itemUnit.Sku,
                    ItemName = itemUnit.ItemName,
                    ItemType = itemUnit.ItemTypeId,
                    Quantity = itemUnit.Quantity,
                    SizeId = itemUnit.SizeId
                }).SingleOrDefault(itemDto => !itemDto.SizeId.HasValue || itemDto.SizeId.Value == size);
                return item;
            }
            catch (RpcException e)
            {
                _logger.LogError(e.Message);
                return new GetByItemTypeDto();
            }
        }
    }
}