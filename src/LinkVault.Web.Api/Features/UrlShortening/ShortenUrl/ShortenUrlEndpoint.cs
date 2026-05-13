using Microsoft.AspNetCore.Http.HttpResults;

namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public static class ShortenUrlEndpoint
{
    public const string Name = nameof(ShortenUrl);

    public static void MapShortenUrlEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handler)
            .AddEndpointFilter<FluentValidationFilter<ShortenUrlRequest>>()
            .AllowAnonymous()
            .WithName(Name);
    }

    public static async Task<Results<Ok<ShortenUrlResponse>, Created<ShortenUrlResponse>, ProblemHttpResult>> Handler(
            ShortenUrlRequest request,
            ShortenUrlHandler handler,
            CancellationToken ct)
    {
        return await handler.HandleAsync(request, ct) switch
        {
            HandleResult<ShortenUrlResponse>.Success success => TypedResults.Ok(success.Value),
            HandleResult<ShortenUrlResponse>.Created created => TypedResults.Created(created.Location, created.Value),
            _ => TypedResults.Problem()
        };
    }
}
