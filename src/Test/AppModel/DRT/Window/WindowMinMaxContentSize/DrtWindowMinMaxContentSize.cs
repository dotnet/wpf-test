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
    public class Harness
    {
        [STAThread]
        public static int Main()
        {
            WindowMinMaxSizeApp theApp = new WindowMinMaxSizeApp();

            try
            {                
                theApp.Run();
            }
            catch (ApplicationException e)
            {
                WindowMinMaxSizeApp.Logger.Log(e);
                return 1;
            }

            WindowMinMaxSizeApp.Logger.Log("Passed");
            return 0;
        }
    }

    public class WindowMinMaxSizeApp : Application
    {
        private static Logger s_logger;

        private Window _win;
        private double _length = 400;
        private double _minLength = 300;
        private double _maxLength = 500;

        public WindowMinMaxSizeApp() : base()
        {
        }

        internal static Logger Logger
        {
            get
            {
                if (s_logger == null)
                {
                    s_logger = new Logger("DrtWindowMinMaxContentSize", "Microsoft & Microsoft", "Testing Window [Min/Max]Width/Height behavior");
                }
                return s_logger;
            }        
        }
        //
        // Test Case:
        //  1) Set Min/Max[Width/Height].  Set Width/Height outside that 
        //      range and verify that the window came up with the min/max
        //      range.
        // 
        //  2) Resize window width/height to beyond the min/max range
        //      and verify that the window does not go beyond that range.
        // 
        //  3) Set window Min[Width/Height] to greater than window Width
        //      and verify that the window is equal to the Min[Width/Height].
        //      Similar test for Max[Width/Height]
        // 
        protected override void OnStartup(StartupEventArgs e) 
        {
            _win = new Window();
            Button b = new Button();
            b.Content = "Hello World";
            _win.Content = b;

            _win.Width = _length - 200;  // 200
            _win.Height = _length - 200; // 200
            _win.MinWidth = _minLength;  // 300
            _win.MinHeight = _minLength; // 300
            _win.MaxWidth = _maxLength;  // 500
            _win.MaxHeight = _maxLength; // 500

            _win.Title = "DRTWindowMinMaxSize";
            _win.Show();  // Window should show up 300. 300

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestInitialSize),
                this);
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestResizing),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestSizeUpdate),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ShutdownApp),
                this);
            base.OnStartup(e);
        }

        private object TestInitialSize(object obj)
        {
            if ((_win.ActualWidth != _minLength) && (_win.ActualHeight != _minLength))
            {
                throw new ApplicationException(String.Format("Window initially showed up with width/height beyond min/max specified -- _win.ActualWidth = {0}, _win.ActualHeight = {1}, _minLength = {2}, _maxLength = {3}", _win.ActualWidth, _win.ActualHeight, _minLength, _maxLength));
            }
            
            return null;
        }
        
        private object TestResizing(object obj)
        {
            _win.Width = 600;
            _win.Height = 600;
            // Window size should be max size (500, 500)
            if (_win.ActualWidth != _maxLength ||
                _win.ActualHeight != _maxLength)
            {
                throw new ApplicationException("Resized beyond _maxLength limits");
            }

            _win.Width = 200;
            _win.Height = 200;
            // Window size should be min size (200, 200)
            if (_win.ActualWidth != _minLength ||
                _win.ActualHeight != _minLength)
            {
                throw new ApplicationException("Resized beyond _minLength limits");
            }
            return null;
        }

        private object TestSizeUpdate(object obj)
        {
            _win.MinWidth = (double)Window.MinWidthProperty.DefaultMetadata.DefaultValue;
            _win.MinHeight = (double)Window.MinHeightProperty.DefaultMetadata.DefaultValue;
            _win.MaxWidth = (double)Window.MaxWidthProperty.DefaultMetadata.DefaultValue;
            _win.MaxHeight = (double)Window.MaxHeightProperty.DefaultMetadata.DefaultValue;

            double minLength = _length + 100d;
            //Size minContentSize = new Size(_contentSize.Width + 100d, _contentSize.Height + 100d);
            
            _win.Width = _length; //400
            _win.MaxWidth = _length; //400
            _win.MinWidth = minLength; //500
            if (_win.ActualWidth != minLength)
            {
                throw new ApplicationException("_win.ActualWidth is not equal to minLength after setting a Window.MinWidth which a greater than Window.Width");
            }
            
            _win.Height = _length; //400
            _win.MaxHeight = _length; //400
            _win.MinHeight = minLength; //500
            if (_win.ActualHeight != minLength)
            {
                throw new ApplicationException("_win.ActualHeight is not equal to minLength after setting a Window.MinHeight which a greater than Window.Height");
            }

            _win.MinWidth = (double)Window.MinWidthProperty.DefaultMetadata.DefaultValue;
            _win.MinHeight = (double)Window.MinHeightProperty.DefaultMetadata.DefaultValue;
            _win.MaxWidth = (double)Window.MaxWidthProperty.DefaultMetadata.DefaultValue;
            _win.MaxHeight = (double)Window.MaxHeightProperty.DefaultMetadata.DefaultValue;

            // Should be enabled when 
            /*if ((_win.ActualWidth != _length) || (_win.ActualHeight != _length))
            {
                throw new ApplicationException("_win ActualWidth & ActualHeight should be 400 here");
            }           */ 

            _win.MinWidth = _maxLength; //500
            _win.MaxWidth = _minLength; //300
            if (_win.ActualWidth != _maxLength)
            {
                throw new ApplicationException("_win.ActualWidth is not equal to maxLength after setting a Window.MaxWidth which a less than Window.Width");
            }            

            _win.MinHeight = _maxLength; //500
            _win.MaxHeight  = _minLength; //300
            if (_win.ActualHeight != _maxLength)
            {
                throw new ApplicationException("_win.ActualHeight is not equal to maxLength after setting a Window.MaxHeight which a less than Window.Height");
            }

            // Should be enabled when 
            /*_win.MinWidth = (double)Window.MinWidthProperty.DefaultMetadata.DefaultValue;
            _win.MinHeight = (double)Window.MinHeightProperty.DefaultMetadata.DefaultValue;
            _win.MaxWidth = (double)Window.MaxWidthProperty.DefaultMetadata.DefaultValue;
            _win.MaxHeight = (double)Window.MaxHeightProperty.DefaultMetadata.DefaultValue;

            if ((_win.ActualWidth != _length) || (_win.ActualHeight != _length))
            {
                throw new ApplicationException("_win ActualWidth & ActualHeight should be 400 here");
            }*/

            return null;
        }
    
        private object ShutdownApp(object obj)
        {
            this.Shutdown();
            return null;
        }        
    }
}

