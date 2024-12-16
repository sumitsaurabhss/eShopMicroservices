using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OrderAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OrderAPI.Services
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly ILogger<RabbitMQListener> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMongoCollection<Order> _orderCollection;
        private IConnection _connection;
        private IModel _channel;
        private string exchangeName = "PlaceOrder";
        private string routingKey = "order-status-key";
        private string queueName = "OrderStatus";

        public RabbitMQListener(ILogger<RabbitMQListener> logger, IServiceScopeFactory serviceScopeFactory, IMongoDatabase database)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _orderCollection = database.GetCollection<Order>("order");
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
                new AmqpTcpEndpoint("localhost"),
                new AmqpTcpEndpoint("rabbit-server")
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
            // we just print this message   
            _logger.LogInformation($"\n\n\n\n\n\n\n\nConsumer Received - {content}\n\n\n\n\n\n\n\n\n\n\n");
            Order order = JsonSerializer.Deserialize<Order>(content);
            var filterDefinition = Builders<Order>.Filter.Eq(x => x.Id, order.Id);
            await _orderCollection.ReplaceOneAsync(filterDefinition, order);
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
