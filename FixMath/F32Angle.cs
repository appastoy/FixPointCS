using System;
using System.Runtime.CompilerServices;

namespace FixedMath
{
    public readonly struct F32Angle : IComparable<F32Angle>, IEquatable<F32Angle>, IComparable
    {
        public static readonly F32 Deg360 = F32.FromInt(360);
        public static readonly F32 Deg180 = F32.FromInt(180);

        [MethodImpl(FixedUtil.Inline)] public static F32 Adjust360(F32 value) => (value >= Deg360 || value <= -Deg360 ? value % Deg360 : value) + (value < 0 ? Deg360 : F32.Zero);
        [MethodImpl(FixedUtil.Inline)] public static F32 Adjust180(F32 value) => Adjust360(value + Deg180) - Deg180;
        [MethodImpl(FixedUtil.Inline)] public static F32Angle Lerp(F32 a, F32 b, F32 t) => Adjust360(F32.Lerp(a, b, t));

        private readonly F32 _value;
        public F32 Radian { [MethodImpl(FixedUtil.Inline)] get => F32.DegToRad(_value); }

        [MethodImpl(FixedUtil.Inline)] private F32Angle(F32 value) => _value = Adjust360(value);
        [MethodImpl(FixedUtil.Inline)] public bool Equals(F32Angle other) => _value == other._value;
        [MethodImpl(FixedUtil.Inline)] public override bool Equals(object obj) => obj is F32 v && Equals(v);
        [MethodImpl(FixedUtil.Inline)] public int CompareTo(F32Angle other) => _value.CompareTo(other._value);
        [MethodImpl(FixedUtil.Inline)] public override string ToString() => _value.ToString();
        [MethodImpl(FixedUtil.Inline)] public override int GetHashCode() => _value.Raw;
        int IComparable.CompareTo(object obj) => obj is F32Angle other ? CompareTo(other) : 
                                                 obj is null ? 1 :
                                                 throw new ArgumentException("F32Angle can only be compared against another F32Angle.");

        // Cast
        [MethodImpl(FixedUtil.Inline)] public static implicit operator F32(F32Angle angle) => angle._value;
        [MethodImpl(FixedUtil.Inline)] public static implicit operator F32Angle(F32 value) => new F32Angle(value);
        [MethodImpl(FixedUtil.Inline)] public static implicit operator F32Angle(int value) => new F32Angle(F32.FromInt(value));

        // Operators
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator -(F32Angle v1) { return -v1._value; }

        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator +(F32Angle v1, F32Angle v2) { return v1._value + v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator -(F32Angle v1, F32Angle v2) { return v1._value - v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator *(F32Angle v1, F32Angle v2) { return v1._value * v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator /(F32Angle v1, F32Angle v2) { return v1._value / v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator %(F32Angle v1, F32Angle v2) { return v1._value % v2._value; }

        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator +(F32Angle v1, int v2) { return v1._value + F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator +(int v1, F32Angle v2) { return F32.FromInt(v1) + v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator -(F32Angle v1, int v2) { return v1._value - F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator -(int v1, F32Angle v2) { return F32.FromInt(v1) - v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator *(F32Angle v1, int v2) { return v1._value * F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator *(int v1, F32Angle v2) { return F32.FromInt(v1) * v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator /(F32Angle v1, int v2) { return v1._value / v2; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator /(int v1, F32Angle v2) { return F32.FromInt(v1) / v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator %(F32Angle v1, int v2) { return v1._value % F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static F32Angle operator %(int v1, F32Angle v2) { return F32.FromInt(v1) % v2._value; }

        [MethodImpl(FixedUtil.Inline)] public static bool operator ==(F32Angle v1, F32Angle v2) { return v1._value == v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator !=(F32Angle v1, F32Angle v2) { return v1._value != v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator <(F32Angle v1, F32Angle v2) { return v1._value < v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator <=(F32Angle v1, F32Angle v2) { return v1._value <= v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator >(F32Angle v1, F32Angle v2) { return v1._value > v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator >=(F32Angle v1, F32Angle v2) { return v1._value >= v2._value; }

        [MethodImpl(FixedUtil.Inline)] public static bool operator ==(int v1, F32Angle v2) { return F32.FromInt(v1) == v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator ==(F32Angle v1, int v2) { return v1._value == F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static bool operator !=(int v1, F32Angle v2) { return F32.FromInt(v1) != v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator !=(F32Angle v1, int v2) { return v1._value != F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static bool operator <(int v1, F32Angle v2) { return F32.FromInt(v1) < v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator <(F32Angle v1, int v2) { return v1._value < F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static bool operator <=(int v1, F32Angle v2) { return F32.FromInt(v1) <= v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator <=(F32Angle v1, int v2) { return v1._value <= F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static bool operator >(int v1, F32Angle v2) { return F32.FromInt(v1) > v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator >(F32Angle v1, int v2) { return v1._value > F32.FromInt(v2); }
        [MethodImpl(FixedUtil.Inline)] public static bool operator >=(int v1, F32Angle v2) { return F32.FromInt(v1) >= v2._value; }
        [MethodImpl(FixedUtil.Inline)] public static bool operator >=(F32Angle v1, int v2) { return v1._value >= F32.FromInt(v2); }
    }
}
