using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace RPI_API.Utils
{
    public class Emitter : IAsyncDisposable
    {
        private readonly Task _initializeTask;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;

        public Emitter()
        {
            string hostname = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost";
            _factory = new ConnectionFactory { HostName = hostname };

            // Initialize Task is to track when emitter is fully setup
            _initializeTask = EmitterAsync();
        }


        private async Task EmitterAsync()
        {
            _connection = await _factory.CreateConnectionAsync();

            // Sets up to give publisher confirms
            var channelOpts = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true,
                outstandingPublisherConfirmationsRateLimiter: new ThrottlingRateLimiter(5) // 5 was randomly decided
            );

            _channel = await _connection.CreateChannelAsync(channelOpts) ;

            await _channel.ExchangeDeclareAsync(exchange: "led_matrix", type: ExchangeType.Topic);
        }

        public async Task<bool> EmitAsync(string message, string topic)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Channel is not initialized.");
            }

            // Waits for emitter to be fully init
            await _initializeTask;

            var body = Encoding.UTF8.GetBytes(message);

            try
            {
                await _channel.BasicPublishAsync(exchange: "led_matrix",
                                         routingKey: topic,
                                         body: body);
                return true;
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"[Emitter] Saw nack or return: {ex.Message}");
                return false;
            }

        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
        }
    }
}