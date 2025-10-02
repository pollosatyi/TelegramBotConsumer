using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TelegramBotPublish.Services.RabbitService;
using WeatherTelegramBot.DTOs;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot.Data
{
    public class WeatherRepo : IWeatherRepo
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;

        public WeatherRepo(AppDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }
        public async Task<IResult> CreateWetherModelAsync(WeatherModel weatherModel)
        {
            await _context.WeatherModels.AddAsync(weatherModel);
            await SaveChanges();
            return Results.Ok(_mapper.Map<ReadModelDto>(weatherModel));
        }

        public async Task<IResult> DeleteWeatherModelAsync(string cityName)
        {
            var weatherModel = await GetWeatherModelAsync(cityName.ToLower());
            if (weatherModel != null)
            {
                _context.WeatherModels.Remove(weatherModel);
                _context.SaveChanges();
                return Results.Ok();
            }
            return Results.NotFound($"Такого города {char.ToUpper(cityName[0]) + cityName.Substring(1)} нет в базе");
        }

        public async Task<WeatherModel?> GetWeatherModelAsync(string cityName)
        {
            try
            {
                var weatherModel = await _context.WeatherModels.FirstOrDefaultAsync(x => x.City == cityName);
                return weatherModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();

        }

        public async Task<IResult> UpdateWeatherModelAsync(WeatherModel updateModel, WeatherModel existingModel)
        {
            try
            {
                existingModel.Temperature = updateModel.Temperature;
                existingModel.Description = updateModel.Description;
                existingModel.WindSpeed = updateModel.WindSpeed;
                existingModel.RequestCount++;
                double newAverangeTemperature = await AverangeTemperature(existingModel.Temperature, updateModel.Temperature, existingModel.RequestCount);
                existingModel.AverageTemperature = newAverangeTemperature;
                await _context.SaveChangesAsync();
                return Results.Ok(_mapper.Map<ReadModelDto>(existingModel));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);

            }
        }

        public async Task<IResult> CreateModel( WeatherModel weatherModel)
        {
            var findCityFromDb = await GetWeatherModelAsync(weatherModel.City);
            if (findCityFromDb == null)
            {
                return await CreateWetherModelAsync(weatherModel);
            }
            else
            {
                return await UpdateWeatherModelAsync(weatherModel, findCityFromDb);
            }
        }

        private Task<double> AverangeTemperature(double temperatureExists, double temperatureUpdate, int requestCount)
        {
            var averange = Math.Round((temperatureExists * (requestCount - 1) + temperatureUpdate) / requestCount, 2);
            return Task.FromResult(averange);
        }

        
    }
}
