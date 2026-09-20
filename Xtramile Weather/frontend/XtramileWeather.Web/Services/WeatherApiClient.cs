using System.Net.Http.Json;
using System.Text.Json;
using XtramileWeather.Web.Api.Contracts;
using XtramileWeather.Web.Models;

namespace XtramileWeather.Web.Services;

public sealed class WeatherApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<SystemStatusResponse> GetStatusAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<SystemStatusResponse>("api/status", JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty status response.");

    public async Task<IReadOnlyList<CountryResponse>> GetCountriesAsync(
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<IReadOnlyList<CountryResponse>>(
                "api/countries",
                JsonOptions,
                cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty country list.");

    public async Task<CountryCitiesResponse> GetCitiesByCountryAsync(
        string countryCode,
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<CountryCitiesResponse>(
                $"api/countries/{Uri.EscapeDataString(countryCode)}/cities",
                JsonOptions,
                cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty city list.");

    public async Task<WeatherResponse> GetWeatherByCityAsync(
        string cityName,
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<WeatherResponse>(
                $"api/weather/{Uri.EscapeDataString(cityName)}",
                JsonOptions,
                cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty weather response.");

    public async Task<CreateWeatherNoteResponse> CreateWeatherNoteAsync(
        CreateWeatherNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/weather/notes",
            request,
            JsonOptions,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CreateWeatherNoteResponse>(
                JsonOptions,
                cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty weather-note response.");
    }
}
