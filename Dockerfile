FROM microsoft/dotnet:latest
COPY . /tomo
WORKDIR /tomo

RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
RUN dotnet publish . -c Release -o out
ENV ASPNETCORE_ENVIRONMENT production
###### Temporary
RUN apt-get update
RUN apt-get install -y smbclient
RUN apt-get clean && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
RUN mkdir /Upload
RUN mount -t cifs $STORAGE_HOST /Upload -o vers=3.0,username=$STORAGE_LOGIN,password=$STORAGE_PASSWORD,dir_mode=0777,file_mode=0777,sec=ntlmssp
######
ENTRYPOINT ["dotnet", "/tomo/src/MyStore/out/MyStore.dll"]
