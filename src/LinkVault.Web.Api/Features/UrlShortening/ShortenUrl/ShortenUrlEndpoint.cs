namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public static class ShortenUrlEndpoint
{
    public const string Name = nameof(ShortenUrl);

    public static void MapShortenUrlEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", ShortenUrlHandler.Handle)
            .AddEndpointFilter<FluentValidationFilter<ShortenUrlRequest>>()
            .AllowAnonymous()
            .WithName(Name);
    }
}
