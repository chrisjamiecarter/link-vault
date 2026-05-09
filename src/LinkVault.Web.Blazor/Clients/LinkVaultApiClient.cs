using LinkVault.Web.Blazor.Contracts.ExpandUrl;
using LinkVault.Web.Blazor.Contracts.ShortenUrl;

namespace LinkVault.Web.Blazor.Clients;

internal sealed class LinkVaultApiClient(HttpClient httpClient)
{
    public async Task<ExpandUrlResponse> ExpandUrlAsync(
        ExpandUrlRequest request,
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"/links/{request.ShortCode}", ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ExpandUrlResponse>(ct))!;
    }

    public async Task<ShortenUrlResponse> ShortenUrlAsync(
        ShortenUrlRequest request,
        CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/links", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ShortenUrlResponse>(ct))!;
    }
}
