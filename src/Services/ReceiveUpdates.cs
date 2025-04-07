using RPI_API.Utils;

namespace RPI_API.Services
{
    public class ReceiveUpdates : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Receiver? _receiver;

        public ReceiveUpdates(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting to receive updates...");

            // Pull receiver from scope
            var scope = _scopeFactory.CreateScope();
            _receiver = scope.ServiceProvider.GetRequiredService<Receiver>();

            // Waits to receive messages from exchange
            await _receiver.ReceiveAsync();
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for updates...");
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
            await _receiver.DisposeAsync();

        }
    }
}
