using Aspire.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddDatabase();
var backend = builder.AddBackend(database);
builder.AddFrontend(backend);

await builder.Build().RunAsync();
