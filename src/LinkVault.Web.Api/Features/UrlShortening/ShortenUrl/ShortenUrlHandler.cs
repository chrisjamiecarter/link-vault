using LinkVault.Core.Database;
using LinkVault.Core.Entities;
using LinkVault.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public static class ShortenUrlHandler
{
    public static async Task<Results<Ok<ShortenUrlResponse>, ProblemHttpResult>> Handle(
        ShortenUrlRequest request,
        LinkVaultDbContext context,
        ShortCodeGeneratorService service,
        CancellationToken ct)
    {
        var existingLink = await context.Links
            .AsNoTracking()
            .FirstOrDefaultAsync(link => link.OriginalUrl == request.OriginalUrl, ct);

        if (existingLink is not null)
        {
            return TypedResults.Ok(new ShortenUrlResponse(existingLink.Id, existingLink.ShortCode));
        }

        var link = Link.Create(
            request.OriginalUrl,
            service.Generate(),
            DateTimeOffset.UtcNow);

        context.Links.Add(link);

        await context.SaveChangesAsync(ct);

        // Change to Results.Created when the GET endpoint is implemented.
        return TypedResults.Ok(new ShortenUrlResponse(link.Id, link.ShortCode));
    }
}
