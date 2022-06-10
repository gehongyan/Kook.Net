---
uid: Guides.TextCommands.DI
title: 依赖注入
---

# 依赖注入

为了方便起见，文本命令服务基于基本的依赖注入服务设计，建议在使用本框架时使用依赖注入。

## 配置依赖注入

1. 创建 [ServiceCollection] 实例
2. 将要使用的模块添加到依赖注入的 ServiceCollection 中
3. 将 ServiceCollection 构建为 [ServiceProvider]
4. 将 ServiceProvider 作为参数传入 @KaiHeiLa.Commands.CommandService.AddModulesAsync* / @KaiHeiLa.Commands.CommandService.AddModuleAsync* , @KaiHeiLa.Commands.CommandService.ExecuteAsync* .

[ServiceCollection]: https://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection
[ServiceProvider]: https://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovider

### 示例

[!code-csharp[IServiceProvider Setup](samples/dependency-injection/dependency-setup.cs)]

## 在模块中的用法

在模块的构造函数中，任何参数都会通过 [IServiceProvider] 进行填充。

任何可公共写的属性也会通过 [IServiceProvider] 进行填充。

[IServiceProvider]: https://docs.microsoft.com/dotnet/api/system.iserviceprovider

> [!NOTE]
> 为属性标记 [DontInjectAttribute] 特性标签可以阻止属性被 [IServiceProvider] 填充。

> [!NOTE]
> 如果将 `CommandService` 或 `IServiceProvider` 作为构造函数参数或未被阻止注入的属性，
> 那么该参数或属性将会被填充为加载此模块的 `CommandService` 或由此构建的 `IServiceProvider`。

### 示例

[!code-csharp[Injection Modules](samples/dependency-injection/dependency-module.cs)]
[!code-csharp[Disallow Dependency Injection](samples/dependency-injection/dependency-module-noinject.cs)]

[DontInjectAttribute]: xref:KaiHeiLa.Commands.DontInjectAttribute