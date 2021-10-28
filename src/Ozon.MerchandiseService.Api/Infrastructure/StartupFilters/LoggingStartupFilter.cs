using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Ozon.MerchandiseService.Api.Middlewares;


namespace Ozon.MerchandiseService.Api.StartupFilters
{
    public class LoggingStartupFilter: IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<RequestLoggingMiddleware>();
                app.UseMiddleware<ResponseLoggingMiddleware>();
                next(app);
            };
        }
    }
}