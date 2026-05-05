namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public static class ExpandUrl
{
    public sealed record Request(string ShortCode);
    public sealed record Response(Guid Id, string OriginalUrl);
}
