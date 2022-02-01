using System.Reflection;

namespace KaiHeiLa;

public class KaiHeiLaConfig
{
    public const int APIVersion = 3;
    
    public LogSeverity LogLevel { get; set; } = LogSeverity.Debug;
    
    public static string Version { get; } =
        typeof(KaiHeiLaConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        typeof(KaiHeiLaConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ??
        "Unknown";
    
    public static readonly string APIUrl = $"https://www.kaiheila.cn/api/v{APIVersion}/";
}