namespace LinkVault.Core.Configurations;

public class QrCodeOptions
{
    public const string Key = "QrCodes";

    public const int MinSizeInPixels = 10;
    public const int MaxSizeInPixels = 750;

    public int SizeInPixels { get; init; } = 300;
}
