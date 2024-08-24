namespace Kook;

/// <summary>
///     表示一个 <see cref="IModule"/> 的类型。
/// </summary>
public enum ModuleType
{
    /// <summary>
    ///     标题模块。
    /// </summary>
    Header,

    /// <summary>
    ///     内容模块。
    /// </summary>
    Section,

    /// <summary>
    ///     图片组模块。
    /// </summary>
    ImageGroup,

    /// <summary>
    ///     容器模块。
    /// </summary>
    Container,

    /// <summary>
    ///     按钮组模块。
    /// </summary>
    ActionGroup,

    /// <summary>
    ///     备注模块。
    /// </summary>
    Context,

    /// <summary>
    ///     分割线模块。
    /// </summary>
    Divider,

    /// <summary>
    ///     文件模块。
    /// </summary>
    File,

    /// <summary>
    ///     音频模块。
    /// </summary>
    Audio,

    /// <summary>
    ///     视频模块。
    /// </summary>
    Video,

    /// <summary>
    ///     倒计时模块。
    /// </summary>
    Countdown,

    /// <summary>
    ///     邀请模块。
    /// </summary>
    Invite
}
