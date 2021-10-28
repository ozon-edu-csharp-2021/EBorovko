using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ozon.MerchandiseService.Api.Middlewares
{
    public class LiveMiddleware
    {
        public LiveMiddleware(RequestDelegate next)
        {
            
        }
        
        public Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return Task.CompletedTask;
        }
        
    }
}