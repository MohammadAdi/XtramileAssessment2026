using Microsoft.EntityFrameworkCore;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence;

public sealed class WeatherDbContext(DbContextOptions<WeatherDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();

    public DbSet<City> Cities => Set<City>();

    public DbSet<WeatherNote> WeatherNotes => Set<WeatherNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherDbContext).Assembly);
}
