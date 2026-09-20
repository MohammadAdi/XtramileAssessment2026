using XtramileWeather.Application;
using XtramileWeather.Api.Middleware;
using XtramileWeather.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration,
    builder.Configuration.GetConnectionString("WeatherDatabase")
    ?? throw new InvalidOperationException("The weather database connection string is missing."));
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

await app.Services.ApplyDatabaseMigrationsAsync();

app.UseExceptionHandler();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseStatusCodePages();
app.UseCors("Frontend");
app.MapControllers();
app.Run();

// Exposes the entry point to the in-process integration test host.
public partial class Program;
