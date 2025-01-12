<img src="./docs/logo/Logo_Labeled.png" alt="logo" height="160"/>

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/gehongyan/Kook.Net/push.yml?branch=master)
![GitHub Top Language](https://img.shields.io/github/languages/top/gehongyan/Kook.Net)
[![Nuget Version](https://img.shields.io/nuget/v/Kook.Net)](https://www.nuget.org/packages/Kook.Net)
[![Nuget](https://img.shields.io/nuget/dt/Kook.Net?color=%230099ff)](https://www.nuget.org/packages/Kook.Net)
[![License](https://img.shields.io/github/license/gehongyan/Kook.Net)](https://github.com/gehongyan/Kook.Net/blob/master/LICENSE)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fgehongyan%2FKook.Net.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fgehongyan%2FKook.Net?ref=badge_shield)
[![Chat on Kook](https://www.kookapp.cn/api/v3/badge/guild?guild_id=1591057729615250)](https://kook.top/EvxnOb)

---

**English** | [简体中文](./README.zh-CN.md)

---

**Kook.Net** is an unofficial C# .NET implementation for [KOOK (KaiHeiLa formerly) API](https://developer.kookapp.cn/doc/intro).

---

## Source & Documentation

Source code is available on [GitHub](https://github.com/gehongyan/Kook.Net),
[Gitee](https://gitee.com/gehongyan/Kook.Net), and [GitCode](https://gitcode.com/gehongyan/Kook.Net).

Documents are available on [kooknet.dev](https://kooknet.dev). (Simplified Chinese available only)

---

## Targets

- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET Standard 2.1](https://learn.microsoft.com/dotnet/standard/net-standard?tabs=net-standard-2-1)
- [.NET Standard 2.0](https://learn.microsoft.com/dotnet/standard/net-standard?tabs=net-standard-2-0)
- [.NET Framework 4.6.2](https://dotnet.microsoft.com/download/dotnet-framework/net462)

> [!TIP]
> Targets other than .NET 9.0 have not been fully tested.

---

## Installation

### Main Package

The main package provides all implementations of official APIs.

- Kook.Net: [NuGet](https://www.nuget.org/packages/Kook.Net/), [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net)

### Individual Packages

Individual components of the main package can be installed separately. These packages are included in the main package.

- Kook.Net.Core: [NuGet](https://www.nuget.org/packages/Kook.Net.Core/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Core)
- Kook.Net.Rest: [NuGet](https://www.nuget.org/packages/Kook.Net.Rest/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Rest)
- Kook.Net.WebSocket: [NuGet](https://www.nuget.org/packages/Kook.Net.WebSocket/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.WebSocket)
- Kook.Net.Webhook: [NuGet](https://www.nuget.org/packages/Kook.Net.Webhook/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Webhook)
- Kook.Net.Pipe: [NuGet](https://www.nuget.org/packages/Kook.Net.Pipe/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Pipe)
- Kook.Net.Commands: [NuGet](https://www.nuget.org/packages/Kook.Net.Commands/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Commands)

### Extension Packages

Extension packages provide additional features or implementations that are not included in the main package.

- Kook.Net.CardMarkup: [NuGet](https://www.nuget.org/packages/Kook.Net.CardMarkup/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.CardMarkup)
- Kook.Net.Webhook.AspNet: [NuGet](https://www.nuget.org/packages/Kook.Net.Webhook.AspNet/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Webhook.AspNet)
- Kook.Net.Webhook.HttpListener: [NuGet](https://www.nuget.org/packages/Kook.Net.Webhook.HttpListener/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Webhook.HttpListener)
- Kook.Net.MessageQueue.InMemory: [NuGet](https://www.nuget.org/packages/Kook.Net.MessageQueue.InMemory/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.MessageQueue.InMemory/)
- Kook.Net.MessageQueue.MassTransit: [NuGet](https://www.nuget.org/packages/Kook.Net.MessageQueue.MassTransit/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.MessageQueue.MassTransit/)
- Kook.Net.DependencyInjection.Microsoft: [NuGet](https://www.nuget.org/packages/Kook.Net.DependencyInjection.Microsoft/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.DependencyInjection.Microsoft/)
- Kook.Net.Hosting: [NuGet](https://www.nuget.org/packages/Kook.Net.Hosting/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Hosting/)

### Experimental Packages

Experimental packages offer implementations of APIs that haven't been officially released or documented. They may
violate developer rules or policies, lack stability guarantees, and are subject to potential changes or removal in the
future.

- Kook.Net.Experimental: [NuGet](https://www.nuget.org/packages/Kook.Net.Experimental/),
  [GitHub Packages](https://github.com/gehongyan/Kook.Net/pkgs/nuget/Kook.Net.Experimental)

---

## License & Copyright

This package is open-source and is licensed under the [MIT license](LICENSE).

Kook.Net was developed with reference to **[Discord.Net](https://github.com/discord-net/Discord.Net)**.

[Discord.Net contributors](https://github.com/discord-net/Discord.Net/graphs/contributors) holds the copyright
for portion of the code in this repository according to [this license](https://github.com/discord-net/Discord.Net/blob/dev/LICENSE).

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fgehongyan%2FKook.Net.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fgehongyan%2FKook.Net?ref=badge_large)

---

## Acknowledgements

<img src="./assets/Discord.Net_Logo.svg" alt="drawing" height="50"/>

Special thanks to [Discord.Net](https://github.com/discord-net/Discord.Net) for such a great project.

<p>
  <img src="./assets/Rider_Icon.svg" height="50" alt="RiderIcon"/>
  <img src="./assets/ReSharper_Icon.png" height="50" alt="Resharper_Icon"/>
</p>

Special thanks to [JetBrains](https://www.jetbrains.com) for providing free licenses for their awesome tools -
[Rider](https://www.jetbrains.com/rider/) and [ReSharper](https://www.jetbrains.com/resharper/) -
to develop Kook.Net.
