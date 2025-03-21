using RabbitMQ.Client.Events;
using System.Text;

namespace RPI_API.Utils
{
    public class MessageHandler
    {

        public async Task HandleWeatherMessage(object sender, BasicDeliverEventArgs eventArgs)
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            Console.WriteLine($"[Weather] Received message: {message}");
            await Task.CompletedTask;
        }
    }
}
