using LinkVault.Core.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ExpandUrl;

public static class ExpandUrl
{
    public static readonly string Name = nameof(ExpandUrl);

    public sealed record Response(Guid Id, string OriginalUrl);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/links/{shortCode}", Handler)
                .AllowAnonymous()
                .WithName(Name)
                .WithTags("links");
        }

        public static async Task<Results<Ok<Response>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>, ProblemHttpResult>> Handler(
            string shortCode,
            LinkVaultDbContext context,
            CancellationToken ct)
        {
            var link = await context.Links
                .AsNoTracking()
                .FirstOrDefaultAsync(link => link.ShortCode == shortCode, ct);

            if (link is null)
            {
                return TypedResults.NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Link Not Found",
                    Detail = $"No link found for short code '{shortCode}'."
                });
            }

            if (!link.IsActive)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Link Inactive",
                    Detail = $"The link with short code '{shortCode}' is inactive."
                });
            }

            if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTimeOffset.UtcNow)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status410Gone,
                    Title = "Link Expired",
                    Detail = $"The link with short code '{shortCode}' expired on {link.ExpiresAt:yyyy-MM-dd}."
                });
            }

            return TypedResults.Ok(new Response(link.Id, link.OriginalUrl));
        }
    }
}
