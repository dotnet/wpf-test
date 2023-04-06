// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*      Base Class for Code PageFunction tests                                       *
*                                                                                   *
*  Description:                                                                     *
*      The base class for code pagefunction tests.                                  *
*                                                                                   *
*                     *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

//#define testmode
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Documents;
using System.Reflection;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public abstract class BaseTestNavApp
    {
        public string Description
        {
            set
            {
                _description = value;
                NavigationHelper.Output(value);
            }
            get
            {
                //if (_description == null) return "unknown description";
                return _description;
            }
        }

        public NavigationWindow MainNavWindow
        {
            get
            {
                if (Application.Current.Windows != null && 
                    Application.Current.Windows.Count >= 1)
                {
                    return Application.Current.MainWindow as NavigationWindow;
                }
                else
                {
                    return null;
                }
            }
        }

        public BaseTestNavApp ()
        {
            if (Log.Current == null)
            {
                if (String.IsNullOrEmpty(Description))
                    NavigationHelper.CreateLog("Unnamed PageFunction test");
                else
                    NavigationHelper.CreateLog(Description);
            }

            NavigationHelper.SetStage(TestStage.Initialize);
            Application.Current.Startup += new StartupEventHandler(BaseTestNavApp_Startup);
            Application.Current.Exit += new ExitEventHandler(BaseTestNavApp_Exit);
        }

        #region QueueItem and associated code (tedious)

        public delegate void TestStep ();

        public delegate void VerificationDelegate ();

        // right now, unused
        protected event TestStep OnTestReady;

        private object DoVerification (object args)
        {
            VerificationDelegate delVerify = args as VerificationDelegate;

            if (delVerify != null)
            {
                NavigationHelper.Output("Performing Verification action.");
                delVerify ();
            }
            else
            {
                NavigationHelper.Output("Verification delegate was null");
            }

            return null;
        }

        private object DoTestStep (object args)
        {
            TestStep _step = args as TestStep;

            if (_step != null)
            {
                NavigationHelper.Output("Performing Test Action.");
                _step ();
            }
            else
            {
                NavigationHelper.Output("Test step delegate was null");
            }

            return null;
        }

        public void PostVerificationItem (VerificationDelegate delVerify)
        {
            NavigationHelper.Output("Posting Verification Item");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (DoVerification), delVerify);
        }

        public void PostTestItem (TestStep delTest)
        {
            NavigationHelper.Output("Posting test action");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (DoTestStep), delTest);
        }
        #endregion

        protected void EnsureVisible (FrameworkElement e)
        {
            // NOT YET IMPL
        }

        protected void DumpState ()
        {
            // NOT YET IMPL
        }

        public void FAIL (string sMsg, bool forceshutdown)
        {
            NavigationHelper.Fail(sMsg);
            //if (forceshutdown)
            //{
            //    this.Shutdown (-1);
            //}
        }

        #region App Level overrides 
        protected void BaseTestNavApp_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.Output("Starting Up.");
            //if (Description != null)
            //{
            //    NavigationHelper.Output(Description);
            //}

            NavigationHelper.SetStage(TestStage.Run);
            CmdLineArgs = e.Args;
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(OnLoad);
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(OnTestStartTime);
        }

        protected string[] CmdLineArgs
        {
            get
            {
                return _cmdlineargs;
            }
            set
            {
                _cmdlineargs = value;
            }
        }


        protected void BaseTestNavApp_Exit(object sender, ExitEventArgs e)
        {
            NavigationHelper.SetStage(TestStage.Cleanup);
            NavigationHelper.Output("Shutting Down App");
            //base.OnExit (e);
        }

        #endregion

        private void OnLoad (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted Event fired." + ((e.Uri != null) ? (" Uri is: " + e.Uri.ToString()) : ""));
        }

        private void OnTestStartTime (object sender, NavigationEventArgs e)
        {
            //test start time can only fire once.
            Application.Current.LoadCompleted -= new LoadCompletedEventHandler (OnTestStartTime);
            if (OnTestReady != null)
            {
                // fire the event
                NavigationHelper.Output("Invoking all listeners to the OnTestReady event");
                OnTestReady ();
                OnTestReady = null;
            }
        }

        #region private
        private string _description;
        private string[] _cmdlineargs;
        #endregion
    }

    public abstract class BaseTestPureApp : Application
    {
        public string Description
        {
            set
            {
                _description = value;
            }
            get
            {
                //if (_description == null) return "unknown description";
                return _description;
            }
        }

        public BaseTestPureApp ()
        {
            NavigationHelper.CreateLog("BaseTestPureApp-derived test");
            NavigationHelper.SetStage(TestStage.Initialize);
        }

        #region QueueItem and associated code (tedious)

        public delegate void TestStep ();

        public delegate void VerificationDelegate ();

        // right now, unused
        protected event TestStep OnTestReady;

        private object DoVerification (object args)
        {
            VerificationDelegate delVerify = args as VerificationDelegate;

            if (delVerify != null)
            {
                NavigationHelper.Output("Performing Verification action.");
                delVerify ();
            }
            else
            {
                NavigationHelper.Output("Verification delegate was null");
            }

            return null;
        }

        private object DoTestStep (object args)
        {
            TestStep _step = args as TestStep;

            if (_step != null)
            {
                NavigationHelper.Output("Performing Test Action.");
                _step ();
            }
            else
            {
                NavigationHelper.Output("Test step delegate was null");
            }

            return null;
        }

        public void PostVerificationItem (VerificationDelegate delVerify)
        {
            NavigationHelper.Output("Posting Verification Item");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (DoVerification), delVerify);
        }

        public void PostTestItem (TestStep delTest)
        {
            NavigationHelper.Output("Posting test action");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (DoTestStep), delTest);
        }
        #endregion

        protected void EnsureVisible (FrameworkElement e)
        {
            // NOT YET IMPL
        }

        protected void DumpState ()
        {
            // NOT YET IMPL
        }

        public void FAIL (string sMsg, bool forceshutdown)
        {
            NavigationHelper.Fail(sMsg);
            //if (forceshutdown)
            //{
            //    this.Shutdown (-1);
            //}
        }

        #region App Level overrides 
        protected override void OnStartup (StartupEventArgs e)
        {
            NavigationHelper.Output("Starting Up.");

            NavigationHelper.SetStage(TestStage.Run);
            CmdLineArgs = e.Args;
            if (Dispatcher.CurrentDispatcher != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (OnReadyState), null);
            }
            else
            {
                NavigationHelper.Output("Dispatcher.CurrentDispatcher was null");
                ThreadPool.QueueUserWorkItem (new WaitCallback (OnTestStartTime));
            }

            base.OnStartup (e);
        }

        protected string[] CmdLineArgs
        {
            get
            {
                return _cmdlineargs;
            }
            set
            {
                _cmdlineargs = value;
            }
        }

        protected override void OnExit (ExitEventArgs e)
        {
            NavigationHelper.SetStage(TestStage.Cleanup);
            NavigationHelper.Output("Shutting Down App");
            base.OnExit (e);
        }

        #endregion

        private object OnReadyState (object args)
        {
            OnTestStartTime (args);
            return null;
        }

        private void OnTestStartTime (object obj)
        {
            //test start time can only fire once.
            if (OnTestReady != null)
            {
                // fire the event
                NavigationHelper.Output("Invoking all listeners to the OnTestReady event");
                OnTestReady ();
                OnTestReady = null;
            }
        }


        #region private
        private string _description;
        private string[] _cmdlineargs;
    #endregion

    }
#if testmode
    public class TestApp : BaseTestNavApp
    {
        public static void Main (string[] args)
        {
            TestApp _app = new TestApp ();

            _app.StartupUri = new Uri("test.xaml", UriKind.RelativeOrAbsolute);
            _app.OnTestReady += new TestStep (_app.P1);
            _app.OnTestReady += new TestStep (_app.P2);

            _app.Run();
        }

        public void P1 ()
        {
            NavigationHelper.Output("P1");
        }

        public void P2 ()
        {
            NavigationHelper.Output("P2");
        }
    }
#endif
}    
