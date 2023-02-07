#!/bin/bash
az acr login --name citizendemoacreastus
cd ..
docker build . -t citizendemo/citizenapi:latest -f CitizenDemo.CitizenAPI/Dockerfile-K8s
docker tag citizendemo/citizenapi:latest citizendemoacreastus.azurecr.io/citizendemo/citizenapi
docker push citizendemoacreastus.azurecr.io/citizendemo/citizenapi

docker build . -t citizendemo/resourceapi:latest -f CitizenDemo.ResourceAPI/Dockerfile-K8s
docker tag citizendemo/resourceapi:latest citizendemoacreastus.azurecr.io/citizendemo/resourceapi
docker push citizendemoacreastus.azurecr.io/citizendemo/resourceapi

docker build . -t citizendemo/loadgenerator:latest -f CitizenDemo.LoadGenerator/Dockerfile-K8s
docker tag citizendemo/loadgenerator:latest citizendemoacreastus.azurecr.io/citizendemo/loadgenerator
docker push citizendemoacreastus.azurecr.io/citizendemo/loadgenerator
