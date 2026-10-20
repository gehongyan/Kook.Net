FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# 先还原（利用 Docker 层缓存）：只拷工程、中央包版本与 NuGet 源配置。
COPY ["nuget.config", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["tools/Kook.Net.Docs.Mcp/Kook.Net.Docs.Mcp.csproj", "tools/Kook.Net.Docs.Mcp/"]
RUN dotnet restore "tools/Kook.Net.Docs.Mcp/Kook.Net.Docs.Mcp.csproj"

# 再拷 MCP 项目源码并发布。
COPY ["tools/Kook.Net.Docs.Mcp/", "tools/Kook.Net.Docs.Mcp/"]
RUN dotnet publish "tools/Kook.Net.Docs.Mcp/Kook.Net.Docs.Mcp.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# 运行配置。
ENV ASPNETCORE_URLS=http://0.0.0.0:3001 \
    KOOK_DOCS_ROOT=/app/docs \
    KOOK_DOCS_API_DIR=api-mcp \
    DOTNET_EnableDiagnostics=0

# 应用程序本体。
COPY --from=build /app/publish ./

# 文档数据与源码（供工具读取）。这些由 docfx 预先生成 / 来自仓库，直接 COPY 进镜像，
# 使容器自包含，运行时不依赖文档站或外部文件系统。
COPY ["docs/api-mcp/", "docs/api-mcp/"]
COPY ["docs/_site/index.json", "docs/_site/index.json"]
COPY ["docs/guides/", "docs/guides/"]
COPY ["docs/quick_reference/", "docs/quick_reference/"]
COPY ["docs/demos/", "docs/demos/"]
COPY ["docs/changelog/", "docs/changelog/"]
COPY ["docs/docfx.json", "docs/docfx.json"]
# get_source 读取源码片段需要源码树（含私有实现）。
COPY ["src/", "src/"]

EXPOSE 3001
ENTRYPOINT ["dotnet", "Kook.Net.Docs.Mcp.dll"]
