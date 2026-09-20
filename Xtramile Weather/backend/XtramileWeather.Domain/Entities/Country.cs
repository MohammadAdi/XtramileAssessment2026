namespace XtramileWeather.Domain.Entities;

public sealed class Country
{
    private Country()
    {
    }

    public Country(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 2 || !code.All(char.IsAsciiLetter))
        {
            throw new ArgumentException(
                "Country code must be a two-letter ISO 3166-1 alpha-2 code.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Country name is required.", nameof(name));
        }

        Code = code.ToUpperInvariant();
        Name = name.Trim();
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;
}
