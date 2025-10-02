using TelegramBotPublish.Services.RabbitService;
using WeatherTelegramBot.DTOs;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot.Data
{
    public interface IWeatherRepo
    {
        Task SaveChanges();

        
        Task<WeatherModel?> GetWeatherModelAsync(string cityName);
        Task<IResult> CreateModel( WeatherModel weatherModel);
        Task<IResult> CreateWetherModelAsync(WeatherModel weatherModel);

        Task<IResult> UpdateWeatherModelAsync(WeatherModel updateWeatherModel, WeatherModel existingWeatherModel);

        Task<IResult> DeleteWeatherModelAsync(string cityName);
    }
}
