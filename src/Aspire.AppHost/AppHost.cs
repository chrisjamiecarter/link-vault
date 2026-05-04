namespace Aspire.AppHost;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var database = builder.AddDatabase();
        var cache = builder.AddCache();

        var migrator = builder.AddMigrator(database);
        var backend = builder.AddBackend(database, cache, migrator);
        builder.AddFrontend(backend);

        await builder.Build().RunAsync();
    }
}
