namespace XtramileWeather.Domain.ValueObjects;

/// <summary>Represents a finite Fahrenheit temperature and its Celsius equivalent.</summary>
public readonly record struct Temperature
{
    public double Fahrenheit { get; }
    public double Celsius => (Fahrenheit - 32d) * 5d / 9d;

    public Temperature(double fahrenheit)
    {
        if (!double.IsFinite(fahrenheit))
        {
            throw new ArgumentOutOfRangeException(nameof(fahrenheit), "Temperature must be finite.");
        }

        Fahrenheit = fahrenheit;
    }
}
