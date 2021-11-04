using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ozon.MerchandiseService.HttpModels;
using Ozon.MerchandiseService.Infrastructure.Commands;
using Ozon.MerchandiseService.Infrastructure.Queries;
using MerchandiseProvidingRequestDto = Ozon.MerchandiseService.HttpModels.MerchandiseProvidingRequestDto;


namespace Ozon.MerchandiseService.Api.Controllers
{
    [ApiController]
    [Route("api/merches")]
    public class MerchApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MerchApiController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
            var provideCommand = new ProvideCommand()
            {
                EmployeeId = provideRequest.EmployeeId,
                EmployeeEmail = provideRequest.EmployeeEmail,
                MerchPackId = provideRequest.MerchPackId
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
            var checkProvidingQuery = new CheckProvidingQuery()
            {
                EmployeeId = checkProvidingRequest.EmployeeId,
            };

            var queryResponse  =  await _mediator.Send(checkProvidingQuery, cancellationToken);
            
            var response = new CheckProvidingResponse()
            {
                MerchandiseProvidingRequests = queryResponse.MerchandiseProvidingRequests.Select(request => new MerchandiseProvidingRequestDto()
                {
                    MerchProvidingRequestId = request.MerchProvidingRequestId,
                    EmployeeId = request.EmployeeId,
                    MerchPackId = request.MerchPackId,
                    Status = request.Status,
                    CreatedDate = request.CreatedDate,
                    CompletedDate = request.CompletedDate,
                })
            };
            return Ok(response);
        }
    }
}