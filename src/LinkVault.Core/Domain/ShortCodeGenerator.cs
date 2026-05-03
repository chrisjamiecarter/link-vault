using System.Security.Cryptography;

namespace LinkVault.Core.Domain;

public static class ShortCodeGenerator
{
    public const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    public static string Generate(int length = 8) =>
        RandomNumberGenerator.GetString(AllowedChars, length);
    
}
