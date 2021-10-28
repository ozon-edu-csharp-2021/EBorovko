using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ozon.MerchandiseService.Api.Middlewares
{
    public class VersionMiddleware
    {
        private readonly string _version;
        private readonly string _serviceName;

        public VersionMiddleware(RequestDelegate next, string version, string serviceName)
        {
            _version = version;
            _serviceName = serviceName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync($"version: {_version}, serviceName: {_serviceName}");
        }
        
    }
}