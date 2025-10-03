
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using StackExchange.Redis;
using System.Text.Json;
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
            builder.Services.AddOutputCache();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
               // options.InstanceName = "WeatherAPI";

            });

            builder.Services.AddScoped<IDatabase>(sp =>
            {
                var connection = sp.GetRequiredService<IConnectionMultiplexer>();
                return connection.GetDatabase();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseOutputCache();

            app.MapPost("api/v1/weatherFromOpenWeatherMap/post", AddCityWeather);

            app.MapGet("api/v1/weatherStatistic/get{city}", GetStatisticsWeatherOfCity).WithName("GetWeatherStatistics");
            

            

            app.Run();
        }

        private static async Task<IResult> GetStatisticsWeatherOfCity(IWeatherRepo weatherepo, string cityName,IMapper mapper, IDistributedCache cache)
        {
           
            try
            {
                var cacheKey = $"weather_{cityName.ToLower()}";
                var cachedData = await cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    var cachedResult = JsonSerializer.Deserialize<ReadStaticticModel>(cachedData);
                    return Results.Ok(cachedResult);
                }
                var weatherModel = await weatherepo.GetWeatherModelAsync(cityName.ToLower());

                if (weatherModel == null) return Results.NotFound($"Погода для города '{cityName}' не найдена");

                var result = mapper.Map<ReadStaticticModel>(weatherModel);

                var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(45));

                var serializedResult=JsonSerializer.Serialize(result);

                await cache.SetStringAsync(cacheKey, serializedResult, cacheOptions);

                return Results.Ok(result);

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
