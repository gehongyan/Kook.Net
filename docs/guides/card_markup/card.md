---
uid: Guides.CardMarkup.Card
title: 卡片
---

# 卡片

## 属性

| 属性 |  类型| 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| theme | string | null | | 卡片主题，可选值为 `primary` `success` `danger` `warning` `info` `secondary` `none` |
| size | string | null | | 卡片尺寸，可选值为 `small` `large` |
| color | string | null | | 卡片颜色，十六进制 RGB 色彩，以 `#` 开头 |

## 元素

每一个卡片需要包含一个 `<modules>` 元素，`<modules>` 元素包含卡片的[组件](modules.md)。

```xml
<card>
    <modules>
        <!-- 卡片组件 -->
    </modules>
</card>
```

## 示例 1

使用默认主题、尺寸、颜色的卡片，并包含一个 [标题模块](modules.md#标题-header)。

[!code-xml[Card 01](samples/card/sample-01.xml)]

## 示例 2

使用 `warning` 主题、`small` 尺寸、`#aaaaaa` 颜色的卡片，并包含一个 [标题模块](modules.md#标题-header)、一个 [图片组模块](modules.md#图片组-images)。

[!code-xml[Card 02](samples/card/sample-02.xml)]

## 示例 3

KooK 消息编辑器中的投票消息模版。

[!code-xml[Card 03](samples/card/sample-03.xml)]
