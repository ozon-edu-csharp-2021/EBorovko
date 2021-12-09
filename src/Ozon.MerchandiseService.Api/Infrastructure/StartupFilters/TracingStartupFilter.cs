using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Ozon.MerchandiseService.Api.Infrastructure.Middlewares;

namespace Ozon.MerchandiseService.Api.Infrastructure.StartupFilters
{
    public class TracingStartupFilter: IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<TracingMiddleware>();
                next(app);
            };
        }
    }
}