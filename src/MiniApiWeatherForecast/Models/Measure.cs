using Newtonsoft.Json;

namespace MiniApiWeatherForecast.Models
{
    public class Measure
    {
        [JsonProperty("@id")]
        public string @Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public LatestReading LatestReading { get; set; } = default!;
        public string Notation { get; set; } = string.Empty;
        public string Parameter { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public int Period { get; set; }
        public string Qualifier { get; set; } = string.Empty;
        public string Station { get; set; } = string.Empty;
        public string StationReference { get; set; } = string.Empty;
        public List<string> Type { get; set; } = default!;
        public string Unit { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ValueType { get; set; } = string.Empty;
    }
}
