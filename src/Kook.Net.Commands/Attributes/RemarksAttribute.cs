namespace Kook.Commands;

// Extension of the Cosmetic Summary, for Groups, Commands, and Parameters
/// <summary>
///     Attaches remarks to your commands.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RemarksAttribute : Attribute
{
    /// <summary>
    ///     Gets the remarks of the command.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Initializes a new <see cref="RemarksAttribute" /> attribute with the specified remarks.
    /// </summary>
    /// <param name="text"></param>
    public RemarksAttribute(string text) => Text = text;
}
