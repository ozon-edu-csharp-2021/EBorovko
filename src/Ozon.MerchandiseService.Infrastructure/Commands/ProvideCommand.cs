using MediatR;

namespace Ozon.MerchandiseService.Infrastructure.Commands
{
    public class ProvideCommand: IRequest<ProvideCommandResponse>
    {
        public int MerchPackId { get; init; }        
        public long EmployeeId { get; init; }
        public string EmployeeEmail { get; init; }
        
    }
}