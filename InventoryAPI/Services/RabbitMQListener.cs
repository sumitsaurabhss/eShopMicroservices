using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventoryAPI.Services
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly ILogger<RabbitMQListener> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private string exchangeName = "PlaceOrder";
        private string routingKey = "order-info-key";
        private string queueName = "OrderInfo";

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
                var _dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                var _messageProducer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();
                _logger.LogInformation($"\n\n\n\n\n\n\n\n\n\n\nConsumer Received - {content}\n\n\n\n\n\n\n\n\n\n\n");
                OrderDto order = JsonSerializer.Deserialize<OrderDto>(content);
                foreach (var item in order.OrderDetails)
                {
                    var inventory = await _dbContext.Inventories.FirstOrDefaultAsync(inventory => inventory.ProductCode == item.ProductCode);
                    if (inventory == null)
                    {
                        item.Status = "Declined";
                    }
                    else if (inventory.Quantity >= item.Quantity)
                    {
                        inventory.Quantity -= item.Quantity;
                        _dbContext.Inventories.Update(inventory);
                        if (inventory.Quantity == 0)
                            _messageProducer.SendMessage(inventory, "StockInformation", "stock-routing-key", "StockQueue");

                        item.Status = "Ready";
                    }
                    else item.Status = "Declined";
                }
                await _dbContext.SaveChangesAsync();
                _messageProducer.SendMessage(order, "PlaceOrder", "order-status-key", "OrderStatus");
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
