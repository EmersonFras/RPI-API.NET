using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPI_API.Utils
{
    public class Receiver : IAsyncDisposable
    {

        private readonly Task _initializeTask;
        private readonly ConnectionFactory _factory;
        private readonly string _bindingKey;
        private readonly Func<object, BasicDeliverEventArgs, Task> _handleMessage;
        private string? _queueName;
        IConnection? _connection;
        IChannel? _channel;


        public Receiver(string bindingKey, Func<object, BasicDeliverEventArgs, Task> handleMessage)
        {
            _factory = new ConnectionFactory { HostName = "localhost" };
            _bindingKey = bindingKey;
            _handleMessage = handleMessage;

            _initializeTask = ReceiverAsync();
        }

        public async Task ReceiverAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(exchange: "led_matrix", type: ExchangeType.Topic);

            QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync();
            _queueName = queueDeclareResult.QueueName;
            await _channel.QueueBindAsync(queue: _queueName, exchange: "led_matrix", routingKey: _bindingKey);
        }

        public async Task ReceiveAsync()
        {
            await _initializeTask;

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync +=  (sender, eventArgs) => _handleMessage(sender, eventArgs);

            
            await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();    
        }
    }
}
