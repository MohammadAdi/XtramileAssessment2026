using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XtramileWeather.Infrastructure.Persistence;

namespace XtramileWeather.Infrastructure;

public static class DatabaseInitialization
{
    public static async Task ApplyDatabaseMigrationsAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
