using System.Text.Json.Serialization;

namespace RPI_API.DTOs
{
    public class DisplayDataDto
    {
        [JsonPropertyName("start_time")]
        public string? StartTime { get; set; }

        [JsonPropertyName("stop_time")]
        public string? StopTime { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        public override string ToString()
        {
            return $"StartTime: {StartTime}, StopTime: {StopTime}, Text: \"{Text}\"";
        }
    }

}
