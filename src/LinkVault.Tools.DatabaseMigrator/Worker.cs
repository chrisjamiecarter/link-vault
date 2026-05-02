using LinkVault.Tools.DatabaseMigrator.Services;
using LinkVault.Web.Api.Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LinkVault.Tools.DatabaseMigrator;

public class Worker(
    IHostApplicationLifetime hostApplicationLifetime,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    public const string ActivitySourceName = nameof(DatabaseMigrator);

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = _serviceProvider.CreateScope();

            var migratorService = scope.ServiceProvider.GetRequiredService<IDatabaseMigratorService>();

            List<DbContext> dbContexts =
            [
                scope.ServiceProvider.GetRequiredService<LinkVaultDbContext>(),
            ];

            foreach (var dbContext in dbContexts)
            {
                await migratorService.MigrateAsync(dbContext, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            activity?.AddException(exception);
            throw;
        }

        _hostApplicationLifetime.StopApplication();
    }
}
