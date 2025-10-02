using System.Text.Json.Serialization;

namespace WeatherTelegramBot.DTOs
{

    public class CreateModelDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("main")]
        public MainDataDto Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public WeatherDescriptionDto[] Weather { get; set; } = Array.Empty<WeatherDescriptionDto>();

        [JsonPropertyName("wind")]
        public WindDataDto Wind { get; set; } = new();
    }

    public class MainDataDto
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class WeatherDescriptionDto
    {
        [JsonPropertyName("main")]
        public string Main { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;
    }

    public class WindDataDto
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Degree { get; set; }
    }
}

