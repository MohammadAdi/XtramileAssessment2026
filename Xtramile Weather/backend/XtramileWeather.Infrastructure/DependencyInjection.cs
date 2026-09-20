using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XtramileWeather.Application.Abstractions.Services;
using XtramileWeather.Application.Countries;
using XtramileWeather.Application.WeatherNotes;
using XtramileWeather.Infrastructure.Persistence;
using XtramileWeather.Infrastructure.Persistence.Repositories;
using XtramileWeather.Infrastructure.Services.OpenWeatherMap;

namespace XtramileWeather.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<WeatherDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IWeatherNoteRepository, WeatherNoteRepository>();
        var baseUrl = configuration[$"{OpenWeatherMapOptions.SectionName}:BaseUrl"]
            ?? throw new InvalidOperationException("The OpenWeatherMap base URL is not configured.");
        var weatherOptions = new OpenWeatherMapOptions
        {
            BaseUrl = baseUrl,
            ApiKey = configuration[$"{OpenWeatherMapOptions.SectionName}:ApiKey"] ?? string.Empty
        };
        services.AddSingleton<IOptions<OpenWeatherMapOptions>>(Options.Create(weatherOptions));
        services.AddHttpClient<IWeatherService, OpenWeatherMapWeatherService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
