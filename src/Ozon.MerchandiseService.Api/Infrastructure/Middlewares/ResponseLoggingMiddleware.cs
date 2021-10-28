using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ozon.MerchandiseService.Api.Middlewares
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ResponseLoggingMiddleware(RequestDelegate next, ILogger<ResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType == "application/grpc")
                await _next(context);
            else
            {
                var originalBodyStream = context.Response.Body;
                using (var memoryBodyStream = new MemoryStream())
                {
                    context.Response.Body = memoryBodyStream;
                    
                    await _next(context);
                    
                    await LogAsync(context.Response);
                    
                    await memoryBodyStream.CopyToAsync(originalBodyStream);
                }
            }
        }
        
        private async Task LogAsync(HttpResponse response)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Response Info");
                sb.AppendLine($"Path: {response.HttpContext.Request.Path}");
                sb.AppendLine("Headers:");
                foreach (var (name, value) in response.Headers)
                    sb.AppendLine($"{name}:{value}");
                
                response.Body.Seek(0, SeekOrigin.Begin);
                string body = await new StreamReader(response.Body).ReadToEndAsync();
                sb.AppendLine($"Body: {body}");
                response.Body.Seek(0, SeekOrigin.Begin);
                
                _logger.LogInformation(sb.ToString());
            }
            catch (Exception)
            {
                _logger.LogError("Logging response error");
            }
        }
    }
}