using LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;
using LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;
using LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

namespace LinkVault.Web.Api.Features.UrlShortening;

public static class MapUrlShorteningEndpoints
{
    public static IEndpointRouteBuilder MapUrlShortening(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/links/")
            .WithTags("links");

        group.MapExpandUrlEndpoint();
        group.MapGenerateQrCodeEndpoint();
        group.MapShortenUrlEndpoint();

        return builder;
    }
}
