using Aspire.ServiceDefaults;
using LinkVault.Core;
using LinkVault.Web.Api.RateLimiters;

namespace LinkVault.Web.Api;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddCore();
        builder.AddApi();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        app.UseRateLimiter<SlidingWindowRateLimiter>();
        app.UseRequestTimeouts();
        app.UseOutputCache();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapGet("/", () => "API service is running.");


        await app.RunAsync();
    }
}
