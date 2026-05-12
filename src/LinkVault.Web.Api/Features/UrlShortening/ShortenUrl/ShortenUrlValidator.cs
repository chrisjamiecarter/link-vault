namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public class ShortenUrlValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return next;
    }
}
