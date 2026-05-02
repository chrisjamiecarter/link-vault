using Aspire.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddDatabase();
var api = builder.AddApi(database);
builder.AddFrontend(api);

await builder.Build().RunAsync();
