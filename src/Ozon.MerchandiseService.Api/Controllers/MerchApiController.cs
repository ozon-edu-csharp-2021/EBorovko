using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OpenTracing;
using Ozon.MerchandiseService.HttpModels;
using Ozon.MerchandiseService.Infrastructure.Commands;
using Ozon.MerchandiseService.Infrastructure.Queries;
using MerchandisePackDto = Ozon.MerchandiseService.HttpModels.MerchandisePackDto;


namespace Ozon.MerchandiseService.Api.Controllers
{
    [ApiController]
    [Route("api/merches")]
    public class MerchApiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDistributedCache _distributedCache;
        private readonly ITracer _tracer;

        public MerchApiController(IMediator mediator, IDistributedCache distributedCache, ITracer tracer)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }


        /// <summary>
        /// Выдать мерч
        /// </summary>
        /// <param name="provideRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("provide")]
        public async Task<ActionResult<ProvideResponse>> Provide(ProvideRequest provideRequest, CancellationToken cancellationToken)
        {
            using var scope = _tracer.BuildSpan(ControllerContext.ActionDescriptor.DisplayName).StartActive(true);
            
            var provideCommand = new ProvideCommand()
            {
                //EmployeeId = provideRequest.EmployeeId,
                EmployeeEmail = provideRequest.EmployeeEmail,
                MerchPackId = provideRequest.MerchPackId,
                ClothingSize = provideRequest.ClothingSize
            };

            var commandResponse =  await _mediator.Send(provideCommand, cancellationToken);
                
            var response = new ProvideResponse
            {
                MerchProvidingRequestId = commandResponse.MerchProvidingRequestId,
                MerchPackId = commandResponse.MerchPackId,
                Status = commandResponse.Status,
            };
            return Ok(response);
        }

        /// <summary>
        /// Проверить выдачу мерча
        /// </summary>
        /// <param name="checkProvidingRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("check-providing")]
        public async Task<ActionResult<CheckProvidingResponse>> CheckProviding(CheckProvidingRequest checkProvidingRequest, CancellationToken cancellationToken)
        {
            using var scope = _tracer.BuildSpan(ControllerContext.ActionDescriptor.DisplayName).StartActive(true);
            
            List<MerchandisePackDto> merchandisePackDtos = null;
            var cache = await _distributedCache.GetStringAsync(checkProvidingRequest.EmployeeEmail, cancellationToken);
            if (!string.IsNullOrEmpty(cache))
                merchandisePackDtos= JsonSerializer.Deserialize<List<MerchandisePackDto>>(cache);
            else
            {
                var checkProvidingQuery = new CheckProvidingQuery()
                {
                    EmployeeEmail = checkProvidingRequest.EmployeeEmail,
                };

                var queryResponse  =  await _mediator.Send(checkProvidingQuery, cancellationToken);
                merchandisePackDtos = queryResponse.MerchandisePacks.Select(pack => new MerchandisePackDto()
                {
                    TypeId = pack.TypeId,
                    Name = pack.Name
                }).ToList();
                 
                string json = JsonSerializer.Serialize(merchandisePackDtos);
                await _distributedCache.SetStringAsync(checkProvidingQuery.EmployeeEmail, json, cancellationToken);
            }

            var response = new CheckProvidingResponse
            {
                MerchandisePacks = merchandisePackDtos
            };
            return Ok(response);
        }
    }
}