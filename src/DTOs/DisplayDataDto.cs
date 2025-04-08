using Newtonsoft.Json;

namespace RPI_API.DTOs
{
    public class DisplayDataDto
    {
        [JsonProperty("start_time")]
        public string? StartTime { get; set; }

        [JsonProperty("stop_time")]
        public string? StopTime { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }

        public override string ToString()
        {
            return $"StartTime: {StartTime}, StopTime: {StopTime}, Text: \"{Text}\"";
        }
    }

}
