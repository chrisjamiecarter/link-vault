using LinkVault.Constants;
using LinkVault.Web.Api;
using LinkVault.Web.Api.Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<LinkVaultDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString(ResourceNames.Database.Name) ?? throw new InvalidOperationException($"Connection string '{ResourceNames.Database.Name}' not found.");
    options.UseSqlServer(connectionString);
});

builder.EnrichSqlServerDbContext<LinkVaultDbContext>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

await app.RunAsync();
