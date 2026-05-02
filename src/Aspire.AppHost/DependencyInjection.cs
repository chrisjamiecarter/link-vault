using LinkVault.Constants;
using Projects;

namespace Aspire.AppHost;

internal static class DependencyInjection
{
    public static IResourceBuilder<ProjectResource> AddBackend(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> database,
        IResourceBuilder<ProjectResource> completionDependency)
    {
        return builder
            .AddProject<WebApiProject>(Resources.WebApi.Name)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(database)
            .WaitForCompletion(completionDependency);
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddDatabase(
    this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddSqlServer(Resources.SqlServer.Name)
            .WithContainerName(Resources.SqlServer.Name)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume(Resources.SqlServer.DataVolumne)
            .WithEndpointProxySupport(false)
            .WithHostPort(Resources.SqlServer.Port)
            .AddDatabase(Resources.Database.Name);
    }

    public static IResourceBuilder<ProjectResource> AddFrontend(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> api)
    {
        return builder
            .AddProject<WebBlazorProject>(Resources.WebBlazor.Name)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(api)
            .WaitFor(api);
    }

    public static IResourceBuilder<ProjectResource> AddMigrator(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> database)
    {
        return builder
            .AddProject<DatabaseMigratorProject>(Resources.DatabaseMigrator.Name)
            .WithReference(database)
            .WaitFor(database);
    }
}
