namespace RPI_API.Models
{
    public class DisplayData
    {
        public int Id { get; set; } = 1;
        public string? StartTime { get; set; }
        public string? StopTime { get; set; }
        public string? Brightness { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}