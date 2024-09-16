namespace Kook.Commands;

/// <summary>
///     包含默认的匹配引号对的帮助类，用于 <see cref="Kook.Commands.CommandServiceConfig"/>。
/// </summary>
internal static class QuotationAliasUtils
{
    /// <summary>
    ///     一个默认的引号对的开闭映射，这可能包含了许多地区和 Unicode 符号中可视为开闭对的符号。
    /// </summary>
    /// <seealso cref="Kook.Commands.CommandServiceConfig.QuotationMarkAliasMap"/>
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
            // { '(', ')' }, // Removed ( and ) because of the usage of keyword by KOOK KMarkdown
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
            { '（', '）' },
            { '【', '】' },
            { '〔', '〕' },
            { '〖', '〗' },
            { '〘', '〙' },
            { '〚', '〛' }
        };
}
