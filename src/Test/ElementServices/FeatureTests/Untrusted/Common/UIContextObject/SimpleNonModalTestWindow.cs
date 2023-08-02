// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Threading; 
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Win32;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// This class implements a fake non-modal window.
    /// </summary>
    public class SimpleNonModalTestWindow: DispatcherObject
    {
        /// <summary>
        /// Create the window with a given parent at a given location.
        /// </summary>
        /// <param name="HwndParent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public SimpleNonModalTestWindow(IntPtr HwndParent, int x, int y, int width, int height) : 
            base()
        {
            _NonModalTestWindow(HwndParent);
            this._parent = HwndParent;
            this._x = x;
            this._y = y;
            this._w = width;
            this._h = height;
        }


        /// <summary>
        /// Create the window with a given parent and default coordinates.
        /// </summary>
        /// <param name="HwndParent"></param>
        public SimpleNonModalTestWindow(IntPtr HwndParent) :
            this(HwndParent, 50, 50, 200, 200)
        {
        }


        /// <summary>
        /// Create the window with given coordinates.
        /// </summary>
        /// <remarks>
        /// Desktop window is used as the "parent".
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public SimpleNonModalTestWindow(int x, int y, int width, int height) : 
            this(NativeMethods.GetDesktopWindow(), x,y,width,height)
        {
        }


        /// <summary>
        /// Create the window with a default parent.
        /// </summary>
        /// <remarks>
        /// Desktop window is used as the "parent".
        /// </remarks>
        public SimpleNonModalTestWindow() : 
            this(NativeMethods.GetDesktopWindow())
        {
        }


        /// <summary>
        /// HwndSource where the window is rendered.
        /// </summary>
        public HwndSource Source
        {
            get
            {
                return this._source;
            }
        }


        /// <summary>
        /// Visual that you want to render on the window.
        /// </summary>
        public Visual RootVisual
        {
            get
            {
                return _source.RootVisual;
            }
            set
            {
                _source.RootVisual = value;
            }
        }


        /// <summary>
        /// Show this window. 
        /// </summary>
        public void Show()
        {
            NativeMethods.SetWindowPos(this._source.Handle, NativeConstants.HWND_TOP, _x, _y, _w, _h, NativeConstants.SWP_SHOWWINDOW);
            NativeMethods.EnableWindow(new HandleRef(null, this._source.Handle), true);
        }


        /// <summary>
        /// Destroy this window.
        /// </summary>
        public void Destroy()
        {
            // destruction is done in the helper window hook.
            NativeMethods.SendMessage(new HandleRef(null, this._source.Handle), NativeConstants.WM_DESTROY, 0, 0);
        }


        /// <summary>
        /// This method actually creates the window.
        /// </summary>
        /// <param name="HwndParent"></param>
        internal void _NonModalTestWindow(IntPtr HwndParent)
        {

            HwndSourceParameters hsParams = new HwndSourceParameters("",_w,_h);
            hsParams.SetPosition(_x,_y);
            hsParams.WindowStyle = NativeConstants.WS_CLIPCHILDREN | 
                NativeConstants.WS_OVERLAPPEDWINDOW | 
                NativeConstants.WS_CLIPSIBLINGS | 
                NativeConstants.WS_SYSMENU | 
                NativeConstants.WS_THICKFRAME;

            hsParams.ExtendedWindowStyle = NativeConstants.WS_EX_LEFT | NativeConstants.WS_EX_WINDOWEDGE;

            hsParams.ParentWindow = HwndParent;
            
            _source = new HwndSource(hsParams);
            _source.AddHook(new HwndSourceHook(Helper));
        }


        /// <summary>
        /// This helper listens for WM_ messages.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="message"></param>
        /// <param name="firstParam"></param>
        /// <param name="secondParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected IntPtr Helper(IntPtr window, int message, IntPtr firstParam, IntPtr secondParam, ref bool handled)
        {
            if (message == NativeConstants.WM_DESTROY)
            {
                // Stop showing the window contents.
                _source.RootVisual = null;

                // NOTE: Non-modal windows do not need to restore focus to parent window
            }

            // Let other messages continue to work.
            handled = false;

            return IntPtr.Zero;
        }


        private IntPtr _parent;

        private HwndSource _source;

        private int _x;

        private int _y;

        private int _w;

        private int _h;
    }
}

