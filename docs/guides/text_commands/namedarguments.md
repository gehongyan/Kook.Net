---
uid: Guides.TextCommands.NamedArguments
title: 命名参数
---

# 命名参数

默认情况下，命令中的参数是按照顺序逐个解析的，位置决定了参数的对应。
如果定义了多个可选参数，对于用户来说，只设置想要传入的可选参数而不全部一一写出会更加方便。

## 设置参数名称

要想用户能够通过指定参数名称来设置对应的参数，您需要创建一个可选参数容器类，
其中需包含了命令会用到的所有可选参数，并为其添加 [NamedArgumentTypeAttribute] 特性标签。

### 示例

```csharp
[NamedArgumentType]
public class NamableArguments
{
    public string First { get; set; }
    public string Second { get; set; }
    public string Third { get; set; }
    public string Fourth { get; set; }
}
```

## 用法

要使用这些可选参数的命令可以通过如下的方式定义：

```csharp
[Command("act")]
public async Task Act(int requiredArg, NamableArguments namedArgs)
```

该命令则可以按如下方式调用：

`.act 42 first: Hello fourth: "A string with spaces must be wrapped in quotes" second: World`

可选参数容器类的类型解析是自动注册的，无需对该容器类创建并注册自定义类型解析。

> [!IMPORTANT]
> 一个命令**只能**有用一个被 [NamedArgumentTypeAttribute]
> 特性标注的可选参数容器类，且**必须**位于参数列表的末位。
> 被该特性标注的类会在解析参数是自动应用 [RemainderAttribute] 特性的效果。

[RemainderAttribute]: xref:Kook.Commands.RemainderAttribute

## 复杂类型

可选参数容器类中的每个属性在进行匹配时，所有注册到文本命令服务内的类型解析都会如以往正常匹配。

如果要将多个相同类型的值读入单个属性中，则可以将属性声明为形如 `IEnumerable<T>` 的类型。

例如：如果可选参数容器类中包含了一个这样的属性：

```csharp
public IEnumerable<int> Numbers { get; set; }
```

那么该命令可以以如下的方式调用：

`.cmd numbers: "1, 2, 4, 8, 16, 32"`

## 补充说明

可选参数容器类中的属性也可以使用 [OverrideTypeReader]。

[OverrideTypeReader]: xref:Kook.Commands.OverrideTypeReaderAttribute

[NamedArgumentTypeAttribute]: xref:Kook.Commands.NamedArgumentTypeAttribute
