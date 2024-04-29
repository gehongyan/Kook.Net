namespace Kook.Commands;

/// <summary>
/// Utility class which contains the default matching pairs of quotation marks for CommandServiceConfig
/// </summary>
internal static class QuotationAliasUtils
{
    /// <summary>
    /// A default map of open-close pairs of quotation marks.
    /// Contains many regional and Unicode equivalents.
    /// Used in the <see cref="CommandServiceConfig"/>.
    /// </summary>
    /// <seealso cref="CommandServiceConfig.QuotationMarkAliasMap"/>
    internal static Dictionary<char, char> DefaultAliasMap =>
        // Output of a gist provided by https://gist.github.com/ufcpp
        // https://gist.github.com/ufcpp/5b2cf9a9bf7d0b8743714a0b88f7edc5
        // This was not used for the implementation because of incompatibility with netstandard1.1
        new()
        {
            { '\"', '\"' },
            { '«', '»' },
            { '‘', '’' },
            { '“', '”' },
            { '„', '‟' },
            { '‹', '›' },
            { '‚', '‛' },
            { '《', '》' },
            { '〈', '〉' },
            { '「', '」' },
            { '『', '』' },
            { '〝', '〞' },
            { '﹁', '﹂' },
            { '﹃', '﹄' },
            { '＂', '＂' },
            { '＇', '＇' },
            { '｢', '｣' },
            // Changed from ( and ) because of the usage of keyword by Kook KMarkdown
            { '（', '）' },
            { '༺', '༻' },
            { '༼', '༽' },
            { '᚛', '᚜' },
            { '⁅', '⁆' },
            { '⌈', '⌉' },
            { '⌊', '⌋' },
            { '❨', '❩' },
            { '❪', '❫' },
            { '❬', '❭' },
            { '❮', '❯' },
            { '❰', '❱' },
            { '❲', '❳' },
            { '❴', '❵' },
            { '⟅', '⟆' },
            { '⟦', '⟧' },
            { '⟨', '⟩' },
            { '⟪', '⟫' },
            { '⟬', '⟭' },
            { '⟮', '⟯' },
            { '⦃', '⦄' },
            { '⦅', '⦆' },
            { '⦇', '⦈' },
            { '⦉', '⦊' },
            { '⦋', '⦌' },
            { '⦍', '⦎' },
            { '⦏', '⦐' },
            { '⦑', '⦒' },
            { '⦓', '⦔' },
            { '⦕', '⦖' },
            { '⦗', '⦘' },
            { '⧘', '⧙' },
            { '⧚', '⧛' },
            { '⧼', '⧽' },
            { '⸂', '⸃' },
            { '⸄', '⸅' },
            { '⸉', '⸊' },
            { '⸌', '⸍' },
            { '⸜', '⸝' },
            { '⸠', '⸡' },
            { '⸢', '⸣' },
            { '⸤', '⸥' },
            { '⸦', '⸧' },
            { '⸨', '⸩' },
            { '【', '】' },
            { '〔', '〕' },
            { '〖', '〗' },
            { '〘', '〙' },
            { '〚', '〛' }
        };
}
