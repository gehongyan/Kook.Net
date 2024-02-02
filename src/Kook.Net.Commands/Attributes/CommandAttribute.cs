namespace Kook.Commands;

/// <summary>
///     Marks the execution information for a command.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandAttribute : Attribute
{
    /// <summary>
    ///     Gets the text that has been set to be recognized as a command.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Specifies the <see cref="RunMode" /> of the command. This affects how the command is executed.
    /// </summary>
    public RunMode RunMode { get; set; } = RunMode.Default;

    /// <summary>
    ///     Indicates whether extra arguments should be ignored for this command.
    /// </summary>
    public bool? IgnoreExtraArgs { get; }

    /// <summary>
    ///     Attaches a summary to your command.
    /// </summary>
    /// <remarks>
    ///     <see cref="Summary"/> overrides the value of this property if present.
    /// </remarks>
    public string Summary { get; set; }

    /// <summary>
    ///     Marks the aliases for a command.
    /// </summary>
    /// <remarks>
    ///     <see cref="AliasAttribute"/> extends the base value of this if present.
    /// </remarks>
    public string[] Aliases { get; set; }

    /// <summary>
    ///     Attaches remarks to your commands.
    /// </summary>
    /// <remarks>
    ///     <see cref="RemainderAttribute"/> overrides the value of this property if present.
    /// </remarks>
    public string Remarks { get; set; }

    /// <inheritdoc />
    public CommandAttribute() => Text = null;

    /// <summary>
    ///     Initializes a new <see cref="CommandAttribute" /> attribute with the specified name.
    /// </summary>
    /// <param name="text">The name of the command.</param>
    public CommandAttribute(string text) => Text = text;

    /// <summary>
    ///     Initializes a new <see cref="CommandAttribute" /> attribute with the specified name
    ///     and mode of whether to ignore extra arguments.
    /// </summary>
    /// <param name="text"> The name of the command. </param>
    /// <param name="ignoreExtraArgs"> Whether to ignore extra arguments. </param>
    /// <param name="summary"> The summary of the command. </param>
    /// <param name="aliases"> The aliases of the command. </param>
    /// <param name="remarks"> The remarks of the command. </param>
    public CommandAttribute(string text, bool ignoreExtraArgs,
        string summary = default, string[] aliases = default, string remarks = default)
    {
        Text = text;
        IgnoreExtraArgs = ignoreExtraArgs;
        Summary = summary;
        Aliases = aliases;
        Remarks = remarks;
    }
}
