---
uid: Guides.TextCommands.Preconditions
title: 先决条件
---

# 先决条件

先决条件可以被用作为命令服务中的权限系统，但其能力也不仅限于权限，
您也可以通过创建自定义先决条件实现更复杂的先决条件逻辑。

有两种可供使用的先决条件：

* [PreconditionAttribute] 可以被应用于模块、组、命令上
* [ParameterPreconditionAttribute] 可以被应用于参数上

有关这两种先决条件的详细信息，请参阅各自的 API 文档。

[PreconditionAttribute]: xref:KaiHeiLa.Commands.PreconditionAttribute
[ParameterPreconditionAttribute]: xref:KaiHeiLa.Commands.ParameterPreconditionAttribute

## 内置的先决条件

@KaiHeiLa.Commands 提供了几个内置的先决条件。

* @KaiHeiLa.Commands.RequireContextAttribute
* @KaiHeiLa.Commands.RequireUserAttribute
* @KaiHeiLa.Commands.RequireBotPermissionAttribute
* @KaiHeiLa.Commands.RequireUserPermissionAttribute

## 用法

要使用先决条件，只需将先决条件特性标记于命令的方法签名上。

### 示例

[!code-csharp[Precondition Usage](samples/preconditions/precondition_usage.cs)]

## 先决条件的析取

命令的先决条件可以存在多个，如果想要其中的部分先决条件满足任一即可被是作为先决条件校验通过，
则需要为先决条件进行分组来表示先决条件的析取。

[PreconditionAttribute] 提供了一个可选的 [Group] 属性，如果为两个或多个先决条件制定了相同的
[Group] 属性，则命令系统在进行先决条件检查时，这些条件中的任何一个满足时，分组内的其他先决条件都将被忽略。

### 示例

[!code-csharp[OR Precondition](samples/preconditions/group_precondition.cs)]

[Group]: xref:KaiHeiLa.Commands.PreconditionAttribute.Group

## 自定义先决条件

要创建自定义先决条件，请根据用途创建一个继承自 [PreconditionAttribute] 或
[ParameterPreconditionAttribute] 的类。

要实现函数上的先决条件，请重写 [CheckPermissionsAsync] 方法。

如果命令调用上下文满足条件，则返回 [PreconditionResult.FromSuccess] 
创建的对象，否则，请返回 [PreconditionResult.FromError] 创建的对象，
如有需要，请在返回的对象中添加错误消息。

> [!NOTE]
> Visual Studio、JetBrains Rider 等集成开发环境中的 IntelliSense
> 智能提示可以帮助您添加抽象类的实现中缺失的成员。

### 示例

[!code-csharp[Custom Precondition](samples/preconditions/require_role.cs)]

[CheckPermissionsAsync]: xref:KaiHeiLa.Commands.PreconditionAttribute.CheckPermissionsAsync*
[PreconditionResult.FromSuccess]: xref:KaiHeiLa.Commands.PreconditionResult.FromSuccess*
[PreconditionResult.FromError]: xref:KaiHeiLa.Commands.PreconditionResult.FromError*