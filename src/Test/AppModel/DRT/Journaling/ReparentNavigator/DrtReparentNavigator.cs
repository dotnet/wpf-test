// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using System.Windows.Media;
using System.Windows.Navigation; 
using System.Windows.Controls;

namespace DRTNavigationApplication
{
    public class DRTNavigationApplication
    {

        [STAThread]
        public static int Main()
        {            
            try
            {
                _navApp = new Application();
                _navApp.StartupUri = new Uri("pack://siteoforigin:,,,/DrtFiles/ReparentNavigator/ReparentStart.xaml", UriKind.RelativeOrAbsolute);
                _navApp.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
                _navApp.Run();
            }
            catch( ApplicationException e )
            {
                _logger.Log(e);
                return 1;
            }

            _logger.Log( "Passed" );
            return 0;
        }
    
        private static void OnLoadCompleted(Object sender, NavigationEventArgs e)
        {
            switch( _myState )
            {
                case State.Start:
                    {
                        _myState = State.FirstLoadCompleted;
                        break;
                    }
                case State.FirstLoadCompleted  : 
                    {
                        _myState = State.SecondLoadCompleted; 
                        Frame frame = LogicalTreeHelper.FindLogicalNode((DependencyObject)_navApp.Windows[0].Content, "FRAME1") as Frame;
                        frame.Source = new Uri("pack://siteoforigin:,,,/DrtFiles/ReparentNavigator/Reparentpage1.xaml", UriKind.RelativeOrAbsolute);
                        break; 
                    }
                case State.SecondLoadCompleted :
                    {
                        _myState = State.ThirdLoadCompleted;
                        Frame frame = LogicalTreeHelper.FindLogicalNode((DependencyObject)_navApp.Windows[0].Content, "FRAME1") as Frame;
                        (((NavigationWindow)_navApp.Windows[0]).Content as DockPanel).Children.Remove(frame);

                        if (((NavigationWindow)_navApp.Windows[0]).CanGoBack == true)
                            throw new ApplicationException();

                        frame = new Frame();
                        frame.Source = new Uri("pack://siteoforigin:,,,/DrtFiles/ReparentNavigator/Reparentpage2.xaml", UriKind.RelativeOrAbsolute);
                        (((NavigationWindow)_navApp.Windows[0]).Content as DockPanel).Children.Add(frame);
                        break;
                    }
                case State.ThirdLoadCompleted : 
                    {
                        _myState = State.FourthLoadCompleted;
                        Frame frame = e.Navigator as Frame;
                        frame.Source = new Uri("pack://siteoforigin:,,,/DrtFiles/ReparentNavigator/Reparentpage3.xaml", UriKind.RelativeOrAbsolute);
                        break;
                    }
                case State.FourthLoadCompleted:
                    {
                        if (((NavigationWindow)_navApp.Windows[0]).CanGoBack == false)
                            throw new ApplicationException();

                        // Post dispatcher msg to close the window
                        Dispatcher.CurrentDispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new DispatcherOperationCallback(ShutDownApplication),
                            null);
                        break; 
                    }
                default: 
                    throw new ApplicationException( "At Invalid state in OnNavigated. Did you get a Navigating event ?"); 
            }
            
            object content = e.Content ; 
            if ( content == null ) 
            {
                throw new ApplicationException("Did not get a content element. Did you really navigate ?");     
            }
        }

        private static object ShutDownApplication(object obj)
        {
            _navApp.Shutdown();
            return null;
        }

        private static State _myState = State.Start; 
        private static Application _navApp;
        private static DRT.Logger _logger = new DRT.Logger("DrtReparentNavigator", "Microsoft", "Testing journal state after INavigators are added or removed");

        enum State
        {
            Start, 
            FirstLoadCompleted,
            SecondLoadCompleted,
            ThirdLoadCompleted,
            FourthLoadCompleted
        }
    }
}
