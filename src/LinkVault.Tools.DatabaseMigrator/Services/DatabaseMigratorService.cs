using Microsoft.EntityFrameworkCore;

namespace LinkVault.Tools.DatabaseMigrator.Services;

internal interface IDatabaseMigratorService
{
    Task MigrateAsync(DbContext dbContext, CancellationToken cancellationToken = default);
}

internal sealed partial class DatabaseMigratorService(
    ILogger<DatabaseMigratorService> logger)
    : IDatabaseMigratorService
{
    private readonly ILogger<DatabaseMigratorService> _logger = logger;

    public async Task MigrateAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        var dbContextName = dbContext.GetType().Name;

        LogStartingMigration(dbContextName);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        LogCheckingDatabaseConnectivity();
        var canConnect = await strategy.ExecuteAsync(async () =>
        {
            return await dbContext.Database.CanConnectAsync(cancellationToken);
        });

        if (!canConnect)
        {
            LogCannotConnectToDatabase(dbContextName);
            return;
        }

        LogCheckingForPendingMigrations();
        var pendingMigrations = await strategy.ExecuteAsync(async () =>
        {
            return await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        });

        var migrationCount = pendingMigrations.Count();

        if (migrationCount is 0)
        {
            LogNoPendingMigrations();
            return;
        }

        LogApplyingPendingMigrations();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });

        LogCompletedMigration(dbContextName);
    }

    [LoggerMessage(LogLevel.Information, "Starting migration for {dbContextName}.")]
    private partial void LogStartingMigration(string dbContextName);

    [LoggerMessage(LogLevel.Information, "Checking database connectivity...")]
    private partial void LogCheckingDatabaseConnectivity();

    [LoggerMessage(LogLevel.Error, "Cannot connect to {dbContextName} database.")]
    private partial void LogCannotConnectToDatabase(string dbContextName);

    [LoggerMessage(LogLevel.Information, "Checking for pending migrations...")]
    private partial void LogCheckingForPendingMigrations();

    [LoggerMessage(LogLevel.Information, "No pending migrations to apply.")]
    private partial void LogNoPendingMigrations();

    [LoggerMessage(LogLevel.Information, "Applying pending migrations...")]
    private partial void LogApplyingPendingMigrations();

    [LoggerMessage(LogLevel.Information, "Completed migration for {dbContextName}.")]
    private partial void LogCompletedMigration(string dbContextName);
}
