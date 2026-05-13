using Microsoft.AspNetCore.Http.HttpResults;

namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public static class ExpandUrlEndpoint
{
    public const string Name = nameof(ExpandUrl);

    public static void MapExpandUrlEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{shortCode}", Handler)
            .AllowAnonymous()
            .WithName(Name)
            .Produces<ExpandUrlResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<FluentValidationFilter<ExpandUrlRequest>>();
    }

    public static async Task<Results<Ok<ExpandUrlResponse>, NotFound, ProblemHttpResult>> Handler(
        [AsParameters] ExpandUrlRequest request,
        ExpandUrlHandler handler,
        CancellationToken ct)
    {
        return await handler.HandleAsync(request, ct) switch
        {
            HandleResult<ExpandUrlResponse>.Success success => TypedResults.Ok(success.Value),
            HandleResult<ExpandUrlResponse>.NotFound notFound => TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Link Not Found",
                detail: notFound.Detail),
            _ => TypedResults.Problem()
        };
    }
}
