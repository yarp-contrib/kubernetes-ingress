using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Yarp.Kubernetes.Protocol;

var builder = WebApplication.CreateBuilder(args);

using var serilog = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(serilog, dispose: false);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args, new Dictionary<string, string>
{
    { "--controller-class", "Yarp.ControllerClass" },
    { "-c", "Yarp.ControllerClass" },
    { "--controller-service-name", "Yarp.ControllerServiceName" },
    { "-s", "Yarp.ControllerServiceName" },
    { "--controller-service-namespace", "Yarp.ControllerServiceNamespace" },
    { "-n", "Yarp.ControllerServiceNamespace" },
    { "--monitor-url", "Yarp.ControllerUrl" },
    { "-m", "Yarp.ControllerUrl" }
});

var isStandalone = string.IsNullOrEmpty(builder.Configuration["Yarp.ControllerUrl"]);

builder.WebHost.UseKubernetesReverseProxyCertificateSelector();
builder.Services.AddHealthChecks();

if (isStandalone)
{
    builder.Services
        .AddKubernetesReverseProxy(builder.Configuration);
}
else
{
    builder.Services
        .Configure<ReceiverOptions>(builder.Configuration.Bind)
        .AddHostedService<Receiver>()
        .AddReverseProxy()
        .LoadFromMessages();
}

var app = builder.Build();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true
});

app.MapReverseProxy();

app.Run();
