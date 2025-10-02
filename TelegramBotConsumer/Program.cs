
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TelegramBotPublish.Services.RabbitService;
using WeatherTelegramBot.Data;
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
            

            

            app.Run();
        }

        private static async Task<IResult> AddCityWeather(IWeatherRepo weatherRepo, RabbitPublish rabbitPublish)
        {
            await rabbitPublish.PutMessageAsync();

            return Results.Ok("Работаю");

        }
    }
}
