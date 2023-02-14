namespace Kook;

/// <summary>
///     Specifies the type of an <see cref="IModule"/>.
/// </summary>
public enum ModuleType
{
    /// <summary>
    ///     A <see cref="HeaderModule"/>.
    /// </summary>
    Header,
    /// <summary>
    ///     A <see cref="SectionModule"/>.
    /// </summary>
    Section,
    /// <summary>
    ///     An <see cref="ImageGroupModule"/>.
    /// </summary>
    ImageGroup,
    /// <summary>
    ///     A <see cref="ContainerModule"/>.
    /// </summary>
    Container,
    /// <summary>
    ///     An <see cref="ActionGroupModule"/>.
    /// </summary>
    ActionGroup,
    /// <summary>
    ///     A <see cref="ContextModule"/>.
    /// </summary>
    Context,
    /// <summary>
    ///     A <see cref="DividerModule"/>.
    /// </summary>
    Divider,
    /// <summary>
    ///     A <see cref="FileModule"/>.
    /// </summary>
    File,
    /// <summary>
    ///     An <see cref="AudioModule"/>.
    /// </summary>
    Audio,
    /// <summary>
    ///     A <see cref="VideoModule"/>.
    /// </summary>
    Video,
    /// <summary>
    ///     A <see cref="CountdownModule"/>.
    /// </summary>
    Countdown,
    /// <summary>
    ///     An <see cref="InviteModule"/>.
    /// </summary>
    Invite
}
