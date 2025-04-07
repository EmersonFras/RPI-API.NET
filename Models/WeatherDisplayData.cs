namespace RPI_API.Models
{
    /*
        TODO: This works fine now but would be better to split
        start/stop time into a display data table seperate since
        it is universal to all displays. 
    */
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
