using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;

namespace Ozon.MerchandiseService.Api.Infrastructure.Middlewares
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITracer _tracer;

        public TracingMiddleware(RequestDelegate next, ITracer tracer)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _tracer.BuildSpan("Ozon.MerchandiseService.Api").StartActive(true);
            
            await _next(context);
        }
    }
}