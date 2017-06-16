#!/bin/bash
dotnet restore --source /integration/nugets --source https://api.nuget.org/v3/index.json
# Compile before the tests so that the CI reports the build as failed if the tests compilation fails
dotnet build -c Release

errorCode=$?
if [ $errorCode -ne 0 ]
then
    printf "Build failed."
    exit $errorCode
fi

# ./wait-for-it.sh $LATEST_MONGO_HOSTNAME:27017 -t 20
# ./wait-for-it.sh $SPECIFIC_MONGO_HOSTNAME:27017 -t 20
# ./wait-for-it.sh $LATEST_RABBIT_HOSTNAME:5672 -t 20
# ./wait-for-it.sh $SPECIFIC_RABBIT_HOSTNAME:5672 -t 20
# ./wait-for-it.sh $LATEST_SQLSERVER_HOSTNAME:1433 -t 20


# This should be replace by Docker Healthcheck s when docker-compose adds support for it
./multi-wait-for-it.sh $LATEST_SQLSERVER_HOSTNAME:1433 $SPECIFIC_RABBIT_HOSTNAME:5672 $LATEST_RABBIT_HOSTNAME:5672 $SPECIFIC_MONGO_HOSTNAME:27017 $LATEST_MONGO_HOSTNAME:27017 $SPECIFIC_SERVICE_HOSTNAME:5000 -t 40

errorCode=$?
if [ $errorCode -ne 0 ]
then
    printf "ERROR: The dependencies containers did not start in the expected timeout."
    exit $errorCode
fi

# We do not need to exit with an error code on tests failing, the CI will look for the tests report file mark the build FAILED if it does not exist.
dotnet test -c Release --no-build --logger "trx;LogFileName=/integration/TestResults/integrationTests.trx" --filter "TestCategory=Integration"

errorCode=$?
if [ $errorCode -ne 0 ]
then
    printf "Tests failed with exit code '$errorCode'. Hiding the exit code."
fi
exit 0
