using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Ozon.MerchandiseService.Api.Extensions;


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
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .AddInfrastructure();
    }
}