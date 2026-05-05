using LinkVault.Web.Blazor.Contracts.ShortenUrl;

namespace LinkVault.Web.Blazor.Clients;

internal sealed class LinkVaultApiClient(HttpClient httpClient)
{
    public async Task<ShortenUrlResponse> ShortenUrlAsync(
        ShortenUrlRequest request, 
        CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/links", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ShortenUrlResponse>(ct))!;
    }
}
