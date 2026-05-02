var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LinkVault_Web_Blazor>("linkvault-web-blazor")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();
