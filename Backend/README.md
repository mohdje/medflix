# Warning
Before running following commands, make sure Docker Desktop is launched

# To build docker image
docker build --rm -t medflix-app:latest .

# To run medlix docker image
docker run -p 5000:5000 -e ASPNETCORE_HTTP_PORT=5000 -e ASPNETCORE_URLS="http://+:5000" --mount source=medflixVol,target=/medflix-app/storage djemo/medflix:latest

# Make a new image that is linked to the docker hub repo
docker image tag medflix-app:latest djemo/medflix:latest

# Push the image to the repo
docker image push djemo/medflix:latest