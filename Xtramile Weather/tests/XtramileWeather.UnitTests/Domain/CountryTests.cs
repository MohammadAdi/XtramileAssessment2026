using XtramileWeather.Domain.Entities;

namespace XtramileWeather.UnitTests.Domain;

public sealed class CountryTests
{
    [Fact]
    public void Constructor_NormalizesCodeAndName()
    {
        var country = new Country("id", " Indonesia ");

        Assert.Equal("ID", country.Code);
        Assert.Equal("Indonesia", country.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("I")]
    [InlineData("IDN")]
    [InlineData("12")]
    public void Constructor_InvalidCode_Throws(string code)
    {
        Assert.Throws<ArgumentException>(() => new Country(code, "Indonesia"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_BlankName_Throws(string name)
    {
        Assert.Throws<ArgumentException>(() => new Country("ID", name));
    }
}
