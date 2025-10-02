using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WeatherTelegramBot.Models
{
    public class WeatherModel
    {
        [Key]
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("windSpeed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("requestCount")]
        public int RequestCount { get; set; }  // Как часто запрашивали город

        [JsonPropertyName("averageTemperature")]
        public double AverageTemperature { get; set; } // Средняя температура
    }
}
