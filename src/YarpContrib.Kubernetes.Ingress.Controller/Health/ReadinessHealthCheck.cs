using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace YarpContrib.Kubernetes.Ingress.Controller.Health;

internal class ReadinessHealthCheck(
  IHttpClientFactory httpClientFactory,
  IOptions<ReadinessHealthCheck.Options> options) 
  : IHealthCheck
{
  public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
  {
      if (options.Value.IsStandalone)
      {
          return HealthCheckResult.Healthy("Running in standalone mode.");
      }

      if (!string.IsNullOrEmpty(options.Value.ControllerUrl))
      {
          var client = httpClientFactory.CreateClient("liveness-check-client");
          var response = await client.GetAsync(options.Value.ControllerUrl, cancellationToken);

          if (response.IsSuccessStatusCode)
          {
              return HealthCheckResult.Healthy("Controller is reachable.");
          }

          return HealthCheckResult.Unhealthy($"Controller is not reachable. Status code: {response.StatusCode}");
      }

      return HealthCheckResult.Unhealthy("Controller URL is not configured.");
  }

  public sealed class Options 
  {
      public bool IsStandalone { get; set; }
      public string? ControllerUrl { get; set; }
  }
}