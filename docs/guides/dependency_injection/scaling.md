---
uid: Guides.DI.Scaling
title: 隐式注入
---

# 隐式注入

依赖注入有很多用例，并且非常适合大规模应用程序。有几种方法可以使大量服务的注册和使用更加容易。

## 使用一系列服务

如果您有许多服务都具有相同的用途，例如处理事件或服务模块，您可以通过一些要求一次注册和注入它们：

- 所有类都需要继承单个接口或抽象类型
- 虽然不是必需的，但最好是接口和类型在调用时具有相同的方法签名
- 您需要注册一个所有类型都可以注入的类

### 隐式注册

通过获取程序集中的所有类型，并检查它们是否实现了指定接口，来进行服务的注册。

[!code-csharp[Registering](samples/implicit-registration.cs)]

> [!NOTE]
> 如上所示，interfaceType 和 activatorType 未定义。对于下面的用例，这些是 `IService` 和 `ServiceActivator`。

### 使用隐式依赖

为了使用隐式依赖，您必须访问您之前注册的激活器类。

[!code-csharp[Accessing the activator](samples/access-activator.cs)]

当访问并调用激活器类的 `ActivateAsync()` 方法时，将执行以下代码：

[!code-csharp[Executing the activator](samples/enumeration.cs)]

至此，所有通过实现 `IService` 接口被注册的类上的自动代码都会被执行并启动。
