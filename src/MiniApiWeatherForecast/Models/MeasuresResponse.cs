namespace MiniApiWeatherForecast.Models
{
    public class MeasuresResponse

    { 
        public string Context { get; set; } = string.Empty;
        public Meta Meta { get; set; } = default!;
        public List<Items> Items { get; set; } = default!;
    }
}
