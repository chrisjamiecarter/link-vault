using Microsoft.JSInterop;

namespace LinkVault.Web.Blazor.Services;

public sealed class ClipboardService(IJSRuntime jSRuntime)
{
    public ValueTask<string> ReadTextAsync()
    {
        return jSRuntime.InvokeAsync<string>("navigator.clipboard.readText");
    }

    public ValueTask WriteTextAsync(string text)
    {
        return jSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}
