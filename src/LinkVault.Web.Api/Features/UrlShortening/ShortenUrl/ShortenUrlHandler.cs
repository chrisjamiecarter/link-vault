using LinkVault.Core.Database;
using LinkVault.Core.Entities;
using LinkVault.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public sealed class ShortenUrlHandler(
        LinkVaultDbContext context,
        ShortCodeGeneratorService service)
    : IHandler<ShortenUrlRequest, ShortenUrlResponse>
{
    public async Task<HandleResult<ShortenUrlResponse>> HandleAsync(
        ShortenUrlRequest request,
        CancellationToken ct)
    {
        var existingLink = await context.Links
            .AsNoTracking()
            .FirstOrDefaultAsync(link => link.OriginalUrl == request.OriginalUrl, ct);

        if (existingLink is not null)
        {
            return new HandleResult<ShortenUrlResponse>.Success(
                new ShortenUrlResponse(existingLink.Id, existingLink.ShortCode));
        }

        var link = Link.Create(
            request.OriginalUrl,
            service.Generate(),
            DateTimeOffset.UtcNow);

        context.Links.Add(link);

        await context.SaveChangesAsync(ct);

        return new HandleResult<ShortenUrlResponse>.Created(
            new ShortenUrlResponse(link.Id, link.ShortCode), $"api//links/{link.ShortCode}");
    }
}
