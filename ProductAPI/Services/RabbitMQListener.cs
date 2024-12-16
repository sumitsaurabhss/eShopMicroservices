using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProductAPI.Services
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly ILogger<RabbitMQListener> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private string exchangeName = "StockInformation";
        private string routingKey = "stock-routing-key";
        private string queueName = "StockQueue";

        public RabbitMQListener(ILogger<RabbitMQListener> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            // create connection  
            var factory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };
            var endpoints = new System.Collections.Generic.List<AmqpTcpEndpoint>
            {
                new AmqpTcpEndpoint("rabbit-server"),
                new AmqpTcpEndpoint("localhost")
            };
            factory.ClientProvidedName = "Inventory";
            _connection = factory.CreateConnection(endpoints);

            // create channel  
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false);
            _channel.QueueBind(queueName, exchangeName, routingKey);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, EventArgs) =>
            {
                var body = EventArgs.Body.ToArray();
                var content = Encoding.UTF8.GetString(body);
                await HandleMessage(content);
                _channel.BasicAck(EventArgs.DeliveryTag, multiple: false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queueName, false, consumer);

            await Task.CompletedTask;
        }

        public async Task HandleMessage(string content)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                // we just print this message   
                _logger.LogInformation($"Consumer Received - {content}");
                if (content == null)
                {
                    throw new ArgumentNullException("Inventory information.");
                }
                InventoryDto inventory = JsonSerializer.Deserialize<InventoryDto>(content);
                var product = await dbContext.Products.FirstOrDefaultAsync(product => product.ProductCode == inventory.ProductCode);
                if (product != null)
                {
                    product.Stock = inventory.Quantity > 0;
                    dbContext.Products.Update(product);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
