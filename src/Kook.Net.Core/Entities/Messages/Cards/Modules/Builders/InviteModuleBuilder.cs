namespace Kook;

/// <summary>
///     Represents a invite module builder for creating an <see cref="InviteModule"/>.
/// </summary>
public class InviteModuleBuilder : IModuleBuilder, IEquatable<InviteModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Invite;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InviteModuleBuilder"/> class.
    /// </summary>
    public InviteModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InviteModuleBuilder"/> class.
    /// </summary>
    /// <param name="code"></param>
    public InviteModuleBuilder(string code) => WithCode(code);

    /// <summary>
    ///     Gets or sets the code of the invite.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the code of the invite.
    /// </returns>
    public string Code { get; set; }

    /// <summary>
    ///     Sets the code of the invite.
    /// </summary>
    /// <param name="code">
    ///     The code of the invite to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public InviteModuleBuilder WithCode(string code)
    {
        Code = code;
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="InviteModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="InviteModule"/> representing the built invite module object.
    /// </returns>
    public InviteModule Build() => new(Code);

    /// <summary>
    ///     Initialized a new instance of the <see cref="InviteModuleBuilder"/> class
    ///     with the specified <paramref name="code"/>.
    /// </summary>
    /// <param name="code">
    ///     The code representing the invite.
    /// </param>
    /// <returns>
    ///     An <see cref="InviteModuleBuilder"/> object that is initialized with the specified <paramref name="code"/>.
    /// </returns>
    public static implicit operator InviteModuleBuilder(string code) => new InviteModuleBuilder()
        .WithCode(code);

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="InviteModuleBuilder"/> is equal to the current <see cref="InviteModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="InviteModuleBuilder"/> is equal to the current <see cref="InviteModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(InviteModuleBuilder left, InviteModuleBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="InviteModuleBuilder"/> is not equal to the current <see cref="InviteModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="InviteModuleBuilder"/> is not equal to the current <see cref="InviteModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(InviteModuleBuilder left, InviteModuleBuilder right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="InviteModuleBuilder"/> is equal to the current <see cref="InviteModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="InviteModuleBuilder"/>, <see cref="Equals(InviteModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="InviteModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="InviteModuleBuilder"/> is equal to the current <see cref="InviteModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is InviteModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="InviteModuleBuilder"/> is equal to the current <see cref="InviteModuleBuilder"/>.</summary>
    /// <param name="inviteModuleBuilder">The <see cref="InviteModuleBuilder"/> to compare with the current <see cref="InviteModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="InviteModuleBuilder"/> is equal to the current <see cref="InviteModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(InviteModuleBuilder inviteModuleBuilder)
    {
        if (inviteModuleBuilder is null) return false;

        return Type == inviteModuleBuilder.Type
            && Code == inviteModuleBuilder.Code;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
