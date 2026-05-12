using Microsoft.AspNetCore.Mvc;

namespace LinkVault.Web.Api.Features.UrlShortening.GenerateQrCode;

public sealed record GenerateQrCodeResponse(string FileName, string MediaType, byte[] ImageBytes);
