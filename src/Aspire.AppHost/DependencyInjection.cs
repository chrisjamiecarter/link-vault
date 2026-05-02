using LinkVault.Constants;

namespace Aspire.AppHost;

internal static class DependencyInjection
{
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

    public static IResourceBuilder<ProjectResource> AddApi(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> database)
    {
        return builder
            .AddProject<Projects.LinkVault_Web_Api>(Resources.WebApi.Name)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(database)
            .WaitFor(database);
    }

    public static IResourceBuilder<ProjectResource> AddFrontend(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> api)
    {
        return builder
            .AddProject<Projects.LinkVault_Web_Blazor>(Resources.WebBlazor.Name)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(api)
            .WaitFor(api);
    }
}
