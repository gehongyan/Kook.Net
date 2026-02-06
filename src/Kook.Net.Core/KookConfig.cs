using System.Reflection;

namespace Kook;

/// <summary>
///     定义 Kook.Net 各种基础行为的配置项。
/// </summary>
public class KookConfig
{
    /// <summary>
    ///     获取 Kook.Net 使用的 API 版本。
    /// </summary>
    public const int APIVersion = 3;

    /// <summary>
    ///     获取 Kook.Net 用于与 KOOK 的语音服务器通信的 API 版本。
    /// </summary>
    public const int VoiceAPIVersion = 1;

    /// <summary>
    ///     获取 Kook.Net 的版本，包括构建号。
    /// </summary>
    /// <returns> 一个包含详细版本信息的字符串，包括构建号；当无法获取构建版本时为 <c>Unknown</c>。 </returns>
    public static string Version { get; } =
        typeof(KookConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(KookConfig).GetTypeInfo().Assembly.GetName().Version?.ToString(3)
        ?? "Unknown";

    /// <summary>
    ///     获取 Kook.Net 在每个请求中使用的用户代理。
    /// </summary>
    public static string UserAgent { get; } = $"KookBot (https://github.com/gehongyan/Kook.Net, v{Version})";

    /// <summary>
    ///     获取 Kook.Net 在每个请求所设置 Accept-Language 请求标头，用于指定所偏好的自然语言和区域设置。
    /// </summary>
    /// <remarks>
    ///     可用值包括：
    ///     <list type="bullet">
    ///         <item> <description> <c>zh-CN</c> - 简体中文（中国） </description> </item>
    ///         <item> <description> <c>en-US</c> - 英语（美国） </description> </item>
    ///     </list>
    ///     要查阅最新支持的语言列表，请参阅 https://developer.kookapp.cn/doc/reference 中的 i18N 节。
    /// </remarks>
    public string AcceptLanguage { get; set; } = "zh-CN";

    /// <summary>
    ///     获取 KOOK API 请求的根 URL。
    /// </summary>
    public static readonly string APIUrl = $"https://www.kookapp.cn/api/v{APIVersion}/";

    /// <summary>
    ///     获取 KOOK 互动表情资源的根 URL。
    /// </summary>
    public static readonly string InteractiveEmoteResourceUrl = "https://img.kookapp.cn/emojis/touzi/";

    /// <summary>
    ///     获取请求超时的默认时间，以毫秒为单位。
    /// </summary>
    public const int DefaultRequestTimeout = 6000;

    /// <summary>
    ///     获取 KOOK 允许的消息的最大长度。
    /// </summary>
    public const int MaxMessageSize = 20000;

    /// <summary>
    ///     获取 KOOK 允许在每个请求中获取的最大用户数。
    /// </summary>
    public const int MaxUsersPerBatch = 50;

    /// <summary>
    ///     获取 KOOK 允许在每个请求中获取的最大消息数。
    /// </summary>
    public const int MaxMessagesPerBatch = 50;

    /// <summary>
    ///     获取 KOOK 允许在每个请求中获取的最大消息模板数。
    /// </summary>
    public const int MaxMessageTemplatesPerBatch = 20;

    /// <summary>
    ///     获取 KOOK 允许在每个请求中获取的最大帖子数。
    /// </summary>
    public const int MaxThreadsPerBatch = 30;

    /// <summary>
    ///     获取默认情况下每个请求允许获取的最大项目数。
    /// </summary>
    public const int MaxItemsPerBatchByDefault = 100;

    /// <summary>
    ///     获取 KOOK 允许的服务器用户昵称的最小长度。
    /// </summary>
    public const int MinNicknameSize = 2;

    /// <summary>
    ///     获取 KOOK 允许的服务器用户昵称的最大长度。
    /// </summary>
    public const int MaxNicknameSize = 64;

    /// <summary>
    ///     获取 KOOK 允许的亲密度分数的最小值。
    /// </summary>
    public const int MinIntimacyScore = 0;

    /// <summary>
    ///     获取 KOOK 允许的亲密度分数的最大值。
    /// </summary>
    public const int MaxIntimacyScore = 2200;

    /// <summary>
    ///     获取每个服务器助力包的生效时长。
    /// </summary>
    public static readonly TimeSpan BoostPackDuration = TimeSpan.FromDays(30);

    /// <summary>
    ///     对每条消息首次回复时累计每日消息可发送条数总量消耗折扣的时间跨度。
    /// </summary>
    public static readonly TimeSpan MessageReplyDiscountTimeSpan = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     获取或设置请求在出现错误时的默认行为。
    /// </summary>
    /// <seealso cref="Kook.RequestOptions.RetryMode"/>
    public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;

    /// <summary>
    ///     获取或设置默认的速率限制回调委托。
    /// </summary>
    /// <remarks>
    ///     若同时设置了此属性与用于各个请求的 <see cref="Kook.RequestOptions.RatelimitCallback"/>，则将优先使用
    ///     <see cref="Kook.RequestOptions.RatelimitCallback"/>。
    /// </remarks>
    public Func<IRateLimitInfo, Task>? DefaultRatelimitCallback { get; set; }

    /// <summary>
    ///     获取或设置将发送到日志事件的最低日志严重性级别。
    /// </summary>
    public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

    /// <summary>
    ///     获取或设置是否应打印初次启动时要打印的日志。
    /// </summary>
    /// <remarks>
    ///     如果设置为 <c>true</c>，则将在启动时打印库的当前版本，以及所使用的 API 版本。
    /// </remarks>
    internal bool DisplayInitialLog { get; set; } = true;

    /// <summary>
    ///     获取由 KOOK 官方发送系统消息的用户的 ID。
    /// </summary>
    internal const ulong SystemMessageAuthorID = 3900775823;

    /// <summary>
    ///     获取或设置 Rest 或 Socket 用户实体的 <see cref="System.Object.ToString"/> 重写方法在格式化字符串时是否考虑双向 Unicode。
    /// </summary>
    /// <remarks>
    ///     默认地，为了支持双向用户名，格式化字符串中的用户名部分的左侧会插入左到右嵌入控制字符（<c>\u2066</c>），
    ///     右侧会插入嵌入段结束控制字符（<c>\u2069</c>），以确保在显示时不会出现混乱。如需禁用此行为，请将此属性设置为 <c>false</c>。
    /// </remarks>
    /// <seealso cref="Kook.Format.UsernameAndIdentifyNumber(Kook.IUser,System.Boolean)"/>
    public bool FormatUsersInBidirectionalUnicode { get; set; } = true;
}
