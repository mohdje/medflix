# How to deploy backend 

## Build docker image
docker build --rm -t medflix-app:latest .

## Make a new image that is linked to the docker hub repo
docker image tag medflix-app:latest djemo/medflix:latest

## Push the image to the repo
docker image push djemo/medflix:latest