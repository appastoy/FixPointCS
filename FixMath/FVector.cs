using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FixedMath
{
    public static class FVector
    {
        [MethodImpl(FixedUtil.Inline)]
        public static F32 Dot(Span<long> a, Span<long> b)
        {
            return F32.FromRaw((int)(Vector.Dot(new Vector<long>(a), new Vector<long>(b)) >> Fixed32.Shift));
        }
    }
}
