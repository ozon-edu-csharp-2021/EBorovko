using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ozon.MerchandiseService.Api.Middlewares
{
    public class ReadyMiddleware
    {
        public ReadyMiddleware(RequestDelegate next)
        {
            
        }
        
        public Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return Task.CompletedTask;
        }
        
    }
}