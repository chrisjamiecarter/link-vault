using LinkVault.Core.Domain;

namespace LinkVault.Core.Data;

public sealed class Link
    : Entity<Guid>
{
    public const int OriginalUrlMaxLength = 2048;
    public const int ShortCodeMinLength = 4;
    public const int ShortCodeMaxLength = 16;
    public const int DefaultExpirationDays = 30;

    // Required for ORM hydration.
    private Link() { }

    private Link(
        string originalUrl,
        string shortCode,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt,
        bool isActive,
        string? qrCodeUrl)
    {
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsActive = isActive;
        QrCodeUrl = qrCodeUrl;
    }

    public string OriginalUrl { get; private set; } = default!;

    public string ShortCode { get; private set; } = default!;
    
    public DateTimeOffset CreatedAt { get; private set; } = default!;
    
    public DateTimeOffset? ExpiresAt { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    public string? QrCodeUrl { get; private set; } = default!;

    /// <summary>
    /// OriginalUrl: valid URL format (absolute URI)	❌ Missing	Entity has OriginalUrlMaxLength constant but no validation in Create()
    /// ShortCode: alphanumeric only	❌ Missing	Entity has constants but no Regex validation in Create()
    /// ExpiresAt: > CreatedAt (if set)	❌ 	Entity has default logic but no validation in Create()
    /// </summary>
    /// <param name="originalUrl"></param>
    /// <param name="shortCode"></param>
    /// <param name="createdAt"></param>
    /// <param name="expiresAt"></param>
    /// <param name="isActive"></param>
    /// <param name="qrCodeUrl"></param>
    /// <returns></returns>
    public static Link Create(
        string originalUrl,
        string shortCode,
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt,
        bool isActive,
        string? qrCodeUrl) => 
        new(
            originalUrl,
            shortCode,
            createdAt,
            expiresAt ?? createdAt.AddDays(DefaultExpirationDays),
            isActive,
            qrCodeUrl);
}
