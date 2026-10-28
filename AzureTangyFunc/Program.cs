using AzureTangyFunc;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();
Startup.Configure(builder.Services, builder.Configuration);
builder.Build().Run();
