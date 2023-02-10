#!/bin/bash
az aks get-credentials --resource-group CitizenDemo-EastUS --name citizendemo-aks-eastus

kubectl config use-context citizendemo-aks-eastus

kubectl apply -f citizendemo.ns.yaml
kubectl apply -f citizendemo.secret.yaml
kubectl apply -f citizendemo.configmap.yaml

kubectl apply -f jaeger.deployment.yaml

kubectl apply -f resourceapi.deployment.yaml
kubectl apply -f citizenapi.deployment.yaml
kubectl apply -f loadgenerator.deployment.yaml

kubectl get all -n citizendemo

kubectl create configmap ama-metrics-prometheus-config --from-file=prometheus-config -n kube-system