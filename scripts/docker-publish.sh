#!/bin/bash
docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD
docker build -t mystore .
docker tag mystore $DOCKER_USERNAME/mystore
docker push $DOCKER_USERNAME/mystore