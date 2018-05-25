FROM aspnetcoremvc/deb

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