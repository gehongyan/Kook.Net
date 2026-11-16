# SimpleBot NativeAOT 发布

此示例是实际的 WebSocket Bot，登录后可响应 `!ping` 并处理卡片按钮。
托管与 NativeAOT 模式均固定直接引用 `Kook.Net.WebSocket` 和 `Kook.Net.Experimental`，不按发布模式切换引用。示例移除了自定义 InMemory 缓冲队列配置，使用客户端默认的同步即时队列。

注意：这里只移除 `samples.props` 导入的直接 `Kook.Net` 引用；Experimental 目前仍传递引用 umbrella 包及 Commands、Pipe 等组件。保留 Experimental 不代表这些组件已获得 NativeAOT 兼容保证，应检查最终应用发布时的全部诊断。
普通 `dotnet run` 保持托管运行；`SimpleBotNativeAot=true` 仅为此可执行项目启用 NativeAOT。
不要把 `PublishAot=true` 全局传给多目标框架的整个仓库。

## Windows x64

安装 `global.json` 指定的 .NET SDK，以及 Visual Studio 的“使用 C++ 的桌面开发”工作负载。
从仓库根目录执行：

```powershell
dotnet publish .\samples\Kook.Net.Samples.SimpleBot -c Release -r win-x64 -p:SimpleBotNativeAot=true -o .\artifacts\simplebot-native
# PowerShell 7：输入不会回显，也无需把 Token 写进命令历史或源代码。
[Environment]::SetEnvironmentVariable("KookDebugToken", (Read-Host "KOOK Bot Token" -MaskInput), "User")
.\artifacts\simplebot-native\Kook.Net.Samples.SimpleBot.exe
```

程序保持现有行为，读取 Windows 用户级 `KookDebugToken`。上面的设置会持久保存到用户环境变量；单独设置 `$env:KookDebugToken` 不满足该读取方式。
请勿提交或分享真实 Token。Bot 应已加入用于测试的服务器，并具备收发消息权限。

## Linux x64：仅发布验证

安装 SDK、原生编译器与 zlib 开发包后，从仓库根目录执行：

```bash
dotnet publish samples/Kook.Net.Samples.SimpleBot -c Release -r linux-x64 -p:SimpleBotNativeAot=true -o artifacts/simplebot-native
```

示例现有的用户级环境变量读取方式不适用于 Linux，故这里不提供未经验证的 Linux 登录命令。发布配置不改变 Token 读取逻辑。

## 验证边界与诊断

发布配置不代表已验证真实 KOOK 登录、消息收发或全部客户端生命周期。
请区分原生发布、进程启动和实际联网测试；仅启动到缺少 Token 的检查不算登录成功。

当前完整 Bot 的发布路径仍可能报告 `IL3050`：
`KookRestApiClient.GetGuildMembersAsync` 中表达式树使用 `Expression.NewArrayInit`。
这里不屏蔽此诊断；它需要进一步验证或改写，不能据此宣称整个客户端零 AOT 风险。
JSON 协议测试通过也不覆盖所有真实客户端路径。
