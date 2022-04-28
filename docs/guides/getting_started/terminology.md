---
uid: Guides.GettingStarted.Terminology
title: 术语
---

# 术语

## 实现指定实体类

KaiHeiLa.Net 分为核心库与两个不同的实现：`KaiHeiLa.Net.Core`、
`KaiHeiLa.Net.Rest` 和 `KaiHeiLa.Net.WebSocket`。作为 Bot 的开发者，
您只需要了解如何使用 `KaiHeiLa.Net.WebSocket` ，但也需要了解它们之间的区别。

`KaiHeiLa.Net.Core` 提供了一组抽象化开黑啦 API 的接口，这些接口保证了 
KaiHeiLa.Net 中的所有实现都保持一致。如果您正在编写与实现无关的库，可仅依赖
核心库来确保您的库能够与 KaiHeiLa.Net 一起在任何类型的实现中都能正常工作。

`KaiHeiLa.Net.Rest` 提供了一组具体的类，用于实现开黑啦 API 中的 HTTP 接口部分。
该实现中的实体以 `Rest` 为前缀，例如 `RestChannel`。

`KaiHeiLa.Net.WebSocket` 提供了一组具体的类，用于实现开黑啦 API 中的 WebSocket 接口部分，
并支持实体的缓存。开发 Bot 时，您应使用此实现。该实现中的实体以 `Socket` 为前缀，
例如 `SocketChannel`。
