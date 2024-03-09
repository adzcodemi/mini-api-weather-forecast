using Newtonsoft.Json;

namespace MiniApiWeatherForecast.Models
{
    public class LatestReading
    {
        [JsonProperty("@id")]
        public string @Id { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string DateTime { get; set; } = string.Empty;
        public string Measure { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
