using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ozon.MerchandiseService.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType != "application/grpc")
                await LogAsync(context.Request);
            
            await _next(context);
        }

        private async Task LogAsync(HttpRequest request)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Request Info");
                sb.AppendLine($"Path: {request.Path.ToString()}");
                sb.AppendLine("Headers:");
                foreach (var (name, value) in request.Headers)
                    sb.AppendLine($"{name}:{value}");
                
                if (request.ContentLength > 0)
                {
                    request.EnableBuffering();
                        
                    var buffer = new byte[request.ContentLength.Value];
                    await request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
                    var body = Encoding.UTF8.GetString(buffer);
                    sb.AppendLine($"Body: {body}");

                    request.Body.Position = 0;
                }
                _logger.LogInformation(sb.ToString());
            }
            catch (Exception)
            {
                _logger.LogError("Logging request error");
            }
            
        }
    }
}