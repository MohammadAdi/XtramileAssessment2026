using XtramileWeather.Domain.Entities;

namespace XtramileWeather.UnitTests.Domain;

public sealed class CityTests
{
    [Fact]
    public void Constructor_ValidValues_ProtectsNormalizedState()
    {
        var city = new City(" Jakarta ", "id", -6.2088m, 106.8456m);

        Assert.Equal("Jakarta", city.Name);
        Assert.Equal("ID", city.CountryCode);
        Assert.Equal(-6.2088m, city.Latitude);
        Assert.Equal(106.8456m, city.Longitude);
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(90)]
    public void Constructor_LatitudeBoundary_IsValid(int latitude)
    {
        var city = new City("Test City", "ID", latitude, 0m);

        Assert.Equal(latitude, city.Latitude);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Constructor_InvalidLatitude_Throws(int latitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new City("Test City", "ID", latitude, 0m));
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Constructor_InvalidLongitude_Throws(int longitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new City("Test City", "ID", 0m, longitude));
    }
}
