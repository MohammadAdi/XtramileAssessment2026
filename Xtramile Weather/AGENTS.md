# Project conventions

## Architecture
- Keep Domain independent of frameworks and other solution projects.
- Application references Domain and owns CQRS handlers, validation, and external interfaces.
- Infrastructure implements Application interfaces and owns EF Core and external HTTP clients.
- API references Application and Infrastructure for dependency registration. Controllers dispatch through MediatR.
- The standalone Blazor frontend communicates over HTTP and never references backend projects.
- Group commands, queries, handlers, validators, and DTOs by feature.
- Use AsNoTracking() explicitly for EF Core reads. Pass cancellation tokens through asynchronous operations.
- Keep secrets on the backend. Never put API keys in Blazor configuration.

## Request flow

### Query flow
1. The controller receives the HTTP request and binds route or query parameters to a MediatR query.
2. FluentValidation validates externally supplied query input through the MediatR pipeline when validation is required.
3. The query handler uses an Application abstraction to retrieve data.
4. Infrastructure implements the abstraction using EF Core or an external API.
5. The handler maps the result to a response DTO.
6. The controller returns the DTO with the appropriate HTTP status code.

### Command flow
1. The controller binds the request body and sends a MediatR command without business logic.
2. FluentValidation validates the command through the MediatR pipeline.
3. The command handler applies application rules and creates or updates Domain entities.
4. The handler uses an Application persistence abstraction.
5. Infrastructure persists the entities through EF Core.
6. The handler returns a response DTO, including a created resource identifier when applicable.
7. The controller returns 201 Created and a location header when a resource is created.

### Layer boundaries
- Controllers must not access DbContext, repositories, or HttpClient directly.
- Controllers must not create or mutate entities directly.
- Application handlers must not reference API or Infrastructure projects.
- Domain entities must not reference EF Core, MediatR, HTTP, or ASP.NET Core.
- Infrastructure implements Application abstractions and must not contain business rules.
- Map API request models to commands in API and map entities to DTOs in Application handlers.
- Never return database entities from API endpoints.

## DTO and contract conventions
- Treat API request models, Application CQRS messages, Application DTOs, and external-provider models as separate contracts with different responsibilities.
- HTTP request-body models belong to `XtramileWeather.Api`. Name them with the `Request` suffix and group them by feature under `Contracts/<Feature>/` when a dedicated model is needed.
- Do not create request DTOs for simple route or query parameters. Map simple values directly to the corresponding MediatR command or query.
- Commands and queries belong to `XtramileWeather.Application` and are Application messages, not HTTP DTOs.
- Query result DTOs belong to `XtramileWeather.Application` and must be colocated with the feature that owns them, for example `Features/Weather/GetWeather/WeatherDto.cs`.
- Do not create a global `Application/DTOs` folder unless a contract is genuinely shared across unrelated features. Prefer feature-local DTOs.
- Command handlers may return a resource identifier directly or a dedicated `Response` model when the caller needs multiple output values. Do not create response wrappers only for symmetry.
- Provider-specific request and response models belong to `XtramileWeather.Infrastructure`, close to the provider implementation, for example `Weather/Models/OpenWeatherResponse.cs`.
- Never expose Domain entities, EF Core entities, or provider-specific models directly from API endpoints.
- Avoid duplicate models when there is no meaningful boundary or responsibility difference.

### Contract placement examples
```text
XtramileWeather.Api/
└── Contracts/
    └── WeatherNotes/
        └── CreateWeatherNoteRequest.cs

XtramileWeather.Application/
└── Features/
    ├── Weather/
    │   └── GetWeather/
    │       ├── GetWeatherQuery.cs
    │       ├── GetWeatherQueryHandler.cs
    │       └── WeatherDto.cs
    └── WeatherNotes/
        └── CreateWeatherNote/
            ├── CreateWeatherNoteCommand.cs
            ├── CreateWeatherNoteCommandHandler.cs
            ├── CreateWeatherNoteCommandValidator.cs
            └── CreateWeatherNoteResponse.cs

XtramileWeather.Infrastructure/
└── Weather/
    └── Models/
        └── OpenWeatherResponse.cs
```

### Contract flow
- GET with simple route/query input: `route/query value -> Query -> Handler -> DTO -> HTTP response`.
- POST with a request body: `HTTP Request model -> Command -> Validator -> Handler -> Domain/Persistence -> Response or identifier -> HTTP response`.
- External integration: `Provider JSON -> Infrastructure provider model -> Application contract/DTO or Domain value`; provider models must not leak beyond Infrastructure.

## API and data contracts
- Store city latitude and longitude as decimal coordinates.
- Keep endpoints under /api; add /api/v1 only when versioning is required.
- Keep response property names consistent and predictable.
- Add pagination only when a dataset can grow beyond a practical response size.

## Validation and domain rules
- Validate all externally supplied input.
- Reject blank city names and notes exceeding the configured length.
- Normalize country codes to uppercase before querying.
- Return validation errors as ProblemDetails with field-level details.
- Do not use data-annotation validation on Domain entities.
- Domain entities protect valid state through constructors or methods.
- Do not expose public setters for mutable entity state.
- Use a dedicated method when entity state changes.
- Do not persist weather-provider response models as Domain entities.

## FluentValidation
- Use FluentValidation for every command and every query that accepts externally supplied input requiring validation.
- Keep each validator in the same feature folder as its command or query. Name it `<MessageName>Validator`.
- Register validators from the Application assembly and execute them through a MediatR pipeline behavior before handlers run.
- Controllers must not contain validation rules, invoke validators directly, or duplicate validation performed by the pipeline.
- Handlers may assume syntactically valid input after pipeline validation, but must still enforce application rules and Domain invariants.
- Convert validation failures to RFC 7807 ProblemDetails with field-level errors and HTTP 400.
- Add validator unit tests for valid input, each important invalid rule, and boundary values.
- Add API integration tests that verify invalid requests return the expected ProblemDetails response.

## External services
- Define external-service contracts in Application and implementations in Infrastructure.
- Keep provider-specific request and response models in Infrastructure.
- Translate provider-specific models into Application DTOs or Domain values before returning data.
- Do not leak provider-specific fields, URLs, credentials, or exceptions through the API.
- Configure credentials and provider settings through user secrets, environment variables, or backend configuration.
- Register external HTTP clients through IHttpClientFactory with explicit base addresses and timeouts.
- Propagate cancellation tokens to every external call.
- Handle timeouts, unavailable responses, rate limits, and malformed provider payloads consistently.
- Log service failures with useful context, without logging credentials or sensitive request data.
- Use fakes or mocks for all external services in automated tests; tests must not make live network calls.
- For weather retrieval, call the configured provider endpoint with the selected city name.

## Persistence and dependency injection
- Use EF Core with SQLite for application persistence. Keep `DbContext`, entity configurations, repositories, and migrations under `XtramileWeather.Infrastructure/Persistence`.
- `WeatherDbContext` is an Infrastructure implementation detail. It must only be injected into Infrastructure repositories, seeders, migrations, and Infrastructure service registrations.
- Do not inject `WeatherDbContext` into controllers, Application handlers, Domain entities, or Blazor components.
- Define `DbSet<T>` only for persisted Domain entities. Configure each entity through a dedicated `IEntityTypeConfiguration<T>` class under `Persistence/Configurations`.
- Keep database-only concerns such as keys, indexes, column constraints, foreign keys, conversions, and seed data in EF Core configurations or Infrastructure seeders.
- Use one `WeatherDbContext` for this assessment. Do not create a DbContext per feature or repository.
- Define repository interfaces in Application and implementations in `Infrastructure/Persistence/Repositories`.
- Create explicit repositories that express aggregate or use-case intent, such as `ICountryRepository`, `ICityRepository`, and `IWeatherNoteRepository`.
- Repository interfaces must expose only operations required by Application features. Use intent-revealing methods such as `GetByCountryCodeAsync`, `ListByCountryCodeAsync`, and `AddAsync`.
- Read repositories must use `AsNoTracking()` and return Domain entities or Application projection DTOs as required by the feature.
- Write repositories must track only entities being changed and persist changes through the DbContext with `SaveChangesAsync(cancellationToken)`.
- Repository implementations must pass cancellation tokens to all EF Core operations.
- Do not expose `IQueryable`, `DbSet<T>`, EF Core tracking APIs, or provider-specific query APIs from repository interfaces.
- Do not introduce a generic repository. Do not wrap `WeatherDbContext` in a generic Unit of Work; EF Core DbContext already provides the unit-of-work boundary for this assessment.
- Use EF Core migrations as the schema-management mechanism. Do not combine migrations with `Database.EnsureCreated()` in the application startup path.
- Do not introduce a separate Persistence solution project for this assessment; persistence is an Infrastructure concern.
- Application owns its service registration extension, e.g. `AddApplication()`. Infrastructure owns infrastructure registration, e.g. `AddInfrastructure(configuration)`. API is the composition root and calls these extensions from `Program.cs`.
- Keep `Program.cs` focused on composition and middleware; do not scatter provider, DbContext, or repository registration details through it when Infrastructure can own them.

## Blazor frontend
- Keep HTTP calls in typed API client services.
- Components must not construct HttpClient or contain endpoint URLs.
- Use cancellation tokens or request versioning to prevent stale weather results.
- Display loading, empty, validation, and failure states.
- Keep frontend request and response models separate from UI state.

## Naming and comments
- Use English for identifiers, comments, XML documentation, UI text, and README content.
- Use PascalCase for types and public members, camelCase for locals and parameters, and an I prefix for interfaces.
- Name asynchronous methods with the Async suffix except framework-defined methods.
- Use file-scoped namespaces and follow .editorconfig.
- Use XML summaries for public domain rules and abstractions when they explain the contract.
- Use concise // comments to explain intent, units, or non-obvious decisions. Do not narrate obvious code.
- Do not retain template comments, commented-out code, or empty placeholder tests.

## Change discipline
- Do not add packages or architectural patterns without a clear requirement.
- Do not introduce AutoMapper, generic repositories, or a CQRS base framework for this test.
- Keep changes small and compile after each completed feature.
- Update tests and README when a public endpoint or configuration changes.

## Verification
- Run dotnet build XtramileWeather.sln --configuration Release.
- Run dotnet test XtramileWeather.sln --configuration Release --no-build --no-restore.
- Tests must run offline with no live weather API requests.
- Update README when setup, configuration, architecture, or implemented behavior changes.

## Definition of done
- A feature is complete only when its API contract, validation, error handling, tests,
  frontend behavior when applicable, and README documentation are updated.
