// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// This will be added in Beta 2 for Raw Touch
    /// </summary>
    public class CustomTouchEventArgs : RoutedEventArgs
    {
        #region Private members

        private int _x;          // touch x client coordinate in pixels
        private int _y;          // touch y client coordinate in pixels
        private int _id;         // touch ID
        private int _mask;       // mask which fields in the structure are valid
        private int _flags;      // flags
        private int _time;       // touch event time
        private int _touchX;     // x size of the touch area in pixels
        private int _touchY;     // y size of the touch area in pixels

        #endregion

        #region Public Properties

        public int LocationX
        {
            get { return _x; }
            set { _x = value; }
        }

        public int LocationY
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public int Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }

        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public int TouchX
        {
            get { return _touchX; }
            set { _touchX = value; }
        }

        public int TouchY
        {
            get { return _touchY; }
            set { _touchY = value; }
        }

        public bool IsPrimaryTouch
        {
            get { return (_flags & MultiTouchNativeMethods.TOUCHEVENTF_PRIMARY) != 0; }
        }

        #endregion 

        public CustomTouchEventArgs()
        {
        }
    }
}
