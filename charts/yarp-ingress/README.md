# YARP Kubernetes Ingress helm chart

This charts installs YARP, as a Kubernetes Ingress resource, using [pre-built images](../../README.md#images) based on [Yarp.Kubernetes.Controller](https://github.com/dotnet/yarp/tree/main/src/Kubernetes.Controller)

## Get Helm repository information

```console
helm repo add yarp-ingress https://yarp-contrib.github.io/kubernetes-ingress
helm repo update
```

## Install chart

Default (standlone) installation:

```console
helm install yarp-ingress yarp-ingress/yarp-ingress
```

### Configuration

To see all configurable options with detailed comments, visit the chart's [values.yaml](./values.yaml), or run these configuration commands:

```console
helm show values yarp-ingress/yarp-ingress
```

The chart installs in [standalone mode](../../README.md#modes) by default and is cloud-provider agnostic, meaning you will have to provision yourself the load-balancer in front of the ingress controller service.

Below some possible configuration values, for different scenarios, to help you bootstrap your installation.

#### Controller + Monitor

```yaml
monitor:
  enabled: true
```

#### Change Ingress class default values

```yaml
ingressClass:
  name: "yarp-internal" # Defaults to 'yarp'
  controllerValue: "microsoft.com/ingress-yarp-internal" # Defaults to microsoft.com/ingress-yarp
  default: true # Defaults to false
  # -- List of additional IngressClass resources to be created as named aliases to the main IngressClass controller defined above
  aliases:
    - yarp-internal-web
    - yarp-internal-backend
```

#### Disable role-based access control (RBAC)

```yaml
rbac:
  create: false
```

#### AWS public-facing NLB with SSL Termination

```yaml
controller:
  service:
    annotations:
      service.beta.kubernetes.io/aws-load-balancer-name: "yarp-public-ingress"
      service.beta.kubernetes.io/aws-load-balancer-scheme: internet-facing
      service.beta.kubernetes.io/aws-load-balancer-type: "external"
      service.beta.kubernetes.io/aws-load-balancer-nlb-target-type: ip
      service.beta.kubernetes.io/aws-load-balancer-ssl-ports: "https"
      service.beta.kubernetes.io/aws-load-balancer-cross-zone-load-balancing-enabled: "true"
      service.beta.kubernetes.io/aws-load-balancer-ssl-cert: "<ACM-CERTIFICATE-ARN>"
    
```
