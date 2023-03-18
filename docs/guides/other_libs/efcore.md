---
uid: Guides.OtherLibs.EFCore
title: Entity Framework Core
---

# 对象关系映射框架 Entity Framework Core

本向导可以指引您配置并基于 SQL Server 数据库使用 Entity Framework Core，本文末尾罗列了其它数据库的信息。

## 先决条件

- 配置好依赖注入服务的 Bot 客户端程序
- SQL Server 数据库实例
- [EF Core CLI 工具](https://docs.microsoft.com/ef/core/cli/dotnet#installing-the-tools)

## 安装所需类库

可通过集成开发环境的用户界面或 dotnet CLI 安装以下类库：

| 名称                                        | 链接                                                                           |
|-------------------------------------------|------------------------------------------------------------------------------|
| `Microsoft.EntityFrameworkCore`           | [链接](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)           |
| `Microsoft.EntityFrameworkCore.SqlServer` | [链接](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer) |

## 配置 DbContext

EF Core 中的 DbContext 是一个抽象类，它提供了一系列方法来操作数据库。数据库上下文及其包装的实体示例如下：

[!code-csharp[DBContext Sample](samples/efcore/dbcontext-sample.cs)]

> [!NOTE]
> 有关创建用于 EF Core
> 的实体模型，请参阅 [EF Core 文档](https://docs.microsoft.com/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-model)。

## 将数据库上下文添加到依赖注入服务容器中

要将此数据库上下文添加到依赖注入服务容器中，只需调用 EF Core 中所提供的扩展方法，示例如下：

[!code-csharp[DBContext Dependency Injection](samples/efcore/dbcontext-injection.cs)]

> [!NOTE]
> 有关如何书写数据库连接字符串，可参考 [EF Core 文档](https://docs.microsoft.com/ef/core/miscellaneous/connection-strings)。

## 迁移

在使用数据库上下文前，需要将代码中的更改迁移到实际的数据库中。有关迁移的更多信息，请参阅
[EF Core 文档](https://docs.microsoft.com/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)。

## 使用数据库上下文

要使用数据库上下文，请通过依赖注入服务将其注入到要使用的类中，示例如下：

[!code-csharp[DBContext Injection Demo](samples/efcore/dbcontext-usage.cs)]

## 使用其它数据库提供程序

此处有一些 EF Core 可用的主流的数据库提供程序，及其相关的文档，通常来说，与上文示例代码中不同的仅有
`DbContextOptions` / `DbContextOptionsBuilder` 中进行配置的部分。

| 名称         | 链接                                                                                               |
|------------|--------------------------------------------------------------------------------------------------|
| MySQL      | [链接](https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-example.html) |
| SQLite     | [链接](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)   |
| PostgreSQL | [链接](https://www.npgsql.org/efcore/)                                                             |
