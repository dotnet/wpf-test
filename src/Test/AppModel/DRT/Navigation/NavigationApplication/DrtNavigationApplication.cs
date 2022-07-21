// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Media;
using System.Windows.Navigation; 
using System.Windows.Controls;

[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/navigationapplication/navappfoo.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/navigationapplication/navappbar.xaml")]

namespace DRT
{
    public class DRTNavigationApplication
    {

        [STAThread]
        public static int Main()
        {
            /*
            Create a Application
            Set the StartupPage Property
            Run the app

            Check that:
                1) Navigating is fired
                2) Navigated if fired
                3) LoadCompleted is fired
            
            Navigate the NavigationWindow to the cross UIElement
            Check that:
                1) Navigating is fired
                2) Navigated if fired
                3) LoadCompleted is fired
            */ 
            
            try
            {
                s_navApp = new Application();
                s_navApp.StartupUri = s_startupUri;
                s_navApp.Navigating += new NavigatingCancelEventHandler(OnNavigating);
                s_navApp.Navigated += new NavigatedEventHandler(OnNavigated); 
                s_navApp.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
                s_navApp.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);
                s_navApp.NavigationProgress += new NavigationProgressEventHandler(OnNavigationProgress);
                s_navApp.Run();
            }
            catch( ApplicationException e )
            {
                s_logger.Log(e);
                return 1;
            }

            s_logger.Log("Passed");
            return 0;
        }

        private static Uri s_startupUri = new Uri("DrtFiles/NavigationApplication/NavAppFoo.xaml", UriKind.RelativeOrAbsolute);
        private static Uri s_secondUri = new Uri("DrtFiles/NavigationApplication/NavAppBar.xaml", UriKind.RelativeOrAbsolute);

        private static void VerifySource(Uri source)
        {
            NavigationWindow navWin = (NavigationWindow)s_navApp.MainWindow;
            if  (navWin.Source != source)
            {
                throw new ApplicationException( "The CLR Source propety is not updated." );
            }
            if ((Uri)navWin.GetValue(Frame.SourceProperty) != source)
            {
                throw new ApplicationException("The Source dependency propety is not updated.");
            }
        }

        private static void OnNavigationProgress(Object source, NavigationProgressEventArgs e)
        {
            switch (s_myState)
            {
                case State.PendingSecond1:
                    VerifySource(s_secondUri);
                    ((NavigationWindow)s_navApp.MainWindow).StopLoading();
                    VerifySource(s_startupUri);
                    s_myState = State.PendingSecondNavigate2;
                    ((NavigationWindow)s_navApp.Windows[0]).Navigate(s_secondUri);
                    VerifySource(s_secondUri);
                    break;
            }
        }

        private static void OnNavigationStopped(object source, NavigationEventArgs e)
        {           
        }

        private static void OnNavigating(Object sender, NavigatingCancelEventArgs e)
        {
            switch( s_myState )
            {
                case State.Start : 
                    s_myState = State.PendingFirst;
                    break; 
                case State.FirstNavigation :                      
                    s_myState = State.PendingSecondCancel;
                    e.Cancel = true;
                    break;
                case State.PendingSecondNavigate1 :
                    s_myState = State.PendingSecond1;
                    VerifySource(s_secondUri);
                    break;
                case State.PendingSecondNavigate2:
                    s_myState = State.PendingSecond;
                    VerifySource(s_secondUri);
                    break;
                case State.SecondNavigation : 
                    s_myState = State.PendingThird;
                    VerifySource(null);
                    break; 
                default: 
                    throw new ApplicationException( "At Invalid state in OnNavigating. Did you get a Navigated event ?"); 
            }
        }

        private static void OnNavigated(Object sender, NavigationEventArgs e)
        {
            switch( s_myState )
            {
                case State.PendingFirst : 
                    s_myState = State.PendingFirstLoadCompleted ;
                    VerifySource(s_startupUri);
                    break; 
                case State.PendingSecond  : 
                    s_myState = State.PendingSecondLoadCompleted ;
                    VerifySource(s_secondUri);
                    break; 
                case State.PendingThird : 
                    s_myState = State.PendingThirdLoadCompleted ;
                    VerifySource(null);
                    break; 
                default: 
                    throw new ApplicationException( "At Invalid state in OnNavigated. Did you get a Navigating event ?"); 
            }
            
            // Check for presence of ContentElement - and that it is a UIElement

            object content = e.Content ; 
            if ( content == null ) 
            {
                throw new ApplicationException("Did not get a content element. Did you really navigate ?");     
            }
        }

        private static void OnLoadCompleted(Object sender, NavigationEventArgs e)
        {
            switch( s_myState )
            {
                case State.PendingFirstLoadCompleted : 
                    s_myState = State.FirstNavigation ;
                    VerifySource(s_startupUri);
                    // The following navigation will be cancelled.
                    ((NavigationWindow)s_navApp.Windows[0]).Navigate( s_secondUri );
                    // Uri shouldn't change
                    VerifySource(s_startupUri);
                    s_myState = State.PendingSecondNavigate1;
                    ((NavigationWindow)s_navApp.Windows[0]).Navigate(s_secondUri);
                    VerifySource(s_secondUri);
                    break; 
                case State.PendingSecondLoadCompleted  : 
                    s_myState = State.SecondNavigation ; 
                    CrossElement myElem = new CrossElement(); 
                    ((NavigationWindow)s_navApp.Windows[0]).Navigate( myElem ) ;
                    VerifySource(null);
                    break; 
                case State.PendingThirdLoadCompleted : 
                    s_myState = State.ThirdNavigation ;
                    // Post dispatcher msg to close the window
                    Dispatcher.CurrentDispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new DispatcherOperationCallback(ShudDownApplication),
                        null);
                    break; 
                default: 
                    throw new ApplicationException( "At Invalid state in OnNavigated. Did you get a Navigating event ?"); 
            }
            
            // Check for presence of ContentElement - and that it is a UIElement

            object content = e.Content ; 
            if ( content == null ) 
            {
                throw new ApplicationException("Did not get a content element. Did you really navigate ?");     
            }
        }

        private static object ShudDownApplication(object obj)
        {
            s_navApp.Shutdown();
            return null;
        }

        private static State s_myState = State.Start; 
        private static Application s_navApp;
        private static Logger s_logger = new Logger("DrtNavigationApplication", "KusumaV", "Testing Application Startup nav events");

        enum State
        {
            Start, 
            PendingFirst, 
            PendingFirstLoadCompleted,
            FirstNavigation, 
            PendingSecondCancel,
            PendingSecond1,
            PendingSecondNavigate1,
            PendingSecond,
            PendingSecondNavigate2,
            PendingSecondLoadCompleted,
            SecondNavigation, 
            PendingThird,
            PendingThirdLoadCompleted,
            ThirdNavigation
        }
    }


    public class CrossElement : UIElement
    {
        public CrossElement()
        {
        }

        protected override void OnRender(DrawingContext ctx)
        {
            ContentPresenter parent = VisualTreeHelper.GetParent(this) as ContentPresenter;

            if (parent != null)
            {
                ctx.DrawLine(new Pen(new SolidColorBrush(Color), 2.0f), new Point(0, 0), new Point(parent.ActualWidth, parent.ActualHeight));
                ctx.DrawLine(new Pen(new SolidColorBrush(Color), 2.0f), new Point(0, parent.ActualHeight), new Point(parent.ActualWidth, 0));
            }
        }
        
        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
    }
}