using Aspire.ServiceDefaults;

namespace LinkVault.Web.Api;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.AddInfrastructure();

        // Move these to a separate extension method.
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapGet("/", () => "API service is running.");

        await app.RunAsync();
    }
}
