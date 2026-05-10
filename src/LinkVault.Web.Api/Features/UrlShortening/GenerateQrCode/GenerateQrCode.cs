using LinkVault.Core.Clients;
using LinkVault.Core.Configurations;
using LinkVault.Core.Database;
using LinkVault.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public static class GenerateQrCode
{
    public static readonly string Name = nameof(GenerateQrCode);

    public sealed record Response(string FileName, string MediaType, byte[] ImageBytes);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/links/{shortCode}/qrcode", Handler)
                .AllowAnonymous()
                .WithName(Name)
                .WithTags("links");
        }

        public static async Task<Results<Ok<Response>, ProblemHttpResult>> Handler(
            string shortCode,
            string baseUrl,
            LinkVaultDbContext context,
            IOptions<QrCodeOptions> options,
            IQrCodeApiClient apiClient,
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

            var shortCodeUrl = $"{baseUrl.TrimEnd('/')}/{link.ShortCode}";

            var imageBytes =
                // Note: Temp Service ?? API ?? Service. Should be API ?? Service when live.
                QrCodeGeneratorService.Generate(shortCodeUrl) ??
                await apiClient.GenerateQrCodeAsync(
                shortCodeUrl,
                options.Value,
                ct) ?? QrCodeGeneratorService.Generate(shortCodeUrl);

            return TypedResults.Ok(new Response(
                $"linkvault-{link.ShortCode}.png",
                "image/png",
                imageBytes));
        }
    }
}
