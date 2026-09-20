namespace XtramileWeather.Domain.Entities;

public sealed class City
{
    private City()
    {
    }

    public City(string name, string countryCode, decimal latitude, decimal longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("City name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(countryCode)
            || countryCode.Length != 2
            || !countryCode.All(char.IsAsciiLetter))
        {
            throw new ArgumentException(
                "Country code must be a two-letter ISO 3166-1 alpha-2 code.",
                nameof(countryCode));
        }

        if (latitude is < -90m or > 90m)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        }

        if (longitude is < -180m or > 180m)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
        }

        Name = name.Trim();
        CountryCode = countryCode.ToUpperInvariant();
        Latitude = latitude;
        Longitude = longitude;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string CountryCode { get; private set; } = string.Empty;

    public decimal Latitude { get; private set; }

    public decimal Longitude { get; private set; }
}
