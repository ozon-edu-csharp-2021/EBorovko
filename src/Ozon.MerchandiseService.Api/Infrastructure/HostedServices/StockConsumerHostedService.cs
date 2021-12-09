using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ozon.MerchandiseService.Infrastructure.Commands;

namespace Ozon.MerchandiseService.Api.Infrastructure.HostedServices
{
    public class StockConsumerHostedService: BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockConsumerHostedService> _logger;

        protected string Topic { get; set; } = "stock_replenished_event";
        
        public StockConsumerHostedService(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<StockConsumerHostedService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId =  _configuration["KafkaOptions:StockGroupId"],
                BootstrapServers = _configuration["KafkaOptions:BootstrapServers"],
            };

            using var c = new ConsumerBuilder<Ignore, string>(config).Build();
            c.Subscribe(Topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _scopeFactory.CreateScope();
                    try
                    {
                        await Task.Yield();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var cr = c.Consume(stoppingToken);
                        if (cr != null)
                        {
                            var message = JsonSerializer.Deserialize<SupplyShippedEvent>(cr.Message.Value);
                            await mediator.Send(new ReplenishStockCommand()
                            {
                                SupplyId = message.SupplyId,
                                Items = message.Items.Select(it => new StockItemQuantityDto()
                                {
                                    Quantity = (int)it.Quantity,
                                    Sku = it.SkuId
                                }).ToArray()
                            }, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error while get consume. Message {ex.Message}");
                    }
                }
            }
            finally
            {
                c.Commit();
                c.Close();
            }
        }
    }
}