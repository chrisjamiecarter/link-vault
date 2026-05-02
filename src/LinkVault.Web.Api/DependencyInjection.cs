using LinkVault.Constants;
using LinkVault.Web.Api.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Web.Api;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var connectionString = builder.Configuration.GetConnectionString(Resources.Database.Name) ?? throw new InvalidOperationException($"Connection string '{Resources.Database.Name}' not found.");

        builder.Services.AddDbContext<LinkVaultDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        builder.EnrichSqlServerDbContext<LinkVaultDbContext>();

        return builder;
    }
}
