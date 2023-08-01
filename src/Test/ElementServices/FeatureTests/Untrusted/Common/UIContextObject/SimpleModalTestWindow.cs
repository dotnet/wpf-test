// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.Win32;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// This class implements a fake modal window.
    /// </summary>
    public class SimpleModalTestWindow: DispatcherObject
    {
        /// <summary>
        /// Create the window with a given parent at a given location.
        /// </summary>
        /// <param name="HwndParent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public SimpleModalTestWindow(IntPtr HwndParent, int x, int y, int width, int height) : 
            base()
        {
            _ModalTestWindow(HwndParent);
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
        public SimpleModalTestWindow(IntPtr HwndParent) : 
            this(HwndParent, 50, 50, 200, 200)
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
            // disable parent window
            bool a = NativeMethods.EnableWindow(new HandleRef(null, this._parent), false);

            // display this modal window
            NativeMethods.SetWindowPos(this._source.Handle, NativeConstants.HWND_TOP, _x, _y, _w, _h, NativeConstants.SWP_SHOWWINDOW);
            NativeMethods.EnableWindow(new HandleRef(null, this._source.Handle), true);

            // Nested Message Loop, this not will return until PopFrame that is called on WM_DESTROY
            this._frame = new DispatcherFrame();
            Dispatcher.PushFrame(this._frame);

            // Raise event
            if (WindowShown != null)
            {
                WindowShown(this, new EventArgs());
            }
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
        /// Event is raised once the window has been shown.
        /// </summary>
        public event EventHandler WindowShown;


        /// <summary>
        /// This method actually creates the window.
        /// </summary>
        /// <param name="HwndParent"></param>
        internal void _ModalTestWindow(IntPtr HwndParent)
        {
            HwndSourceParameters hwndSourceParams = new HwndSourceParameters("",_w,_h);

            hwndSourceParams.WindowStyle = NativeConstants.WS_POPUP | NativeConstants.WS_CLIPCHILDREN | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPSIBLINGS | NativeConstants.WS_SYSMENU | NativeConstants.WS_THICKFRAME;
            hwndSourceParams.ExtendedWindowStyle = NativeConstants.WS_EX_LEFT | NativeConstants.WS_EX_DLGMODALFRAME | NativeConstants.WS_EX_WINDOWEDGE;
            hwndSourceParams.SetPosition(_x,_y);

            _source = new HwndSource(hwndSourceParams);
           
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
                // stop showing window contents
               // _source.RootVisual = null.

                // Re-enable and set focus to its parent.
                NativeMethods.EnableWindow(new HandleRef(null, _parent), true);
                NativeMethods.SetForegroundWindow(new HandleRef(null, _parent));

                // PopFrame (break out of nested message loop)
                Debug.Assert (this._frame != null, "this._frame is null -- we don't have a nested message loop to break out of!");
                this._frame.Continue = false;
            }

            handled = false;
            return IntPtr.Zero;
        }


        private IntPtr _parent;

        private HwndSource _source;

        private DispatcherFrame _frame = null;

        private int _x;

        private int _y;

        private int _w;

        private int _h;
    }
}

