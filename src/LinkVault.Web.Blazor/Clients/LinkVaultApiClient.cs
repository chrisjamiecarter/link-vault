using LinkVault.Web.Blazor.Contracts.ExpandUrl;
using LinkVault.Web.Blazor.Contracts.ShortenUrl;
using LinkVault.Web.Blazor.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LinkVault.Web.Blazor.Clients;

internal sealed class LinkVaultApiClient(HttpClient httpClient)
{
    public async Task<ExpandUrlResponse> ExpandUrlAsync(
        ExpandUrlRequest request,
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"/links/{request.ShortCode}", ct);
        await EnsureSuccessAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<ExpandUrlResponse>(ct))!;
    }

    public async Task<ShortenUrlResponse> ShortenUrlAsync(
        ShortenUrlRequest request,
        CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/links", request, ct);
        await EnsureSuccessAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<ShortenUrlResponse>(ct))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ProblemDetails? problem = null;

        var contentType = response.Content.Headers.ContentType?.MediaType;
        if (contentType is "application/problem+json")
        {
            problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
        }

        problem ??= new ProblemDetails
        {
            Status = (int)response.StatusCode,
            Title = response.ReasonPhrase ?? "Unknown error"
        };

        throw new ApiException(response.StatusCode, problem);
    }
}
