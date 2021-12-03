using System.Collections.Generic;
using MediatR;

namespace Ozon.MerchandiseService.Infrastructure.Commands
{
    public class ReplenishStockCommand: IRequest<Unit>
    {
        public long SupplyId { get; set; }
        public IReadOnlyCollection<StockItemQuantityDto> Items { get; set; }
    }

    public class StockItemQuantityDto
    {
        public long Sku { get; set; }
        public int Quantity { get; set; }
    }
    
}