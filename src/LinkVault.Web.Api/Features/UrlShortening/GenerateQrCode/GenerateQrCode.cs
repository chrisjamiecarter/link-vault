namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public static class GenerateQrCode
{
    public sealed record Request(string ShortCode);
    public sealed record Response(Guid Id, string QrCodeUrl);
}
