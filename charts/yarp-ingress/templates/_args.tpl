{{- define "yarp-ingress.controller.args" -}}
- --environment=Production
- --urls=http://*:{{ .Values.controller.containerPorts.http }};http://*:{{ .Values.controller.containerPorts.https }};http://*:10264
- --controller-class={{ .Values.ingressClass.controllerValue }}
- --controller-service-name={{ include "yarp-ingress.fullname" . }}
- --controller-service-namespace={{ .Release.Namespace }}
{{- if .Values.monitor.enabled }}
- --monitor-url={{ printf "http://%s-monitor.%s.svc.cluster.local:8000" (include "yarp-ingress.fullname" .) .Release.Namespace }}/api/dispatch
{{- end }}
{{- end -}}

{{- define "yarp-ingress.monitor.args" -}}
- --environment=Production
- --urls=http://*:{{ .Values.monitor.containerPorts.http }};http://*:10264
- --controller-class={{ .Values.ingressClass.controllerValue }}
- --controller-service-name={{ include "yarp-ingress.fullname" . }}
- --controller-service-namespace={{ .Release.Namespace }}
{{- end -}}
