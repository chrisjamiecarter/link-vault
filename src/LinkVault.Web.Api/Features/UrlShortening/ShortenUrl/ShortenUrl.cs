using LinkVault.Core.Database;
using LinkVault.Core.Entities;
using LinkVault.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public static class ShortenUrl
{
    public sealed record Request(string OriginalUrl);
    public sealed record Response(Guid Id, string shortCode);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("links", Handler)
                .WithTags("links");
        }

        public static async Task<IResult> Handler(
            Request request,
            LinkVaultDbContext context,
            ShortCodeGeneratorService service,
            CancellationToken ct)
        {
            var existingLink = await context.Links
                .Where(link => link.OriginalUrl == request.OriginalUrl)
                .FirstOrDefaultAsync(ct);

            if (existingLink is not null)
            {
                return Results.Ok(new Response(existingLink.Id, existingLink.ShortCode));
            }

            var link = Link.Create(
                request.OriginalUrl,
                service.Generate(),
                DateTimeOffset.UtcNow);

            context.Links.Add(link);

            await context.SaveChangesAsync(ct);

            // Change to Results.Created when the GET endpoint is implemented.
            return Results.Ok(new Response(link.Id, link.ShortCode));
        }
    }
}
