using LinkVault.Core.Configurations;
using LinkVault.Core.Entities;
using Microsoft.Extensions.Options;

namespace LinkVault.Core.Domain;

public class LinkOptionsValidator : IValidateOptions<LinkOptions>
{
    public ValidateOptionsResult Validate(string? name, LinkOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail($"{LinkOptions.Key} options not found.");
        }

        if (options.ShortCodeLength < Link.ShortCodeMinLength || options.ShortCodeLength > Link.ShortCodeMaxLength)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ShortCodeLength)} ({options.ShortCodeLength}) must be between {Link.ShortCodeMinLength} and {Link.ShortCodeMaxLength}.");
        }

        return ValidateOptionsResult.Success;
    }
}
