using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Windef
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT : IEquatable<POINT>
    {
        internal readonly int X;
        internal readonly int Y;

        internal POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(POINT other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is POINT && Equals((POINT) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public static bool operator ==(POINT a, POINT b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(POINT a, POINT b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
    }
}