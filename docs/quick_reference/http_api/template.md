---
uid: Guides.QuickReference.Http.Template
title: 消息模板相关接口
---

# 消息模板相关接口

预声明变量

```csharp
readonly KookRestClient _restClient = null;

RestMessageTemplate restMessageTemplate = null;
IMessageTemplate messageTemplate = null;
```

### [获取模板列表]

GET `/api/v3/template/list`

```csharp
ulong id = default; // 要获取的消息模板 ID

// API 请求，获取所有消息模板
IReadOnlyCollection<RestMessageTemplate> messageTemplates = await _restClient.GetMessageTemplatesAsync();
// API 请求，获取指定的消息模板
RestMessageTemplate messageTemplate = await _restClient.GetMessageTemplateAsync(id);
```

### [创建模板]

POST `/api/v3/template/create`

```csharp
string title = default; // 模板标题
string content = default; // 模板内容
TemplateType type = TemplateType.Twig; // 模板类型，默认为 Twig
TemplateMessageType messageType = TemplateMessageType.KMarkdown; // 模板消息类型，默认为 KMarkdown
ulong? testChannelId = null; // 测试频道 ID
JsonElement? testData = null; // 测试数据

// API 请求，创建消息模板
RestMessageTemplate messageTemplate = await _restClient.CreateMessageTemplateAsync(
    title, content, type, messageType, testChannelId, testData);
```

### [更新模板]

POST `/api/v3/template/update`

```csharp
RestMessageTemplate restMessageTemplate = null; // 要更新的消息模板
string title = default; // 模板标题
string content = default; // 模板内容
TemplateType type = TemplateType.Twig; // 模板类型，默认为 Twig
TemplateMessageType messageType = TemplateMessageType.KMarkdown; // 模板消息类型，默认为 KMarkdown
ulong? testChannelId = null; // 测试频道 ID
JsonElement? testData = null; // 测试数据

// API 请求，更新消息模板
await restMessageTemplate.ModifyAsync(x =>
{
    x.Title = title;
    x.Content = content;
    x.Type = type;
    x.MessageType = messageType;
    x.TestChannelId = testChannelId;
    x.TestData = testData;
})
```

### [删除模板]

POST `/api/v3/template/delete`

```csharp
RestMessageTemplate restMessageTemplate = null; // 要删除的消息模板

// API 请求，更新消息模板
await restMessageTemplate.DeleteAsync();
```

[获取模板列表]: https://developer.kookapp.cn/doc/http/template#获取模板列表
[创建模板]: https://developer.kookapp.cn/doc/http/template#创建模板
[更新模板]: https://developer.kookapp.cn/doc/http/template#更新模板
[删除模板]: https://developer.kookapp.cn/doc/http/template#删除模板
