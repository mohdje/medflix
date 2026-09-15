#Build react client and backoffice apps
FROM node:22-alpine AS frontend 

WORKDIR /medflix-frontend

COPY ./Client/ReactApp .

RUN npm ci

RUN npm run build
RUN mv build client_build

RUN npm run backoffice-build
RUN mv build backoffice_build

#Build .Net App
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backendbuild

WORKDIR /medflix-build

COPY ./Backend/WebHostStreaming/WebHostStreaming /WebHostStreaming

WORKDIR /WebHostStreaming

RUN rm -rf ./wwwroot/home/*
COPY --from=frontend /medflix-frontend/client_build ./wwwroot/home

RUN rm -rf ./wwwroot/backoffice/*
COPY --from=frontend /medflix-frontend/backoffice_build ./wwwroot/backoffice

RUN dotnet publish "./WebHostStreaming.csproj" -c Release -o /release

#Serve .Net App
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /medflix-app

COPY --from=backendbuild /release .

EXPOSE 5000

ENTRYPOINT [ "dotnet", "MedflixWebHost.dll" ]