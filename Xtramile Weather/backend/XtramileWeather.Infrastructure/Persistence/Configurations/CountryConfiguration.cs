using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence.Configurations;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");
        builder.HasKey(country => country.Code);

        builder.Property(country => country.Code)
            .HasMaxLength(2)
            .IsFixedLength()
            .ValueGeneratedNever();
        builder.Property(country => country.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasData(
            new { Code = "AU", Name = "Australia" },
            new { Code = "ID", Name = "Indonesia" },
            new { Code = "MY", Name = "Malaysia" },
            new { Code = "SG", Name = "Singapore" },
            new { Code = "US", Name = "United States" });
    }
}
