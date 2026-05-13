using Microsoft.AspNetCore.Http.HttpResults;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public static class GenerateQrCodeEndpoint
{
    public const string Name = nameof(GenerateQrCode);

    public static void MapGenerateQrCodeEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{shortCode}/qrcode", Handler)
            .AllowAnonymous()
            .WithName(Name)
            .CacheOutput()
            .Produces<GenerateQrCodeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<FluentValidationFilter<GenerateQrCodeRequest>>();
    }

    public static async Task<Results<Ok<GenerateQrCodeResponse>, NotFound, ProblemHttpResult>> Handler(
        [AsParameters] GenerateQrCodeRequest request,
        GenerateQrCodeHandler handler,
        CancellationToken ct)
    {
        return await handler.HandleAsync(request, ct) switch
        {
            HandleResult<GenerateQrCodeResponse>.Success success => TypedResults.Ok(success.Value),
            HandleResult<GenerateQrCodeResponse>.NotFound notFound => TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Link Not Found",
                detail: notFound.Detail),
            _ => TypedResults.Problem()
        };
    }
}
