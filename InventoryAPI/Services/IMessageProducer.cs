namespace InventoryAPI.Services
{
    public interface IMessageProducer
    {
        public void SendMessage<T> (T message, string exchangeName, string routingKey, string queueName);
    }
}
