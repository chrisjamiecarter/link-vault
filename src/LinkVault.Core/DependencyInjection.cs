using LinkVault.Constants;
using LinkVault.Core.Database;
using LinkVault.Core.Database.Schemas;
using LinkVault.Core.Domain;
using LinkVault.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LinkVault.Core;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddCore(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .AddDbContext()
            .AddServices()
            .AddOptions();

        return builder;
    }

    private static IHostApplicationBuilder AddDbContext(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Resources.Database.Name) ?? throw new InvalidOperationException($"Connection string '{Resources.Database.Name}' not found.");

        builder.Services.AddDbContextFactory<LinkVaultDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable(Dbo.MigrationsHistory.Table, Dbo.MigrationsHistory.Schema.Value);
            });

            if (builder.Environment.IsDevelopment())
            {
                options
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            }
        });

        builder.EnrichSqlServerDbContext<LinkVaultDbContext>();


        return builder;
    }

    private static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ShortCodeGeneratorService>();

        return builder;
    }

    private static IHostApplicationBuilder AddOptions(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptionsWithValidateOnStart<LinkOptions>()
            .Bind(builder.Configuration.GetSection(LinkOptions.Key));

        builder.Services.AddSingleton<IValidateOptions<LinkOptions>, LinkOptionsValidator>();

        return builder;
    }
}
