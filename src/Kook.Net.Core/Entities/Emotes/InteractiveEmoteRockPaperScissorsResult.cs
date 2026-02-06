namespace Kook;

/// <summary>
///     表示一个石头剪刀布的随机结果。
/// </summary>
public record InteractiveEmoteRockPaperScissorsResult : InteractiveEmoteRollResult
{
    internal InteractiveEmoteRockPaperScissorsResult(int rawValue, string image)
        : base(rawValue, image)
    {
    }

    /// <summary>
    ///     获取石头剪刀布的值。
    /// </summary>
    public RockPaperScissorsValue Value => RawValue switch
    {
        1 or 2 => RockPaperScissorsValue.Scissor,
        3 or 4 => RockPaperScissorsValue.Rock,
        5 or 6 => RockPaperScissorsValue.Paper,
        _ => (RockPaperScissorsValue)RawValue
    };

    private string DebuggerDisplay => $"RockPaperScissor: {Value} ({RawValue})";

    /// <inheritdoc />
    public override string ToString() => $"{Value} ({RawValue})";
}
