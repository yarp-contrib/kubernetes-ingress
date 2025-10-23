# YARP Kubernetes Ingress Helm chart

This chart installs YARP as a Kubernetes Ingress resource using [pre-built images](../../README.md#images) based on [Yarp.Kubernetes.Controller](https://github.com/dotnet/yarp/tree/main/src/Kubernetes.Controller).

## Install

```console
helm repo add yarp-ingress https://yarp-contrib.github.io/kubernetes-ingress
helm repo update
```

Default (standalone) installation:

```console
helm install yarp-ingress yarp-ingress/yarp-ingress
```

## Configuration

To see all configurable options with detailed comments, see [values](#values) section below, or run these configuration commands:

```console
helm show values yarp-ingress/yarp-ingress
```

The chart installs in [standalone mode](../../README.md#modes) by default and is cloud-provider agnostic. You must provision the load balancer that fronts the ingress controller service yourself.

Below are some example configuration snippets for different scenarios to help you bootstrap your installation.

### Controller + Monitor

```yaml
monitor:
  enabled: true
```

### Change Ingress class default values

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

### Disable role-based access control (RBAC)

```yaml
rbac:
  create: false
```

### AWS public-facing NLB with SSL Termination

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

## Values

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| controller.affinity | object | `{}` |  |
| controller.autoscaling.enabled | bool | `false` |  |
| controller.autoscaling.maxReplicas | int | `10` |  |
| controller.autoscaling.minReplicas | int | `1` |  |
| controller.autoscaling.targetCPUUtilizationPercentage | int | `50` |  |
| controller.autoscaling.targetMemoryUtilizationPercentage | int | `50` |  |
| controller.containerPorts.http | int | `8000` |  |
| controller.containerPorts.https | int | `8443` |  |
| controller.containerSecurityContext.allowPrivilegeEscalation | bool | `false` |  |
| controller.containerSecurityContext.capabilities.add[0] | string | `"NET_BIND_SERVICE"` |  |
| controller.containerSecurityContext.capabilities.drop[0] | string | `"ALL"` |  |
| controller.containerSecurityContext.readOnlyRootFilesystem | bool | `true` |  |
| controller.containerSecurityContext.runAsGroup | int | `101` |  |
| controller.containerSecurityContext.runAsNonRoot | bool | `true` |  |
| controller.containerSecurityContext.runAsUser | int | `101` |  |
| controller.containerSecurityContext.seccompProfile.type | string | `"RuntimeDefault"` |  |
| controller.extraEnvVars | list | `[]` |  |
| controller.image.pullPolicy | string | `"IfNotPresent"` |  |
| controller.image.repository | string | `"ghcr.io/yarp-contrib/kubernetes-ingress/controller"` |  |
| controller.image.tag | string | `""` |  |
| controller.imagePullSecrets | list | `[]` |  |
| controller.livenessProbe.failureThreshold | int | `5` |  |
| controller.livenessProbe.httpGet.path | string | `"/health/live"` |  |
| controller.livenessProbe.httpGet.port | int | `10264` |  |
| controller.livenessProbe.httpGet.scheme | string | `"HTTP"` |  |
| controller.livenessProbe.initialDelaySeconds | int | `10` |  |
| controller.livenessProbe.periodSeconds | int | `10` |  |
| controller.livenessProbe.successThreshold | int | `1` |  |
| controller.livenessProbe.timeoutSeconds | int | `1` |  |
| controller.nodeSelector | object | `{}` |  |
| controller.pdb.minAvailable | int | `1` |  |
| controller.pdb.unhealthyPodEvictionPolicy | string | `""` |  |
| controller.podAnnotations | object | `{}` |  |
| controller.podLabels | object | `{}` |  |
| controller.podSecurityContext | object | `{}` |  |
| controller.readinessProbe.failureThreshold | int | `3` |  |
| controller.readinessProbe.httpGet.path | string | `"/health/ready"` |  |
| controller.readinessProbe.httpGet.port | int | `10264` |  |
| controller.readinessProbe.httpGet.scheme | string | `"HTTP"` |  |
| controller.readinessProbe.initialDelaySeconds | int | `10` |  |
| controller.readinessProbe.periodSeconds | int | `10` |  |
| controller.readinessProbe.successThreshold | int | `1` |  |
| controller.readinessProbe.timeoutSeconds | int | `1` |  |
| controller.replicaCount | int | `1` |  |
| controller.resources.requests.cpu | string | `"100m"` |  |
| controller.resources.requests.memory | string | `"90Mi"` |  |
| controller.service.annotations | object | `{}` | Annotations to be added to the external controller service. |
| controller.service.appProtocol | bool | `true` | Declare the app protocol of the external HTTP and HTTPS listeners or not. Supersedes provider-specific annotations for declaring the backend protocol. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#application-protocol |
| controller.service.clusterIP | string | `""` | Pre-defined cluster internal IP address of the external controller service. Take care of collisions with existing services. This value is immutable. Set once, it can not be changed without deleting and re-creating the service. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#choosing-your-own-ip-address |
| controller.service.clusterIPs | list | `[]` | Pre-defined cluster internal IP addresses of the external controller service. Take care of collisions with existing services. This value is immutable. Set once, it can not be changed without deleting and re-creating the service. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#choosing-your-own-ip-address |
| controller.service.enableHttp | bool | `true` | Enable the HTTP listener on both controller services or not. |
| controller.service.enableHttps | bool | `true` | Enable the HTTPS listener on both controller services or not. |
| controller.service.enabled | bool | `true` |  |
| controller.service.externalIPs | list | `[]` | List of node IP addresses at which the external controller service is available. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#external-ips |
| controller.service.externalTrafficPolicy | string | `""` | External traffic policy of the external controller service. Set to "Local" to preserve source IP on providers supporting it. Ref: https://kubernetes.io/docs/tasks/access-application-cluster/create-external-load-balancer/#preserving-the-client-source-ip |
| controller.service.ipFamilies | list | `["IPv4"]` | List of IP families (e.g. IPv4, IPv6) assigned to the external controller service. This field is usually assigned automatically based on cluster configuration and the `ipFamilyPolicy` field. Ref: https://kubernetes.io/docs/concepts/services-networking/dual-stack/#services |
| controller.service.ipFamilyPolicy | string | `"SingleStack"` | Represents the dual-stack capabilities of the external controller service. Possible values are SingleStack, PreferDualStack or RequireDualStack. Fields `ipFamilies` and `clusterIP` depend on the value of this field. Ref: https://kubernetes.io/docs/concepts/services-networking/dual-stack/#services |
| controller.service.labels | object | `{}` | Labels to be added to both controller services. |
| controller.service.loadBalancerClass | string | `""` | Load balancer class of the external controller service. Used by cloud providers to select a load balancer implementation other than the cloud provider default. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#load-balancer-class |
| controller.service.loadBalancerIP | string | `""` | Deprecated: Pre-defined IP address of the external controller service. Used by cloud providers to connect the resulting load balancer service to a pre-existing static IP. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#loadbalancer |
| controller.service.loadBalancerSourceRanges | list | `[]` | Restrict access to the external controller service. Values must be CIDRs. Allows any source address by default. |
| controller.service.nodePorts.http | string | `""` | Node port allocated for the external HTTP listener. If left empty, the service controller allocates one from the configured node port range. |
| controller.service.nodePorts.https | string | `""` | Node port allocated for the external HTTPS listener. If left empty, the service controller allocates one from the configured node port range. |
| controller.service.nodePorts.tcp | object | `{}` | Node port mapping for external TCP listeners. If left empty, the service controller allocates them from the configured node port range. Example: tcp:   8080: 30080 |
| controller.service.nodePorts.udp | object | `{}` | Node port mapping for external UDP listeners. If left empty, the service controller allocates them from the configured node port range. Example: udp:   53: 30053 |
| controller.service.ports.http | int | `80` | Port the external HTTP listener is published with. |
| controller.service.ports.https | int | `443` | Port the external HTTPS listener is published with. |
| controller.service.sessionAffinity | string | `""` | Session affinity of the external controller service. Must be either "None" or "ClientIP" if set. Defaults to "None". Ref: https://kubernetes.io/docs/reference/networking/virtual-ips/#session-affinity |
| controller.service.targetPorts.http | string | `"http"` | Port of the ingress controller the external HTTP listener is mapped to. |
| controller.service.targetPorts.https | string | `"https"` | Port of the ingress controller the external HTTPS listener is mapped to. |
| controller.service.trafficDistribution | string | `""` | Traffic distribution policy of the external controller service. Set to "PreferClose" to route traffic to endpoints that are topologically closer to the client. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#traffic-distribution |
| controller.service.type | string | `"LoadBalancer"` | Type of the external controller service. Ref: https://kubernetes.io/docs/concepts/services-networking/service/#publishing-services-service-types |
| controller.serviceAccount.annotations | object | `{}` |  |
| controller.serviceAccount.automount | bool | `true` |  |
| controller.serviceAccount.create | bool | `true` |  |
| controller.serviceAccount.name | string | `""` |  |
| controller.tolerations | list | `[]` |  |
| fullnameOverride | string | `""` |  |
| ingressClass.aliases | list | `[]` | List of additional IngressClass resources to be created as named aliases to the main IngressClass controller defined above |
| ingressClass.annotations | object | `{}` |  |
| ingressClass.controllerValue | string | `"microsoft.com/ingress-yarp"` |  |
| ingressClass.default | bool | `false` |  |
| ingressClass.name | string | `"yarp"` |  |
| ingressClass.parameters | object | `{}` |  |
| monitor.affinity | object | `{}` |  |
| monitor.autoscaling.enabled | bool | `false` |  |
| monitor.autoscaling.maxReplicas | int | `10` |  |
| monitor.autoscaling.minReplicas | int | `1` |  |
| monitor.autoscaling.targetCPUUtilizationPercentage | int | `50` |  |
| monitor.autoscaling.targetMemoryUtilizationPercentage | int | `50` |  |
| monitor.containerPorts.http | int | `8000` |  |
| monitor.containerSecurityContext.allowPrivilegeEscalation | bool | `false` |  |
| monitor.containerSecurityContext.capabilities.drop[0] | string | `"ALL"` |  |
| monitor.containerSecurityContext.readOnlyRootFilesystem | bool | `true` |  |
| monitor.containerSecurityContext.runAsGroup | int | `101` |  |
| monitor.containerSecurityContext.runAsNonRoot | bool | `true` |  |
| monitor.containerSecurityContext.runAsUser | int | `101` |  |
| monitor.containerSecurityContext.seccompProfile.type | string | `"RuntimeDefault"` |  |
| monitor.enabled | bool | `false` | Enable deployment of the monitor component as a separate service from the controller |
| monitor.extraEnvVars | list | `[]` |  |
| monitor.image.pullPolicy | string | `"IfNotPresent"` |  |
| monitor.image.repository | string | `"ghcr.io/yarp-contrib/kubernetes-ingress/monitor"` |  |
| monitor.image.tag | string | `""` |  |
| monitor.imagePullSecrets | list | `[]` |  |
| monitor.livenessProbe.failureThreshold | int | `5` |  |
| monitor.livenessProbe.httpGet.path | string | `"/health/live"` |  |
| monitor.livenessProbe.httpGet.port | int | `10264` |  |
| monitor.livenessProbe.httpGet.scheme | string | `"HTTP"` |  |
| monitor.livenessProbe.initialDelaySeconds | int | `10` |  |
| monitor.livenessProbe.periodSeconds | int | `10` |  |
| monitor.livenessProbe.successThreshold | int | `1` |  |
| monitor.livenessProbe.timeoutSeconds | int | `1` |  |
| monitor.nodeSelector | object | `{}` |  |
| monitor.pdb.minAvailable | int | `1` |  |
| monitor.pdb.unhealthyPodEvictionPolicy | string | `""` |  |
| monitor.podAnnotations | object | `{}` |  |
| monitor.podLabels | object | `{}` |  |
| monitor.podSecurityContext | object | `{}` |  |
| monitor.readinessProbe.failureThreshold | int | `3` |  |
| monitor.readinessProbe.httpGet.path | string | `"/health/ready"` |  |
| monitor.readinessProbe.httpGet.port | int | `10264` |  |
| monitor.readinessProbe.httpGet.scheme | string | `"HTTP"` |  |
| monitor.readinessProbe.initialDelaySeconds | int | `10` |  |
| monitor.readinessProbe.periodSeconds | int | `10` |  |
| monitor.readinessProbe.successThreshold | int | `1` |  |
| monitor.readinessProbe.timeoutSeconds | int | `1` |  |
| monitor.replicaCount | int | `1` |  |
| monitor.resources.requests.cpu | string | `"100m"` |  |
| monitor.resources.requests.memory | string | `"90Mi"` |  |
| monitor.service.annotations | object | `{}` |  |
| monitor.serviceAccount.annotations | object | `{}` |  |
| monitor.serviceAccount.automount | bool | `true` |  |
| monitor.serviceAccount.create | bool | `true` |  |
| monitor.serviceAccount.name | string | `""` |  |
| monitor.tolerations | list | `[]` |  |
| nameOverride | string | `""` |  |
| rbac.create | bool | `true` |  |