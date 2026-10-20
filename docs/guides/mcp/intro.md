---
uid: Guides.Mcp.Intro
title: 文档 MCP 服务
---

# 文档 MCP 服务

Kook.Net 提供一个 [MCP](https://modelcontextprotocol.io/)（Model Context Protocol）服务，
将本类库的 API 元数据、源码与指南文档暴露给支持 MCP 的 AI 客户端，如各类 AI 编辑器与命令行工具。
在开发时让 AI 直接查询 Kook.Net 的类型签名、成员、实现源码和概念文档，
而不必让它凭记忆臆测或翻阅网页。

该服务随文档站的发布流程一同更新，每当文档站发布新版本，MCP 服务镜像也会重新构建并部署，
因此它提供的信息与当前文档版本保持一致。

该服务以 Streamable HTTP 协议对外提供，公开地址为：

```
https://mcp.kooknet.dev/
```

## 工具

服务内置以下工具，供 AI 客户端调用：

| 工具 | 作用 |
| --- | --- |
| `search_docs` | 在整个文档站中全文检索，按关键字找 API 或指南。 |
| `find_api` | 按名称模糊查找 API 条目，返回候选的 uid 与完全限定名。 |
| `get_api` | 按 uid 精确获取类型或成员的签名、摘要、参数与返回值。 |
| `get_type_members` | 列出某个类型的直接成员（方法、属性、事件、字段等）。 |
| `get_source` | 按 uid 返回类型或成员定义处的源码片段。 |
| `read_source_file` | 按仓库相对路径读取某个源码文件的内容。 |
| `list_guides` | 列出指南、速查手册、示例说明与更新日志等概念文档。 |
| `read_guide` | 读取一篇概念文档的原始内容。 |

典型的调用链是：先用 `search_docs` 或 `find_api` 定位到目标，拿到 uid 后用 `get_api`
查看签名与文档，用 `get_type_members` 展开成员，再用 `get_source` 查看实现源码；
概念性问题则走 `list_guides` 与 `read_guide`。

> [!NOTE]
> 该服务提供的源码包含 internal / private 等非公开可访问性的实现细节，
> 方便 AI 理解类库内部行为。这些细节不会出现在公开的 API 文档站上。

## 配置

在支持 MCP 的客户端里添加一个 Streamable HTTP 类型的服务器，指向 `https://mcp.kooknet.dev/` 即可。

### GitHub Copilot

用 Copilot CLI 添加：

```bash
copilot mcp add --transport http kook-net-doc https://mcp.kooknet.dev/
```

对应写入配置后的形式为：

```json
{
  "kook-net-doc": {
    "type": "http",
    "url": "https://mcp.kooknet.dev/",
    "tools": ["*"]
  }
}
```

### Claude Code

用 Claude Code CLI 添加远程服务器：

```bash
claude mcp add --transport http kook-net-doc https://mcp.kooknet.dev/
```

添加后可用 `claude mcp list` 查看，或在会话中用 `/mcp` 查看服务器状态与工具。

### Cursor

在项目的 `.cursor/mcp.json`（或全局 `~/.cursor/mcp.json`）中配置：

```json
{
  "mcpServers": {
    "kook-net-doc": {
      "url": "https://mcp.kooknet.dev/"
    }
  }
}
```

配置后在 Cursor 设置的 MCP 面板中可看到该服务器及其工具。

### Visual Studio Code

在工作区的 `.vscode/mcp.json` 中配置（顶层键为 `servers`，远程服务器用 `type: http`）：

```json
{
  "servers": {
    "kook-net-doc": {
      "type": "http",
      "url": "https://mcp.kooknet.dev/"
    }
  }
}
```

也可用命令面板的 **MCP: Add Server** 交互添加。启用后在 Agent 模式下即可调用其工具。

### Kiro

用 Kiro CLI 添加：

```bash
kiro-cli mcp add --name kook-net-doc --url https://mcp.kooknet.dev/
```

默认写入全局配置（`~/.kiro/settings/mcp.json`）；加 `--scope workspace` 则只写入当前工作区（`.kiro/settings/mcp.json`）。
也可直接编辑配置文件：

```json
{
  "mcpServers": {
    "kook-net-doc": {
      "url": "https://mcp.kooknet.dev/"
    }
  }
}
```

### Antigravity

在 Antigravity 的 MCP 配置文件中添加远程服务器：

```json
{
  "mcpServers": {
    "kook-net-doc": {
      "serverUrl": "https://mcp.kooknet.dev/"
    }
  }
}
```

也可在设置的 MCP 面板中添加远程服务器并填入上述地址。

### Windsurf

在 `~/.codeium/windsurf/mcp_config.json` 中添加远程服务器：

```json
{
  "mcpServers": {
    "kook-net-doc": {
      "serverUrl": "https://mcp.kooknet.dev/"
    }
  }
}
```

Windsurf 的远程服务器使用 `serverUrl` 字段。

### Codex

在 `~/.codex/config.toml` 中添加远程服务器（Codex 使用 TOML 配置）：

```toml
[mcp_servers.kook-net-doc]
enabled = true
type = "http"
url = "https://mcp.kooknet.dev/"
```

### Gemini CLI

在 `~/.gemini/settings.json`（或项目内 `.gemini/settings.json`）中添加远程服务器：

```json
{
  "mcpServers": {
    "kook-net-doc": {
      "httpUrl": "https://mcp.kooknet.dev/"
    }
  }
}
```

Gemini CLI 的 Streamable HTTP 服务器使用 `httpUrl` 字段。
