using Microsoft.EntityFrameworkCore;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<WeatherModel> WeatherModels { get; set; }
    }
}
