using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Application.Features.SystemStatus;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/status")]
public sealed class SystemStatusController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<SystemStatusResponse> Get(CancellationToken cancellationToken)
        => sender.Send(new GetSystemStatusQuery(), cancellationToken);
}
