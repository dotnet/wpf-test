// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

using System.Text;


using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;


namespace DRT
{
    public class WindowHeightWidthTopLeftApp : Application
    {
        [STAThread]
        public static int Main()
        {
            WindowHeightWidthTopLeftApp theApp = new WindowHeightWidthTopLeftApp();            
            try
            {
                theApp.Run();
            }
            catch (ApplicationException e)
            {
                theApp._logger.Log(e);
                return 1;
            }
            
            theApp._logger.Log( "Passed" );
            return 0;
        }

        private Logger _logger = new Logger("DrtWindowHeightWidthTopLeft", "Microsoft", "Testing Height/Width/Top/Left and RestoreBounds APIs on Window and the expected values at different stages");

        private Window _win;
        private Size _size = new Size(400,400);
        private Point _location = new Point(400,400);

        //
        // Test steps:
        //  1) After creating the Window object, verify that querying Width/Height/Top/Left returns the default value                
        // 
        //  2) Set value and verify getting Width/Height/Top/Left returns correct value  
        // 
        //  3) Set WindowState to maxmized
        //  
        //  4) Verify getting W/H/T/L
        //
        //  5) Show Window
        //         
        //  5) Restore the Window to Normal
        //
        //  6) Verify getting Width/Height/Top/Left
        //
        //  7) Test RestoreBounds with WindowStyle.SingleBorderWindow and 
        //     WindowStyle.ToolWindow
        // 
        //  8) Test setting Top/Left/Width/Height when window is maximized
        //     or minimized.  Make sure that this works for ToolWindow too.
        // 
        protected override void OnStartup(StartupEventArgs e) 
        {
            _win = new Window();
            Button b = new Button();
            b.Content = "Hello World";
            _win.Content = b;

            _win.Title = "DrtWindowSizeLocation";

            TestPropertiesBeforeShow();
            _win.Show();

            _win.WindowState = WindowState.Normal;

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestPropertiesAfterShow),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestRestoreBounds),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestTLWH),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ShutdownApp),
                this);
            base.OnStartup(e);
        }

        private void TestPropertiesBeforeShow()
        {
            double length;

            length = _win.Width;
            if (!Double.IsNaN(length))
            {
                throw new ApplicationException("Querying for Width before setting it or showing window should have returned default value (NaN), it returned " + length);
            }

            length = _win.Height;
            if (!Double.IsNaN(length))
            {
                throw new ApplicationException("Querying for Height before setting it or showing window should have returned default value (NaN), it returned " + length);
            }

            length = _win.Left;
            if (!Double.IsNaN(length))
            {
                throw new ApplicationException("Querying for Left before setting it or showing window should have returned default value (NaN), it returned " + length);
            }

            length = _win.Top;
            if (!Double.IsNaN(length))
            {
                throw new ApplicationException("Querying for Top before setting it or showing window should have returned default value (NaN), it returned " + length);
            }

            _win.Width = _size.Width;
            _win.Height = _size.Height;

            _win.Left = _location.X;
            _win.Top = _location.Y;

            VerifyPropertiesForCorrectValues();

            _win.WindowState = WindowState.Minimized;

            VerifyPropertiesForCorrectValues();
        }

        private object TestPropertiesAfterShow(object obj)
        {
            VerifyPropertiesForCorrectValues();
            return null;
        }

        private void VerifyPropertiesForCorrectValues()
        {
            if (_win.Visibility != Visibility.Visible) 
            {
                if (_win.Width != _size.Width)
                {
                    throw new ApplicationException(String.Format("Did not retreive expected value for Width before window was shown. Expected : {0} Actual {1}", _size.Width, _win.Width));
                }
                if (_win.Height != _size.Height)
                {
                    throw new ApplicationException(String.Format("Did not retreive expected value for Height before window was shown. Expected : {0} Actual {1}", _size.Height, _win.Height));
                }                
            }
            else if (_win.RenderSize != _size)
            {
                throw new ApplicationException(String.Format("Did not retreive expected value for RenderSize. Expected : {0} Actual : {1}", _size.ToString(), _win.RenderSize.ToString()));
            }

            if (_win.Left != _location.X)
            {
                throw new ApplicationException("Did not retreive correct value for Left");
            }

            if (_win.Top != _location.Y)
            {
                throw new ApplicationException("Did not retreive correct value for Top");
            }
        }

        private object TestRestoreBounds(object obj)
        {
            VerifyRestoreBounds();
            _win.WindowStyle = WindowStyle.ToolWindow;
            VerifyRestoreBounds();
            _win.WindowStyle = WindowStyle.SingleBorderWindow;
            return null;
        }

        private void VerifyRestoreBounds()
        {
            _win.WindowState = WindowState.Maximized;
            Rect rb = _win.RestoreBounds;
            _win.WindowState = WindowState.Normal;
            Rect rect = new Rect(_win.Left, _win.Top, _win.Width, _win.Height);

            if ((rb.X != rect.X) || 
                (rb.Y != rect.Y) || 
                (rb.Width != rect.Width) || 
                (rb.Height != rect.Height))
            {
                throw new ApplicationException(String.Format("RestoreBounds before restoring the window ({0}) did not match the actual normal window rect ({1})", rb, rect));
            }
        }

        private object TestTLWH(object obj)
        {
            _win.WindowState = WindowState.Maximized;
            VerifyTLWH();
            _win.WindowState = WindowState.Minimized;
            VerifyTLWH();
            _win.WindowStyle = WindowStyle.ToolWindow;
            _win.WindowState = WindowState.Maximized;
            VerifyTLWH();
            _win.WindowState = WindowState.Minimized;
            VerifyTLWH();
            return null;
        }
        
        private void VerifyTLWH()
        {
            WindowState state = _win.WindowState;
            _win.Top = 300;
            _win.Left = 300;
            _win.Width = 300;
            _win.Height = 300;
            _win.WindowState = WindowState.Normal;

            if ((_win.Top != 300) ||
                (_win.Left != 300) ||
                (_win.Width != 300) ||
                (_win.Height != 300))
            {
                throw new ApplicationException(String.Format("Window restored to the incorrect rect when T/L/W/H was specified with window {0}.  Expected T={1}, L={1}, W={1}, H={1}.  Actual T={2}, L={3}, W={4}, H={5}.", state, 300, _win.Top, _win.Left, _win.Width, _win.Height));
            }

            _win.Left = _location.X;
            _win.Top = _location.Y;
            _win.Width = _size.Width;
            _win.Height = _size.Height;
        }
        
        private object ShutdownApp(object obj)
        {
            this.Shutdown();
            return null;
        }        
    }
}

