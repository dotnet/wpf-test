// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT for time delayed (1.5 minutes default) navigation inside the navigation application
    /// </summary>
    public partial class NavigationTests : Application
    {
        #region constants
        /// <summary>
        /// A default time delay of 1.5 minutes
        /// </summary>
        public static long TimeDelayedNavigation_DEFAULT_DELAY = 90000;
        
        /// <summary>
        /// Current State in Test App
        /// </summary>
        enum TimeDelayedNavigation_State
        {
            UnInited,
            Inited,
            NavigatingToPage1,
            Page1LoadComplete,
            NavigatingToPage2,
            Page2LoadComplete
        };

        #endregion
       
        #region properties and fields
       
        /// <summary>
        /// Delay time in milliseconds. Initialized to TimeDelayedNavigation_DEFAULT_DELAY
        /// </summary>
        protected long TimeDelayedNavigation_delay = TimeDelayedNavigation_DEFAULT_DELAY;

        /// <summary>
        /// The Delay property specifies time to wait in milliseconds
        /// before performing a navigate operation
        /// </summary>
        public long TimeDelayedNavigationDelay
        {
            get { return TimeDelayedNavigation_delay; }
            set { TimeDelayedNavigation_delay = value; }
        }
        
        /// <summary>
        /// current state in navigation app
        /// </summary>
        TimeDelayedNavigation_State _timeDelayedNavigation_currentState = TimeDelayedNavigation_State.UnInited;
             
        //static AutomationFramework fmwk = null;
        //NavigationWindow TimeDelayedNavigation_navWin = null;

        #endregion

        #region event handlers

        void TimeDelayedNavigation_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("TimeDelayedNavigation");
            Application.Current.StartupUri = new Uri(@"TimeDelayedNavigation_Page1.xaml", UriKind.RelativeOrAbsolute);

            NavigationHelper.Output("onStartup start windows count = " + Windows.Count);            
            //LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            //Navigated += new NavigatedEventHandler(OnNavigated);
            //Navigating += new NavigatingCancelEventHandler(OnNavigating);              
            //base.OnStartup(e);
            _timeDelayedNavigation_currentState = TimeDelayedNavigation_State.Inited;
            NavigationHelper.SetStage(TestStage.Run);
            NavigationHelper.Output("onStartup end windows count = " + Windows.Count);
        }


        protected void TimeDelayedNavigation_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("OnLoadCompleted start uri " + e.Uri);

            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            if (navWin == null)
                NavigationHelper.Fail("Could not grab reference to NavigationWindow");

            switch(_timeDelayedNavigation_currentState)
            {
                case TimeDelayedNavigation_State.NavigatingToPage1:
                    navWin.Title = "Delayed Navigation";
                    navWin.ContentRendered += new EventHandler(OnContentRendered_TimeDelayedNavigation_nw);
                    _timeDelayedNavigation_currentState = TimeDelayedNavigation_State.Page1LoadComplete;
                    break;

                case TimeDelayedNavigation_State.NavigatingToPage2:
                    _timeDelayedNavigation_currentState = TimeDelayedNavigation_State.Page2LoadComplete;
                    break;
            }        

            Log.Current.CurrentVariation.LogMessage("OnLoadCompleted end");
        }

        protected void OnContentRendered_TimeDelayedNavigation_nw(object sender, EventArgs e)
        {
            NavigationHelper.Output("OnContentRendered start");
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            if (navWin == null)
                NavigationHelper.Output("Could not grab reference to NavigationWindow");

            switch (_timeDelayedNavigation_currentState)
            {
                case TimeDelayedNavigation_State.Page1LoadComplete:
                        NavigationHelper.Output("Sleeping " + TimeDelayedNavigationDelay + " ms");
                        new Thread(new ThreadStart(TimeDelayedNavigation_Sleep)).Start();
                        break;

                case TimeDelayedNavigation_State.Page2LoadComplete:
                        NavigationHelper.Pass("Successfully completed time-delayed navigation");
                        break;
            }
            NavigationHelper.Output("OnContentRendered end");
        }

        private void TimeDelayedNavigation_Sleep()
        {
            // use TimeSpan instead of this downcast from long to int
            Thread.Sleep((int)TimeDelayedNavigationDelay);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new DispatcherOperationCallback(TimeDelayedNavigation_NavigateNext), this);
        }

        private object TimeDelayedNavigation_NavigateNext(object o)
        {
            NavigationHelper.Output("Awake");
            NavigationHelper.Output("Navigating to Page 2");
            ((NavigationWindow)Application.Current.MainWindow).Navigate(new Uri(@"TimeDelayedNavigation_Page2.xaml", UriKind.RelativeOrAbsolute));
            return null;
        }

        public void TimeDelayedNavigation_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("OnNavigating start uri = " + e.Uri);

            switch (_timeDelayedNavigation_currentState)
            {
                case TimeDelayedNavigation_State.Inited:
                    _timeDelayedNavigation_currentState = TimeDelayedNavigation_State.NavigatingToPage1;
                    break;

                case TimeDelayedNavigation_State.Page1LoadComplete:
                    _timeDelayedNavigation_currentState = TimeDelayedNavigation_State.NavigatingToPage2;                    
                    break;
            }

            NavigationHelper.Output("OnNavigating end");
        }

        public void TimeDelayedNavigation_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("OnNavigated start uri = " + e.Uri);
            NavigationHelper.Output("OnNavigated end");           
        }

        #endregion

    }

}
