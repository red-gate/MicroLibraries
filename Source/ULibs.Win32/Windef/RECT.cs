using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Windef
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT : IEquatable<RECT>
    {
        internal readonly int Left;
        internal readonly int Top;
        internal readonly int Right;
        internal readonly int Bottom;

        internal RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        internal int Width => Right - Left;
        internal int Height => Bottom - Top;

        public bool Equals(RECT other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        internal bool Contains(POINT vPoint)
        {
            return vPoint.X >= Left
                   && vPoint.X <= Right
                   && vPoint.Y >= Top
                   && vPoint.Y <= Bottom;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RECT && Equals((RECT) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode*397) ^ Top;
                hashCode = (hashCode*397) ^ Right;
                hashCode = (hashCode*397) ^ Bottom;
                return hashCode;
            }
        }

        public static bool operator ==(RECT a, RECT b)
        {
            return a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
        }

        public static bool operator !=(RECT a, RECT b)
        {
            return a.Left != b.Left || a.Top != b.Top || a.Right != b.Right || a.Bottom != b.Bottom;
        }

        public override string ToString()
        {
            return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
        }
    }
}