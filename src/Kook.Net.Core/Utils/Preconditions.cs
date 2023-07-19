namespace Kook;

/// <summary>
///     Provides methods to check preconditions.
/// </summary>
public static class Preconditions
{
    #region Objects
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <c>null</c>.</exception>
    public static void NotNull<T>(T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }

    private static ArgumentNullException CreateNotNullException(string name, string msg)
    {
        if (msg == null)
            return new ArgumentNullException(paramName: name);
        else
            return new ArgumentNullException(paramName: name, message: msg);
    }
    #endregion

    #region Strings
    /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
    public static void NotEmpty(string obj, string name, string msg = null) { if (obj.Length == 0) throw CreateNotEmptyException(name, msg); }
    /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <c>null</c>.</exception>
    public static void NotNullOrEmpty(string obj, string name, string msg = null)
    {
        if (obj == null)
            throw CreateNotNullException(name, msg);
        if (obj.Length == 0)
            throw CreateNotEmptyException(name, msg);
    }
    /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <c>null</c>.</exception>
    public static void NotNullOrWhitespace(string obj, string name, string msg = null)
    {
        if (obj == null)
            throw CreateNotNullException(name, msg);
        if (obj.Trim().Length == 0)
            throw CreateNotEmptyException(name, msg);
    }

    private static ArgumentException CreateNotEmptyException(string name, string msg)
        => new ArgumentException(message: msg ?? "Argument cannot be blank.", paramName: name);
    #endregion

    #region Numerics
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(sbyte obj, sbyte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(byte obj, byte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(short obj, short value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(ushort obj, ushort value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(int obj, int value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(uint obj, uint value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(long obj, long value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(ulong obj, ulong value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(Guid obj, Guid value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(sbyte? obj, sbyte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(byte? obj, byte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(short? obj, short value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(ushort? obj, ushort value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(int? obj, int value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(uint? obj, uint value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(long? obj, long value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(ulong? obj, ulong value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
    /// <exception cref="ArgumentException">Value may not be equal to <paramref name="value"/>.</exception>
    public static void NotEqual(Guid? obj, Guid value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }

    private static ArgumentException CreateNotEqualException<T>(string name, string msg, T value)
        => new ArgumentException(message: msg ?? $"Value may not be equal to {value}.", paramName: name);

    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(sbyte obj, sbyte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(byte obj, byte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(short obj, short value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(ushort obj, ushort value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(int obj, int value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(uint obj, uint value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(long obj, long value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(ulong obj, ulong value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(sbyte? obj, sbyte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(byte? obj, byte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(short? obj, short value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(ushort? obj, ushort value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(int? obj, int value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(uint? obj, uint value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(long? obj, long value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at least <paramref name="value"/>.</exception>
    public static void AtLeast(ulong? obj, ulong value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }

    private static ArgumentException CreateAtLeastException<T>(string name, string msg, T value)
        => new ArgumentException(message: msg ?? $"Value must be at least {value}.", paramName: name);

    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(sbyte obj, sbyte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(byte obj, byte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(short obj, short value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(ushort obj, ushort value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(int obj, int value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(uint obj, uint value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(long obj, long value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(ulong obj, ulong value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(sbyte? obj, sbyte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(byte? obj, byte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(short? obj, short value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(ushort? obj, ushort value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(int? obj, int value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(uint? obj, uint value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(long? obj, long value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be greater than <paramref name="value"/>.</exception>
    public static void GreaterThan(ulong? obj, ulong value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }

    private static ArgumentException CreateGreaterThanException<T>(string name, string msg, T value)
        => new ArgumentException(message: msg ?? $"Value must be greater than {value}.", paramName: name);

    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(sbyte obj, sbyte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(byte obj, byte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(short obj, short value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(ushort obj, ushort value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(int obj, int value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(uint obj, uint value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(long obj, long value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(ulong obj, ulong value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(sbyte? obj, sbyte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(byte? obj, byte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(short? obj, short value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(ushort? obj, ushort value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(int? obj, int value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(uint? obj, uint value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(long? obj, long value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be at most <paramref name="value"/>.</exception>
    public static void AtMost(ulong? obj, ulong value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }

    private static ArgumentException CreateAtMostException<T>(string name, string msg, T value)
        => new ArgumentException(message: msg ?? $"Value must be at most {value}.", paramName: name);

    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(sbyte obj, sbyte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(byte obj, byte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(short obj, short value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(ushort obj, ushort value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(int obj, int value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(uint obj, uint value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(long obj, long value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(ulong obj, ulong value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(sbyte? obj, sbyte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(byte? obj, byte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(short? obj, short value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(ushort? obj, ushort value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(int? obj, int value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(uint? obj, uint value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(long? obj, long value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
    /// <exception cref="ArgumentException">Value must be less than <paramref name="value"/>.</exception>
    public static void LessThan(ulong? obj, ulong value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }

    private static ArgumentException CreateLessThanException<T>(string name, string msg, T value)
        => new ArgumentException(message: msg ?? $"Value must be less than {value}.", paramName: name);
    #endregion
}
