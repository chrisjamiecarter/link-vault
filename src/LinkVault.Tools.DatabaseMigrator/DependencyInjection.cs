using LinkVault.Tools.DatabaseMigrator.Services;

namespace LinkVault.Tools.DatabaseMigrator;

internal static class DependencyInjection
{
    public static IHostApplicationBuilder AddDatabaseMigrator(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddHostedService<Worker>();

        builder.Services.AddOpenTelemetry()
                        .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

        builder.Services.AddScoped<IDatabaseMigratorService, DatabaseMigratorService>();

        return builder;
    }
}
