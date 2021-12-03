using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using OpenTracing;
using Ozon.MerchandiseService.Grpc;
using Ozon.MerchandiseService.Infrastructure.Commands;
using Ozon.MerchandiseService.Infrastructure.Queries;


namespace Ozon.MerchandiseService.Api.GrpcServices
{
    public class MerchApiGrpcService: MerchApiGrpc.MerchApiGrpcBase
    {
        private readonly IMediator _mediator;
        private readonly IDistributedCache _distributedCache;
        private readonly ITracer _tracer;

        public MerchApiGrpcService(IMediator mediator, IDistributedCache distributedCache, ITracer tracer)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }
            
        /// <summary>
        /// Выдать мерч
        /// </summary>
        /// <param name="provideRequest"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ProvideResponse> Provide(ProvideRequest provideRequest, ServerCallContext context)
        {
            using var scope = _tracer.BuildSpan(nameof(CheckProviding)).StartActive(true);
            
            var provideCommand = new ProvideCommand()
            {
                MerchPackId = provideRequest.MerchPackId,
                //EmployeeId = provideRequest.EmployeeId,
                EmployeeEmail = provideRequest.EmployeeEmail,
                ClothingSize =  provideRequest.ClothingSize
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
            using var scope = _tracer.BuildSpan(nameof(CheckProviding)).StartActive(true);
            
            List<CheckProvidingResponseUnit> merchPacks = null;
            var cache = await _distributedCache.GetStringAsync(request.EmployeeEmail, context.CancellationToken);
            if (!string.IsNullOrEmpty(cache))
                merchPacks = JsonSerializer.Deserialize<List<CheckProvidingResponseUnit>>(cache);
            else
            {
                var checkProvidingQuery = new CheckProvidingQuery()
                {
                    EmployeeEmail = request.EmployeeEmail,
                };
                
                var queryResponse  =  await _mediator.Send(checkProvidingQuery, context.CancellationToken);
                merchPacks = queryResponse.MerchandisePacks.Select(pack => new CheckProvidingResponseUnit()
                {
                    MerchPackTypeId =  pack.TypeId,
                    MerchPackName =  pack.Name
                }).ToList();

                string json = JsonSerializer.Serialize(merchPacks);
                await _distributedCache.SetStringAsync(request.EmployeeEmail, json);
            }

            var response = new CheckProvidingResponse();
            response.ProvidingRequests.AddRange(merchPacks);
            
            return response;
        }
    }
}