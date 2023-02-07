#!/bin/bash
kubectl delete -f resourceapi.deployment.yaml
kubectl delete -f citizenapi.deployment.yaml
kubectl delete -f loadgenerator.deployment.yaml
kubectl delete -f jaeger.deployment.yaml
kubectl delete -f citizendemo.configmap.yaml
kubectl delete -f citizendemo.secret.yaml
kubectl delete -f citizendemo.ns.yaml


kubectl get all -n citizendemo