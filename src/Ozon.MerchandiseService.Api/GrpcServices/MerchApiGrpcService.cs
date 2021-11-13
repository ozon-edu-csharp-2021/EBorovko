using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Ozon.MerchandiseService.Grpc;
using Ozon.MerchandiseService.Infrastructure.Commands;
using Ozon.MerchandiseService.Infrastructure.Queries;


namespace Ozon.MerchandiseService.Api.GrpcServices
{
    public class MerchApiGrpcService: MerchApiGrpc.MerchApiGrpcBase
    {
        private readonly IMediator _mediator;

        public MerchApiGrpcService(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
            
        /// <summary>
        /// Выдать мерч
        /// </summary>
        /// <param name="provideRequest"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ProvideResponse> Provide(ProvideRequest provideRequest, ServerCallContext context)
        {
            var provideCommand = new ProvideCommand()
            {
                MerchPackId = provideRequest.MerchPackId,
                EmployeeId = provideRequest.EmployeeId,
                EmployeeEmail = provideRequest.EmployeeEmail
            };
            
            var commandResponse =  await _mediator.Send(provideCommand, context.CancellationToken);
                
            return new ProvideResponse
            {
                MerchProvidingRequestId = commandResponse.MerchProvidingRequestId,
                MerchPackId = commandResponse.MerchPackId,
                Status = commandResponse.Status,
            };
        }

        /// <summary>
        /// Проверить выдачу мерча
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<CheckProvidingResponse> CheckProviding(CheckProvidingRequest request, ServerCallContext context)
        {
            var checkProvidingQuery = new CheckProvidingQuery()
            {
                EmployeeId = request.EmployeeId,
            };

            var queryResponse  =  await _mediator.Send(checkProvidingQuery, context.CancellationToken);

            var providingRequests = queryResponse.MerchandisePacks.Select(pack => new CheckProvidingResponseUnit()
            {
                MerchPackId =  pack.Id,
                MerchPackTypeId =  pack.TypeId,
                MerchPackName =  pack.Name
            });
            var response = new CheckProvidingResponse();
            response.ProvidingRequests.AddRange(providingRequests);
            
            return response;
        }
    }
}