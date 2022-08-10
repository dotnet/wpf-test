// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Security.Permissions;
using System.Security;
using System.Text;


using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;


namespace DRT
{
    public class CrossElement : UIElement
    {
        public CrossElement()
        {
        }

        protected override void OnRender(DrawingContext ctx)
        {
            ContentPresenter parent = VisualTreeHelper.GetParent(this) as ContentPresenter;
            if ( parent != null )
            {
                ctx.DrawLine(
                    new Pen(new SolidColorBrush(Color), 2.0f),
                    new Point(0, 0),
                    new Point((int)parent.ActualWidth, (int)parent.ActualHeight));
                ctx.DrawLine(
                    new Pen(new SolidColorBrush(Color), 2.0f),
                    new Point(0, (int)parent.ActualHeight),
                    new Point((int)parent.ActualWidth, 0));
            }
        }
        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
    }

    public class WindowFunctionalityApp : Application
    {
        [STAThread]
        public static int Main()
        {    
            WindowFunctionalityApp theApp = new WindowFunctionalityApp();            
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

        private Window _win;
        private double _maxLength = 500;
        private double _minLength = 200;
        private double _length = 300;
        private double _location = 100;
        private double _lengthAboveBound = 600;
        private double _lengthLessBound = 100;
        private double _moveLocation = 500;
        private State _state = State.SizeAndLocationInitialSet;
#if LOADED_BEFORE_RENDER
        private EventState _eventState = EventState.Start;
#else
        private EventState _eventState = EventState.Loaded;
#endif
        private string _title = "Welcome to WCP Window";

        DispatcherOperationCallback _runTest;
        DispatcherOperationCallback _verifyTest;

        private Logger _logger = new Logger("DrtWindowFunctionality", "Microsoft", "Various Window Functionalities DRT");
        
#if LOADED_BEFORE_RENDER
        bool _fLoaded;
#endif
        bool _fContentRendered;
        bool _fActivated;
        bool _fDeactivated;
        bool _fStateChanged;
        bool _fSizeChanged;
        bool _fClosing;

        int _countLocationChanged;
        
        enum State
        {
            SizeAndLocationInitialSet = 0,
            VerifySizeAndLocationInitialSet,
            SizeBounds,
            VerifySizeBounds,
            Move,
            VerifyMove,
            ResizeMode_NoResize,
            VerifyResizeMode_NoResize,
            ResizeMode_CanMinimize,
            VerifyResizeMode_CanMinimize,
            ResizeMode_CanResize,
            VerifyResizeMode_CanResize,
            ResizeMode_CanResizeWithGrip,
            VerifyResizeMode_CanResizeWithGrip,
            Minimized,
            VerifyMinimized,
            Normal,
            VerifyNormal,
            Maximized,
            VerifyMaximized,
            Text,
            VerifyText
        }

        enum EventState
        {
            Start = 0,
            Loaded,            
            Activated,
            ContentRendered
        }
        public WindowFunctionalityApp() : base()
        {
            _runTest = new DispatcherOperationCallback(RunTest);
            _verifyTest = new DispatcherOperationCallback(VerifyTest);
        }

        /*
         * Test Case:
         *  0a) Tests that setting Visibility is async
         *
         *  1)  Set size within that bound
         *      Set Location of the Window
         *      Verify Size and Location of the Window
         * 
         *  2)  Set size outside contentsize bounds
         *      Verify we are within content size bounds
         * 
         *  3)  Move Window
         *      Verify Move
         * 
         *  6)  Set ResizeMode to all 4 possible values
         *      Verify that the correct Win32 Styles are
         *      enabled/disabled.
         * 
         *  7)  Set window state to Minimized
         *      Check state
         * 
         *  8)  Set to Normal
         *      Check state
         * 
         *  9)  set to Maximized 
         *      Check State
         * 
         *  10) Set Test
         *      Verify Text
         * 
         *  Verify that all events were fired at least once except Closed, since it's
         *  fired at the way end. we already check that in another DRT
         * */
        protected override void OnStartup(StartupEventArgs e) 
        {
            _win = new Window();
            _win.Content = new CrossElement();

#if LOADED_BEFORE_RENDER
            _win.Loaded += new RoutedEventHandler( OnLoaded );
#endif
            _win.Activated += new EventHandler( OnActivated );
            _win.ContentRendered += new EventHandler(OnContentRendered);
            _win.Deactivated += new EventHandler( OnDeactivated );
            _win.StateChanged += new EventHandler( OnStateChanged );
            _win.SizeChanged += new SizeChangedEventHandler( OnSizeChanged );
            _win.Closing += new CancelEventHandler( OnClosing );
            _win.Title = "Window 2";
            
            _win.Visibility = Visibility.Visible;
            TestVisibilityAsynchronity();
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                _verifyTest,
                this);
            base.OnStartup(e);
        }

        private void TestVisibilityAsynchronity()
        {
            WindowInteropHelper wih = new WindowInteropHelper(_win);
            IntPtr intPtr = wih.Handle;
            
            if (intPtr != IntPtr.Zero)
            {
                throw new ApplicationException(String.Format("Setting Visible on Window is not async. win.Handle returned {0}", intPtr));
            }
        }
        
        private object RunTest(object obj)
        {
            switch(_state)
            {
                case State.SizeAndLocationInitialSet:
                    _win.MaxWidth = _maxLength;
                    _win.MaxHeight = _maxLength;
                    _win.MinWidth = _minLength;
                    _win.MinHeight = _minLength;
                    _win.Height = _length;
                    _win.Width = _length;

                    _countLocationChanged = 0;
                    _win.LocationChanged += new EventHandler(OnLocationChanged);
                    _win.Top = _location;
                    _win.Left = _location;
                    _win.LocationChanged -= new EventHandler(OnLocationChanged);
                    _state = State.VerifySizeAndLocationInitialSet;
                    break;
                case State.SizeBounds:
                    _win.Height = _lengthAboveBound;
                    _win.Width = _lengthLessBound;
                    _state = State.VerifySizeBounds;
                    break;
                case State.Move:
                    _win.Top = _moveLocation;
                    _win.Left = _moveLocation;
                    _state = State.VerifyMove;
                    break;
                case State.ResizeMode_NoResize:
                    _win.ResizeMode = ResizeMode.NoResize;
                    _state = State.VerifyResizeMode_NoResize;
                    break;
                case State.ResizeMode_CanMinimize:
                    _win.ResizeMode = ResizeMode.CanMinimize;
                    _state = State.VerifyResizeMode_CanMinimize;
                    break;
                case State.ResizeMode_CanResize:
                    _win.ResizeMode = ResizeMode.CanResize;
                    _state = State.VerifyResizeMode_CanResize;
                    break;
                case State.ResizeMode_CanResizeWithGrip:
                    _win.ResizeMode = ResizeMode.CanResizeWithGrip;
                    _state = State.VerifyResizeMode_CanResizeWithGrip;
                    break;
                case State.Minimized:
                    _win.ResizeMode = ResizeMode.CanResize;
                    _win.WindowState = WindowState.Minimized;
                    _state = State.VerifyMinimized;
                    break;
                case State.Normal:
                    _win.WindowState = WindowState.Normal;
                    _state = State.VerifyNormal;
                    break;
                case State.Maximized:         
                    _win.WindowState = WindowState.Maximized;
                    _state = State.VerifyMaximized;
                    break;
                case State.Text:
                    _win.Top = 300;
                    _win.Left = 300;
                    _win.Height = _length;
                    _win.Top = _length;
                    _win.Title = _title;
                    _state = State.VerifyText;
                    break;
                default:
                    break;
            }

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                _verifyTest,
                this);

            return null;
        }


        private object VerifyTest(object obj)
        {
            int style;
            switch(_state)
            {
                case State.VerifySizeAndLocationInitialSet:
                    Check((_win.ActualHeight == _length) && (_win.ActualWidth == _length) &&
                        (_win.Top == _location) && (_win.Left == _location));
                    _state = State.SizeBounds;
                    break;
                case State.VerifySizeBounds:
                    Check( (_win.ActualHeight != _lengthAboveBound) && (_win.ActualWidth != _lengthLessBound));
                    _state = State.Move;
                    break;
                case State.VerifyMove:
                    Check( (_win.Top == _moveLocation) && (_win.Left == _moveLocation) );
                    _state = State.ResizeMode_NoResize;
                    break;
                case State.VerifyResizeMode_NoResize:
                    style = WindowFunctionalityApp.IntPtrToInt32(WindowFunctionalityApp.GetWindowLong((new WindowInteropHelper(_win)).Handle, WindowFunctionalityApp.GWL_STYLE));
                    Check((style & (WindowFunctionalityApp.WS_MINIMIZEBOX | WindowFunctionalityApp.WS_MAXIMIZEBOX | WindowFunctionalityApp.WS_THICKFRAME)) == 0);
                    _state = State.ResizeMode_CanMinimize;
                    break;
                case State.VerifyResizeMode_CanMinimize:
                    style = WindowFunctionalityApp.IntPtrToInt32(WindowFunctionalityApp.GetWindowLong((new WindowInteropHelper(_win)).Handle, WindowFunctionalityApp.GWL_STYLE));
                    Check((style & (WindowFunctionalityApp.WS_MAXIMIZEBOX | WindowFunctionalityApp.WS_THICKFRAME)) == 0);
                    Check((style & WindowFunctionalityApp.WS_MINIMIZEBOX) != 0);
                    _state = State.ResizeMode_CanResize;
                    break;
                case State.VerifyResizeMode_CanResize:
                    style = WindowFunctionalityApp.IntPtrToInt32(WindowFunctionalityApp.GetWindowLong((new WindowInteropHelper(_win)).Handle, WindowFunctionalityApp.GWL_STYLE));
                    Check(((style & (WindowFunctionalityApp.WS_MINIMIZEBOX | WindowFunctionalityApp.WS_MAXIMIZEBOX | WindowFunctionalityApp.WS_THICKFRAME)) == (WindowFunctionalityApp.WS_MINIMIZEBOX | WindowFunctionalityApp.WS_MAXIMIZEBOX | WindowFunctionalityApp.WS_THICKFRAME)));
                    _state = State.ResizeMode_CanResizeWithGrip;
                    break;
                case State.VerifyResizeMode_CanResizeWithGrip:
                    style = WindowFunctionalityApp.IntPtrToInt32(WindowFunctionalityApp.GetWindowLong((new WindowInteropHelper(_win)).Handle, WindowFunctionalityApp.GWL_STYLE));
                    Check(((style & (WindowFunctionalityApp.WS_MINIMIZEBOX | WindowFunctionalityApp.WS_MAXIMIZEBOX | WindowFunctionalityApp.WS_THICKFRAME)) == (WindowFunctionalityApp.WS_MINIMIZEBOX | WindowFunctionalityApp.WS_MAXIMIZEBOX | WindowFunctionalityApp.WS_THICKFRAME)));
                    _state = State.Minimized;
                    break;
                case State.VerifyMinimized:
                    Check( _win.WindowState == WindowState.Minimized );
                    _state = State.Normal;
                    break;
                case State.VerifyNormal:
                    Check( _win.WindowState == WindowState.Normal );
                    _state = State.Maximized;
                    break;
                case State.VerifyMaximized:
                    Check( _win.WindowState == WindowState.Maximized );
                    _state = State.Text;
                    break;
                case State.VerifyText:
                    Check( _win.Title == _title );
                    break;
                default:
                    break;
            }

            if (_state != State.VerifyText)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    _runTest,
                    this);
            }
            else
            {
                // close window
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(CloseWindow),
                    this);
            }
            return null;
        }

        private void Check( bool result )
        {
            if (result == true)
            {
                return;
            }

            string exceptionString = null;

            switch (_state)
            {
                case State.VerifySizeAndLocationInitialSet:
                    exceptionString  = "Either of Top, Left, Height, Width not set correctly";
                    break;
                case State.VerifySizeBounds:
                    exceptionString = "Max/Min Bounds not enforced correctly";
                    break;
                case State.VerifyMove:
                    exceptionString = "Either Top, Left not set correctly";
                    break;
                case State.ResizeMode_NoResize:
                    exceptionString = "ResizeMode.NoResize -- failed";
                    break;
                case State.ResizeMode_CanMinimize:
                    exceptionString = "ResizeMode.CanMinimize -- failed";
                    break;
                case State.ResizeMode_CanResize:
                    exceptionString = "ResizeMode.CanResize -- failed";
                    break;
                case State.ResizeMode_CanResizeWithGrip:
                    exceptionString = "ResizeMode.CanResizeWithGrip -- failed";
                    break;
                case State.VerifyMaximized:
                    exceptionString = "WindowState = Maximized -- failed";
                    break;
                case State.VerifyMinimized:
                    exceptionString = "WindowState = Minimized -- failed";
                    break;
                case State.VerifyNormal:
                    exceptionString = "WindowState = Normal -- failed";
                    break;
                case State.VerifyText:
                    exceptionString = "Window.Text failed";
                    break;
                default:
                    break;
            }

            throw new ApplicationException( exceptionString );
        }

        public void VerifyEvents()
        {
            bool success = true;
            string exceptionString = null;

#if LOADED_BEFORE_RENDER
            if ( _fLoaded == false )
            {
                success = false;
                exceptionString = "Loaded event not fired";
            }            
#endif
            if ( _fActivated == false )
            {
                success = false;
                exceptionString = "Activated event not fired";
            }

            if (_fContentRendered == false)
            {
                success = false;
                exceptionString = "ContentRendered event not fired";
            }

            if ( _fDeactivated == false )
            {
                success = false;
                exceptionString = "Deactivated event not fired";
            }

            if ( _fStateChanged == false )
            {
                success = false;
                exceptionString = "StateChanged event not fired";
            }

            if ( _fSizeChanged == false )
            {
                success = false;
                exceptionString = "SizeChanged event not fired";
            }

            if ( _countLocationChanged != 2 )
            {
                success = false;
                exceptionString = String.Format("LocationChanged event not fired twice.  It was fired {0} times", _countLocationChanged);
            }

            if ( _fClosing == false )
            {
                success = false;
                exceptionString = "Closing event not fired";
            }

            if ( success == false )
            {
                throw new ApplicationException(exceptionString);
            }
        }

#if LOADED_BEFORE_RENDER
        private void OnLoaded(object sender, EventArgs args)
        {
            switch (_eventState)
            {
                case EventState.Start: 
                    _eventState = EventState.Loaded;
                    _fLoaded = true;
                    break;

                default: 
                    throw new ApplicationException("The order of event firing is wrong"); 
             }
        }        
#endif

        private void OnActivated(object sender, EventArgs args)
        {
            switch (_eventState)
            {
                case EventState.Loaded :
                    _eventState = EventState.Activated;                    
                    break;                
            }
            _fActivated = true;
        }

        private void OnContentRendered(object sender, EventArgs args)
        {
            switch (_eventState)
            {
                case EventState.Activated :
                    _eventState = EventState.ContentRendered;
                    _fContentRendered = true;
                    break;

                default :
                    throw new ApplicationException("The order of event firing is wrong");
            }
        }

        private void OnDeactivated(object sender, EventArgs args)
        {
            _fDeactivated = true;
        }

        private void OnStateChanged(object sender, EventArgs args)
        {
            _fStateChanged = true;
        }

        private void OnSizeChanged(object sender, EventArgs args)
        {
            _fSizeChanged = true;
        }

        private void OnLocationChanged(object sender, EventArgs args)
        {
            _countLocationChanged++;
        }

        private void OnClosing(object sender, CancelEventArgs args)
        {
            _fClosing = true;
        }

        private object CloseWindow( object obj )
        {
            _win.Close();
            return null;
        }

        private static int WS_THICKFRAME = 0x00040000;
        private static int WS_MINIMIZEBOX = 0x00020000;
        private static int WS_MAXIMIZEBOX = 0x00010000;

        private static int GWL_STYLE = -16;
        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("user32.dll",
#if WIN64
         EntryPoint="GetWindowLongPtr",
#endif
         CharSet=CharSet.Auto, SetLastError=true)
        ]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
    }

}

