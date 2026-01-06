# titanic-csharp

C# libraries for [Titanic!](https://osu.titanic.sh)

> [!CAUTION]
> These libraries use deprecated `WebClient` intentionally to maintain compatibility with older .NET Framework versions and modded osu! clients. They are **not** recommended for modern C# applications.

## Packages

| Package           | Description                             |
|-------------------|-----------------------------------------|
| `Titanic.API`     | API wrapper for Titanic!                |
| `Titanic.Updater` | Updater library for modded osu! clients |

## Installation

Add the NuGet packages or reference the projects directly:

```
dotnet add package Titanic.API
dotnet add package Titanic.Updater
```

## Titanic.API Usage

```csharp
using Titanic.API;
using Titanic.API.Requests;

var api = new TitanicAPI();

// Blocking request
var user = new GetUserRequest(1).BlockingPerform(api);

// Async request with callbacks
new GetUserRequest(1).Perform(api,
    onSuccess: user => Console.WriteLine(user.Name),
    onError: ex => Console.WriteLine(ex.Message)
);

// Further documentation can be found here:
// https://api.titanic.sh/docs
```

## Titanic.Updater Usage

```csharp
using Titanic.Updater;
using Titanic.Updater.Models;
using Titanic.Updater.Versioning;

UpdateManagerSettings settings = new()
{
    Exit = () => Console.WriteLine("Exit called!"),
    ReplaceCurrentExecutable = false,
    IncludeClientIdentifierInOutputPath = true,
};

var clientInfo = new ModdedClientInformation
{
    ClientIdentifier = "my-client",
    VersionKind = OsuVersionKind.BuildNumber,
    InstalledVersion = "b20130815",
    InstalledStream = "stable"
};

UpdateManager manager = new(settings);
ModdedReleaseEntry? update = manager.CheckUpdateForClient(client);

if (update != null)
{
    if (!update.IsExtractable)
    {
        Console.WriteLine($"Update found: {update.Version}, but not extractable");
        return;
    }
    Console.WriteLine($"Update found: {update.Version}");
    DownloadedUpdate downloadedUpdate = manager.DownloadClientUpdate(client, update);
    Console.WriteLine($"Update downloaded to {downloadedUpdate.Path}");
    manager.InstallClientUpdate(downloadedUpdate);
}
```
