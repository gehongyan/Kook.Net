---
uid: Guides.TextCommands.TypeReaders
title: 类型解析
---

# 类型解析

通过类型解析，命令中传入的各种参数可以被自动转换为各种类型。

默认支持的类型解析包括：

* `bool`
* `char`
* `sbyte`/`byte`
* `ushort`/`short`
* `uint`/`int`
* `ulong`/`long`
* `float`, `double`, `decimal`
* `string`
* `enum`
* `DateTime`/`DateTimeOffset`/`TimeSpan`
* 任何可空的值类型 (例如：`int?`, `bool?`)
* 任何 `IChannel`/`IMessage`/`IUser`/`IRole` 的实现类

## 自定义类型解析

要自定义类型解析，可以创建一个类，引入 @Kook 和 @Kook.Commands
命名空间，继承 @Kook.Commands.TypeReader，并重写 [ReadAsync] 方法，
该方法内为将输入的字符串解析为指定类型的逻辑。

如果类型解析成功，请将解析结果传入 [TypeReaderResult.FromSuccess] 返回；
如果类型解析失败，请将错误信息传入 [TypeReaderResult.FromError] 返回。

> [!NOTE]
> Visual Studio、JetBrains Rider 等集成开发环境中的 IntelliSense
> 智能提示可以帮助您添加抽象类的实现中缺失的成员。

[TypeReaderResult]: xref:Kook.Commands.TypeReaderResult

[TypeReaderResult.FromSuccess]: xref:Kook.Commands.TypeReaderResult.FromSuccess*

[TypeReaderResult.FromError]: xref:Kook.Commands.TypeReaderResult.FromError*

[ReadAsync]: xref:Kook.Commands.TypeReader.ReadAsync*

### 示例

[!code-csharp[TypeReaders](samples/typereaders/typereader.cs)]

## 注册自定义类型解析

自定义类型解析无法被文本命令服务自动发现，需要显式注册。

要添加自定义类型解析，请调用 [CommandService.AddTypeReader] 方法。

> [!IMPORTANT]
> 自定义类型解析注册需要在模块发现前进行，否则自定义类型接解析无法正常工作。

[CommandService.AddTypeReader]: xref:Kook.Commands.CommandService.AddTypeReader*

### 示例

[!code-csharp[Adding TypeReaders](samples/typereaders/typereader-register.cs)]
