using FluentValidation;
using LinkVault.Core.Entities;
using LinkVault.Core.Services;

namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public sealed class ExpandUrlValidator : AbstractValidator<ExpandUrlRequest>
{
    public ExpandUrlValidator()
    {
        RuleFor(request => request.ShortCode)
            .NotEmpty()
            .Length(Link.ShortCodeMinLength, Link.ShortCodeMaxLength)
            .Must(link => link.All(c => ShortCodeGeneratorService.AllowedChars.Contains(c)));
    }
}
