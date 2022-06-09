#!/bin/bash

# dotnet restore --source /integration/nugets --source https://api.nuget.org/v3/index.json
# Compile before the tests so that the CI reports the build as failed if the tests compilation fails
dotnet build -c Release

errorCode=$?
if [ $errorCode -ne 0 ]
then
    printf "Build failed."
    exit $errorCode
fi

# This should be replace by Docker Healthcheck s when docker-compose adds support for it
./multi-wait-for-it.sh $SPECIFIC_RABBIT_HOSTNAME:5672 $LATEST_RABBIT_HOSTNAME:5672 $SPECIFIC_MONGO_HOSTNAME:27017 $LATEST_MONGO_HOSTNAME:27017 $SPECIFIC_SERVICE_HOSTNAME:5000 $LATEST_SQLSERVER_HOSTNAME:1433 $SPECIFIC_KESTREL_SELFHOST_HOSTNAME:40500 -t 120

errorCode=$?
if [ $errorCode -ne 0 ]
then
    printf "ERROR: The dependencies containers did not start in the expected timeout."
    exit $errorCode
fi

# We do not need to exit with an error code on tests failing, the CI will look for the tests report file mark the build FAILED if it does not exist.
dotnet test -c Release --no-build --logger "trx;LogFileName=/integration/TestResults/integrationTests.trx" --filter "TestCategory=Integration"
