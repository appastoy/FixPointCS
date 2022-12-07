using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FixedMath.Fixed2d
{
    public sealed class F32Transform2d
    {
        [Flags]
        enum DirtyFlags : byte
        {
            None,
            LocalRight   = 0b_00_0001,
            LocalToWorld = 0b_00_0010,
            WorldToLocal = 0b_00_0100,
            Matrix       = 0b_00_0110,
            Angle        = 0b_00_1000,
            Right        = 0b_01_0000,
            Scale        = 0b_10_0000,
            AllWorld     = 0b_11_1110
        }

        private F32Vec2 _localPosition;
        private F32Angle _localAngle;
        private F32Vec2 _cachedLocalRight;
        private F32Vec2 _localScale;

        private F32Matrix3x2 _cachedLocalToWorld;
        private F32Matrix3x3 _cachedWorldToLocal;
        private F32Angle _cachedAngle;
        private F32Vec2 _cachedRight;
        private F32Vec2 _cachedScale;
        private DirtyFlags _dirtyFlags;

        private F32Transform2d? _parent = null;
        private List<F32Transform2d>? _children = null;

        public F32Vec2 LocalPosition
        {
            get => _localPosition;
            set
            {
                if (_localPosition == value) 
                    return;

                _localPosition = value;
                SetDirty(DirtyFlags.Matrix);
            }
        }

        public F32Vec2 LocalScale
        {
            get => _localScale;
            set
            {
                if (_localScale == value)
                    return;

                _localScale = value;
                SetDirty(DirtyFlags.Matrix | DirtyFlags.Scale);
            }
        }

        public F32Angle LocalAngle
        {
            get => _localAngle;
            set
            {
                if (_localAngle == value)
                    return;

                _localAngle = value;
                SetDirty(DirtyFlags.Matrix | DirtyFlags.Angle | DirtyFlags.Right);
                _dirtyFlags |= DirtyFlags.LocalRight;
            }
        }

        public F32Vec2 LocalRight
        {
            get => __localRight;
            set
            {
                var normalized = F32Vec2.Normalize(value);
                if (__localRight != normalized)
                    SetLocalRight(normalized);
            }
        }

        public F32Vec2 LocalUp
        {
            get
            {
                ref var right = ref __localRight;
                return new F32Vec2(-right.Y, right.X);
            }
            set
            {
                var normalized = F32Vec2.Normalize(value);
                if (LocalUp != normalized)
                    SetLocalRight(new F32Vec2(normalized.Y, -normalized.X));
            }
        }

        public F32Vec2 Position
        {
            get => __localToWorld.Column2;
            set
            {
                if (__localToWorld.Column2 == value)
                    return;

                _cachedLocalToWorld.Column2 = value;
                _localPosition = _parent != null ? _parent.__worldToLocal.MultiplyPoint(value) : value;
                _dirtyFlags |= DirtyFlags.WorldToLocal;
                if (_children != null)
                {
                    foreach (var child in _children)
                        child.SetDirty(DirtyFlags.Matrix);
                }
            }
        }

        public F32Angle Angle
        {
            get
            {
                if (_dirtyFlags.HasFlagFast(DirtyFlags.Angle))
                {
                    ref var r = ref __right;
                    _cachedAngle = F32.RadToDeg(F32.Atan2(r.Y, r.X));
                    _dirtyFlags &= ~(DirtyFlags.Angle | DirtyFlags.Right);
                }
                return _cachedAngle;
            }
            set
            {
                if (_cachedAngle == value)
                    return;

                _cachedAngle = value;
                _localAngle = _cachedAngle - (_parent?.Angle ?? F32.Zero);
                _dirtyFlags &= ~DirtyFlags.Angle;
                _dirtyFlags |= DirtyFlags.LocalRight;
                SetDirty(DirtyFlags.Matrix | DirtyFlags.Right);
            }
        }

        public F32Vec2 Right
        {
            get => __right;
            set
            {
                var normalized = F32Vec2.Normalize(value);
                if (__right != normalized)
                    SetRight(normalized);
            }
        }

        public F32Vec2 Up
        {
            get
            {
                ref var r = ref __right;
                return new F32Vec2(-r.Y, r.X);
            }
            set
            {
                var normalized = F32Vec2.Normalize(value);
                if (Up != normalized)
                    SetRight(new F32Vec2(normalized.Y, -normalized.X));
            }
        }

        public F32Vec2 Scale
        {
            get
            {
                if (_dirtyFlags.HasFlagFast(DirtyFlags.Scale))
                {
                    ref var matrix = ref __localToWorld;
                    _cachedScale = new F32Vec2(F32Vec2.Length(matrix.Column0), F32Vec2.Length(matrix.Column2));
                    _dirtyFlags &= ~DirtyFlags.Scale;
                }
                return _cachedScale;
            }
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 TransformPoint(F32Vec2 point) => __localToWorld.MultiplyPoint(point);

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 TransformVector(F32Vec2 vector) => __localToWorld.MultiplyVector(vector);

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 TransformDirection(F32Vec2 direction)
        {
            /*
                rotationMatrix = c -s
                                 s  c

                transformed = (c*dx -s*dy, s*dx  c*dy)
            */
            var d = direction.Normalized;
            ref var right = ref __right;
            return new F32Vec2(
                right.X * d.X - right.Y * d.Y, 
                right.Y * d.X + right.X * d.Y
            );
        }

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 InverseTransformPoint(F32Vec2 point) => __worldToLocal.MultiplyPoint(point);

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 InverseTransformVector(F32Vec2 vector) => __worldToLocal.MultiplyVector(vector);

        [MethodImpl(FixedUtil.Inline)]
        public F32Vec2 InverseTransformDirection(F32Vec2 direction)
        {
            /*
                c -s
                s  c

                det = 1 / (c*c+s*s)

                transformed = (c*det*dx + s*det*dy, c*det*dy - s*det*dx)
            */
            var d = direction.Normalized;
            ref var right = ref __right;
            var invDet = F32.One / (right.X*right.X + right.Y*right.Y);
            var cdet = right.X * invDet;
            var sdet = right.Y * invDet;

            return new F32Vec2(
                cdet * d.X + sdet * d.Y,
                cdet * d.Y - sdet * d.X
            );
        }

        public void SetParent(F32Transform2d parent)
        {
            if (_parent == parent)
                return;

            _parent?.RemoveChild(this);
            _parent = parent;
            _parent?.AddChild(this);
        }

        public void AddChild(F32Transform2d child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (_children?.Contains(child) ?? false)
                return;

            _children ??= new List<F32Transform2d>();
            _children.Add(child);
            child._parent = this;
            child.SetDirty(DirtyFlags.AllWorld);
        }

        public bool RemoveChild(F32Transform2d child)
        {
            if (child == null)
                return false;

            if (_children == null ||
                !_children.Remove(child))
                return false;

            child._parent = null;
            child.SetDirty(DirtyFlags.AllWorld);
            return true;
        }

        public void ClearChildren()
        {
            if (_children == null)
                return;

            foreach (var child in _children)
            {
                child._parent = null;
                child.SetDirty(DirtyFlags.AllWorld);
            }
        }

        public F32Transform2d? FindChild(Predicate<F32Transform2d> match)
        {
            return _children?.Find(match);
        }

        private ref F32Vec2 __localRight
        {
            get
            {
                if (_dirtyFlags.HasFlagFast(DirtyFlags.LocalRight))
                {
                    var radian = _localAngle.Radian;
                    _cachedLocalRight = new F32Vec2(F32.Cos(radian), F32.Sin(radian));
                    _dirtyFlags &= ~DirtyFlags.LocalRight;
                }
                return ref _cachedLocalRight;
            }
        }

        private ref F32Vec2 __right
        {
            get
            {
                if (_dirtyFlags.HasFlagFast(DirtyFlags.Right))
                {
                    _cachedRight = __localToWorld.Column0.Normalized;
                    _dirtyFlags &= ~DirtyFlags.Right;
                }
                return ref _cachedRight;
            }
        }

        private ref F32Matrix3x2 __localToWorld
        {
            get
            {
                if (_dirtyFlags.HasFlagFast(DirtyFlags.LocalToWorld))
                {
                    if (_parent == null)
                    {
                        ref var localRight = ref __localRight;
                        var c = localRight.X;
                        var s = localRight.Y;

                        _cachedLocalToWorld = new F32Matrix3x2(
                            c * _localScale.X, -s * _localScale.Y, _localPosition.X,
                            s * _localScale.X, c * _localScale.Y, _localPosition.Y);
                        _cachedRight = localRight;
                        _cachedAngle = _localAngle;
                        _cachedScale = _localScale;
                        _dirtyFlags &= ~(DirtyFlags.LocalRight | DirtyFlags.Angle | DirtyFlags.Right | DirtyFlags.Scale | DirtyFlags.LocalToWorld);
                    }
                    else
                    {
                        _cachedLocalToWorld = _parent.__localToWorld.Multiply(new F32Matrix3x2(_localPosition, _localAngle, _localScale));
                        _dirtyFlags &= ~DirtyFlags.LocalToWorld;
                    }
                }
                return ref _cachedLocalToWorld;
            }
        }

        private ref F32Matrix3x3 __worldToLocal
        {
            get
            {
                if (_dirtyFlags.HasFlagFast(DirtyFlags.WorldToLocal))
                {
                    _cachedWorldToLocal = __localToWorld.InversedMatrix;
                    _dirtyFlags &= ~DirtyFlags.WorldToLocal;
                }
                return ref _cachedWorldToLocal;
            }
        }

        private void SetDirty(DirtyFlags dirtyFlags)
        {
            _dirtyFlags |= dirtyFlags;
            if (_children != null)
            {
                foreach (var child in _children)
                    child.SetDirty(dirtyFlags);
            }
        }

        private void SetLocalRight(F32Vec2 direction)
        {
            _cachedLocalRight = direction;
            _dirtyFlags &= ~DirtyFlags.LocalRight;
            _localAngle = F32.RadToDeg(F32.Atan2(_cachedLocalRight.Y, _cachedLocalRight.X));
            SetDirty(DirtyFlags.Matrix | DirtyFlags.Angle | DirtyFlags.Right);
        }

        private void SetRight(F32Vec2 normalized)
        {
            _cachedRight = normalized;
            _cachedAngle = F32.RadToDeg(F32.Atan2(_cachedRight.Y, _cachedRight.X));
            _localAngle = _cachedAngle - (_parent?.Angle ?? F32.Zero);
            _dirtyFlags &= ~(DirtyFlags.Angle | DirtyFlags.Right);
            _dirtyFlags |= DirtyFlags.LocalRight;
            SetDirty(DirtyFlags.Matrix);
        }
    }
}
