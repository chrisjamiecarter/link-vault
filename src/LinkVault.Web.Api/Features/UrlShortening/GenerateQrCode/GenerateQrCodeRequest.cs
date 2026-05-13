using Microsoft.AspNetCore.Mvc;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public sealed record GenerateQrCodeRequest(
    string ShortCode,
    [FromQuery(Name = "baseUrl")] string? BaseUrl);
