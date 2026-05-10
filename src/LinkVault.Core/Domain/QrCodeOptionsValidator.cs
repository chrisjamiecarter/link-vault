using LinkVault.Core.Configurations;
using Microsoft.Extensions.Options;

namespace LinkVault.Core.Domain;

public class QrCodeOptionsValidator : IValidateOptions<QrCodeOptions>
{
    public ValidateOptionsResult Validate(string? name, QrCodeOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail($"{QrCodeOptions.Key} options not found.");
        }

        if (options.SizeInPixels < QrCodeOptions.MinSizeInPixels || options.SizeInPixels > QrCodeOptions.MaxSizeInPixels)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.SizeInPixels)} ({options.SizeInPixels}) must be between {QrCodeOptions.MinSizeInPixels} and {QrCodeOptions.MaxSizeInPixels}.");
        }

        return ValidateOptionsResult.Success;
    }
}
