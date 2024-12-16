using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderAPI.Services
{
    public class MessageProducer : IMessageProducer
    {
        public void SendMessage<T>(T message, string exchangeName, string routingKey, string queueName)
        {
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
            factory.ClientProvidedName = "Order";
            IConnection connection = factory.CreateConnection(endpoints);

            using IModel channel = connection.CreateModel();
            //string exchangeName = "PlaceOrder";
            //string routingKey = "order-info-key";
            //string queueName = "OrderInfo";
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false);
            channel.QueueBind(queueName, exchangeName, routingKey);

            string jsonString = JsonSerializer.Serialize(message);
            byte[] messageBody = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish(exchangeName, routingKey, basicProperties: null, messageBody);
            channel.Close();
        }
    }
}
