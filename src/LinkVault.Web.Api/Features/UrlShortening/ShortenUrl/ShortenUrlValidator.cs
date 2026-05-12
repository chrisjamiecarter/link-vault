using FluentValidation;
using LinkVault.Core.Entities;
using LinkVault.Core.Security;

namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public class ShortenUrlValidator : AbstractValidator<ShortenUrlRequest>
{
    public ShortenUrlValidator()
    {
        RuleFor(request => request.OriginalUrl)
            .NotEmpty()
            .MaximumLength(Link.OriginalUrlMaxLength)
            .Must(BeAValidAbsoluteHttpUri)
            .Must(NotContainXss);
    }

    private static bool BeAValidAbsoluteHttpUri(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && uri.Scheme is "http" or "https";

    private static bool NotContainXss(string url)
        => !XssDetector.IsUnsafe(url);
}
