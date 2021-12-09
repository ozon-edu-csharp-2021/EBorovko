using System;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTracing;
using Ozon.MerchandiseService.Api.GrpcServices;
using Ozon.MerchandiseService.Api.Infrastructure.HostedServices;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Contracts;
using Ozon.MerchandiseService.Infrastructure.Configuration;
using Ozon.MerchandiseService.Infrastructure.Handlers;
using Ozon.MerchandiseService.Infrastructure.MessageBrocker;
using Ozon.MerchandiseService.Infrastructure.Repositories;
using Ozon.MerchandiseService.Infrastructure.Repositories.Infrastructure;
using Ozon.MerchandiseService.Infrastructure.Services;
using Ozon.MerchandiseService.Infrastructure.Services.Interfaces;
using Ozon.MerchandiseService.Infrastructure.Services.StockApiService;
using OzonEdu.StockApi.Grpc;

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
            AddHostedServices(services);
            AddDatabaseComponents(services);
            AddRepositories(services);
            AddApplicationServices(services);
            AddKafkaServices(services);
            
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = Configuration["RedisConnectionOptions:ConnectionString"]);

            services.AddSingleton<ITracer>(sp =>
            {
                var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var reporter = new RemoteReporter.Builder().WithLoggerFactory(loggerFactory).WithSender(new UdpSender())
                    .Build();
                var tracer = new Tracer.Builder(serviceName)
                    // The constant sampler reports every span.
                    .WithSampler(new ConstSampler(true))
                    // LoggingReporter prints every reported span to the logging framework.
                    .WithReporter(reporter)
                    .Build();
                return tracer;
            });
            
            
        }

        private void AddRepositories(IServiceCollection services)
        {
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
        
        public void AddKafkaServices(IServiceCollection services)
        {
            services.AddScoped<IProducerBuilderWrapper, ProducerBuilderWrapper>();
        }

        public void AddHostedServices(IServiceCollection services)
        {
            services.AddHostedService<StockConsumerHostedService>();
            services.AddHostedService<EmployeeConsumerHostedService>();
        }

        public void AddApplicationServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(ProvideCommandHandler).Assembly);
            
            services.AddGrpcClient<StockApiGrpc.StockApiGrpcClient>(options =>
            {
                options.Address = new Uri(Configuration["StockApiUri"]);
            });
            services.AddSingleton<IStockApiService, StockApiService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();
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