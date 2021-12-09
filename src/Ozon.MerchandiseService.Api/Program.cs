using Microsoft.Extensions.Hosting;
using Ozon.MerchandiseService.Api.Infrastructure.Extensions;
using Serilog;


namespace Ozon.MerchandiseService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) => configuration
                    .ReadFrom
                    .Configuration(context.Configuration)
                    .WriteTo.Console())
                .ConfigurePorts()
                .AddInfrastructure();
    }
}