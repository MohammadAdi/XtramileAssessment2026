using NSubstitute;
using XtramileWeather.Application.Countries;
using XtramileWeather.Application.Features.WeatherNotes.CreateWeatherNote;
using XtramileWeather.Application.WeatherNotes;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.UnitTests.Features.WeatherNotes;

public sealed class CreateWeatherNoteCommandHandlerTests
{
    [Fact]
    public async Task Handle_KnownCity_PersistsNoteAndReturnsIdentifier()
    {
        var cityRepository = Substitute.For<ICityRepository>();
        var noteRepository = Substitute.For<IWeatherNoteRepository>();
        cityRepository.ExistsAsync(4, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CreateWeatherNoteCommandHandler(cityRepository, noteRepository);

        var response = await handler.Handle(
            new CreateWeatherNoteCommand(4, "  Rain expected.  "),
            CancellationToken.None);

        Assert.NotNull(response);
        await noteRepository.Received(1).AddAsync(
            Arg.Is<WeatherNote>(note => note.Id == response.Id
                && note.CityId == 4
                && note.Text == "Rain expected."),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownCity_DoesNotPersistNote()
    {
        var cityRepository = Substitute.For<ICityRepository>();
        var noteRepository = Substitute.For<IWeatherNoteRepository>();
        cityRepository.ExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CreateWeatherNoteCommandHandler(cityRepository, noteRepository);

        var response = await handler.Handle(
            new CreateWeatherNoteCommand(999, "Rain expected."),
            CancellationToken.None);

        Assert.Null(response);
        await noteRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }
}
