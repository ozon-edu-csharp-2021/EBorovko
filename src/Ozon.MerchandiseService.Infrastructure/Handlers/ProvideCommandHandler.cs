using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
//using Ozon.MerchandiseService.Domain.Entities.MerchandiseItem;
using Ozon.MerchandiseService.Infrastructure.Commands;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;

namespace Ozon.MerchandiseService.Infrastructure.Handlers
{
    public class ProvideCommandHandler: IRequestHandler<ProvideCommand, ProvideCommandResponse>
    {
        private readonly IMerchandiseProvidingRequestRepository _merchandiseProvidingRequestRepository;
        private readonly IStockApiService _stockApiService;
        private readonly IDateTimeService _dateTimeService;

        public ProvideCommandHandler(
            IMerchandiseProvidingRequestRepository merchandiseProvidingRequestRepository,
            IStockApiService stockApiService,
            IDateTimeService dateTimeService)
        {
            _merchandiseProvidingRequestRepository = merchandiseProvidingRequestRepository ?? throw new ArgumentNullException(nameof(merchandiseProvidingRequestRepository));
            _stockApiService = stockApiService ?? throw new ArgumentNullException(nameof(stockApiService));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        }
        
        public async Task<ProvideCommandResponse> Handle(ProvideCommand request, CancellationToken cancellationToken)
        {
            var merchRequest =  await _merchandiseProvidingRequestRepository.FindByMerchPackIdAndEmployeeIdAsync(request.MerchPackId, request.EmployeeId, cancellationToken);
            
            if (merchRequest.IsDone && !merchRequest.CheckOneYearPassFromProviding(_dateTimeService.Now))
                throw new Exception("Merch pack of such type already provide to employee");
            
            if (merchRequest.InWork)
                throw new Exception("Request for мerch pack of such type exists. Waiting stock supplying");
            
            var newMerchRequest = new MerchandiseProvidingRequest(request.EmployeeId, request.EmployeeEmail, request.MerchPackId, _dateTimeService.Now);
            
            
            var quantities = await Task.WhenAll(newMerchRequest.SkuIds.Select(id => _stockApiService.GetAvailableQuantityBySkuId(id)));
            bool areAllAvailable = quantities.All(quantity => quantity > 0);
            if (!areAllAvailable)
                newMerchRequest.Wait();
            else
            {
                await Task.WhenAll(newMerchRequest.SkuIds.Select(id => _stockApiService.ReserveBySkuId(id)));
                newMerchRequest.Complete(_dateTimeService.Now);
            }
            
            await _merchandiseProvidingRequestRepository.CreateAsync(newMerchRequest, cancellationToken);

            return new ProvideCommandResponse
            {
                MerchProvidingRequestId = newMerchRequest.Id,
                MerchPackId = newMerchRequest.MerchandisePackType.Id,
                Status = newMerchRequest.CurrentStatus.Name
            };
        }
    }
}