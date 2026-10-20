using System.Diagnostics;
using System.Text.Json;
using Kook.Net.Docs.Mcp;
using Kook.Net.Docs.Mcp.Api;
using Kook.Net.Docs.Mcp.Guides;
using Kook.Net.Docs.Mcp.Search;
using Kook.Net.Docs.Mcp.Source;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

// Kook.Net 文档 MCP 服务（Streamable HTTP 传输）。
//
// 数据来源为 docfx 的构建产物：
//   - api/*.yml       （docfx metadata 从源码 XML 注释生成的 ManagedReference）
//   - _site/index.json（docfx build 的 ExtractSearchIndex 后处理器生成）
// 因此运行前需在 docs 工程下执行一次 `docfx`（或分别 `docfx metadata` / `docfx build`）。
//
// 工程根目录定位：环境变量 KOOK_DOCS_ROOT > 当前目录向上找 docfx.json > 程序目录向上找。
// 容器部署时把产物目录挂进容器并用 KOOK_DOCS_ROOT 指向它。
//
// MCP 端点默认映射在根路径（POST/GET /），客户端用 Streamable HTTP 连接
// http://<host>:<port>/。监听地址由 ASPNETCORE_URLS 控制，默认 http://0.0.0.0:3001。

// 控制台日志使用 Serilog。日志内容约定为 "[Tag] 消息 [字段]{值}"，值同时是结构化属性。
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .WriteTo.Console(LogEventLevel.Debug,
        "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(Log.Logger, dispose: true);

    // 文档路径与数据源服务。路径在启动时解析一次，解析失败直接退出并给出指引。
    builder.Services.AddSingleton(_ => DocsPaths.Resolve());
    builder.Services.AddSingleton<SearchIndexService>();
    builder.Services.AddSingleton<ApiModelService>();
    builder.Services.AddSingleton<GuideService>();
    builder.Services.AddSingleton<SourceService>();

    // MCP 服务：Streamable HTTP 传输 + 从本程序集按特性发现工具，
    // 并通过 call-tool 过滤器记录每次工具调用的工具名、参数摘要、耗时与成败。
    builder.Services
        .AddMcpServer()
        .WithHttpTransport()
        .WithToolsFromAssembly()
        .WithRequestFilters(filters =>
        {
            filters.AddCallToolFilter(next => async (context, cancellationToken) =>
            {
                Microsoft.Extensions.Logging.ILogger logger =
                    context.Services?.GetService(typeof(ILogger<Program>)) as Microsoft.Extensions.Logging.ILogger
                    ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

                string tool = context.Params?.Name ?? "(unknown)";
                string args = SummarizeArguments(context.Params?.Arguments);
                ToolCallLog.CallStarted(logger, tool, args);

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    var result = await next(context, cancellationToken);
                    sw.Stop();
                    ToolCallLog.CallCompleted(logger, tool, result.IsError ?? false, sw.ElapsedMilliseconds);
                    return result;
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    ToolCallLog.CallFailed(logger, ex, tool, sw.ElapsedMilliseconds);
                    throw;
                }
            });
        });

    WebApplication app = builder.Build();

    // 把 MCP 端点映射到根路径。
    app.MapMcp();

    // 未显式配置监听地址时，默认绑定容器可访问的 0.0.0.0:3001。
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
        app.Urls.Add("http://0.0.0.0:3001");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "[MCP] 服务启动失败 [Reason]{Reason}", ex.Message);
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}

// 把工具参数压成一行简短摘要用于日志；仅为可观测性，不记录完整大段内容。
static string SummarizeArguments(IDictionary<string, JsonElement>? arguments)
{
    if (arguments is null || arguments.Count == 0)
        return "(none)";

    IEnumerable<string> pairs = arguments.Select(kv =>
    {
        string value = kv.Value.ValueKind == JsonValueKind.String
            ? kv.Value.GetString() ?? string.Empty
            : kv.Value.GetRawText();
        if (value.Length > 120)
            value = value[..120] + "…";
        return $"{kv.Key}={value}";
    });

    return string.Join(", ", pairs);
}
