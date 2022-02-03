using System.Reflection;

namespace KaiHeiLa;

public class KaiHeiLaConfig
{
    /// <summary> 
    ///     Returns the API version KaiHeiLa.Net uses. 
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the API version that KaiHeiLa.Net uses to communicate with KaiHeiLa.
    ///     <para>A list of available API version can be seen on the official 
    ///     <see href="https://developer.kaiheila.cn/doc/reference">KaiHeiLa API documentation</see>
    ///     .</para>
    /// </returns>
    public const int APIVersion = 3;
    
    /// <summary>
    ///     Gets the KaiHeiLa.Net version, including the build number.
    /// </summary>
    /// <returns>
    ///     A string containing the detailed version information, including its build number; <c>Unknown</c> when
    ///     the version fails to be fetched.
    /// </returns>
    public static string Version { get; } =
        typeof(KaiHeiLaConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        typeof(KaiHeiLaConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ??
        "Unknown";

    /// <summary>
    ///     Gets the user agent that KaiHeiLa.Net uses in its clients.
    /// </summary>
    /// <returns>
    ///     The user agent used in each KaiHeiLa.Net request.
    /// </returns>
    public static string UserAgent { get; } = $"KaiHeiLaBot (https://github.com/gehongyan/KaiHeiLa.Net, v{Version})";
    
    public static readonly string APIUrl = $"https://www.kaiheila.cn/api/v{APIVersion}/";
    
    public const int DefaultRequestTimeout = 6000;
    

    /// <summary>
    ///     Gets or sets how a request should act in the case of an error, by default.
    /// </summary>
    /// <returns>
    ///     The currently set <see cref="RetryMode"/>.
    /// </returns>
    public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;

    /// <summary>
    ///     Gets or sets the default callback for ratelimits.
    /// </summary>
    /// <remarks>
    ///     This property is mutually exclusive with <see cref="RequestOptions.RatelimitCallback"/>.
    /// </remarks>
    public Func<IRateLimitInfo, Task> DefaultRatelimitCallback { get; set; }

    /// <summary>
    ///     Gets or sets the minimum log level severity that will be sent to the Log event.
    /// </summary>
    /// <returns>
    ///     The currently set <see cref="LogSeverity"/> for logging level.
    /// </returns>
    public LogSeverity LogLevel { get; set; } = LogSeverity.Debug;

    /// <summary>
    ///     Gets or sets whether the initial log entry should be printed.
    /// </summary>
    /// <remarks>
    ///     If set to <c>true</c>, the library will attempt to print the current version of the library, as well as
    ///     the API version it uses on startup.
    /// </remarks>
    internal bool DisplayInitialLog { get; set; } = true;
}