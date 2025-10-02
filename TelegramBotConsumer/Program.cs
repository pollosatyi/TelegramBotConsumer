
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TelegramBotConsumer.DTOs;
using TelegramBotPublish.Services.RabbitService;
using WeatherTelegramBot.Data;
using WeatherTelegramBot.DTOs;
using WeatherTelegramBot.Profiles;

namespace TelegramBotConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(WeatherProfile));
            builder.Services.AddScoped<IWeatherRepo, WeatherRepo>();
            builder.Services.AddScoped<RabbitPublish>();
            var connectionString = builder.Configuration.GetConnectionString("PostgresDbConnection");

            var postgresConnectionString = new NpgsqlConnectionStringBuilder(connectionString);

            postgresConnectionString.Password = builder.Configuration["Password"];
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(postgresConnectionString.ToString()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapPost("api/v1/weatherFromOpenWeatherMap/post", AddCityWeather);

            app.MapGet("api/v1/weatherStatistic/get{city}", GetStatisticsWeatherOfCity).;
            

            

            app.Run();
        }

        private static async Task<IResult> GetStatisticsWeatherOfCity(IWeatherRepo weatherepo, string cityName,IMapper mapper)
        {
           
            try
            {
                var weatherModel = await weatherepo.GetWeatherModelAsync(cityName);

                if (weatherModel == null) return Results.NotFound($"Погода для города '{cityName}' не найдена");

                return Results.Ok(mapper.Map<ReadStaticticModel>(weatherModel));

            }
            catch (Exception ex)
            {

                return Results.Problem($"Ошибка: {ex.Message}");
            }
        }

        private static async Task<IResult> AddCityWeather(IWeatherRepo weatherRepo, RabbitPublish rabbitPublish)
        {
            await rabbitPublish.PutMessageAsync();

            return Results.Ok("Запись в базу сделана");

        }
    }
}
