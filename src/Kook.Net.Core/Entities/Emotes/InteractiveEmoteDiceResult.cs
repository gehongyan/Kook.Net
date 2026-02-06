using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个骰子的随机结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record InteractiveEmoteDiceResult : InteractiveEmoteRollResult
{
    internal InteractiveEmoteDiceResult(int rawValue, string image)
        : base(rawValue, image)
    {
    }

    /// <summary>
    ///     获取骰子的值。
    /// </summary>
    public DiceValue Value => (DiceValue)RawValue;

    private string DebuggerDisplay => $"Dice: {Value} ({RawValue})";

    /// <inheritdoc />
    public override string ToString() => $"{Value} ({RawValue})";
}