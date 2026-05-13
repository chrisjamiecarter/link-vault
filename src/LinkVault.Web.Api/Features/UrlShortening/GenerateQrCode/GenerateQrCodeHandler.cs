using LinkVault.Core.Clients;
using LinkVault.Core.Configurations;
using LinkVault.Core.Database;
using LinkVault.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public sealed class GenerateQrCodeHandler(
        LinkVaultDbContext context,
        IQrCodeApiClient apiClient,
        IOptions<QrCodeOptions> options)
    : IHandler<GenerateQrCodeRequest, GenerateQrCodeResponse>
{
    private readonly LinkVaultDbContext _context = context;
    private readonly IQrCodeApiClient _apiClient = apiClient;
    private readonly IOptions<QrCodeOptions> _options = options;

    public async Task<HandleResult<GenerateQrCodeResponse>> HandleAsync(
        GenerateQrCodeRequest request,
        CancellationToken ct)
    {
        var link = await _context.Links
            .FirstOrDefaultAsync(link => link.ShortCode == request.ShortCode, ct);

        if (link is null)
        {
            return new HandleResult<GenerateQrCodeResponse>.NotFound(
                $"No link found for short code '{request.ShortCode}'.");
        }

        if (link.QrCodeImage is not null && link.QrCodeImage.Length > 0)
        {
            return new HandleResult<GenerateQrCodeResponse>.Success(
                new GenerateQrCodeResponse(
                    GetFileName(link.ShortCode),
                    "image/png",
                    link.QrCodeImage));
        }

        var shortCodeUrl = $"{request.BaseUrl?.TrimEnd('/')}/{link.ShortCode}";

        var imageBytes =
            // Note: Temp Service ?? API ?? Service. Should be API ?? Service when live.
            QrCodeGeneratorService.Generate(shortCodeUrl) ??
            await _apiClient.GenerateQrCodeAsync(
            shortCodeUrl,
            _options.Value,
            ct) ?? QrCodeGeneratorService.Generate(shortCodeUrl);

        link.SetQrCodeImage(imageBytes);

        await _context.SaveChangesAsync(ct);

        return new HandleResult<GenerateQrCodeResponse>.Success(
            new GenerateQrCodeResponse(
                GetFileName(link.ShortCode),
                "image/png",
                imageBytes));
    }

    private static string GetFileName(string shortCode) => $"linkvault-{shortCode}.png";
}
