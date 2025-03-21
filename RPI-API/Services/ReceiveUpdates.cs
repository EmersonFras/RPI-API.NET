using RPI_API.Utils;

namespace RPI_API.Services
{
    public class ReceiveUpdates : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Receiver _receiver;

        public ReceiveUpdates(IServiceScopeFactory scopeFactory, Receiver receiver)
        {
            _scopeFactory = scopeFactory;
            _receiver = receiver;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting to receive updates...");
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
