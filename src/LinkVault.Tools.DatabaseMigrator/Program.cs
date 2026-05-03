using Aspire.ServiceDefaults;
using LinkVault.Core;

namespace LinkVault.Tools.DatabaseMigrator;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();
        builder.AddCore();
        builder.AddDatabaseMigrator();

        var host = builder.Build();
        await host.RunAsync();
    }
}
