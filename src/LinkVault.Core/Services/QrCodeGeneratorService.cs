using QRCoder;

namespace LinkVault.Core.Services;

/// <summary>
/// Service for generating QR codes for the shortened URLs.
/// Uses the QRCoder library to create QR codes as PNG byte arrays.
/// </summary>
public static class QrCodeGeneratorService
{
    /// <summary>
    /// The number of pixels each dark/light module fo the QR code will occupy in the final QR code image.
    /// </summary>
    public const int PixelsPerModule = 20;

    /// <summary>
    /// Generates a QR code as a PNG byte array using local QRCoder library.
    /// Used as fallback when external API fails.
    /// </summary>
    public static byte[] Generate(string shortCodeUrl)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(shortCodeUrl, QRCodeGenerator.ECCLevel.Default);
        using var qrCode = new PngByteQRCode(data);
        return qrCode.GetGraphic(PixelsPerModule);
    }
}
