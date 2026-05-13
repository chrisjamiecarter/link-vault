using LinkVault.Core.Configurations;

namespace LinkVault.Core.Clients;

public interface IQrCodeApiClient
{
    Task<byte[]?> GenerateQrCodeAsync(string shortCodeUrl, QrCodeOptions options, CancellationToken ct = default);
}
