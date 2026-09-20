# Xtramile Weather

A practical-test solution with an ASP.NET Core Web API, a separate standalone Blazor WebAssembly frontend, SQLite persistence, CQRS through MediatR, and offline automated tests.

## Implementation status

Implemented:
- Four backend projects with Onion Architecture dependency boundaries.
- MediatR registration and a status query dispatched through a thin controller.
- Dependent country and city lookup endpoints with ISO country codes and city coordinates.
- FluentValidation executed through a MediatR pipeline with validation errors returned as RFC 7807 Problem Details.
- EF Core SQLite persistence for seeded countries and cities, with explicit repositories and migrations.
- Persistent weather notes created through a validated MediatR command and EF Core repository.
- API exception handling using Problem Details and an explicit frontend CORS policy.
- OpenWeatherMap integration behind an Application interface, including current weather metrics and temperature conversion.
- Standalone Blazor frontend with country/city selection, weather details, and weather-note creation.
- Fahrenheit-to-Celsius conversion with edge-case unit tests.
- In-process endpoint, persistence, validation, and CORS integration tests.

## Repository structure

```text
backend/
  XtramileWeather.Domain/
  XtramileWeather.Application/
  XtramileWeather.Infrastructure/
  XtramileWeather.Api/
frontend/
  XtramileWeather.Web/
tests/
  XtramileWeather.UnitTests/
  XtramileWeather.IntegrationTests/
```

The root solution includes all seven projects. Feature folders are created as their implementations are added.

## Architecture

| Project | Responsibility | Project references |
| --- | --- | --- |
| Domain | Entities, value objects, business rules | None |
| Application | CQRS, handlers, validation, external interfaces | Domain |
| Infrastructure | EF Core, repositories, external HTTP services | Application, Domain |
| API | HTTP endpoints and dependency composition | Application, Infrastructure |
| Web | Standalone browser UI and HTTP API client | None |

Dependencies point inward. Domain has no framework or project dependencies. Application
depends only on Domain and defines use cases and interfaces. Infrastructure depends on
Application to implement persistence and external-service interfaces. API is the
composition root and references Application and Infrastructure only to register and
connect those implementations. The standalone frontend communicates with API over HTTP
and has no backend project references.

CQRS separates reads from state-changing operations. Controllers contain HTTP concerns
only and dispatch messages through MediatR, so they do not depend directly on EF Core,
repositories, or external weather clients. FluentValidation runs in a MediatR pipeline
before externally supplied input reaches a handler.

Query flow:

```text
HTTP GET -> Controller -> MediatR Query -> ValidationBehavior -> Query Handler
         -> Application interface -> Infrastructure service/repository -> Response DTO
```

Command flow:

```text
HTTP POST -> API Request model -> Controller -> MediatR Command -> ValidationBehavior
          -> Command Handler -> Domain entity -> Application repository interface
          -> EF Core repository -> SQLite -> Response model
```

For example, `GET /api/weather/{cityName}` dispatches a weather query through an
`IWeatherService` abstraction. `POST /api/weather/notes` maps its HTTP request model to a
command, creates a protected Domain entity in its handler, and persists it through
`IWeatherNoteRepository`. Provider models and EF Core types remain in Infrastructure,
while API responses use Application contracts. This keeps each use case independently
testable and prevents transport or persistence details from leaking into Domain.

## Requirements

- .NET SDK 8.0 or newer capable of targeting net8.0.
- .NET 8 and ASP.NET Core 8 runtimes for running the backend and tests.
- NuGet access for the initial package restore.
- No Node.js installation is required.

The solution targets .NET 8. It was created using the locally installed .NET SDK 9.0.312 and .NET 8 runtime.

## Restore, build, and test

Run these commands from the repository root:

```powershell
dotnet restore XtramileWeather.sln
dotnet build XtramileWeather.sln --configuration Release --no-restore
dotnet test XtramileWeather.sln --configuration Release --no-build --no-restore
```

After packages are restored and projects are built, test execution requires no network access. Integration tests host the API in process using WebApplicationFactory and do not call OpenWeatherMap.

Run only the unit tests with:

```powershell
dotnet test tests/XtramileWeather.UnitTests/XtramileWeather.UnitTests.csproj --configuration Release --no-build --no-restore
```

Run only the integration tests with:

```powershell
dotnet test tests/XtramileWeather.IntegrationTests/XtramileWeather.IntegrationTests.csproj --configuration Release --no-build --no-restore
```

Warnings are treated as errors through Directory.Build.props.

## Run locally

Start the backend in one terminal:

```powershell
dotnet run --project backend/XtramileWeather.Api --launch-profile http
```

Start the frontend in another terminal:

```powershell
dotnet run --project frontend/XtramileWeather.Web --launch-profile http
```

Open http://localhost:5100, select a country and city, and wait for the current weather
to load. The weather-note form is enabled after a successful weather response.

The backend listens at http://localhost:5200. GET /api/status returns:

```json
{ "status": "Ready" }
```

Country endpoints support dependent country/city selection:

```text
GET /api/countries
GET /api/countries/{countryCode}/cities
```

`countryCode` accepts a case-insensitive ISO 3166-1 alpha-2 code. The city response
contains decimal latitude and longitude for future use. Weather lookup uses the provider's
city-name endpoint. Unknown country codes return `404`;
malformed codes return validation details with `400`.

Weather notes are created with:

```text
POST /api/weather/notes
```

```json
{
  "cityId": 4,
  "text": "Heavy rain after lunch."
}
```

Successful requests return `201 Created`, a `Location` header, and the persisted note
identifier. A missing city returns `404`. Non-positive city identifiers, blank notes,
and notes longer than 500 characters return RFC 7807 validation details with `400`.
City lookup responses include the integer `id` used by this command.

Externally supplied query input is validated in Application by FluentValidation before
its handler runs. API validation middleware converts failures into RFC 7807 responses
whose `errors` object uses JSON field names such as `countryCode`.

Country and city data is stored in SQLite and seeded by the initial EF Core migration.
It covers Australia, Indonesia, Malaysia, Singapore, and the United States. Application
handlers access it through explicit country and city repository interfaces; Infrastructure
implements those repositories with no-tracking EF Core queries.

Local launch profiles use HTTP. Configure HTTPS and deployment-specific origins before deploying.

## Configuration

- Backend connection string: ConnectionStrings:WeatherDatabase in backend/XtramileWeather.Api/appsettings.json.
- Allowed frontend origins: Cors:AllowedOrigins in the same file.
- Frontend API address: ApiBaseUrl in frontend/XtramileWeather.Web/wwwroot/appsettings.json.
- OpenWeatherMap base URL: OpenWeatherMap:BaseUrl in backend/XtramileWeather.Api/appsettings.json.
- OpenWeatherMap API key: OpenWeatherMap:ApiKey in backend/XtramileWeather.Api/appsettings.Local.json.

If a port changes, update the corresponding launch profile, frontend API URL, and CORS origin.

Blazor configuration is public browser content. The local OpenWeatherMap configuration file is ignored by Git; never put provider credentials in frontend configuration or source control.

## EF Core and state management

SQLite stores the country and city reference data and retains weather notes. Startup
applies EF Core migrations; it does not use
`Database.EnsureCreated()`.

`WeatherDbContext`, entity configurations, repositories, and migrations remain inside
Infrastructure. Country and city handlers depend on Application repository interfaces.
Read repositories explicitly use `AsNoTracking()` and propagate cancellation tokens.
Integration tests use an isolated in-memory SQLite database and run the same migrations
without creating persistent test files.

The frontend weather-note form enables after weather is loaded for a selected city,
submits through the typed API client, and displays loading, success, and failure states.
Frontend API contracts are separate from component state. Components do not construct
`HttpClient` or embed endpoint URLs; all HTTP operations are provided by the typed
`WeatherApiClient`. The page owns its selected location, current weather, loading flags,
and user-facing errors; it clears dependent city and weather state when the parent
selection changes. The note component separately owns its draft, submission state, and
success or failure message, and resets that state when the selected city changes.

## Design decisions

- **SQLite with migrations:** SQLite provides durable local state without requiring a
  separate database server. Startup applies versioned EF Core migrations rather than
  mixing migrations with `Database.EnsureCreated()`.
- **Explicit repositories:** `ICountryRepository`, `ICityRepository`, and
  `IWeatherNoteRepository` expose only operations needed by Application use cases. A
  generic repository or additional unit-of-work wrapper is not used because it would
  obscure intent and duplicate EF Core behavior.
- **MediatR and FluentValidation:** CQRS messages define individual use cases, handlers
  contain orchestration, and pipeline validation keeps controllers free from duplicated
  validation rules.
- **Manual boundary mapping:** API request models, Application messages and response
  DTOs, Domain entities, frontend contracts, and OpenWeatherMap models are deliberately
  separate. Mapping is explicit; AutoMapper is not required for this solution's small
  contracts.
- **External weather abstraction:** Application owns `IWeatherService`, while
  Infrastructure owns the provider HTTP client and provider-specific JSON models. This
  keeps API credentials on the backend and allows automated tests to use a fake service
  without live network calls.
- **Standalone Blazor WebAssembly:** The frontend communicates only through public HTTP
  contracts. It can be deployed independently and cannot access backend persistence or
  provider credentials.
- **GUID weather-note identifiers:** Notes receive identifiers in Domain before they are
  persisted, allowing the command to return the created resource identifier directly.

## AI prompt configuration

Project-specific AI-assisted development instructions are stored in `AGENTS.md` at the
repository root. Tools that support repository instruction files should load it before
planning or changing code. It defines:

- Onion Architecture boundaries and permitted project dependencies.
- CQRS request flows, contract placement, and MediatR/FluentValidation conventions.
- EF Core, repository, migration, external-service, and secret-handling rules.
- Frontend state and typed API-client conventions.
- Naming, documentation, verification commands, and the definition of done.

`AGENTS.md` contains engineering instructions only; credentials and machine-specific
values do not belong in AI prompt files. Provider credentials remain in the ignored
backend-local configuration or environment/user secrets. If the architecture or required
verification workflow changes, update both `AGENTS.md` and this README so human and
AI-assisted contributions follow the same rules.

## Code and comment standards

All code comments, XML documentation, UI text, and documentation use English.

- Follow .editorconfig for indentation, whitespace, and namespace style.
- Use descriptive names instead of comments that repeat the code.
- Use XML summaries to clarify public domain rules and interface contracts.
- Use short inline comments only for intent, units, and non-obvious decisions.
- Do not keep generated sample endpoints, empty tests, or commented-out code.
- Use PascalCase for types and public members and camelCase for local variables and parameters.
