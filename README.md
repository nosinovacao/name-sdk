# NAME: Self-Contained Dependencies Management
NAME is a service dependencies management library designed to expose and access services information, written in C#. 

Its goal is to make it easier and simpler to detect service dependencies problems in a world where there are more and more services per application. It provides human and machine friendly dependencies information without the need for external tools.

### Main Features
* Self-contained
* Developer friendly service dependencies definition
* Human and machine readable service dependencies status
* Deep dependencies health checks
* Third-party service dependencies checks
* Support for **optional** central registration
* .Net Core and .NET 4.5+ compatible

## Getting Started 
### Installing on ASP.NET Web API

    Install-Package NAME.WebApi

The configuration file is present in `App_Start/NAMEConfig.cs` and the dependencies definition file is `dependencies.json`.

### Installing on ASP.NET Core
1. Install the NuGet package

    Install-Package NAME.AspNetCore

2. Add the NAME middleware in the *Configure* method of your *Startup.cs* file.
```csharp
// NAME middleware should be registered first so that the custom
// header is set before any other middleware has a chance to 
// send a response.
app.UseNAME(config =>
{
    Assembly a = typeof(Startup).GetTypeInfo().Assembly;
    config.APIName = a.GetName().Name;
    config.APIVersion = a.GetName().Version.ToString();
});
```

3. Create the `dependencies.json` file at the root of the project, the following example is a good starting point.
```json
{
  "infrastructure_dependencies": [
    {
      "os_name": "debian",
      "type": "OperatingSystem",
      "min_version": "8",
      "max_version": "*"
    }
  ],
  "service_dependencies": [
    {
      "name": "Internal Service",
      "min_version": "1.2.53",
      "max_version": "1.8",
      "connection_string": {
        "locator": "JSONPath",
        "file": "appsettings.json",
        "expression": "ConnectionStrings.InternalServiceUri"
      }
    }
  ]
}
```
## Documentation
See the [Wiki](https://github.com/nosinovacao/name-sdk/wiki) for full documentation, examples, operational details and other information.

## Building and testing
We define our build using [Cake](https://github.com/cake-build/cake/), this allows us to define a common ground for developers on different operating systems, but it requires .Net 4.5 or Mono 4.2.3, so make sure you have those dependencies setup.

Bootstrap scripts are provided for both Windows and Linux environments. Keep in mind that not all tests will run on Linux, because some projects are targeting .NET 4.5+.

To build and run unit tests on Windows execute the command:
    
    powershell ./build.ps1

To build and run unit tests on Linux execute the command:

    ./build.sh

<!--
## What does it do?
#### 1) Developer friendly dependencies defintion
Specify your dependencies in a developer friendly JSON format. Keep you existing connection settings, using built-in locators.

#### 2) Deep dependencies checks
Realtime recursive dependencies health check, including minimum/maximum version checks.

Optionally abort the application startup if any dependency reports an unhealthy state.
#### 3) Dependencies status exposure
Expose the current status of the dependencies through a manifest endpoint in a machine readable JSON format and human friendly table layout.
#### 4) Register your services in a central location
Optionally register all your services against a central registration service, with regular health checks and manifest snapshots using the established registration interface.

You can use our [existing Registry solution](https://github.com/nosinovacao//name-registry-api) for central registration.
-->

## Contributing
We really appreciate your interest in contributing to NAME. 👍

All we ask is that you follow some simple guidelines, so please read the [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

Thank you, [contributors](https://github.com/nosinovacao/name-sdk/graphs/contributors)!

## License
Copyright © NOS Inovaçâo.

This project is licensed under the BSD 3-Clause License - see the [LICENSE](LICENSE) file for details