using MediatR;

namespace Ozon.MerchandiseService.Infrastructure.Queries
{
    public class CheckProvidingQuery: IRequest<CheckProvidingQueryResponse>
    {
        public string EmployeeEmail { get; set; }
    }
}