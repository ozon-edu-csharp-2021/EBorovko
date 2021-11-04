namespace Ozon.MerchandiseService.HttpModels
{
    public class ProvideRequest
    {
        public int MerchPackId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeEmail { get; set; }
    }
}