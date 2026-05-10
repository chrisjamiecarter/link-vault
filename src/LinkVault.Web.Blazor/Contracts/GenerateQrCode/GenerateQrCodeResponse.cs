namespace LinkVault.Web.Blazor.Contracts.GenerateQrCode;

internal sealed record GenerateQrCodeResponse( string FileName, string MediaType, byte[] ImageBytes);
