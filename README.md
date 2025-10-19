# YARP Kubernetes Ingress

Kubernetes ingress using [YARP](https://github.com/dotnet/yarp) as a reverse proxy and load balancer.

## Modes

The ingress solution can run in two modes:

- Standalone controller: a single application that both watches cluster Ingress resources and handles routing.
- Controller + Monitor: the monitor watches cluster Ingress resources and builds an YARP configuration; the controller retrieves that configuration from the monitor and performs routing.

## Images

Pre-built images are published to GitHub Container Registry.

### Controller

```sh
docker pull ghcr.io/yarp-contrib/kubernetes-ingress/controller:latest
```

Available input arguments:

- *--controller-class (-c)*: IngressClass controller name (maps to IngressClass.spec.controller). Default: `microsoft.com/ingress-yarp`
- *--controller-service-name (-s)*: Name of the Kubernetes Service where the controller is running. Default: `yarp-ingress`
- *--controller-service-namespace (-n)*: Namespace of the Kubernetes Service where the controller is running. Default: `yarp`
- *--monitor-url (-m)*: URL where the monitor exposes its dispatch endpoint. Must include the path */api/dispatch*. Required when controller and monitor run separately.

### Monitor

```sh
docker pull ghcr.io/yarp-contrib/kubernetes-ingress/monitor:latest
```

Available input arguments:

- *--controller-class (-c)*: IngressClass controller name (maps to IngressClass.spec.controller). Default: `microsoft.com/ingress-yarp`
- *--controller-service-name (-s)*: Name of the Kubernetes Service where the controller is running. Default: `yarp-ingress`
- *--controller-service-namespace (-n)*: Namespace of the Kubernetes Service where the controller is running. Default: `yarp`

## Installation

To install the controller on your cluster, there's available this [yarp-ingress](https://github.com/yarp-contrib/helm-charts) Helm chart. Based on the images above and it supports both modes.

## Considerations

- This controller uses [Yarp.Kubernetes.Controller](https://github.com/dotnet/yarp/tree/main/src/Kubernetes.Controller) internally and so it supports all its [optional Ingress annotations](https://github.com/dotnet/yarp/blob/main/samples/KubernetesIngress.Sample/README.md#annotations).
- Both applications (controller and monitor) make available `/health/live` and `/health/ready` endpoints on port *10264*. Currently they only return a 200/OK response, with no further internal logic.
- SSL/TLS termination is currently not (directly) supported.
