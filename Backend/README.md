# How to deploy backend 

## Build docker image
docker build --rm -t medflix-app:latest .

## Make a new image that is linked to the docker hub repo
docker image tag medflix-app:latest djemo/medflix:latest

## Push the image to the repo
docker image push djemo/medflix:latest

## Pull the image from the repo from the server
docker image pull djemo/medflix:latest

## Run medlix docker image
docker run -p 5000:5000 -e ASPNETCORE_HTTP_PORT=5000 -e ASPNETCORE_URLS="http://+:5000" --mount source=medflixVol,target=/medflix-app/storage djemo/medflix:latest

