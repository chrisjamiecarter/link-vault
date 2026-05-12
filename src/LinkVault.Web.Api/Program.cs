using Aspire.ServiceDefaults;
using LinkVault.Core;
using LinkVault.Web.Api.RateLimiters;

namespace LinkVault.Web.Api;

internal static class Program
{
    /// <summary>
    /// 10 MB
    /// </summary>
    public const long MaxRequestBodySize = 10 * 1024 * 1024;

    /// <summary>
    /// 32 KB
    /// </summary>
    public const int MaxRequestHeadersTotalSize = 32 * 1024;

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddCore();
        builder.AddApi();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = MaxRequestBodySize;
            options.Limits.MaxRequestHeadersTotalSize = MaxRequestHeadersTotalSize;
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();
        app.UseExceptionHandler(exceptionApp =>
        {
            exceptionApp.Run(async context =>
            {
                var problemDetailsService = context.RequestServices
                    .GetRequiredService<IProblemDetailsService>();

                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails =
                    {
                        Title = "An unexpected error occurred",
                        Status = StatusCodes.Status500InternalServerError,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
                    }
                });
            });
        });
        app.UseHttpsRedirection();
        app.UseRateLimiter<SlidingWindowRateLimiter>();
        app.UseRequestTimeouts();
        app.UseOutputCache();

        app.MapEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapGet("/", () => "API service is running.");

        await app.RunAsync();
    }
}
