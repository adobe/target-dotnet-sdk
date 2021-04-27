# Adobe Target .Net SDK

[![GitHub Actions Status](https://github.com/adobe/target-dotnet-sdk/workflows/Build/badge.svg?branch=main)](https://github.com/adobe/target-dotnet-sdk/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/adobe/target-dotnet-sdk?branch=main&includeBuildsFromPullRequest=false)](https://github.com/adobe/target-dotnet-sdk/actions)

The Adobe Target .Net SDK uses the [Target Delivery API] to retrieve and deliver personalized experiences.

# Table of contents

- [Adobe Target .Net SDK](#adobe-target-net-sdk)
- [Table of contents](#table-of-contents)
  - [Getting started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Build](#build)
  - [Releases](#releases)
    - [Using nugets built locally in your project](#using-nugets-built-locally-in-your-project)
    - [Contributing](#contributing)
    - [Licensing](#licensing)

## Getting started

### Prerequisites

At a minimum [.NET Core SDK 2.0](https://dotnet.microsoft.com/download/dotnet-core/2.0) is required to build, test, and generate NuGet packages.  
Note: the SDK itself multi-targets .NET Standard 2.0 and .NET 5

**macOS/Linux**

On macOS or Linux we recommend [Visual Studio Code](https://code.visualstudio.com/) with the [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp). See the [getting started guide](https://code.visualstudio.com/docs/languages/dotnet) for VS Code and .NET.

**Windows:**

On Windows, we recommend installing [the latest Visual Studio 2019](https://www.visualstudio.com/vs/).

### Installation  

To get started with Target Node.js SDK, just add it as a dependency by [installing from NuGet](https://www.nuget.org/packages/Adobe.Target.Client).

### Super Simple to Use

Please take a look at [our documentation](https://adobetarget-sdks.gitbook.io/docs/sdk-reference-guides/dotnet-sdk) to learn how to use the .NET SDK.

### Sample Apps

There's a couple of sample apps showing sample sync/async Target SDK usage under [SampleApp project](SampleApp).  
To switch between sync and async sample apps, just modify `StartupObject` property in [SampleApp project file](SampleApp/SampleApp.csproj) accordingly.

### Build

To build everything and generate NuGet packages, run [dotnet Cake](https://cakebuild.net/) CLI commands. Binaries and NuGet packages will be dropped in an *Artefacts* directory at the repo root.

```bash
# Build sdk, run tests, generate coverage and pack
dotnet cake

# Alternatively, run separate Cake tasks
dotnet cake --target=Build
dotnet cake --target=Test
dotnet cake --target=Pack
```

Each project can also be built individually directly through the CLI or your editor/IDE. You can open the solution file [Adobe.Target.Client.sln](Adobe.Target.Client.sln) in repo root to load all sdk, sample and test projects.

## Releases

We publish NuGet packages to [nuget.org](https://www.nuget.org/packages/Adobe.Target.Client) for each release.

### Contributing

Contributions are welcome! Read the [Contributing Guide](./.github/CONTRIBUTING.md) for more information.

### Licensing

This project is licensed under the Apache V2 License. See [LICENSE](LICENSE) for more information.

[back to top](#table-of-contents)

[Target Delivery API]: https://developers.adobetarget.com/api/delivery-api/
