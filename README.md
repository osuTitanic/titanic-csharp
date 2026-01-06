# Titanic.API

A C# API wrapper for [Titanic!](https://osu.titanic.sh)

> [!CAUTION]
> This library uses the deprecated `WebClient` class intentionally to maintain compatibility with older .NET Framework versions and modded osu! clients. It is **not** recommended for modern C# applications.

## Installation

Add the NuGet package or reference the project directly:

```
dotnet add package Titanic.API
```

## Usage

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
