// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Simple P1 test case that verifies that an InvalidOperationException is thrown
    /// when we perform a navigate operation on a window from another thread
    /// </summary>
    public partial class NavigationTests : Application
    {
        NavigationWindow _threadedNavigateInvalid_navWin = null;
        //Application ThreadedNavigateInvalid_navApp = null;

        enum ThreadedNavigateInvalid_State
        {
            UnInit,
            Init,
        };

        ThreadedNavigateInvalid_State _threadedNavigateInvalid_currentState = ThreadedNavigateInvalid_State.UnInit;

        void ThreadedNavigateInvalid_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("ThreadedNavigateInvalid");
            this.StartupUri = new Uri("ThreadedNavigateInvalid_Page1.xaml", UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(TestStage.Run);
            _threadedNavigateInvalid_currentState = ThreadedNavigateInvalid_State.Init;
            //ThreadedNavigateInvalid_navApp = Application.Current as Application;
            //navApp.Navigated += new NavigatedEventHandler(OnNavigated);
            //navApp.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            //navApp.Exit += new ExitEventHandler(OnExit);            
            
        }

        void ThreadedNavigateInvalid_Exit(object sender, ExitEventArgs e)
        {
            NavigationHelper.Output("Shutting down");
            NavigationHelper.SetStage(TestStage.Cleanup);
        }

        void ThreadedNavigateInvalid_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_threadedNavigateInvalid_currentState)
            {
                case ThreadedNavigateInvalid_State.Init:
                    _threadedNavigateInvalid_navWin = this.MainWindow as NavigationWindow;
                    break;
            }
        }

        void ThreadedNavigateInvalid_LoadCompleted(object sender, NavigationEventArgs e)
        {
            switch (_threadedNavigateInvalid_currentState)
            {
                case ThreadedNavigateInvalid_State.Init:
                    new Thread(new ThreadStart(ThreadedNavigateInvalid_NavigateNext)).Start();
                    break;                
            }
        }

        private void ThreadedNavigateInvalid_NavigateNext()
        {
            bool exceptionThrown = true;
            try
            {
                // should throw InvalidOperationException
                _threadedNavigateInvalid_navWin.Navigate("Some text");
                // navWin.Title = "This is a test";
                exceptionThrown = false;
            }
            catch (InvalidOperationException ioe)
            {
                NavigationHelper.Pass("Caught InvalidOperationException on calling navWin.Navigate() from different thread\n" + ioe.ToString());
            }

            if (!exceptionThrown)
            {
                NavigationHelper.Fail("navWin.Navigate() from a different thread didn't throw InvalidOperationException");
            }
        }
    }
}
