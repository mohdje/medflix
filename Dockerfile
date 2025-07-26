#Build react app
FROM node:18-alpine AS frontbuild 

WORKDIR /medflix-frontend

COPY ./Client/ReactApp/package*.json .

RUN npm install

COPY ./Client/ReactApp .

RUN npm run build

#Build .Net App
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backendbuild

WORKDIR /medflix-build

#Copy dll with same path inside image because csproj is using this relative path
COPY ./Backend/MoviesAPI/MoviesAPI/bin/Release/net8.0 /MoviesAPI/MoviesAPI/bin/Release/net8.0
COPY ./Backend/WebHostStreaming /WebHostStreaming

WORKDIR /WebHostStreaming/WebHostStreaming

RUN rm -rf /wwwroot/home/*
COPY --from=frontbuild /medflix-frontend/build /wwwroot/home

RUN dotnet restore "./WebHostStreaming.csproj" --disable-parallel 
RUN dotnet publish "./WebHostStreaming.csproj"  -c release -o /release --no-restore

#Serve .Net App
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /medflix-app

COPY --from=backendbuild /release .

EXPOSE 5000

ENTRYPOINT [ "dotnet", "MedflixWebHost.dll" ]