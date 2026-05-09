using LinkVault.Constants;
using Projects;

namespace Aspire.AppHost;

internal static class DependencyInjection
{
    public static IResourceBuilder<ProjectResource> AddBackend(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> database,
        IResourceBuilder<RedisResource> cache,
        IResourceBuilder<ProjectResource> completionDependency)
    {
        return builder
            .AddProject<WebApiProject>(Resources.WebApi.Name)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(database)
            .WaitFor(database)
            .WithReference(cache)
            .WaitFor(cache)
            .WaitForCompletion(completionDependency);
    }

    public static IResourceBuilder<RedisResource> AddCache(
        this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddRedis(Resources.Cache.Name)
            .WithContainerName(Resources.Cache.Name)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume(Resources.Cache.DataVolume)
            .WithRedisInsight(config =>
            {
                // This is the container name.
                config.WithContainerName(Resources.Cache.RedisInsight);
            },
            // This is the resource name.
            containerName: Resources.Cache.RedisInsight);
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddDatabase(
        this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddSqlServer(Resources.SqlServer.Name)
            .WithContainerName(Resources.SqlServer.Name)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume(Resources.SqlServer.DataVolume)
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
