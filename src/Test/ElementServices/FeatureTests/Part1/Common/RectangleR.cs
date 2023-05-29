// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Microsoft.Test.Input.MultiTouch
{
    public struct RectangleR
    {
        #region Fields

        private const int Digits = 4;

        private double _x;
        private double _y;
        private double _width;
        private double _height;

        public static readonly RectangleR Empty = new RectangleR();

        #endregion

        #region Constructor

        public RectangleR(double xVal, double yVal, double widthVal, double heightVal)
        {
            _x = xVal;
            _y = yVal;
            _width = widthVal;
            _height = heightVal;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="r"></param>
        public RectangleR(RectangleR r)
        {
            _x = r.X;
            _y = r.Y;
            _width = r.Width;
            _height = r.Height;
        }

        #endregion

        #region Properties

        public double X
        {
            get
            {
                return Math.Round(_x, Digits);
            }
            set
            {
                _x = value;
            }
        }

        public double Y
        {
            get
            {
                return Math.Round(_y, Digits);
            }
            set
            {
                _y = value;
            }
        }

        public double Width
        {
            get
            {
                return Math.Round(_width, Digits);
            }
            set
            {
                _width = value;
            }
        }

        public double Height
        {
            get
            {
                return Math.Round(_height, Digits);
            }
            set
            {
                _height = value;
            }
        }

        public PointR TopLeft
        {
            get
            {
                return new PointR(X, Y);
            }
        }

        public PointR BottomRight
        {
            get
            {
                return new PointR(X + Width, Y + Height);
            }
        }

        public PointR Center
        {
            get
            {
                return new PointR(X + Width / 2d, Y + Height / 2d);
            }
        }

        public double MaxSide
        {
            get
            {
                return Math.Max(_width, _height);
            }
        }

        public double MinSide
        {
            get
            {
                return Math.Min(_width, _height);
            }
        }

        public double Diagonal
        {
            get
            {
                return Utils.Distance(TopLeft, BottomRight);
            }
        }

        #endregion

        #region Operators

        public static explicit operator RectangleF(RectangleR r)
        {
            return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleR)
            {
                RectangleR r = (RectangleR)obj;
                return (X == r.X && Y == r.Y && Width == r.Width && Height == r.Height);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((RectangleF)this).GetHashCode();
        }

        #endregion
    }
}
