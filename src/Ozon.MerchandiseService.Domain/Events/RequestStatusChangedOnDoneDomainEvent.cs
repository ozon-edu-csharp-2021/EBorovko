using MediatR;

namespace Ozon.MerchandiseService.Domain.Events
{
    public class RequestStatusChangedOnDoneDomainEvent: INotification
    {
        public long RequestId { get;}

        public RequestStatusChangedOnDoneDomainEvent(long requestId)
        {
            RequestId = requestId;
        }
        
    }
}