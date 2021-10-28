using System.Threading.Tasks;
using Grpc.Core;
using Ozon.MerchandiseService.Grpc;

namespace Ozon.MerchandiseService.Api.GrpcServices
{
    public class MerchApiGrpcService: MerchApiGrpc.MerchApiGrpcBase
    {
        /// <summary>
        /// Выдать мерч
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ProvideResponse> Provide(ProvideRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ProvideResponse() { IsProviding = true }); 
        }

        /// <summary>
        /// Проверить выдачу мерча
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<CheckProvidingResponse> CheckProviding(CheckProvidingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CheckProvidingResponse() { IsProviding = true });
        }
    }
}