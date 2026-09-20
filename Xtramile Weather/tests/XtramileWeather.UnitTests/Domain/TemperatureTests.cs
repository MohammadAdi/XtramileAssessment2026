using XtramileWeather.Domain.ValueObjects;

namespace XtramileWeather.UnitTests.Domain;

public sealed class TemperatureTests
{
    [Theory]
    [InlineData(32, 0)]
    [InlineData(212, 100)]
    [InlineData(-40, -40)]
    [InlineData(-4, -20)]
    public void Celsius_ConvertsKnownFahrenheitValues(double fahrenheit, double expected)
    {
        Assert.Equal(expected, new Temperature(fahrenheit).Celsius, precision: 6);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_RejectsNonFiniteValues(double fahrenheit)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Temperature(fahrenheit));
    }
}
