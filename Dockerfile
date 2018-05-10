FROM microsoft/dotnet:latest
COPY . /tomo
WORKDIR /tomo

RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
RUN dotnet publish . -c Release -o out
ENV ASPNETCORE_ENVIRONMENT production
ENTRYPOINT ["dotnet", "/tomo/src/MyStore/out/MyStore.dll"]
