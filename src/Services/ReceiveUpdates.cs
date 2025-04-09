using RPI_API.Utils;

namespace RPI_API.Services
{
    public class ReceiveUpdates : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReceiveUpdates(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting to receive updates...");

            // Pull receiver from scope
            var scope = _scopeFactory.CreateScope();
            var receiver = scope.ServiceProvider.GetRequiredService<Receiver>();

            // Waits to receive messages from exchange
            while (!stoppingToken.IsCancellationRequested)
            {
                try 
                {
                    await receiver.ReceiveAsync(stoppingToken);
                    Console.WriteLine("[Receiving Host] Connected to RabbitMQ, consuming messages.");
                    break;
                } 
                catch
                {
                    Console.WriteLine("[Receiving Host] Error receiving messages. Retrying in 10 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("[Receiving Host] Waiting for updates...");
                try
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException) { /* expected on shutdown */ }
                finally
                {
                    await receiver.DisposeAsync();
                }
            }
        }
    }
}
