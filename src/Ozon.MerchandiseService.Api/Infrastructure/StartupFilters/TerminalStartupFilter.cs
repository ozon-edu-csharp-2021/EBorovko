using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Ozon.MerchandiseService.Api.Middlewares;

namespace Ozon.MerchandiseService.Api.StartupFilters
{
    public class TerminalStartupFilter: IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Map("/version", builder =>
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    string version = assembly.GetName().Version?.ToString() ?? "no version";
                    builder.UseMiddleware<VersionMiddleware>(version, "Ozon.MerchandiseService.Api");
                });

                app.Map("/ready", builder => builder.UseMiddleware<ReadyMiddleware>());
                app.Map("/live", builder => builder.UseMiddleware<LiveMiddleware>());
                next(app);
            };
        }
    }
}