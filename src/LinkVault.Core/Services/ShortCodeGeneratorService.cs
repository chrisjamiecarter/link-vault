using LinkVault.Core.Domain;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace LinkVault.Core.Services;

public class ShortCodeGeneratorService(IOptions<LinkOptions> options)
{
    public const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    private readonly int _shortCodeLength = options.Value.ShortCodeLength;

    public string Generate() =>
        RandomNumberGenerator.GetString(AllowedChars, _shortCodeLength);
}
