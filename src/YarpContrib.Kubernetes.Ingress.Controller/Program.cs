using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Yarp.Kubernetes.Controller;
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
    { "-m", "ControllerUrl" }
});

    var options = new ReceiverOptions();
    builder.Configuration.Bind(options);

    var yarp = new YarpOptions();
    builder.Configuration.GetSection("Yarp").Bind(yarp);

    Console.WriteLine("Starting args " + string.Join(' ', args));
    Console.WriteLine("Starting config " + options.ControllerUrl + " - " + builder.Configuration["ControllerUrl"] + " - " + JsonSerializer.Serialize(yarp));

var isStandalone = string.IsNullOrEmpty(builder.Configuration["ControllerUrl"]);

if (isStandalone)
{
    builder.Services
        .AddKubernetesReverseProxy(builder.Configuration);
}
else
{
    builder.WebHost.UseKubernetesReverseProxyCertificateSelector();
    
    builder.Services
        .Configure<ReceiverOptions>(builder.Configuration.Bind)
        .AddHostedService<Receiver>()
        .AddReverseProxy()
        .LoadFromMessages();
}

var app = builder.Build();

app.Map("/health/live", async c => {
    var options = new ReceiverOptions();
    builder.Configuration.Bind(options);
    var logger = c.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Liveness check, controller url: {IsStandalone} {ControllerUrl}", isStandalone, options.ControllerUrl);
    await c.Response.WriteAsync("Alive");
});
app.Map("/health/ready", async c => await c.Response.WriteAsync("Ready"));

app.MapReverseProxy();

app.Run();
