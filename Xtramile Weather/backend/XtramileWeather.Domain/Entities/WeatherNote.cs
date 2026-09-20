namespace XtramileWeather.Domain.Entities;

public sealed class WeatherNote
{
    public const int MaximumTextLength = 500;

    private WeatherNote()
    {
    }

    public WeatherNote(int cityId, string text, DateTimeOffset createdAtUtc)
    {
        if (cityId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cityId), "City identifier must be positive.");
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Note text is required.", nameof(text));
        }

        var normalizedText = text.Trim();
        if (normalizedText.Length > MaximumTextLength)
        {
            throw new ArgumentException(
                $"Note text must not exceed {MaximumTextLength} characters.",
                nameof(text));
        }

        Id = Guid.NewGuid();
        CityId = cityId;
        Text = normalizedText;
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public Guid Id { get; private set; }

    public int CityId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }
}
