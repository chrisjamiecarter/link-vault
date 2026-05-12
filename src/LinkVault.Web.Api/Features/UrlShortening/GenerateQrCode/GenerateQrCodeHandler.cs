using LinkVault.Core.Clients;
using LinkVault.Core.Configurations;
using LinkVault.Core.Database;
using LinkVault.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public static class GenerateQrCodeHandler
{
    public static async Task<Results<Ok<GenerateQrCodeResponse>, ProblemHttpResult>> Handle(
        string shortCode,
        string baseUrl,
        LinkVaultDbContext context,
        IOptions<QrCodeOptions> options,
        IQrCodeApiClient apiClient,
        CancellationToken ct)
    {
        var link = await context.Links
            .FirstOrDefaultAsync(link => link.ShortCode == shortCode, ct);

        if (link is null)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Link Not Found",
                detail: $"No link found for short code '{shortCode}'.");
        }

        if (link.QrCodeImage is not null && link.QrCodeImage.Length > 0)
        {
            return TypedResults.Ok(new GenerateQrCodeResponse(
                GetFileName(link.ShortCode),
                "image/png",
                link.QrCodeImage));
        }

        var shortCodeUrl = $"{baseUrl.TrimEnd('/')}/{link.ShortCode}";

        var imageBytes =
            // Note: Temp Service ?? API ?? Service. Should be API ?? Service when live.
            QrCodeGeneratorService.Generate(shortCodeUrl) ??
            await apiClient.GenerateQrCodeAsync(
            shortCodeUrl,
            options.Value,
            ct) ?? QrCodeGeneratorService.Generate(shortCodeUrl);

        link.SetQrCodeImage(imageBytes);

        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(new GenerateQrCodeResponse(
            GetFileName(link.ShortCode),
            "image/png",
            imageBytes));
    }

    private static string GetFileName(string shortCode) => $"linkvault-{shortCode}.png";
}
