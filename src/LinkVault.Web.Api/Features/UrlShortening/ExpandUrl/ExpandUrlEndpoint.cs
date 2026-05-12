namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public static class ExpandUrlEndpoint
{
    public const string Name = nameof(ExpandUrl);

    public static void MapExpandUrlEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{shortCode}", ExpandUrlHandler.Handle)
                .AllowAnonymous()
                .WithName(Name);
    }
}
