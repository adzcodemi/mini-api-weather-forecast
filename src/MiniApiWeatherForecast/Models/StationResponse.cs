namespace MiniApiWeatherForecast.Models
{
    public class StationResponse
    {
        public string Context { get; set; } = string.Empty;
        public Meta Meta { get; set; } = default!;
        public Items Items { get; set; } = default!;
    }
}
