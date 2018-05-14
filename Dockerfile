FROM debian:latest

#### install packages needed
RUN apt update
RUN apt install -y sudo wget gpg apt-transport-https git screen

#### install dotnet
RUN wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
RUN sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
RUN wget -q https://packages.microsoft.com/config/debian/9/prod.list
RUN sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
RUN sudo apt-get update
RUN apt install -y  dotnet-sdk-2.0.0
####

RUN mkdir /Upload

ENV DB_HOST=$DB_HOST
ENV USER_NAME=$USER_NAME
ENV PASS=$PASS
ENV UPLOAD_DIR=$UPLOAD_DIR

#### Git
RUN git clone https://github.com/tomaszjekel/myStore.git
RUN dotnet publish ./myStore -c Release -o out
####

WORKDIR /myStore/src/MyStore/out/
ENTRYPOINT ["dotnet",  "/myStore/src/MyStore/out/MyStore.dll"]