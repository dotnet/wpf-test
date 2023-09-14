// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Microsoft.Test.Input.MultiTouch
{
    public struct SizeR
    {
        #region Fields

        public static readonly SizeR Empty;

        private double _cx;
        private double _cy;

        #endregion

        #region Constructor

        public SizeR(double cX, double cY)
        {
            _cx = cX;
            _cy = cY;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="sz"></param>
        public SizeR(SizeR sz)
        {
            _cx = sz.Width;
            _cy = sz.Height;
        }

        #endregion

        #region Properties

        public double Width
        {
            get
            {
                return _cx;
            }
            set
            {
                _cx = value;
            }
        }

        public double Height
        {
            get
            {
                return _cy;
            }
            set
            {
                _cy = value;
            }
        }

        #endregion

        #region Operators

        public static explicit operator SizeF(SizeR sz)
        {
            return new SizeF((float)sz.Width, (float)sz.Height);
        }

        public static bool operator ==(SizeR sz1, SizeR sz2)
        {
            return (sz1.Width == sz2.Width && sz1.Height == sz2.Height);
        }

        public static bool operator !=(SizeR sz1, SizeR sz2)
        {
            return (sz1.Width != sz2.Width || sz1.Height != sz2.Height);
        }

        public override bool Equals(object obj)
        {
            if (obj is SizeR)
            {
                SizeR sz = (SizeR)obj;
                return (Width == sz.Width && Height == sz.Height);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((SizeR)this).GetHashCode();
        }

        #endregion
    }
}
