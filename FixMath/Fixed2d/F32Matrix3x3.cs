using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FixedMath.Fixed2d
{
    public struct F32Matrix3x3
    {
        private const long _0 = Fixed32.Zero;
        private const long _1 = Fixed32.One;

        public static readonly F32Matrix3x3 Identity = new F32Matrix3x3(
                _1, _0, _0,
                _0, _1, _0,
                _0, _0, _1
            );

        public long m00;
        public long m01;
        public long m02;
        public long m10;
        public long m11;
        public long m12;
        public long m20;
        public long m21;
        public long m22;

        public F32 M00 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m00); [MethodImpl(FixedUtil.Inline)] set => m00 = value.Raw; }
        public F32 M01 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m01); [MethodImpl(FixedUtil.Inline)] set => m01 = value.Raw; }
        public F32 M02 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m02); [MethodImpl(FixedUtil.Inline)] set => m02 = value.Raw; }
        public F32 M10 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m10); [MethodImpl(FixedUtil.Inline)] set => m10 = value.Raw; }
        public F32 M11 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m11); [MethodImpl(FixedUtil.Inline)] set => m11 = value.Raw; }
        public F32 M12 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m12); [MethodImpl(FixedUtil.Inline)] set => m12 = value.Raw; }
        public F32 M20 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m20); [MethodImpl(FixedUtil.Inline)] set => m20 = value.Raw; }
        public F32 M21 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m21); [MethodImpl(FixedUtil.Inline)] set => m21 = value.Raw; }
        public F32 M22 { [MethodImpl(FixedUtil.Inline)] get => F32.FromRaw((int)m22); [MethodImpl(FixedUtil.Inline)] set => m22 = value.Raw; }

        public F32Vec3 Column0 { [MethodImpl(FixedUtil.Inline)] get => new F32Vec3(M00, M10, M20); [MethodImpl(FixedUtil.Inline)] set { M00 = value.X; M10 = value.Y; M20 = value.Z; } }
        public F32Vec3 Column1 { [MethodImpl(FixedUtil.Inline)] get => new F32Vec3(M01, M11, M21); [MethodImpl(FixedUtil.Inline)] set { M01 = value.X; M11 = value.Y; M21 = value.Z; } }
        public F32Vec3 Column2 { [MethodImpl(FixedUtil.Inline)] get => new F32Vec3(M02, M12, M22); [MethodImpl(FixedUtil.Inline)] set { M02 = value.X; M12 = value.Y; M22 = value.Z; } }

        public F32Matrix3x3 TransposedMatrix
        {
            [MethodImpl(FixedUtil.Inline)]
            get => new F32Matrix3x3(
                m00, m10, m20,
                m01, m11, m21,
                m02, m12, m22
            );
        }

        public F32Matrix3x3 InversedMatrix
        {
            [MethodImpl(FixedUtil.Inline)]
            get
            {
                /*
                    m00 m01 m02
                    m10 m11 m12
                    m20 m21 m22

                    det = m11*m22 -m12*m21 -m10*m22  m12*m20  m10*m21 -m11*m20 
                         -m01*m22  m02*m21  m00*m22 -m02*m20 -m00*m21  m01*m20
                          m01*m12 -m02*m11 -m00*m12  m02*m10  m00*m11 -m01*m10
                          
                          m00 * m11
                          m01 * m12
                          m02 * m10
                          m10 * m21
                          m11 * m22
                          m12 * m20
                          m20 * m01
                          m21 * m02
                          m22 * m00 

                         -m00 * m21  
                         -m01 * m22  
                         -m02 * m20 
                         -m10 * m01
                         -m11 * m02
                         -m12 * m00
                         -m20 * m11
                         -m21 * m12
                         -m22 * m10
                */

                var thisSpan = AsSpan();

                Span<long> col0 = stackalloc long[9] { m11, m12, m10, m21, m22, m20, m01, m02, m00 };
                Span<long> col1 = stackalloc long[9] { m11, m12, m10, m21, m22, m20, m01, m02, m00 };

                var invDet = F32.One / (FVector.Dot(thisSpan, col0) - FVector.Dot(thisSpan, col1));

                return new F32Matrix3x3(
                        M00 * invDet, M10 * invDet, M20 * invDet,
                        M01 * invDet, M11 * invDet, M21 * invDet,
                        M02 * invDet, M12 * invDet, M22 * invDet
                    );
            }
        }

        [MethodImpl(FixedUtil.Inline)]
        internal F32Matrix3x3(
            long m00, long m01, long m02,
            long m10, long m11, long m12,
            long m20, long m21, long m22)
        {
            this.m00 = m00; this.m01 = m01; this.m02 = m02;
            this.m10 = m10; this.m11 = m11; this.m12 = m12;
            this.m20 = m20; this.m21 = m21; this.m22 = m22;
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Matrix3x3(
            F32 m00, F32 m01, F32 m02,
            F32 m10, F32 m11, F32 m12,
            F32 m20, F32 m21, F32 m22)
        {
            this.m00 = m00.Raw; this.m01 = m01.Raw; this.m02 = m02.Raw;
            this.m10 = m10.Raw; this.m11 = m11.Raw; this.m12 = m12.Raw;
            this.m20 = m20.Raw; this.m21 = m21.Raw; this.m22 = m22.Raw;
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Matrix3x3(F32Vec3 column0, F32Vec3 column1, F32Vec3 column2)
        {
            m00 = column0.X.Raw; m01 = column1.X.Raw; m02 = column2.X.Raw;
            m10 = column0.Y.Raw; m11 = column1.Y.Raw; m12 = column2.Y.Raw;
            m20 = column0.Z.Raw; m21 = column1.Z.Raw; m22 = column2.Z.Raw;
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Matrix3x3(F32Vec2 translation, F32Angle angle, F32Vec2 scale)
        {
            /*
                T X R X S
                
                1   0   tx     c  -s   0     sx  0   0
                0   1   ty  X  s   c   0  X  0   sy  0
                0   0   1      0   0   1     0   0   1

                c  -s   tx     sx  0   0
                s   c   ty  X  0   sy  0
                0   0   1      0   0   1

                c*sx -s*sy  tx
                s*sx  c*sy  ty
                0     0     1
            */

            var radian = angle.Radian;
            var cos = F32.Cos(radian);
            var sin = F32.Sin(radian);

            m00 = (cos * scale.X).Raw;  m01 = (-sin * scale.Y).Raw;  m02 = translation.X.Raw;
            m10 = (sin * scale.X).Raw;  m11 = ( cos * scale.Y).Raw;  m12 = translation.Y.Raw;
            m20 = _0;         m21 = _0;          m22 = _1;
        }

        [MethodImpl(FixedUtil.Inline)]
        public Span<long> AsSpan() => MemoryMarshal.Cast<F32Matrix3x3, long>(MemoryMarshal.CreateSpan(ref this, 1));

        [MethodImpl(FixedUtil.Inline)]
        public void Transpose()
        {
            (m01, m10) = (m10, m01);
            (m02, m20) = (m20, m02);
            (m12, m21) = (m21, m12);
        }

        public F32Matrix3x3 Multiply(F32Matrix3x2 other)
        {
            var thisSpan = AsSpan();
            var thisRow0 = thisSpan.Slice(0, 2);
            var thisRow1 = thisSpan.Slice(3, 2);
            var thisRow2 = thisSpan.Slice(6, 2);
            var thisRow0_3 = thisSpan.Slice(0, 3);
            var thisRow1_3 = thisSpan.Slice(3, 3);
            var thisRow2_3 = thisSpan.Slice(6, 3);
            Span<long> otherCol0 = stackalloc long[] { other.m00, other.m10 };
            Span<long> otherCol1 = stackalloc long[] { other.m01, other.m11 };
            Span<long> otherCol2_3 = stackalloc long[] { other.m02, other.m12, _1 };

            return new F32Matrix3x3(
                    FVector.Dot(thisRow0, otherCol0), FVector.Dot(thisRow0, otherCol1), FVector.Dot(thisRow0_3, otherCol2_3),
                    FVector.Dot(thisRow1, otherCol0), FVector.Dot(thisRow1, otherCol1), FVector.Dot(thisRow1_3, otherCol2_3),
                    FVector.Dot(thisRow2, otherCol0), FVector.Dot(thisRow2, otherCol1), FVector.Dot(thisRow2_3, otherCol2_3)
                );
        }

        public F32Matrix3x3 Multiply(F32Matrix3x3 other)
        {
            var thisSpan = AsSpan();
            var thisRow0 = thisSpan.Slice(0, 3);
            var thisRow1 = thisSpan.Slice(3, 3);
            var thisRow2 = thisSpan.Slice(6, 3);
            Span<long> otherCol0 = stackalloc long[] { other.m00, other.m10, other.m20 };
            Span<long> otherCol1 = stackalloc long[] { other.m01, other.m11, other.m21 };
            Span<long> otherCol2 = stackalloc long[] { other.m02, other.m12, other.m22 };

            return new F32Matrix3x3(
                    FVector.Dot(thisRow0, otherCol0), FVector.Dot(thisRow0, otherCol1), FVector.Dot(thisRow0, otherCol2),
                    FVector.Dot(thisRow1, otherCol0), FVector.Dot(thisRow1, otherCol1), FVector.Dot(thisRow1, otherCol2),
                    FVector.Dot(thisRow2, otherCol0), FVector.Dot(thisRow2, otherCol1), FVector.Dot(thisRow2, otherCol2)
                );
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 MultiplyPoint(F32Vec2 point)
        {
            var thisRow = AsSpan();
            Span<long> pointCol = stackalloc long[] { point.X.Raw, point.Y.Raw, _1 };

            if (m20 == _0 && m21 == _0 && m22 == _1)
            {
                return new F32Vec2(
                    FVector.Dot(thisRow.Slice(0, 3), pointCol),
                    FVector.Dot(thisRow.Slice(3, 3), pointCol)
                );
            }

            var invW = F32.One / FVector.Dot(thisRow.Slice(6, 3), pointCol);
            return new F32Vec2(
                FVector.Dot(thisRow.Slice(0, 3), pointCol) * invW,
                FVector.Dot(thisRow.Slice(3, 3), pointCol) * invW
            );
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 MultiplyVector(F32Vec2 vector)
        {
            var thisRow = AsSpan();
            Span<long> pointCol = stackalloc long[] { vector.X.Raw, vector.Y.Raw };

            return new F32Vec2(
                FVector.Dot(thisRow.Slice(0, 2), pointCol),
                FVector.Dot(thisRow.Slice(3, 2), pointCol)
            );
        }

        [MethodImpl(FixedUtil.Inline)] public static F32Matrix3x3 operator *(F32Matrix3x3 a, F32Matrix3x3 b) => a.Multiply(b);
        [MethodImpl(FixedUtil.Inline)] public static F32Vec2 operator *(F32Matrix3x3 a, F32Vec2 b) => a.MultiplyVector(b);

        [MethodImpl(FixedUtil.Inline)]
        public static explicit operator F32Matrix3x2(F32Matrix3x3 m)
        {
            return new F32Matrix3x2(
                m.m00, m.m01, m.m02,
                m.m10, m.m11, m.m12
            );
        }
    }
}
