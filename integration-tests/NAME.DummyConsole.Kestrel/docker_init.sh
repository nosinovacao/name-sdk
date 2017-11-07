#!/bin/bash
dotnet restore --source /service/nugets --source https://api.nuget.org/v3/index.json
# Compile before the tests so that the CI reports the build as failed if the tests compilation fails
dotnet build -c Release

dotnet run
