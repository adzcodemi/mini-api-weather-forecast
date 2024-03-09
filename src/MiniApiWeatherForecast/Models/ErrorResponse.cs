namespace MiniApiWeatherForecast.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<ErrorDetail> Detail { get; set; } = default!;
    }
}
