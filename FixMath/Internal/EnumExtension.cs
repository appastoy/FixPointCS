using System;
using System.Globalization;

namespace FixedMath
{
    internal static class EnumExtension
    {
        public static bool HasFlagFast<T>(this T _enum, T value)
            where T : struct, Enum, IConvertible
        {
            return (_enum.ToInt32(CultureInfo.InvariantCulture) & value.ToInt32(CultureInfo.InvariantCulture)) != 0;
        }
    }
}
