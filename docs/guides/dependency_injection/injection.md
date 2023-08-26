---
uid: Guides.DI.Injection
title: 注入实例
---

# 注入实例

在 `IServiceProvider` 中注册的服务后可以注入到任何注册的类中，这可以通过属性或构造函数来实现。

> [!NOTE]
> 如上所述，依赖项*和*目标类必须都进行注册，服务提供程序才可以解析它。

## 通过构造函数注入

服务可以从类的构造函数中注入。
这是首选的方法，因为这可以将只读字段与提供的服务关联在一起，且在类的外部不可访问。

[!code-csharp[Constructor Injection](samples/ctor-injecting.cs)]

## 通过属性注入

也可以通过属性进行注入。

[!code-csharp[Property Injection](samples/property-injecting.cs)]

> [!WARNING]
> 依赖项注入不会解析属性注入中缺少的服务，也不会选择构造函数作为替代。如果尝试注入公共可访问属性时其服务缺失，应用程序将抛出错误。


## 使用提供程序本身

也可以将提供程序本身注入到类中，可用于以下多种用例：

- 允许库（如 Kook.Net）在内部访问提供程序
- 注入可选依赖项
- 如有必要，可以直接在提供程序上调用方法，这通常用于创建作用域

[!code-csharp[Provider Injection](samples/provider.cs)]

> [!NOTE]
> 请记住，提供程序将选择“最大”的可用构造函数。
> 如果选择引入多个构造函数，请记住，如在某一个构造函数中缺失了某项服务，提供程序可能会选择另一个可用的构造函数，而不是抛出异常。
