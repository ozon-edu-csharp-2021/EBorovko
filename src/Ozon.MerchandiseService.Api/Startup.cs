using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Ozon.MerchandiseService.Api.GrpcServices;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Contracts;
using Ozon.MerchandiseService.Infrastructure.Configuration;
using Ozon.MerchandiseService.Infrastructure.Handlers;
using Ozon.MerchandiseService.Infrastructure.Repositories;
using Ozon.MerchandiseService.Infrastructure.Repositories.Infrastructure;
using Ozon.MerchandiseService.Infrastructure.Services;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;

namespace Ozon.MerchandiseService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddDatabaseComponents(services);
            services.AddMediatR(typeof(ProvideCommandHandler).Assembly);
            
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton<IStockApiService, MockStockApiService>();
            
            services.AddScoped<IMerchandiseProvidingRequestRepository, MerchandiseRequestRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }

        public void AddDatabaseComponents(IServiceCollection services)
        {
            services.Configure<DatabaseConnectionOptions>(Configuration.GetSection(nameof(DatabaseConnectionOptions)));
            services.AddScoped<IDbConnectionFactory<NpgsqlConnection>, NpgsqlConnectionFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IChangeTracker, ChangeTracker>();
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MerchApiGrpcService>();
                endpoints.MapControllers();
            });
        }
    }
}