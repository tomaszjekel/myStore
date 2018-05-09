FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/MyStore/*.csproj ./
COPY src/MyStore.Domain/*.csproj ./
COPY src/MyStore.Infrastructure/*.csproj ./
COPY src/MyStore.Services/*.csproj ./

RUN dotnet restore

# Copy everything else and build
COPY src/. ./
RUN dotnet publish . -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT production
ENTRYPOINT dotnet src/MyStore.dll
