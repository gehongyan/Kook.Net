using System.Runtime.CompilerServices;

namespace KaiHeiLa
{
    internal static class Permissions
    {
        public const int MaxBits = 53;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, ChannelPermission flag)
            => GetValue(allow, deny, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, GuildPermission flag)
            => GetValue(allow, deny, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(ulong allow, ulong deny, ulong flag)
        {
            if (HasFlag(allow, flag))
                return PermValue.Allow;
            else if (HasFlag(deny, flag))
                return PermValue.Deny;
            else
                return PermValue.Inherit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, ChannelPermission flag)
            => GetValue(value, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, GuildPermission flag)
            => GetValue(value, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(ulong value, ulong flag) => HasFlag(value, flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, ChannelPermission flag)
            => SetValue(ref rawValue, value, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, GuildPermission flag)
            => SetValue(ref rawValue, value, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong rawValue, bool? value, ulong flag)
        {
            if (value.HasValue)
            {
                if (value == true)
                    SetFlag(ref rawValue, flag);
                else
                    UnsetFlag(ref rawValue, flag);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ChannelPermission flag)
            => SetValue(ref allow, ref deny, value, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, GuildPermission flag)
            => SetValue(ref allow, ref deny, value, (ulong) flag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ulong flag)
        {
            if (value.HasValue)
            {
                switch (value)
                {
                    case PermValue.Allow:
                        SetFlag(ref allow, flag);
                        UnsetFlag(ref deny, flag);
                        break;
                    case PermValue.Deny:
                        UnsetFlag(ref allow, flag);
                        SetFlag(ref deny, flag);
                        break;
                    default:
                        UnsetFlag(ref allow, flag);
                        UnsetFlag(ref deny, flag);
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasFlag(ulong value, ulong flag) => (value & flag) == flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref ulong value, ulong flag) => value |= flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetFlag(ref ulong value, ulong flag) => value &= ~flag;
    }
}
