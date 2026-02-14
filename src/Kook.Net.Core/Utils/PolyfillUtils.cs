namespace Kook;

internal static class PolyfillUtils
{
#if NETSTANDARD2_0 || NETFRAMEWORK
    extension(DateTimeOffset)
    {
        public static DateTimeOffset UnixEpoch => new(621355968000000000L, TimeSpan.Zero);
    }
#endif
}
