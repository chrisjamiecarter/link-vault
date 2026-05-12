using FluentValidation;
using LinkVault.Constants;
using LinkVault.Web.Api.Features.UrlShortening;
using LinkVault.Web.Api.Features.UrlShortening.ShortenUrl;
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

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["instance"] =
                    context.HttpContext.Request.Path.Value;
            };
        });
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton<FixedWindowRateLimiter>();
        builder.Services.AddSingleton<SlidingWindowRateLimiter>();

        builder.Services.AddRequestTimeouts();
        builder.Services.AddOutputCache();

        builder.Services.AddScoped<ShortenUrlHandler>();

        builder.Services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        return builder;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        app.MapUrlShortening();

        return app;
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

                    var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();

                    await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails =
                        {
                            Title = "Too Many Requests",
                            Status = StatusCodes.Status429TooManyRequests,
                            Detail = $"You have exceeded the allowed number of requests. Please try again after {rateLimiterOptions.Window.TotalSeconds} seconds."
                        }
                    });

                    return;
                }
            }

            await next();
        });
    }
}
