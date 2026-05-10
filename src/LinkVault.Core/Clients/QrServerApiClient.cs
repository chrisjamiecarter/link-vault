using LinkVault.Core.Configurations;

namespace LinkVault.Core.Clients;

public sealed class QrServerApiClient : IQrCodeApiClient
{
    public const string BaseAddress = "https://api.qrserver.com/v1/";

    public const string CreateQrCodeEndpoint = "create-qr-code/";
    
    private readonly HttpClient _httpClient;

    public QrServerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseAddress);
    }

    public async Task<byte[]?> GenerateQrCodeAsync(
        string shortCodeUrl,
        QrCodeOptions options,
        CancellationToken ct = default)
    {
        try
        {
            var encodedUrl = Uri.EscapeDataString(shortCodeUrl);
            var requestUri = $"{CreateQrCodeEndpoint}?data={encodedUrl}&size={options.SizeInPixels}x{options.SizeInPixels}";
            var response = await _httpClient.GetAsync(requestUri, ct);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync(ct);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
