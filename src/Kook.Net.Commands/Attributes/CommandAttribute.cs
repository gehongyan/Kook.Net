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
    public CommandAttribute(string text, bool ignoreExtraArgs)
    {
        Text = text;
        IgnoreExtraArgs = ignoreExtraArgs;
    }
}
