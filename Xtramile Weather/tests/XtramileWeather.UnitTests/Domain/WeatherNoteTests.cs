using XtramileWeather.Domain.Entities;

namespace XtramileWeather.UnitTests.Domain;

public sealed class WeatherNoteTests
{
    [Fact]
    public void Constructor_NormalizesValidNote()
    {
        var localTimestamp = new DateTimeOffset(2026, 9, 20, 12, 0, 0, TimeSpan.FromHours(7));

        var note = new WeatherNote(4, "  Heavy rain after lunch.  ", localTimestamp);

        Assert.NotEqual(Guid.Empty, note.Id);
        Assert.Equal(4, note.CityId);
        Assert.Equal("Heavy rain after lunch.", note.Text);
        Assert.Equal(localTimestamp.ToUniversalTime(), note.CreatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsBlankText(string text)
    {
        Assert.Throws<ArgumentException>(() => new WeatherNote(4, text, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_RejectsTextOverMaximumLength()
    {
        var text = new string('a', WeatherNote.MaximumTextLength + 1);

        Assert.Throws<ArgumentException>(() => new WeatherNote(4, text, DateTimeOffset.UtcNow));
    }
}
