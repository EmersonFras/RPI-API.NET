using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;
using RPI_API.Data;
using RPI_API.Models;
using System.Text;

namespace RPI_API.Utils
{
    public class MessageHandler
    {

        private readonly WeatherDisplayContext _context;

        public MessageHandler(WeatherDisplayContext context)
        {
            _context = context;
        }


        public async Task HandleWeatherMessage(object sender, BasicDeliverEventArgs eventArgs)
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            Console.WriteLine($"[Weather] Received message: {message}");

            var entry = await _context.WeatherDisplayData.FirstOrDefaultAsync();

            if (entry != null)
            {
                // Message format is a number. Identify what type of message and then save in DB.
                if (eventArgs.RoutingKey == "update.weather")
                {
                    entry.Temperature = message;
                } 
                else if (eventArgs.RoutingKey == "update.weatercode")
                {
                    entry.WeatherCode = message;
                }

                entry.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                Console.WriteLine($"[Weather] Database updated successfully");
            }
            else
            {
                Console.WriteLine($"[Weather] No entry found in the database");
            }
        }
    }
}
