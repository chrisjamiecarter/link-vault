using LinkVault.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public sealed class ExpandUrlHandler(LinkVaultDbContext context)
    : IHandler<ExpandUrlRequest, ExpandUrlResponse>
{
    private readonly LinkVaultDbContext _context = context;

    public async Task<HandleResult<ExpandUrlResponse>> HandleAsync(
        ExpandUrlRequest request,
        CancellationToken ct)
    {
        var link = await _context.Links
            .AsNoTracking()
            .FirstOrDefaultAsync(link => link.ShortCode == request.ShortCode, ct);

        if (link is null)
        {
            return new HandleResult<ExpandUrlResponse>.NotFound(
                $"No link found for short code '{request.ShortCode}'.");
        }

        if (!link.IsActive)
        {
            return new HandleResult<ExpandUrlResponse>.NotFound(
                $"No link with short code '{request.ShortCode}' is inactive.");
        }

        if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTimeOffset.UtcNow)
        {
            return new HandleResult<ExpandUrlResponse>.NotFound(
                $"No link with short code '{request.ShortCode}' has expired.");
        }

        return new HandleResult<ExpandUrlResponse>.Success(
            new ExpandUrlResponse(link.Id, link.OriginalUrl));
    }
}
