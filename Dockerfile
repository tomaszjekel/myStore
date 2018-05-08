FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY myStore/src/MyStore/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY myStore/src/MyStore/. ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT production
ENTRYPOINT dotnet myStore/src/MyStore.dll