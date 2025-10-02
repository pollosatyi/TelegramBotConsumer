namespace WeatherTelegramBot.DTOs
{
    public class ReadModelDto
    {
        public string City { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; }
        public double WindSpeed { get; set; }
    }
}
