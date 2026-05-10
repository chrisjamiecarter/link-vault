using System.Buffers.Text;

namespace LinkVault.Web.Blazor.Contracts.GenerateQrCode;

internal sealed record GenerateQrCodeRequest(string ShortCode, string BaseUrl);
