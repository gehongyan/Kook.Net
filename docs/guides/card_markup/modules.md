---
uid: Guides.CardMarkup.Modules
title: 模块
---

# 模块

[KooK 开发者文档 - 卡片消息 - 模块](https://developer.kookapp.cn/doc/cardmessage#%E6%A8%A1%E5%9D%97)

## 标题 `header`

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [plain](elements.md#普通文本-plain) | 1 | 标题文本 |

[!code-xml[Header](samples/definitions/module-header.xml)]

## 内容 `section`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| mode | string | right |  | 其它元素的放置位置 |

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| text | 1 | 内容模块的文本元素 |
| text/[plain](elements.md#普通文本-plain) | 1，与 `text/markdown` 互斥 | 内容模块的文本元素（纯文本） |
| text/[kmarkdown](elements.md#kmarkdown-kmarkdown) | 1，与 `text/plain` 互斥 | 内容模块的文本元素（KMarkdown） |
| accessory | 0-1 | 内容模块的其它元素 |
| accessory/[image](elements.md#图片-image) | 1，与 `accessory/button` 互斥 | 内容模块的其它元素（图片） |
| accessory/[button](elements.md#按钮-button) | 1，与 `accessory/image` 互斥 | 内容模块的其它元素（按钮） |

[!code-xml[Section](samples/definitions/module-section.xml)]

## 图片组 `images`

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [image](elements.md#图片-image) | 1-9 | 图片 |

[!code-xml[Images](samples/definitions/module-images.xml)]

## 容器 `container`

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [image](elements.md#图片-image) | 1-9 | 图片 |

[!code-xml[Container](samples/definitions/module-container.xml)]

## 交互 `actions`

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [button](elements.md#按钮-button) | 1-4 | 按钮 |

[!code-xml[Actions](samples/definitions/module-actions.xml)]

## 备注 `context`

| 元素 | 数量 | 说明 |
| --- | --- | --- |
| [plain](elements.md#普通文本-plain) | 与 `kmarkdown` `image` 合计至多 10，至少 1 | 纯文本 |
| [kmarkdown](elements.md#kmarkdown-kmarkdown) | 与 `plain` `image` 合计至多 10，至少 1 | KMarkdown |
| [image](elements.md#图片-image) | 与 `plain` `kmarkdown` 合计至多 10，至少 1 | 图片 |

[!code-xml[Context](samples/definitions/module-context.xml)]

## 分割线 `divider`

[!code-xml[Divider](samples/definitions/module-divider.xml)]

## 文件 (文件/视频) `file` `video`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| src | anyURI | null | ✅ | 文件 URL，必须以 `https://` 开头 |
| title | string | null |  | 文件标题 |

[!code-xml[File](samples/definitions/module-file.xml)]
[!code-xml[Video](samples/definitions/module-video.xml)]

## 文件 (音频) `audio`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| src | anyURI | null | ✅ | 文件 URL，必须以 `https://` 开头 |
| title | string | null |  | 文件标题 |
| cover | anyURI | null |  | 封面 URL，必须以 `https://` 开头 |

[!code-xml[Audio](samples/definitions/module-audio.xml)]

## 倒计时 `countdown`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| start | ulong | null | ⚠️ | 起始的毫秒时间戳，仅 `mode="second"` 时需要 |
| end | ulong | null | ✅ | 到期的毫秒时间戳 |
| mode | string | null | ✅ | 显示模式，可选值为 `day` `hour` `second` |

[!code-xml[Countdown](samples/definitions/module-countdown.xml)]

## 邀请 `invite`

| 属性 | 类型 | 默认值 | 必需 | 说明 |
| --- | --- | --- | --- | --- |
| code | string | null | ✅ | 邀请链接或者邀请码 |

[!code-xml[Invite](samples/definitions/module-invite.xml)]
