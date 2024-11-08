#Build react app
# FROM node:15.4 as frontbuild 

# WORKDIR /medflix-frontend

# COPY /development/Front/react-streaming-client/package*.json .

# RUN npm install

# COPY /development/Front/react-streaming-client .

# RUN npm run build --prefix

#Build .Net App
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backendbuild

WORKDIR /medflix-build

#Copy dll with same path inside image because csproj is using this relative path
COPY ./MoviesAPI/MoviesAPI/bin/Release/net8.0 /MoviesAPI/MoviesAPI/bin/Release/net8.0
COPY ./WebHostStreaming /WebHostStreaming

WORKDIR /WebHostStreaming/WebHostStreaming

RUN dotnet restore "./WebHostStreaming.csproj" --disable-parallel 
RUN dotnet publish "./WebHostStreaming.csproj"  -c release -o /release --no-restore

WORKDIR /release

# COPY --from=frontbuild /medflix-frontend/build /release/view

#Serve .Net App
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /medflix-app

COPY --from=backendbuild /release .

EXPOSE 5000

ENTRYPOINT [ "dotnet", "MedflixWebHost.dll" ]