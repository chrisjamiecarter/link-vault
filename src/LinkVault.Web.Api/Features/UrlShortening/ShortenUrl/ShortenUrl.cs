using LinkVault.Core.Database;
using LinkVault.Core.Entities;
using LinkVault.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;

public static class ShortenUrl
{
    public static readonly string Name = nameof(ShortenUrl);

    public sealed record Request(string OriginalUrl);
    public sealed record Response(Guid Id, string ShortCode);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("links", Handler)
                .AllowAnonymous()
                .WithName(Name)
                .WithTags("links");
        }

        public static async Task<Results<Ok<Response>, ProblemHttpResult>> Handler(
            Request request,
            LinkVaultDbContext context,
            ShortCodeGeneratorService service,
            CancellationToken ct)
        {
            var existingLink = await context.Links
                .AsNoTracking()
                .FirstOrDefaultAsync(link => link.OriginalUrl == request.OriginalUrl, ct);

            if (existingLink is not null)
            {
                return TypedResults.Ok(new Response(existingLink.Id, existingLink.ShortCode));
            }

            var link = Link.Create(
                request.OriginalUrl,
                service.Generate(),
                DateTimeOffset.UtcNow);

            context.Links.Add(link);

            await context.SaveChangesAsync(ct);

            // Change to Results.Created when the GET endpoint is implemented.
            return TypedResults.Ok(new Response(link.Id, link.ShortCode));
        }
    }
}
