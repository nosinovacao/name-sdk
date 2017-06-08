# NAME: Self-Contained Dependencies Management
NAME is a dependencies management library designed to expose and access dependencies information, written in C#. Its goal is to make it easier and simpler to detect dependencies problems in a world where there are more and more services per application. It provides human and machine friendly dependencies information without the need for external tools.

## Main Features
* Self-contained
* Developer friendly dependencies defintion
* Deep dependecies health checks
* Human and machine reada dependencies status
* Support for central registration
* Infrastructure checks
* .Net Core and .NET 4.5+ compatible.

## Documentation
See the [Wiki](./wiki) for full documentation, examples, operational details and other information.

## Building and testing
The build is defined using Cake. 

A build script is provided for both Windows and Linux environments. Keep in mind that not all tests will run on Linux, because of the projects targeting .Net 4.5.

On Windows execute the powershell script.
```powershell
powershell ./build.ps1
```

## What does it do?
#### 1) Developer friendly dependencies defintion
Specify your dependencies in a developer friendly JSON format. Keep you existing connection settings, using built-in locators.

#### 2) Deep dependecies checks
Realtime recursive dependencies health check, including minimum/maximum version checks.

Optionally abort the application startup if any dependency reports an unhealthy state
#### 3) Dependencies status exposure
Expose the current status of the dependencies through a manifest endpoint in a machine readable JSON format and human friendly table layout.
#### 4) Register your services in a central location
Optionally register all your services against a central registration service, with regular health checks and manifest snapshots using the established registration interface.

Use our [existing Registry solution](../name-registry-api) for central registration.
