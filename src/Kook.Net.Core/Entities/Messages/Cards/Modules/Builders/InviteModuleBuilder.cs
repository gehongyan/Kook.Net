using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="InviteModule"/> 模块的构建器。
/// </summary>
public class InviteModuleBuilder : IModuleBuilder, IEquatable<InviteModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Invite;

    /// <summary>
    ///     初始化一个 <see cref="InviteModuleBuilder"/> 类的新实例。
    /// </summary>
    public InviteModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="InviteModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="code"> 邀请代码。 </param>
    public InviteModuleBuilder(string code)
    {
        Code = code;
    }

    /// <summary>
    ///     获取或设置邀请代码。
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    ///     设置邀请代码。
    /// </summary>
    /// <param name="code"> 邀请代码。 </param>
    /// <returns> 当前构建器。 </returns>
    public InviteModuleBuilder WithCode(string code)
    {
        Code = code;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="InviteModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="InviteModule"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Code"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Code"/> 为空或空白字符串。
    /// </exception>
    [MemberNotNull(nameof(Code))]
    public InviteModule Build()
    {
        if (Code == null)
            throw new ArgumentNullException(nameof(Code), "The code of the invite cannot be null.");
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("The code of the invite cannot be empty or whitespace.", nameof(Code));
        return new InviteModule(Code);
    }

    /// <inheritdoc cref="Kook.InviteModuleBuilder(System.String)" />
    public static implicit operator InviteModuleBuilder(string code) => new(code);

    /// <inheritdoc />
    [MemberNotNull(nameof(Code))]
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="InviteModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="InviteModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(InviteModuleBuilder? left, InviteModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="InviteModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="InviteModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(InviteModuleBuilder? left, InviteModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is InviteModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] InviteModuleBuilder? inviteModuleBuilder)
    {
        if (inviteModuleBuilder is null) return false;

        return Type == inviteModuleBuilder.Type
            && Code == inviteModuleBuilder.Code;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as InviteModuleBuilder);
}
