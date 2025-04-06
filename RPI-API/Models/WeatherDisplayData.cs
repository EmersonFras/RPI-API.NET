namespace RPI_API.Models
{
    public class WeatherDisplayData
    {
        public int Id { get; set; } = 1;
        public string? Text { get; set; }
        public string? StartTime { get; set; }
        public string? StopTime { get; set; }
        public string? Temperature { get; set; }
        public string? WeatherCode { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
