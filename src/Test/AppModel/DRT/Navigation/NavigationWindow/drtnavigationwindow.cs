// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Collections;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Automation;

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually. 
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/navigationwindow/navwindowfoo.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/navigationwindow/navwindowfoo2.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/navigationwindow/navwindowbar.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/navigationwindow/navwindowbar2.xaml")]

namespace DRTNavigationWindow
{
    public class NavigationWindowTestApplication : Application
    {

        [STAThread]
        public static int Main()
        {
            /*
            Creates an app, a Navigation window on App's thread, and a NavigationWindow on worker thread T2.  It tests
            the following on both NavigationWindows

            Set the Uri property on the NavigationWindow to some arbitrary Xaml. 
                
                 Check that: 
                    1) A Navigating event is fired. 
                    2) A Navigated event is fired.        
                    3) You can access the content of the NavigationWindow - via the content property. 
            4) A ContentRendered event is fired

            Call a Navigate method that takes a different Xaml file. 

                 Check that: 
                   1) A Navigating event is fired. 
                   2) A Navigated event is fired. 
                   3) You can access the content of the NavigationWindow - via the content property. 
           4) a ContentRendered event is fired

            Create an element. Call a Navigate method that takes an element.                 

                 Check that: 
                   1) A Navigating event is fired. 
                   2) A Navigated event is fired. 
                   3) You can access the content of the NavigationWindow - via the content property & you get the same element you had before. 
           4) A ContentRendered event is fired

            */ 

            try
            {
                Application theApp = new NavigationWindowTestApplication();
                theApp.Run();
            }
            catch( ApplicationException e )
            {
                _logger.Log(e);
                return 1;
            }

            _logger.Log( "Passed" );
            return 0;
        }

        internal static DRT.Logger _logger = new DRT.Logger("DrtNavigationWindow", "Microsoft/Microsoft", "Testing NavigationWindow navigation events");   
        private ArrayList _windows = new ArrayList();
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        
        protected override void OnStartup(StartupEventArgs e) 
        {

            NavigationWindowTest nwt1 = new NavigationWindowTest("NavigationWindowOnAppThread", "NavWindowFoo.xaml", @"drtfiles\navigationwindow\NavWindowBar.xaml");
            nwt1.Window.Closed += new EventHandler(W1ClosedHandler);
            lock(_windows.SyncRoot)
            {
                _windows.Add(nwt1.Window);
            }
            Thread t2 = new Thread(new ThreadStart(TestNavigationWindowOnT2));
            t2.SetApartmentState(ApartmentState.STA);
            t2.Start();

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _autoResetEvent.WaitOne();
            base.OnStartup(e);
        }

        private void TestNavigationWindowOnT2()
        {
            NavigationWindowTest nwt2 = new NavigationWindowTest("NavigationWindowOnThread2", "NavWindowFoo2.xaml", @"drtfiles\navigationwindow\NavWindowBar2.xaml");
            nwt2.Window.Closed += new EventHandler(W2ClosedHandler);
            lock(_windows.SyncRoot)
            {
                _windows.Add(nwt2.Window);
            }
            _autoResetEvent.Set();
            Dispatcher.Run();
        }

        private void W1ClosedHandler(object sender, EventArgs args)
        {
            Window win = sender as Window;
            _logger.Log(String.Format("(NavigationWindow Name = {0}) Closed event fired", win.Name));
            lock(_windows.SyncRoot)
            {
                _windows.Remove(sender);
                if (_windows.Count == 0)
                {
                    this.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        (DispatcherOperationCallback) delegate(object notused)
                        {
                            Shutdown();
                            return null;
                        },
                        null);
                }
            }
        }

        private void W2ClosedHandler(object sender, EventArgs args)
        {
            Window win = sender as Window;
            _logger.Log(String.Format("(NavigationWindow Name = {0}) Closed event fired", win.Name));

            lock(_windows.SyncRoot)
            {
                _windows.Remove(sender);
                if (_windows.Count == 0)
                {
                    this.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        (DispatcherOperationCallback) delegate(object notused)
                        {
                            Shutdown();
                            return null;
                        },
                        null);
                }
            }
            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        }
    }

    public class NavigationWindowTest
    {
        enum State
        {
            Start, 
            PendingFirst, 
            FirstNavigation, 
            FirstContentRendered,
            PendingSecond,
            SecondNavigation,
            SecondContentRendered, 
            PendingThird,
            ThirdNavigation,
            ThirdContentRendered
        }
        
        private NavigationWindow _win;
        private State _myState;
        private string _barXaml;
        private string _fooXaml;

        public NavigationWindowTest(string name, string fooXaml, string barXaml)
        {
            _fooXaml = fooXaml;
            _barXaml = barXaml;
            
            _win = new NavigationWindow();
            _win.Title = name;
            _win.Width = 400;
            _win.Height = 400;
            _win.Name = name;
            _win.ResizeMode= ResizeMode.CanResizeWithGrip;
            
            _myState = State.Start; 
            
            _win.Navigating += new NavigatingCancelEventHandler(OnNavigating);
            _win.Navigated += new NavigatedEventHandler(OnNavigated);
            _win.ContentRendered += new EventHandler(OnContentRendered);      
            _win.Loaded += new RoutedEventHandler(OnLoaded);

            _win.Source=new Uri(@"DrtFiles\NavigationWindow\" + _fooXaml, UriKind.RelativeOrAbsolute); 
            _win.Show();

            // Verify named parts of NW's ControlTemplate.
            // [Frame's template is verified in DrtControls.]
            ContentPresenter cp = (ContentPresenter)_win.Template.FindName("PART_NavWinCP", _win);
            if (cp == null)
                throw new ApplicationException("PART_NavWinCP not found.");
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            NavigationWindowTestApplication._logger.Log(String.Format("(NavigationWindow Name = {0}) Loaded event fired", _win.Name));
        }
        
        private void OnNavigating(Object sender, NavigatingCancelEventArgs e)
        {
            NavigationWindowTestApplication._logger.Log(String.Format("(NavigationWindow Name = {0}) Navigating event fired", _win.Name));
            
            switch (_myState)
            {
                case State.Start : 
                    _myState = State.PendingFirst; 
                    break; 
                case State.FirstContentRendered : 
                    _myState = State.PendingSecond; 
                    break; 
                case State.SecondContentRendered : 
                    _myState = State.PendingThird; 
                    break; 
                default: 
                    throw new ApplicationException( String.Format("(NavigationWindow Name = {0}) At Invalid state in OnNavigating. Did you get a Navigated event ?", _win.Name)); 
            }
        }

        private void OnNavigated(Object sender, NavigationEventArgs e)
        {
            NavigationWindowTestApplication._logger.Log(String.Format("(NavigationWindow Name = {0}) Navigated event fired", _win.Name));

            switch( _myState )
            {
                case State.PendingFirst : 
                    _myState = State.FirstNavigation;                     
                    break; 
                case State.PendingSecond  : 
                    _myState = State.SecondNavigation;                     
                    break; 
                case State.PendingThird : 
                    _myState = State.ThirdNavigation;                    
                    break; 
                default: 
                    throw new ApplicationException( String.Format("(NavigationWindow Name = {0}) At Invalid state in OnNavigated. Did you get a Navigating event ?", _win.Name)); 
            }
            
            // Check for presence of ContentElement - and that it is a UIElement

            object content = _win.Content; 
            if ( content == null ) 
            {
                throw new ApplicationException(String.Format("(NavigationWindow Name = {0}) Did not get a content element. Did you really navigate ?", _win.Name));     
            }

            // TODO: When we have a parser - we should be validating the tree we get with what we expected.  
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            NavigationWindowTestApplication._logger.Log(String.Format("(NavigationWindow Name = {0}) ContentRendered event fired", _win.Name));

            switch (_myState)
                {
                    case State.FirstNavigation : 
                        _myState = State.FirstContentRendered;                    
                        _win.Navigate( new Uri(_barXaml, UriKind.RelativeOrAbsolute));
                        break; 
                    case State.SecondNavigation  : 
                        _myState = State.SecondContentRendered;                     
                        CrossElement myElem = new CrossElement(); 
                        _win.Navigate( myElem );                     
                        break; 
                    case State.ThirdNavigation :                                         
                        _myState = State.ThirdContentRendered;
                        // Post dispatcher msg to close the window
                        Dispatcher.CurrentDispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new DispatcherOperationCallback(CloseWindow),
                            this);
                        break; 
                    default: 
                        throw new ApplicationException(String.Format("(NavigationWindow Name = {0}) At Invalid state in ContentRendered. Did you get a ContentRendered event ?", _win.Name)); 
                }       
        }

        private object CloseWindow(object obj)
        {
            NavigationWindowTestApplication._logger.Log(String.Format("(NavigationWindow Name = {0}) Calling Window.Close", _win.Name));

            _win.Close();
            
            return null;
        }

        public Window Window
        {
            get{ return _win;}
        }
        
    }

    public class CrossElement : UIElement
    {   
        protected override void OnRender(DrawingContext ctx)
        {
            ContentPresenter parent = VisualTreeHelper.GetParent(this) as ContentPresenter;
            if (parent != null)
            {
                ctx.DrawLine(
                    new Pen(new SolidColorBrush(Color), 2.0f),
                    new Point(0, 0),
                    new Point(parent.ActualWidth, parent.ActualHeight));
                ctx.DrawLine(
                    new Pen(new SolidColorBrush(Color), 2.0f),
                    new Point(0, parent.ActualHeight),
                    new Point(parent.ActualWidth, 0));
            }
        }
 
        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
    }    
}


