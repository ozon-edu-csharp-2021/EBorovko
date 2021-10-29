using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ozon.MerchandiseService.HttpModels;

namespace Ozon.MerchandiseService.Api.Controllers
{
    [ApiController]
    [Route("api/merches")]
    public class MerchApiController : ControllerBase
    {
        /// <summary>
        /// Выдать мерч
        /// </summary>
        /// <param name="provideRequest"></param>
        /// <returns></returns>
        [HttpPost("provide")]
        public Task<ActionResult<ProvideResponse>> Provide(ProvideRequest provideRequest)
        {
            return Task.FromResult<ActionResult<ProvideResponse>>(Ok(new ProvideResponse() { IsProvided = true }));
        }
        
        /// <summary>
        /// Проверить выдачу мерча
        /// </summary>
        /// <param name="checkProvidingRequest"></param>
        /// <returns></returns>
        [HttpPost("check-providing")]
        public Task<ActionResult<CheckProvidingResponse>> CheckProviding(CheckProvidingRequest checkProvidingRequest)
        {
            return Task.FromResult<ActionResult<CheckProvidingResponse>>(Ok(new CheckProvidingResponse() { IsProvided = true }));
        }
    }
}