using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.Contracts;
using Ozon.MerchandiseService.Infrastructure.Commands;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;

namespace Ozon.MerchandiseService.Infrastructure.Handlers
{
    public class ReplenishStockCommandHandler: IRequestHandler<ReplenishStockCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockApiService _stockApiService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ILogger<ReplenishStockCommandHandler> _logger;
        private readonly IMerchandiseProvidingRequestRepository _merchandiseProvidingRequestRepository;

        public ReplenishStockCommandHandler(
            IMerchandiseProvidingRequestRepository merchandiseProvidingRequestRepository,
            IUnitOfWork unitOfWork,
            IStockApiService stockApiService,
            IDateTimeService dateTimeService,
            ILogger<ReplenishStockCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _stockApiService = stockApiService ?? throw new ArgumentNullException(nameof(stockApiService));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _merchandiseProvidingRequestRepository = merchandiseProvidingRequestRepository ?? throw new ArgumentNullException(nameof(merchandiseProvidingRequestRepository));
        }

        public async Task<Unit> Handle(ReplenishStockCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.StartTransaction(cancellationToken);

            var merchRequests =
                await _merchandiseProvidingRequestRepository.GetAllWithStatus(Status.InWork.Id, cancellationToken);

            // уведомление о поступлении товара пришло, но все равно нужно проверять наличие всех SKU в паке
            foreach (var merchRequest in merchRequests)
            {
                // TODO добавить size в запрос
                int size = 1;
                var skuItems =
                    await Task.WhenAll(merchRequest.SkuIds.Select(skuId =>
                        _stockApiService.GetByItemTypeAsync(skuId, size)));
                bool areAllAvailable = skuItems.All(item => item.Quantity > 0);
                if (areAllAvailable)
                {
                    var areGiveOut = await _stockApiService.GiveOutItemsAsync(skuItems.Select(skuItem => skuItem.Id));
                    if (areGiveOut)
                        merchRequest.Complete(_dateTimeService.Now);
                }

                int num = await _merchandiseProvidingRequestRepository.UpdateAsync(merchRequest, cancellationToken);
                if(num == 0)
                    _logger.LogWarning($"Merch request with id {merchRequest.Id} doesn't update");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }


    }
}