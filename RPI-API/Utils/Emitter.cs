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
            _factory = new ConnectionFactory { HostName = "localhost" };
            _initializeTask = EmitterAsync();
        }


        private async Task EmitterAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "display_update", type: ExchangeType.Topic);
        }

        public async Task EmitAsync(string message, string topic)
        {
            await _initializeTask;

            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "display_update",
                                     routingKey: topic,
                                     body: body);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
        }
    }
}