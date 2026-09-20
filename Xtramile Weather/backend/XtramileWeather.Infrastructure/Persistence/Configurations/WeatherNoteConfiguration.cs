using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence.Configurations;

public sealed class WeatherNoteConfiguration : IEntityTypeConfiguration<WeatherNote>
{
    public void Configure(EntityTypeBuilder<WeatherNote> builder)
    {
        builder.ToTable("WeatherNotes");
        builder.HasKey(weatherNote => weatherNote.Id);
        builder.Property(weatherNote => weatherNote.Id).ValueGeneratedNever();
        builder.Property(weatherNote => weatherNote.Text)
            .HasMaxLength(WeatherNote.MaximumTextLength)
            .IsRequired();
        builder.Property(weatherNote => weatherNote.CreatedAtUtc).IsRequired();
        builder.HasIndex(weatherNote => weatherNote.CityId);
        builder.HasOne<City>()
            .WithMany()
            .HasForeignKey(weatherNote => weatherNote.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
