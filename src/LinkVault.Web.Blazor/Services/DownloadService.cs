using Microsoft.JSInterop;

namespace LinkVault.Web.Blazor.Services;

public sealed class DownloadService(IJSRuntime jSRuntime)
{
    public async ValueTask DownloadFileAsync(string fileName, byte[] bytes, string mediaType = "application/octet-stream")
    {
        using var steam = new MemoryStream(bytes);
        var dotnetStream = new DotNetStreamReference(steam);
        await jSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, dotnetStream, mediaType);
    }
}
