// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// General gesture testing event args
    /// </summary>
    public class GestureEventArgs : System.EventArgs
    {
        #region Private Fields

        private int _size;
        private int _arguments;
        private int _y;
        private int _x;
        private Point _deltaLocation;
        private bool _validDeltas;
        private int _deltaArgs;

        #endregion

        public GestureEventArgs()
        {
        }

        #region Public Properties
        
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public int Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        public int LocationY
        {
            get { return _y; }
            set { _y = value; }
        }

        public int LocationX
        {
            get { return _x; }
            set { _x = value; }
        }

        public Point Location
        {
            get { return new Point(_x, _y); }
        }

        public int DeltaArgs
        {
            get { return _deltaArgs; }
            set { _deltaArgs = value; }
        }

        public Point DeltaLocation
        {
            get { return _deltaLocation; }
            set { _deltaLocation = value; }
        }

        public bool ValidDeltas
        {
            get { return _validDeltas; }
            set { _validDeltas = value; }
        }

        #endregion
        
    }
}
