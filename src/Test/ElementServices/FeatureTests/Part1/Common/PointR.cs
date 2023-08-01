// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Microsoft.Test.Input.MultiTouch
{
    public struct PointR
    {
        public static readonly PointR Empty;

        public double X, Y;
        public int T;
        public int ID;

        public PointR(double x, double y)
            : this(x, y, 0, -1)
        {
        }

        public PointR(double x, double y, int t, int id)
        {
            X = x;
            Y = y;
            T = t;
            ID = id;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="p"></param>
        public PointR(PointR p)
        {
            X = p.X;
            Y = p.Y;
            T = p.T;
            ID = p.ID;
        }

        public static explicit operator PointF(PointR p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        public static bool operator ==(PointR p1, PointR p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }

        public static bool operator !=(PointR p1, PointR p2)
        {
            return (p1.X != p2.X || p1.Y != p2.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is PointR)
            {
                PointR p = (PointR)obj;
                return (X == p.X && Y == p.Y);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((PointF)this).GetHashCode();
        }
    }
}
