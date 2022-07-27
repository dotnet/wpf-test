// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;

using System.Text;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using System.Runtime.InteropServices;
using System.Windows.Automation;


namespace DRT
{

    public class DrtLayeredWindowsSuite : DrtTestSuite
    {
        public DrtLayeredWindowsSuite() : base("LayeredWindows")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DrtControls.NotUsingDrtWindow(DRT);

            // Tests

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(CreatePerPixelAlphaWindow),
                    new DrtTest(CreatePerPixelTransparencyWindow),
                    new DrtTest(MoveSizeWindow),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(CreatePerPixelAlphaWindow),
                    new DrtTest(CreatePerPixelTransparencyWindow),
                    new DrtTest(Cleanup),
                };
            }
        }


        private void Start()
        {
        }

        private void Cleanup()
        {
        }

        private void CreatePerPixelAlphaWindow()
        {
            CreatePerPixelAlphaWindow(true, false);
        }
        private void CreatePerPixelTransparencyWindow()
        {
            CreatePerPixelAlphaWindow(false, true);
        }
        private void CreatePerPixelAlphaWindow(bool usesPerPixelOpacity, bool usesPerPixelTransparency)
        {
            HwndSourceParameters parameters = new HwndSourceParameters("Per-Pixel Alpha Layered Window");

            _desiredRect = new RECT(200, 200, 400, 300);
            _deltaWidth = 1;

            parameters.SetPosition(_desiredRect.left, _desiredRect.top);
            parameters.SetSize(_desiredRect.Width, _desiredRect.Height);
            parameters.UsesPerPixelOpacity = usesPerPixelOpacity;
#if TESTBUILD_NET_ATLEAST_45

            parameters.UsesPerPixelTransparency = usesPerPixelTransparency;
#endif
            _hwndSource = new HwndSource(parameters);

            _gridRoot = new Grid();
            _hwndSource.RootVisual = _gridRoot;

            Ellipse ellipse = new Ellipse();

            ColorAnimation colorAnimation = new ColorAnimation(Color.FromArgb(128, 0, 0, 255), Color.FromArgb(128, 0, 255, 0), new TimeSpan(0, 0, 0, 0, 1000));
            colorAnimation.RepeatBehavior = RepeatBehavior.Forever;
            colorAnimation.AutoReverse = true;

            SolidColorBrush brush = new SolidColorBrush(Colors.Orange);
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

            ellipse.Fill = brush;

            _gridRoot.Children.Add(ellipse);
        }

        private void MoveSizeWindow()
        {
            if (_desiredRect.Width == 250)
            {
                _deltaWidth = -1;
            }

            _desiredRect.right += _deltaWidth;

            DrtBase.SetWindowPos(_hwndSource.Handle, IntPtr.Zero, 0, 0, _desiredRect.Width, _desiredRect.Height, DrtBase.SWP_NOMOVE | DrtBase.SWP_NOZORDER | DrtBase.SWP_NOACTIVATE);

            DRT.ResumeAt(new DrtTest(CheckWindowRect));
        }

        private void CheckWindowRect()
        {
            // Wait till we settle, then check that the window is the right size
            DRT.WaitForCompleteRender();

            RECT rect = new RECT();
            GetWindowRect(_hwndSource.Handle, ref rect);

            DRT.AssertEqual(_desiredRect.left, rect.left, "WindowRect.left");
            DRT.AssertEqual(_desiredRect.top, rect.top, "WindowRect.top");
            DRT.AssertEqual(_desiredRect.right, rect.right, "WindowRect.right");
            DRT.AssertEqual(_desiredRect.bottom, rect.bottom, "WindowRect.bottom");

            if (_desiredRect.Width > 200)
            {
                DRT.ResumeAt(new DrtTest(MoveSizeWindow));
                DRT.Pause(1);
            }
        }

        HwndSource _hwndSource;
        RECT _desiredRect;
        int _deltaWidth;
        Grid _gridRoot;

        [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x = 0;
            public int y = 0;

            public POINT() {
            }

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public int Width { get { return right - left; } }
            public int Height { get { return bottom - top; } }

            public static RECT FromXYWH(int x, int y, int width, int height) {
                return new RECT(x, y, x + width, y + height);
            }
        }
    }
}

