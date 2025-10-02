using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TelegramBotConsumer.DTOs
{
    public class ReadStaticticModel
    {
        
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("requestCount")]
        public int RequestCount { get; set; }  // Как часто запрашивали город

        [JsonPropertyName("averageTemperature")]
        public double AverageTemperature { get; set; }
    }
}
