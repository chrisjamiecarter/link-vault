using LinkVault.Constants;

namespace Aspire.AppHost;

internal static class DependencyInjection
{
    public static IResourceBuilder<SqlServerDatabaseResource> AddDatabase(
        this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddSqlServer(ResourceNames.SqlServer.Name)
            .WithContainerName(ResourceNames.SqlServer.Name)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume()
            .AddDatabase(ResourceNames.Database.Name);
    }

    public static IResourceBuilder<ProjectResource> AddApi(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> database)
    {
        return builder
            .AddProject<Projects.LinkVault_Web_Api>(ResourceNames.WebApi.Name)
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
            .AddProject<Projects.LinkVault_Web_Blazor>(ResourceNames.WebBlazor.Name)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(api)
            .WaitFor(api);
    }
}
