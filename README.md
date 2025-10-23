# YARP Kubernetes Ingress

Kubernetes ingress using [YARP](https://github.com/dotnet/yarp) as a reverse proxy and load balancer.

## Modes

The ingress solution can run in two modes:

- Standalone controller: a single application that both watches cluster Ingress resources and handles routing.
- Controller + Monitor: the monitor watches cluster Ingress resources and builds an YARP configuration; the controller retrieves that configuration from the monitor and performs routing.

## Installation

The only installation method currently supported is through [yarp-ingress](./charts/yarp-ingress/) Helm chart.

### Get repository information

```console
helm repo add yarp-ingress https://yarp-contrib.github.io/kubernetes-ingress
helm repo update
```

### Install chart

```console
helm install yarp-ingress yarp-ingress/yarp-ingress
```

This installs YARP ingress on the cluster in the default configuration.

Check chart's [README](./charts//yarp-ingress/README.md) and [values.yaml](./charts/yarp-ingress/values.yaml) for information on how to install in different modes and cloud providers.

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

## Considerations

- This controller uses [Yarp.Kubernetes.Controller](https://github.com/dotnet/yarp/tree/main/src/Kubernetes.Controller) internally and so it supports all its [optional Ingress annotations](https://github.com/dotnet/yarp/blob/main/samples/KubernetesIngress.Sample/README.md#annotations).
- Both applications (controller and monitor) make available `/health/live` and `/health/ready` endpoints on port *10264*. Currently they only return a 200/OK response, with no further internal logic.
- SSL/TLS termination is currently not (directly) supported. You can do the termination yourself, prior to sending the traffic to the controller's service.
