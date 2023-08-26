---
uid: Guides.DI.Services
title: 命令框架中的依赖注入
---

# 命令框架中的依赖注入

在文本命令框架服务模块中，依赖注入的非常简单。

您可以将任何服务注入到模块中，而无需将模块注册到提供程序中。Kook.Net会在内部解析您的依赖项。

## 注册服务

前文描述的行为允许已注册的成员作为可用构造函数的参数，因此 Socket 客户端和配置类将会被自动解析，并使用 CommandService(client, config) 重载。

[!code-csharp[Service Registration](samples/service-registration.cs)]

## 模块内的用法

在模块的构造函数中，任何参数都将被你所传入的 @System.IServiceProvider 填充。

任何公共可设置属性也将以相同的方式填充。

[!code-csharp[Module Injection](samples/modules.cs)]

如果你接受 `CommandService` 或 `IServiceProvider` 作为构造函数的参数或可注入的属性，这些条目将会被模块所加载的
`CommandService` 和传入的 `IServiceProvider` 填充。

> [!NOTE]
> 在属性上标记 [DontInjectAttribute] 特性将会阻止该属性被注入。

## 服务

模块是瞬态的，会在每次请求时重新实例化，因此如果需要在多个命令执行之间保持值，建议创建单例服务来包装。

[!code-csharp[Services](samples/services.cs)]
