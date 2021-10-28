using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ozon.MerchandiseService.Api.Filters;
using Ozon.MerchandiseService.Api.Interceptors;
using Ozon.MerchandiseService.Api.StartupFilters;


namespace Ozon.MerchandiseService.Api.Extensions
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder AddInfrastructure(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((_, services) =>
            {
                services.AddSingleton<IStartupFilter, TerminalStartupFilter>();
                services.AddSingleton<IStartupFilter, LoggingStartupFilter>();
                
                services.AddSingleton<IStartupFilter, SwaggerStartupFilter>();
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Ozon.MerchandiseService.Api", Version = "v1"});
                    options.CustomSchemaIds(x => x.FullName);

                    var xmlFileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
                    options.IncludeXmlComments(xmlFilePath);
                });
                
                services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());
                services.AddGrpc(options => options.Interceptors.Add<LoggingInterceptor>());
            });
            
            return hostBuilder;
        }
        
    }
}