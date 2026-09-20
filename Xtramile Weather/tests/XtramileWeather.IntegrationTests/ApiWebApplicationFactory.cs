using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using XtramileWeather.Infrastructure.Persistence;

namespace XtramileWeather.IntegrationTests;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<WeatherDbContext>>();
            services.RemoveAll<WeatherDbContext>();
            services.AddSingleton(_ =>
            {
                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open();
                return connection;
            });
            services.AddDbContext<WeatherDbContext>((serviceProvider, options) =>
                options.UseSqlite(serviceProvider.GetRequiredService<SqliteConnection>()));
        });
    }
}
