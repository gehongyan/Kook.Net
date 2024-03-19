---
uid: Guides.CardMarkup.Elements
title: 元素
---

# 元素

[KooK 开发者文档 - 卡片消息 - 元素](https://developer.kookapp.cn/doc/cardmessage#%E5%85%83%E7%B4%A0)

## 普通文本 `plain`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| emoji | bool | true |  | 如果为 true，会把 emoji 的 shortcut 转为 emoji |

[!code-xml[Plain](samples/definitions/element-plain.xml)]

## KMarkdown `kmarkdown`

[!code-xml[KMarkdown](samples/definitions/element-kmarkdown.xml)]

## 图片 `image`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| src | anyURI | null | ✅ | 图片 URL，必须以 `https://` 开头 |
| alt | string | null |  | 图片的替代文本 |
| size | string | null | | 图片的尺寸，可选值为 `small` `large` |
| circle | bool | false | | 是否显示为圆形 |

[!code-xml[Image](samples/definitions/element-image.xml)]

## 按钮 `button`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| theme | string | null | | 按钮主题，可选值为 `primary` `success` `danger` `warning` `info` `secondary` |
| value | string | null |  | 按钮需要传递的 value |
| click | string | null | | 按钮事件类型，可选值为 `link` `return-val` |

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [plain](#普通文本-plain) | 1，与 kmarkdown 互斥 | 按钮文本 |
| [kmarkdown](#kmarkdown-kmarkdown) | 1，与 plain 互斥 | 按钮文本 |

[!code-xml[Button](samples/definitions/element-button.xml)]
