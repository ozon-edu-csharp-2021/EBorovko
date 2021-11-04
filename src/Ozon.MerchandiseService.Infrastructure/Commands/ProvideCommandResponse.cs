namespace Ozon.MerchandiseService.Infrastructure.Commands
{
    public class ProvideCommandResponse
    {
        public long MerchProvidingRequestId { get; init; }
        public int MerchPackId { get; init; }
        public string Status { get; init; }
    }
}