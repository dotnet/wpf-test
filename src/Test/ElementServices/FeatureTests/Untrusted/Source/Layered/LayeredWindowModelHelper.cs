// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: LayeredWindow model helper routines, builds layered window based on model state.
 *
 
  
 * Revision:         $Revision: 7 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;


using System.Reflection;
using System.Windows.Interop;
using System.Windows.Shapes;

using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.LayeredWindow
{
    /// <summary>
    /// Helper routines for LayeredWindowModel.
    /// </summary>
    internal class LayeredWindowModelHelper
    {
        public LayeredWindowModelHelper(LayeredWindowModelState modelState)
        {
            _state = modelState;
        }

        /// <summary>
        /// Create a normal wpf window for catching and counting clicks that pass through the layered window.
        /// </summary>
        public Window BuildBackgroundWindow(int width, int height)
        {
            _backgroundWindow = new Window();
            _backgroundWindow.PreviewMouseDown += this.OnClick;
            _backgroundWindow.Background = Brushes.White;
            _backgroundWindow.Top = 0;
            _backgroundWindow.Left = 0;
            _backgroundWindow.Width = width;
            _backgroundWindow.Height = height;
            _backgroundWindow.Show();

            return _backgroundWindow;
        }

        /// <summary>
        /// Build a layered window and returns its HwndSource.
        /// </summary>
        public HwndSource BuildLayeredWindow()
        {
            // Create layered HwndSource.
            // 
            HwndSourceParameters sourceParams = new HwndSourceParameters("Layered Window", 500, 500);
            sourceParams.PositionX = 0;
            sourceParams.PositionY = 0;

#if TESTBUILD_NET_ATLEAST_46
            sourceParams.UsesPerPixelTransparency = true;
#else
            sourceParams.UsesPerPixelOpacity = true;
#endif

            sourceParams.WindowStyle = Styles.WS_OVERLAPPED
                | Styles.WS_VISIBLE
                | _state.Style;

            sourceParams.ExtendedWindowStyle = _state.ExStyle | _state.IgnoredExStyle | _state.StrippedExStyle;

            _source = new HwndSource(sourceParams);

            //
            // Construct control.
            //

            // 
            _button = new Button();
            _button.Click += this.OnClick;
            _button.Width = 250;
            _button.Height = 250;

            _button.Visibility = _state.ElementVisibility;
            _button.Opacity = _state.Opacity;
            _button.IsEnabled = _state.IsEnabled;
            _button.IsHitTestVisible = _state.IsHitTestVisible;

            // 





            // 





            //    // Put brush in button.
            //    // b.OpacityMask = blah blah
            //}

            Ellipse corner = new Ellipse();
            corner.Fill = Brushes.Orange;
            corner.Width = 50;
            corner.Height = 50;
            Canvas.SetTop(corner, 500 - 25);
            Canvas.SetLeft(corner, 500 - 25);

            Canvas c = new Canvas();
            c.Width = 500;
            c.Height = 500;
            c.Children.Add(_button);
            c.Children.Add(corner);

            _source.RootVisual = (Visual)c;

            return _source;
        }

        private int _layeredClickCount = 0;
        public int LayeredClickCount
        {
            get
            {
                return _layeredClickCount;
            }
            set // Always reset to zero.
            {
                _layeredClickCount = 0;
            }
        }

        public Button Button
        {
            get
            {
                return _button;
            }
        }


        private int _backgroundClickCount;
        public int BackgroundClickCount
        {
            get
            {
                return _backgroundClickCount;
            }
            set // Always reset to zero.
            {
                _backgroundClickCount = 0;
            }
        }

        private void OnClick(object sender, RoutedEventArgs args)
        {
            if (sender == _button)
            {
                _layeredClickCount++;
            }
            else if (sender == _backgroundWindow)
            {
                _backgroundClickCount++;
            }
            else
            {
                throw new NotSupportedException("Click received from unknown sender: " + sender);
            }
        }

        /// <summary>
        /// Move button
        /// </summary>
        public void MoveButton(double left, double top)
        {
            Canvas.SetLeft(_button, left);
            Canvas.SetTop(_button, top);
        }

        /// <summary>
        /// Position layered window over background window.
        /// </summary>
        public void ArrangeTestWindows()
        {
            // Put both windows in the same place.
            // Moving the layered window puts it on top.
            MoveBackgroundWindow(0, 0);
            MoveLayeredWindow(0, 0);
        }

        /// <summary>
        /// Move the layered window relative to screen origin and put it on top.
        /// </summary>
        void MoveLayeredWindow(int left, int top)
        {
            // PInvoke SetWindowPos
            // 
            NativeMethods.SetWindowPos(_source.Handle,NativeConstants.HWND_TOP, left, top, 500, 500, NativeConstants.SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Move the background window relative to screen origin.
        /// </summary>
        void MoveBackgroundWindow(double left, double top)
        {
            _backgroundWindow.Left = left;
            _backgroundWindow.Top = top;
        }

        /// <summary>
        /// Wrap call to native ShowWindow call on the layered window.
        /// </summary>
        public void LayeredShowWindowCall(int cmd)
        {
            NativeMethods.ShowWindow(new HandleRef(_source, _source.Handle), cmd);
        }

        /// <summary>
        /// Get an a Windows constant value by name. Reflects on static LayeredWindow class typeKey
        /// to get value for static constant constant.
        /// </summary>
        private int GetWindowsConstant(string constant, string typeKey)
        {
            Type constantType = Type.GetType("Avalon.Test.CoreUI.Source.LayeredWindow." + typeKey);
            FieldInfo fi = constantType.GetField(constant);

            return (int)fi.GetValue(null);
        }



        private Window _backgroundWindow;

        private HwndSource _source;
        private Button _button = null;

        LayeredWindowModelState _state;
    }

    // 

    internal static class Styles
    {
        public const int WS_OVERLAPPED = 0x00000000;
        //public const int WS_POPUP            = 0x80000000;
        public const int WS_CHILD = 0x40000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_GROUP = 0x00020000;
        public const int WS_TABSTOP = 0x00010000;

        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;


        public const int WS_TILED = WS_OVERLAPPED;
        public const int WS_ICONIC = WS_MINIMIZE;
        public const int WS_SIZEBOX = WS_THICKFRAME;
        public const int WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;

        public const int WS_OVERLAPPEDWINDOW = 0x00cf0000;
    }

    internal static class ExStyles
    {
        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;

        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;

        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LTRREADING = 0x00000000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;

        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;

        public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const int WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);

        public const int WS_EX_LAYERED = 0x00080000;

        public const int WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
        public const int WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring

        public const int WS_EX_COMPOSITED = 0x02000000;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int None = 0x00000000;
    }
}


