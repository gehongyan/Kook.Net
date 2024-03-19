---
uid: Guides.CardMarkup.Structures
title: 结构体
---

# 结构体

[KOOK 开发者文档 - 卡片消息 - 结构体](https://developer.kookapp.cn/doc/cardmessage#%E7%BB%93%E6%9E%84%E4%BD%93)

## 区域文本 `paragraph`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| cols | integer | null | ✅ | 区域文本的列数，范围为 1-3 |

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [plain](elements.md#普通文本-plain) | 与 `kmarkdown` 合计至多 50，至少 1 | 区域文本的内容 |
| [kmarkdown](elements.md#kmarkdown-kmarkdown) | 与 `plain` 合计至多 50，至少 1 | 区域文本的内容 |

[!code-xml[Paragraph](samples/definitions/structure-paragraph.xml)]
