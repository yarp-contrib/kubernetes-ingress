using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

using var serilog = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(serilog, dispose: false);

builder.Configuration
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        { "Yarp:ControllerClass", "microsoft.com/ingress-yarp" },
        { "Yarp:ControllerServiceName", "yarp-ingress" },
        { "Yarp:ControllerServiceNamespace", "yarp" },
    }!)
    .AddEnvironmentVariables()
    .AddCommandLine(args, new Dictionary<string, string>
    {
        { "--controller-class", "Yarp:ControllerClass" },
        { "-c", "Yarp:ControllerClass" },
        { "--controller-service-name", "Yarp:ControllerServiceName" },
        { "-s", "Yarp:ControllerServiceName" },
        { "--controller-service-namespace", "Yarp:ControllerServiceNamespace" },
        { "-n", "Yarp:ControllerServiceNamespace" }
    });

builder.Services
    .AddHealthChecks()
    .Services
    .AddKubernetesIngressMonitor(builder.Configuration)
    .AddControllers()
    .AddKubernetesDispatchController();

var app = builder.Build();

app.UseRouting().UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health/live").RequireHost("*:10264");
    endpoints.MapHealthChecks("/health/ready").RequireHost("*:10264");
});

app.Run();