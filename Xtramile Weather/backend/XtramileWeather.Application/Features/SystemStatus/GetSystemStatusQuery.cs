using MediatR;

namespace XtramileWeather.Application.Features.SystemStatus;

public sealed record GetSystemStatusQuery : IRequest<SystemStatusResponse>;

public sealed record SystemStatusResponse(string Status);

public sealed class GetSystemStatusHandler : IRequestHandler<GetSystemStatusQuery, SystemStatusResponse>
{
    public Task<SystemStatusResponse> Handle(GetSystemStatusQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new SystemStatusResponse("Ready"));
}
