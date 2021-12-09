using System;
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
    public class EmployeeConsumerHostedService: BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmployeeConsumerHostedService> _logger;

        protected string Topic { get; set; } = "employee_notification_event";
        
        public EmployeeConsumerHostedService(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<EmployeeConsumerHostedService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId =  _configuration["KafkaOptions:EmployeeGroupId"],
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
                            var message = JsonSerializer.Deserialize<NotificationEvent>(cr.Message.Value);
                            var payload = (MerchDeliveryEventPayload) message?.Payload;
                            await mediator.Send(new ProvideCommand
                            {
                                EmployeeEmail = message.EmployeeEmail,
                                MerchPackId =  (int)payload.MerchType,
                                ClothingSize = (int)payload.ClothingSize,
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