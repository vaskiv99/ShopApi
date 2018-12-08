#!/bin/sh

docker login e="$DOCKER_EMAIL" -u="$DOCKER_USER" -p="$DOCKER_PASS"
if [ "$TRAVIS_BRANCH" = "master" ]; then
    TAG="latest"
else
    TAG="$TRAVIS_BRANCH"

fi
docker build -t $TRAVIS_REPO_SLUG | tr '[:upper:]' '[:lower:]':$TAG .
docker push $TRAVIS_REPO_SLUG | tr '[:upper:]' '[:lower:]'