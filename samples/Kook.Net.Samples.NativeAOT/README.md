# Kook.Net NativeAOT 示例

此示例演示如何在 NativeAOT 编译模式下使用 Kook.Net。

## 功能特性

- ✅ REST API 调用（使用 JSON 源生成）
- ✅ WebSocket 连接
- ✅ 事件处理
- ✅ 消息发送和接收
- ❌ Commands 框架（不支持，因为依赖反射）

## 运行示例

### 开发模式运行

```bash
export KOOK_TOKEN="your-bot-token-here"
dotnet run
```

### 发布为 NativeAOT

```bash
dotnet publish -c Release

# 运行已编译的原生可执行文件
export KOOK_TOKEN="your-bot-token-here"
./bin/Release/net8.0/linux-x64/publish/Kook.Net.Samples.NativeAOT
```

## NativeAOT 限制

1. **Commands 框架不可用**: `Kook.Net.Commands` 使用反射进行命令发现和参数绑定，这与 NativeAOT 不兼容。
   - 解决方案：手动处理消息并实现命令逻辑

2. **JSON 序列化**: Kook.Net 使用源生成的 `KookJsonSerializerContext` 来支持 NativeAOT。
   - 所有 API 模型都已注册用于源生成
   - 自定义类型可能需要额外的序列化配置

3. **程序集大小**: NativeAOT 编译的可执行文件会比常规 .NET 应用程序大，但启动速度更快，内存占用更低。

## 测试 Bot

运行 Bot 后，在 Kook 频道中发送 `!ping` 命令，Bot 会回复 "Pong! 🏓"。

## 性能优势

NativeAOT 编译的应用程序具有以下优势：

- **快速启动**: 无需 JIT 编译
- **较低内存占用**: 无需加载整个 .NET 运行时
- **单文件部署**: 可执行文件包含所有依赖项
- **更好的代码保护**: 原生代码更难反编译

## 相关资源

- [.NET NativeAOT 文档](https://learn.microsoft.com/dotnet/core/deploying/native-aot/)
- [Kook.Net 文档](https://kooknet.dev/)
