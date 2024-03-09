using Newtonsoft.Json;

namespace MiniApiWeatherForecast.Models
{
    public class Items
    {
        [JsonProperty("@id")]
        public string @Id { get; set; } = string.Empty;
        public string EaRegionName { get; set; } = string.Empty;
        public int Easting { get; set; }
        public string GridReference { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Long { get; set; }
        public LatestReading LatestReading { get; set; } = default!;
        public List<Measure> Measures { get; set; } = default!;
        public int Northing { get; set; }
        public string Notation { get; set; } = string.Empty;
        public string StationReference { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
