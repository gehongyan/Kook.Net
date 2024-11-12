using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Kook;

#nullable enable

/// <summary>
///     提供用于检查先决条件的方法。
/// </summary>
internal static class Preconditions
{
    #region Objects

    /// <exception cref="ArgumentNullException"> <paramref name="obj"/> 不可为 <c>null</c>. </exception>
    public static void NotNull<T>([NotNull] T? obj, string name, string? msg = null) where T : class
    {
        if (obj == null)
            throw CreateNotNullException(name, msg);
    }

    private static ArgumentNullException CreateNotNullException(string name, string? msg) =>
        new(paramName: name, message: msg);

    public static void EnsureMessageProperties<T>(MessageProperties<T> properties)
    {
        if (properties is null)
            throw new ArgumentNullException(nameof(properties));
        bool contentSet = !string.IsNullOrEmpty(properties.Content);
        bool cardsSet = properties.Cards?.Count > 0;
        bool templateSet = properties.TemplateId.HasValue;
        if (contentSet && cardsSet)
            throw new ArgumentException("Content and Cards cannot be set at the same time.");
        if (!contentSet && !cardsSet && !templateSet)
            throw new ArgumentException("If neither Content nor Cards are set, TemplateId must be set.");
        if (templateSet && properties.Parameters is null)
            throw new ArgumentException("When TemplateId is set, Parameters must also be set.");
    }

    #endregion

    #region Strings

    /// <exception cref="ArgumentException"> <paramref name="obj"/> 不可为空白内容. </exception>
    public static void NotEmpty(string obj, string name, string? msg = null)
    {
        if (obj.Length == 0)
            throw CreateNotEmptyException(name, msg);
    }

    /// <exception cref="ArgumentException"> <paramref name="obj"/> 不可为空白内容. </exception>
    /// <exception cref="ArgumentNullException"> <paramref name="obj"/> 不可为 <c>null</c>. </exception>
    public static void NotNullOrEmpty([NotNull] string? obj, string name, string? msg = null)
    {
        if (obj == null || obj.Length == 0)
            throw CreateNotNullException(name, msg);
    }
    /// <exception cref="ArgumentException"> <paramref name="obj"/> 不可为空白内容. </exception>
    /// <exception cref="ArgumentNullException"> <paramref name="obj"/> 不可为 <c>null</c>. </exception>
    public static void NotNullOrWhitespace([NotNull] string? obj, string name, string? msg = null)
    {
        if (obj == null || obj.Trim().Length == 0)
            throw CreateNotNullException(name, msg);
    }

    private static ArgumentException CreateNotEmptyException(string name, string? msg) =>
        new ArgumentException(message: msg ?? "Argument cannot be blank.", paramName: name);

    #endregion

    #region Numerics

#if NET7_0_OR_GREATER
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual<T>(T? obj, T value, string name, string? msg = null)
        where T : struct, IEqualityOperators<T, T, bool>
    {
        if (obj.HasValue && obj == value)
            throw CreateNotEqualException(name, msg, value);
    }
#endif

    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(sbyte obj, sbyte value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(byte obj, byte value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(short obj, short value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(ushort obj, ushort value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(int obj, int value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(uint obj, uint value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(long obj, long value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(ulong obj, ulong value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(Guid obj, Guid value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(sbyte? obj, sbyte value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(byte? obj, byte value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(short? obj, short value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(ushort? obj, ushort value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(int? obj, int value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(uint? obj, uint value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(long? obj, long value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(ulong? obj, ulong value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可与 <paramref name="value"/> 相等。 </exception>
    public static void NotEqual(Guid? obj, Guid value, string name, string? msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }

    private static ArgumentException CreateNotEqualException<T>(string name, string? msg, T value) =>
        new ArgumentException(message: msg ?? $"Value may not be equal to {value}.", paramName: name);

#if NET7_0_OR_GREATER
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast<T>(T? obj, T value, string name, string? msg = null)
        where T : struct, IComparisonOperators<T, T, bool>
    {
        if (obj.HasValue && obj < value)
            throw CreateAtLeastException(name, msg, value);
    }
#endif

    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(sbyte obj, sbyte value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(byte obj, byte value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(short obj, short value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(ushort obj, ushort value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(int obj, int value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(uint obj, uint value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(long obj, long value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(ulong obj, ulong value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(sbyte? obj, sbyte value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(byte? obj, byte value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(short? obj, short value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(ushort? obj, ushort value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(int? obj, int value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(uint? obj, uint value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(long? obj, long value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可小于 <paramref name="value"/>。 </exception>
    public static void AtLeast(ulong? obj, ulong value, string name, string? msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }

    private static ArgumentException CreateAtLeastException<T>(string name, string? msg, T value) =>
        new ArgumentException(message: msg ?? $"Value must be at least {value}.", paramName: name);

#if NET7_0_OR_GREATER
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan<T>(T? obj, T value, string name, string? msg = null)
        where T : struct, IComparisonOperators<T, T, bool>
    {
        if (obj.HasValue && obj <= value)
            throw CreateGreaterThanException(name, msg, value);
    }
#endif

    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(sbyte obj, sbyte value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(byte obj, byte value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(short obj, short value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(ushort obj, ushort value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(int obj, int value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(uint obj, uint value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(long obj, long value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(ulong obj, ulong value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(sbyte? obj, sbyte value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(byte? obj, byte value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(short? obj, short value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(ushort? obj, ushort value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(int? obj, int value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(uint? obj, uint value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(long? obj, long value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须大于 <paramref name="value"/>。 </exception>
    public static void GreaterThan(ulong? obj, ulong value, string name, string? msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }

    private static ArgumentException CreateGreaterThanException<T>(string name, string? msg, T value) =>
        new ArgumentException(message: msg ?? $"Value must be greater than {value}.", paramName: name);

#if NET7_0_OR_GREATER
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost<T>(T? obj, T value, string name, string? msg = null)
        where T : struct, IComparisonOperators<T, T, bool>
    {
        if (obj.HasValue && obj > value)
            throw CreateAtMostException(name, msg, value);
    }
#endif

    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(sbyte obj, sbyte value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(byte obj, byte value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(short obj, short value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(ushort obj, ushort value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(int obj, int value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(uint obj, uint value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(long obj, long value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(ulong obj, ulong value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(sbyte? obj, sbyte value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(byte? obj, byte value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(short? obj, short value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(ushort? obj, ushort value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(int? obj, int value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(uint? obj, uint value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(long? obj, long value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 不可大于 <paramref name="value"/>。 </exception>
    public static void AtMost(ulong? obj, ulong value, string name, string? msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }

    private static ArgumentException CreateAtMostException<T>(string name, string? msg, T value) =>
        new ArgumentException(message: msg ?? $"Value must be at most {value}.", paramName: name);

#if NET7_0_OR_GREATER
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan<T>(T? obj, T value, string name, string? msg = null)
        where T : struct, IComparisonOperators<T, T, bool>
    {
        if (obj.HasValue && obj >= value)
            throw CreateLessThanException(name, msg, value);
    }
#endif

    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(sbyte obj, sbyte value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(byte obj, byte value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(short obj, short value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(ushort obj, ushort value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(int obj, int value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(uint obj, uint value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(long obj, long value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(ulong obj, ulong value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(sbyte? obj, sbyte value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(byte? obj, byte value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(short? obj, short value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(ushort? obj, ushort value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(int? obj, int value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(uint? obj, uint value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(long? obj, long value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException"> <paramref name="obj" /> 必须小于 <paramref name="value"/>。 </exception>
    public static void LessThan(ulong? obj, ulong value, string name, string? msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }

    private static ArgumentException CreateLessThanException<T>(string name, string? msg, T value) =>
        new ArgumentException(message: msg ?? $"Value must be less than {value}.", paramName: name);

    #endregion
}
