using Aspire.ServiceDefaults;
using BlazorBlueprint.Components;
using LinkVault.Constants;
using LinkVault.Web.Blazor.Clients;
using LinkVault.Web.Blazor.Components;

namespace LinkVault.Web.Blazor;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddHttpClient<LinkVaultApiClient>("linkvault-api", (_, client) =>
        {
            client.BaseAddress = new Uri($"http+https://{Resources.WebApi.Name}");
            
            // TEMP DEV.
            client.Timeout = TimeSpan.FromSeconds(500);
        });

        builder.Services.AddRequestTimeouts();
        builder.Services.AddOutputCache();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
            });

        // Add BlazorBlueprint services (primitives, toast, dialog).
        builder.Services.AddBlazorBlueprintComponents();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        app.UseRequestTimeouts();
        app.UseOutputCache();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        await app.RunAsync();
    }
}
