namespace LinkVault.Web.Api;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApi(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();

        return builder;
    }
}
