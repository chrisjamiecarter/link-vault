using LinkVault.Constants;
using LinkVault.Web.Api.Features;
using LinkVault.Web.Api.RateLimiters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;

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

        builder.Services.AddEndpoints(AssemblyReference.Assembly);

        return builder;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = [.. assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false} && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))];

        services.TryAddEnumerable(serviceDescriptors);
        
        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

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
