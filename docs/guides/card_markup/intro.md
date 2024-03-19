---
uid: Guides.CardMarkup.Intro
title: XML 卡片消息入门
---

# XML 卡片消息

[Kook.CardMarkup](xref:Kook.CardMarkup) 命名空间提供了将使用 XML 标记语言定义的卡片消息反序列化为 [ICard](xref:Kook.ICard) 对象的方法。

## 入门

下面的示例中，我们创建一个简单的，由标题、分割线和 9 张图片组成的一个卡片消息。

### XML 标记

创建一个 XML 文件，定义卡片消息的内容：

[!code-xml[Sample Card](samples/intro/sample-card.xml)]

#### XML 声明

文件第一行为 XML 声明，指定 XML 版本和字符编码：

> [!WARNING]
> 字符编码必须是 `UTF-8`。

> [!TIP]
> XML 版本分为 1.0 和 1.1 两个版本，绝大多数时候建议使用 1.1 版本。

[!code-xml[Sample Card - XML Declaration](samples/intro/sample-card.xml#L1)]

#### 卡片消息

XML 根元素为 `<card-message>`，代表一个卡片消息，每一个 `<card-message>` 元素可以包含多个 `<card>` 元素。

`<card-message>` 元素上需要指定 XML 命名空间，以及 XML Schema 文件的位置：

[!code-xml[Sample Card - Root Element](samples/intro/sample-card.xml#L3-L5)]

- `xmlns=https://kooknet.dev` 指定了默认 XML 命名空间，卡片消息所有的元素均在该命名空间下。
- `xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"` 指定了 XML Schema 实例命名空间，并设置命名空间前缀为 `xsi`。
- `xsi:schemaLocation="https://kooknet.dev https://kooknet.dev/card-message.xsd"` 调用了 `xsi` 命名空间下的 `schemaLocation` 属性，指定了 `https://kooknet.dev` 命名空间下的 XML Schema 文件位置在 `https://kooknet.dev/card-message.xsd`。

#### 卡片

`<card>` 元素代表一个卡片，每一个 `<card>` 元素包含一个 `<modules>` 元素，用于包含卡片的组件。

关于卡片，请参阅 [卡片](card.md)。

### 反序列化

使用 [Kook.CardMarkup.CardMarkupSerializer](xref:Kook.CardMarkup.CardMarkupSerializer) 将 XML 卡片消息反序列化为 [ICard](xref:Kook.ICard) 对象：

> [!WARNING]
> `Try...` 方法只适用于同步调用。

> [!NOTE]
> 此示例传入参数为 XML 文件的 `FileInfo` 类实例。
> 所有方法均有传入参数为 `Stream` 或 `string` 的重载。
> 传出参数的类型均为 `IEnumerable<ICard>` 或 `Task<IEnumerable<ICard>>`。

[!code-csharp[Deserialize Card](samples/intro/deserialize-sample-card.cs)]

### 渲染效果

该 XML 卡片消息等效于以下 JSON 格式的卡片消息：

[!code-json[Sample Card](samples/intro/sample-card.json)]

渲染效果如下：

![Sample Card](samples/intro/sample-card.png)
