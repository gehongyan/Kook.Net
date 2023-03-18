---
uid: Guides.GettingStarted.Installation
title: 安装 Kook.Net
---

# 安装 Kook.Net

Kook.Net 通过 NuGet 分发，推荐通过 NuGet 包管理工具安装，
如有需要，也可从源代码进行编译。

## 支持的平台

Kook.Net 目前支持的目标框架包括

- [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)
- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET Standard 2.1](https://learn.microsoft.com/dotnet/standard/net-standard?tabs=net-standard-2-1)
- [.NET Standard 2.0](https://learn.microsoft.com/dotnet/standard/net-standard?tabs=net-standard-2-0)
- [.NET Framework 4.6.2](https://dotnet.microsoft.com/download/dotnet-framework/net462)

## 通过 NuGet 包管理器安装

支持的 NuGet 源有：

- [NuGet Gallery](https://nuget.org)
- [GitHub Packages](https://github.com/gehongyan?tab=packages&repo_name=Kook.Net)

### 使用 Visual Studio

1. 找到 `解决方案资源管理器` 窗口，在 Bot 项目下找到 `依赖项`
2. 右键点击 `依赖项`，选择 `管理 NuGet 程序包`

   ![img.png](images/install/install-vs-dependencies.png)

3. 在 `浏览` 选项卡中，搜索 `Kook.Net`

   > [!NOTE]
   > 如要安装预览版 Kook.Net，请勾选 `包括预发行版`，否则，预览版搜索结果无法展示在列表中。

   ![img.png](images/install/install-vs-nuget.png)

4. 选择 `Kook.Net`，点击 `安装`

### 使用 JetBrains Rider

1. 找到 `Explorer` 窗口，在 Bot 项目下找到 `Dependencies`
2. 右键点击 `Dependencies`，选择 `Manage NuGet Packages`

   ![img.png](images/install/install-rider-dependencies.png)

3. 在 `Packages` 选项卡中，搜索 `Kook.Net`

   > [!NOTE]
   > 如要安装预览版 Kook.Net，请勾选 `Prerelease` ，否则，预览版搜索结果无法展示在列表中。

4. 右键点击 `Kook.Net`，点击 `Install Kook.Net ...`

   ![img.png](images/install/install-rider-nuget.png)

### 使用 Visual Studio Code

1. 找到 Bot 项目的 `*.csproj` 文件
2. 添加 `Kook.Net` 到 `*.csproj` 中

   [!code[SampleProject.csproj](samples/project.xml)]

### 使用 dotnet CLI

1. 启动终端
2. 导航至 Bot 项目中 `*.csproj` 文件的所在目录
3. 执行 `dotnet add package Kook.Net`

   > [!NOTE]
   > 如要安装预览版 Kook.Net，使用 dotnet CLI 添加 Kook.Net 时需附加 `--prerelease` 选项，
   > 否则，dotnet CLI 将只尝试为项目添加稳定版本的 NuGet 包。

## 从源代码编译

要从源代码编译 Kook.Net，请参考：

### 使用 Visual Studio

- [Visual Studio 2022](https://visualstudio.microsoft.com/zh-hans/vs/) 或更新版本。
- [.NET 6 SDK]

安装 Visual Studio 期间需选择 .NET 6 工作负载。

### 使用 JetBrains Rider

- [JetBrains 2021.3](https://www.jetbrains.com.cn/rider/) 或更新版本。
- [.NET 6 SDK]

### 使用 Command Line

* [.NET 6 SDK]

[.NET 6 SDK]: https://dotnet.microsoft.com/download
