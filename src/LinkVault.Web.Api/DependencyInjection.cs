using LinkVault.Constants;
using LinkVault.Web.Api.RateLimiters;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LinkVault.Web.Api;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApi(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddRedisClient(Resources.Cache.Name);

        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton<FixedWindowRateLimiter>();
        builder.Services.AddSingleton<SlidingWindowRateLimiter>();

        builder.Services.AddRequestTimeouts();
        builder.Services.AddOutputCache();

        return builder;
    }

    public static IApplicationBuilder UseRateLimiter<TLimiter>(this IApplicationBuilder app)
        where TLimiter : class, IRateLimiter
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.Use(async (context, next) =>
        {
            var rateLimiterOptions = context.RequestServices.GetRequiredService<IOptions<RateLimiterOptions>>().Value;
            
            var path = context.Request.Path.Value?.ToLowerInvariant();
            if (!rateLimiterOptions.ExemptPaths.Contains(path))
            {
                var limiter = context.RequestServices.GetRequiredService<TLimiter>();
                var clientKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                if (!await limiter.IsAllowedAsync(clientKey))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers.RetryAfter = rateLimiterOptions.Window.TotalSeconds.ToString(CultureInfo.InvariantCulture);
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                    return;
                }
            }

            await next();
        });
    }
}
