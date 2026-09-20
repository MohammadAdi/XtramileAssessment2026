using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace XtramileWeather.Api.Middleware;

public sealed class ValidationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IProblemDetailsService problemDetailsService)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            var errors = exception.Errors
                .GroupBy(failure => JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName))
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
            problemDetails.Extensions["errors"] = errors;

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var written = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problemDetails,
                Exception = exception
            });

            if (!written)
            {
                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: context.RequestAborted);
            }
        }
    }
}
