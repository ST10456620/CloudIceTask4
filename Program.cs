using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using CoffeeNChill.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddSingleton(new CoffeeNChill.Services.TableStorageService("UseDevelopmentStorage=true"));

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

//Service

var host = new HostBuilder()
.ConfigureFunctionsWebApplication()
.ConfigureServices(services =>{
    services.AddSingleton(sp =>
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureFilesConnection");
        return new FileShareService(connectionString!, "staff-docs");
    });
}).Build();

host.Run();
builder.Build().Run();
