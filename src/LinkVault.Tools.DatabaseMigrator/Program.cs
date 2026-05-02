using Aspire.ServiceDefaults;
using LinkVault.Web.Api;

namespace LinkVault.Tools.DatabaseMigrator;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.AddInfrastructure();
        builder.AddPresentation();

        var host = builder.Build();
        await host.RunAsync();
    }
}
