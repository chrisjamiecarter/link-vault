namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public static class GenerateQrCodeEndpoint
{
    public const string Name = nameof(GenerateQrCode);

    public static void MapGenerateQrCodeEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{shortCode}/qrcode", GenerateQrCodeHandler.Handle)
            .AllowAnonymous()
            .WithName(Name)
            .CacheOutput();
    }
}
