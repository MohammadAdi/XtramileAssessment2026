using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence.Configurations;

public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(city => city.Id);

        builder.Property(city => city.Id).ValueGeneratedOnAdd();
        builder.Property(city => city.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(city => city.CountryCode)
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();
        builder.Property(city => city.Latitude).HasPrecision(9, 6);
        builder.Property(city => city.Longitude).HasPrecision(9, 6);

        builder.HasIndex(city => new { city.CountryCode, city.Name }).IsUnique();
        builder.HasOne<Country>()
            .WithMany()
            .HasForeignKey(city => city.CountryCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new { Id = 1, Name = "Sydney", CountryCode = "AU", Latitude = -33.8688m, Longitude = 151.2093m },
            new { Id = 2, Name = "Melbourne", CountryCode = "AU", Latitude = -37.8136m, Longitude = 144.9631m },
            new { Id = 3, Name = "Brisbane", CountryCode = "AU", Latitude = -27.4698m, Longitude = 153.0251m },
            new { Id = 4, Name = "Jakarta", CountryCode = "ID", Latitude = -6.2088m, Longitude = 106.8456m },
            new { Id = 5, Name = "Surabaya", CountryCode = "ID", Latitude = -7.2575m, Longitude = 112.7521m },
            new { Id = 6, Name = "Bandung", CountryCode = "ID", Latitude = -6.9175m, Longitude = 107.6191m },
            new { Id = 7, Name = "Kuala Lumpur", CountryCode = "MY", Latitude = 3.1390m, Longitude = 101.6869m },
            new { Id = 8, Name = "George Town", CountryCode = "MY", Latitude = 5.4141m, Longitude = 100.3288m },
            new { Id = 9, Name = "Johor Bahru", CountryCode = "MY", Latitude = 1.4927m, Longitude = 103.7414m },
            new { Id = 10, Name = "Singapore", CountryCode = "SG", Latitude = 1.3521m, Longitude = 103.8198m },
            new { Id = 11, Name = "New York", CountryCode = "US", Latitude = 40.7128m, Longitude = -74.0060m },
            new { Id = 12, Name = "Los Angeles", CountryCode = "US", Latitude = 34.0522m, Longitude = -118.2437m },
            new { Id = 13, Name = "Chicago", CountryCode = "US", Latitude = 41.8781m, Longitude = -87.6298m });
    }
}
