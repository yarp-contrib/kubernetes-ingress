using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
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
    { "--controller-class", "Yarp:ControllerClass" },
    { "-c", "Yarp:ControllerClass" },
    { "--controller-service-name", "Yarp:ControllerServiceName" },
    { "-s", "Yarp:ControllerServiceName" },
    { "--controller-service-namespace", "Yarp:ControllerServiceNamespace" },
    { "-n", "Yarp:ControllerServiceNamespace" },

    { "--monitor-url", "ControllerUrl" },
    { "-m", "ControllerUrl" },

    { "--server-certificate-enabled", "Yarp:ServerCertificates" },
    { "--default-ssl-certificate-secret-name", "Yarp:DefaultSslCertificate" }
});

var isStandalone = string.IsNullOrEmpty(builder.Configuration["ControllerUrl"]);

if (isStandalone)
{
    builder.WebHost.UseKubernetesReverseProxyCertificateSelector();
    builder.Services.AddKubernetesReverseProxy(builder.Configuration);
}
else
{
    builder.Services
        .AddHealthChecks()
        .Services
        .Configure<ReceiverOptions>(builder.Configuration.Bind)
        .AddHostedService<Receiver>()
        .AddReverseProxy()
        .LoadFromMessages();
}

var app = builder.Build();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.MapReverseProxy();

app.Run();
