using LinkVault.Core.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public static class ExpandUrlHandler
{
    public static async Task<Results<Ok<ExpandUrlResponse>, ProblemHttpResult>> Handle(
        string shortCode,
        LinkVaultDbContext context,
        CancellationToken ct)
    {
        var link = await context.Links
            .AsNoTracking()
            .FirstOrDefaultAsync(link => link.ShortCode == shortCode, ct);

        if (link is null)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Link Not Found",
                detail: $"No link found for short code '{shortCode}'.");
        }

        if (!link.IsActive)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Link Inactive",
                detail: $"The link with short code '{shortCode}' is inactive.");
        }

        if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTimeOffset.UtcNow)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status410Gone,
                title: "Link Expired",
                detail: $"The link with short code '{shortCode}' has expired.");
        }

        return TypedResults.Ok(new ExpandUrlResponse(link.Id, link.OriginalUrl));
    }
}

