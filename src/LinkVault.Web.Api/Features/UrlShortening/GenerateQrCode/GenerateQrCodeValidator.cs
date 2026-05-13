using FluentValidation;
using LinkVault.Core.Entities;
using LinkVault.Core.Services;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public sealed class GenerateQrCodeValidator : AbstractValidator<GenerateQrCodeRequest>
{
    public GenerateQrCodeValidator()
    {
        RuleFor(request => request.ShortCode)
            .NotEmpty()
            .Length(Link.ShortCodeMinLength, Link.ShortCodeMaxLength)
            .Must(link => link.All(c => ShortCodeGeneratorService.AllowedChars.Contains(c)));

        When(request => request.BaseUrl is not null, () =>
        {
            RuleFor(request => request.BaseUrl!)
                .Must(BeAValidAbsoluteHttpUri)
                .WithMessage("Base URL must be a valid absolute HTTP or HTTPS URL.");
        });
    }

    private static bool BeAValidAbsoluteHttpUri(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && uri.Scheme is "http" or "https";
}
