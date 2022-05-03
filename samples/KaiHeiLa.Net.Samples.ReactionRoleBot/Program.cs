using KaiHeiLa;
using KaiHeiLa.Net.Samples.ReactionRoleBot.Configurations;
using KaiHeiLa.Net.Samples.ReactionRoleBot.Extensions;
using KaiHeiLa.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Log.Information("Starting KaiHeiLa.Net API Helper");

        using IHost host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            //.AddGlobalErrorHandler()
            .ConfigureServices(ConfigureServices)
            .UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Verbose()
                    //.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose);
            });
        return hostBuilder;
    }

    /// <summary>
    ///     配置服务
    /// </summary>
    /// <param name="hostContext"></param>
    /// <param name="services"></param>
    private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
    {
        // 根配置
        IConfigurationRoot configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .Build();

        // 开黑啦 Webhook 配置
        KaiHeiLaBotConfigurations kaiHeiLaBotConfigurations = new();
        configurationRoot.GetSection(nameof(KaiHeiLaBotConfigurations))
            .Bind(kaiHeiLaBotConfigurations);

        // 服务配置
        services
            // 静态配置
            .AddSingleton(kaiHeiLaBotConfigurations)

            // 开黑啦客户端程序
            .AddSingleton<KaiHeiLaBotClientExtension>()
            .AddHostedService(p => p.GetRequiredService<KaiHeiLaBotClientExtension>())
            .AddSingleton(_ =>
                new KaiHeiLaSocketClient(new KaiHeiLaSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Debug
                }))

            .AddHttpClient();
    }
}