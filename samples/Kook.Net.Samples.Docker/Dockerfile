FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /samples
COPY ["Kook.Net.Samples.Docker/Kook.Net.Samples.Docker.csproj", "Kook.Net.Samples.Docker/"]
RUN dotnet restore "Kook.Net.Samples.Docker/Kook.Net.Samples.Docker.csproj"
COPY . .
WORKDIR "/samples/Kook.Net.Samples.Docker"
RUN dotnet build "Kook.Net.Samples.Docker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kook.Net.Samples.Docker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kook.Net.Samples.Docker.dll"]
