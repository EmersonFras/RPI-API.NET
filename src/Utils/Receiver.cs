using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPI_API.Utils
{
    public class Receiver : IAsyncDisposable
    {

        // private readonly Task _initializeTask;
        private readonly ConnectionFactory _factory;
        private readonly string _bindingKey;
        private readonly Func<object, BasicDeliverEventArgs, Task> _handleMessage;

        private string? _queueName;
        IConnection? _connection;
        IChannel? _channel;


        public Receiver(string bindingKey, Func<object, BasicDeliverEventArgs, Task> handleMessage)
        {
            string hostname = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost";
            _factory = new ConnectionFactory { 
                HostName = hostname, 
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // Retries automatically every 10 seconds
            };
            _bindingKey = bindingKey;
            _handleMessage = handleMessage;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try 
                {
                    _connection = await _factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                    await _channel.ExchangeDeclareAsync(exchange: "led_matrix", type: ExchangeType.Topic);

                    QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync();
                    _queueName = queueDeclareResult.QueueName;
                    await _channel.QueueBindAsync(queue: _queueName, exchange: "led_matrix", routingKey: _bindingKey);
                    break;
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("[Receiver] Error Initializing RabbitMQ. Retrying in 30 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
            }
        }

        public async Task ReceiveAsync(CancellationToken cancellationToken = default)
        {
            // Waits for Receiver to be init
            await InitializeAsync(cancellationToken);

            if (_channel is null)
                throw new InvalidOperationException("Channel was not initialized.");
            if (_queueName is null)
                throw new InvalidOperationException("Queue name was not initialized.");
                
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
