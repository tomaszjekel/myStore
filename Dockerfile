FROM microsoft/dotnet:latest
COPY . /tomo
WORKDIR /tomo
RUN mkdir /Upload

#RUN ["dotnet", "restore"]
#RUN ["dotnet", "build"]

ENV DB_HOST=$DB_HOST
ENV USER_NAME=$USER_NAME
ENV PASS=$PASS
ENV UPLOAD_DIR=$UPLOAD_DIR

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
RUN dotnet publish . -c Release -o out
ENV ASPNETCORE_ENVIRONMENT production

ENTRYPOINT ["dotnet", "/tomo/src/MyStore/out/MyStore.dll"]
