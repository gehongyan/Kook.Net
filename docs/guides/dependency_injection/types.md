---
uid: Guides.DI.Dependencies
title: 生命周期
---

# 生命周期

依赖项可以有三种生命周期。每种生命周期都有不同的用例。

> [!WARNING]
> 当使用接口 IServiceType 和类 ImplementationType 注册类型时，只有 IServiceType 可用于注入，而 ImplementationType 将用于底层实例。

## 单例

单例服务在首次请求时创建单个实例，并应用程序在整个生命周期中维护该单一实例。
在单例中更改的任何值都将在依赖于它的所有实例中更改，因为它们都具有对这一单例服务的相同引用。

### 注册

[!code-csharp[Singleton Example](samples/singleton.cs)]

> [!NOTE]
> KOOK 客户端和命令服务等类型都是单例的，因为它们应该在整个应用程序中持续存在，并与对该对象的所有引用共享其状态。

## 作用域

作用域服务在每次请求时创建一个新实例，但在“作用域”范围内保留。
只要服务在创建的作用域中可见，就会为该类型的所有引用重用同一实例。
这意味着您可以在执行期间重用同一实例，并在请求处于活动状态时保持服务的状态。

### 注册

[!code-csharp[Scoped Example](samples/scoped.cs)]

> [!NOTE]
> 在不使用 HTTP 或类似 EF Core 等库的情况下，作用域在 KOOK Bot 中不常见。

## 瞬时

瞬时服务在每次请求时创建一个新实例，并且不在目标服务的引用之间共享其状态。
它适用于需要很少状态的轻量级类型，以便在执行后快速释放。

### 注册

[!code-csharp[Transient Example](samples/transient.cs)]

> [!NOTE]
> Kook.Net 模块的行为与瞬时类型完全相同，并且仅在命令执行所需的时间内存在。
> 这就是为什么建议应用程序使用单例服务来跟踪跨命令执行的数据。
