namespace MiniApiWeatherForecast.Models
{
    public class Meta
    {
        public string Publisher { get; set; } = string.Empty;
        public string Licence { get; set; } = string.Empty;
        public string Documentation { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public List<string> HasFormat { get; set; } = default!;
    }
}
