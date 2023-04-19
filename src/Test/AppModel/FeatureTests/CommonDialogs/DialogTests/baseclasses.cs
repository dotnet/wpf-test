// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test;
using Microsoft.Test.Logging;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec
{
    // Special wrapper to consolidate all the weird logging wrappers found in the CommonDialogs area.
    public class FrameworkLoggerWrapper
    {
        private Result _newInfraResult;
        private TestResult _oldInfraResult;
        private bool _createdNewVariationAlready = false;

        public FrameworkLoggerWrapper()
        {}

        // Can't log without creating a variation, but if the parent process doesn't create a variation to use here,
        // make a note of this for later Close() behavior via createdNewVariationAlready.
        private void EnsureVariationExists()
        {
            if ((Variation.Current == null) && !_createdNewVariationAlready)
            {
                _createdNewVariationAlready = true;
                Log.Current.CreateVariation("Variation Start");
            }
        }

        // Lets old tests set a result using old APIs but get translated at the same time...
        public Microsoft.Test.Logging.TestResult Result
        {
            set
            {
                TestLog.Current.Result = value;
                this._oldInfraResult = value;
                switch (this._oldInfraResult.ToString())
                {
                    case "Pass":
                        this._newInfraResult = Microsoft.Test.Result.Pass;
                        break;
                    case "Fail":
                        this._newInfraResult = Microsoft.Test.Result.Fail;
                        break;
                    case "Ignore":
                        this._newInfraResult = Microsoft.Test.Result.Ignore;
                        break;
                    default:
                        this._newInfraResult = Microsoft.Test.Result.Fail;
                        break;
                }
            }
            get
            {
                return this._oldInfraResult;
            }
        }

        public Microsoft.Test.Result NewResult
        {
            get
            {
                return this._newInfraResult;
            }
        }

        // Wrapp LogStatus/Evidence, and if there is no variation (Happens frequently in cleanup code)
        // Log it "Dangerously".  Equivalent to GlobalLog.Log*
        public void LogStatus(string message)
        {
            EnsureVariationExists();
            if (Variation.Current != null)
            {
                Variation.Current.LogMessage(message);
            }
            else
            {
                LogManager.LogMessageDangerously(message);
            }
        }
        public void LogEvidence(string message)
        {
            EnsureVariationExists();
            if (Variation.Current != null)
            {
                Variation.Current.LogMessage(message);
            }
            else
            {
                LogManager.LogMessageDangerously(message);
            }
        }

        // Close the log, either in the old or new fashion
        public void Close()
        {
            EnsureVariationExists();
            Variation.Current.LogMessage("Variation closing, Result = " + _newInfraResult.ToString());
            if ((_createdNewVariationAlready) && (Variation.Current != null))
            {
                Variation.Current.LogResult(_newInfraResult);
                Variation.Current.Close();
            }
            else
            {
                TestLog.Current.Result = OldInfraResultFromNew(_newInfraResult);
                TestLog.Current.Close();
            }
            
        }
        // Method to convert result enums back if we need to use the "vintage" logging system.
        private TestResult OldInfraResultFromNew(Result newResult)
        {
            switch (newResult.ToString())
            {
                default:
                case "Fail":
                    return TestResult.Fail;
                case "Pass":
                    return TestResult.Pass;
                case "Ignore":
                    return TestResult.Ignore;
            }
        }
    }

    public abstract class BaseTestNavApp : Application
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

        public NavigationWindow MainNavWindow
        {
            get
            {
                if (Windows != null && this.Windows.Count >= 1)
                {
                    return this.Windows[0] as NavigationWindow;
                }
                else
                {
                    return null;
                }
            }
        }

        public BaseTestNavApp ()
        {
            fw = new FrameworkLoggerWrapper();
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
                fw.LogEvidence ("Performing Verification action.");
                delVerify ();
            }
            else
            {
                fw.LogEvidence ("Verification delegate was null");
            }

            return null;
        }

        private object DoTestStep (object args)
        {
            TestStep _step = args as TestStep;

            if (_step != null)
            {
                fw.LogEvidence ("Performing Test Action.");
                _step ();
            }
            else
            {
                fw.LogEvidence ("Test step delegate was null");
            }

            return null;
        }

        public void PostVerificationItem (VerificationDelegate delVerify)
        {
            fw.LogEvidence ("Posting Verification Item");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (DoVerification), delVerify);
        }

        public void PostTestItem (TestStep delTest)
        {
            fw.LogEvidence ("Posting test action");
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
            fw.LogEvidence(sMsg);
            fw.Result = TestResult.Fail;
            if (forceshutdown)
            {
                this.Shutdown (-1);
            }
        }

        #region App Level overrides 
        protected override void OnStartup (StartupEventArgs e)
        {
            fw.LogEvidence ("\nStarting Up.");
            if (Description != null)
            {
                fw.LogEvidence (Description);
            }

            //fw.Stage = TestStage.Run;
            CmdLineArgs = e.Args;
            this.LoadCompleted += new LoadCompletedEventHandler (OnLoad);
            this.LoadCompleted += new LoadCompletedEventHandler (OnTestStartTime);
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

            fw.LogEvidence ("Shutting Down App");
            fw.Close();
            base.OnExit (e);
        }

        #endregion

        private void OnLoad (object sender, NavigationEventArgs e)
        {
            fw.LogEvidence ("LoadCompleted Event fired." + ((e.Uri != null) ? (" Uri is: " + e.Uri.ToString()) : ""));
        }

        private void OnTestStartTime (object sender, NavigationEventArgs e)
        {
            //test start time can only fire once.
            this.LoadCompleted -= new LoadCompletedEventHandler (OnTestStartTime);
            if (OnTestReady != null)
            {
                // fire the event
                fw.LogEvidence ("Invoking all listeners to the OnTestReady event");
                OnTestReady ();
                OnTestReady = null;
            }
        }


        #region protected members
        protected static FrameworkLoggerWrapper fw;
        #endregion

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
            fw = new FrameworkLoggerWrapper();
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
                fw.LogEvidence ("Performing Verification action.");
                delVerify ();
            }
            else
            {
                fw.LogEvidence ("Verification delegate was null");
            }

            return null;
        }

        private object DoTestStep (object args)
        {
            TestStep _step = args as TestStep;

            if (_step != null)
            {
                fw.LogEvidence ("Performing Test Action.");
                _step ();
            }
            else
            {
                fw.LogEvidence ("Test step delegate was null");
            }

            return null;
        }

        public void PostVerificationItem (VerificationDelegate delVerify)
        {
            fw.LogEvidence ("Posting Verification Item");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (DoVerification), delVerify);
        }

        public void PostTestItem (TestStep delTest)
        {
            fw.LogEvidence ("Posting test action");
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
            fw.LogEvidence(sMsg);
            fw.Result = TestResult.Fail;
            if (forceshutdown)
            {
                this.Shutdown (-1);
            }
        }

        #region App Level overrides 
        protected override void OnStartup (StartupEventArgs e)
        {
            fw.LogEvidence ("\nStarting Up.");
            if (Description != null)
            {
                fw.LogEvidence (Description);
            }

            //fw.Stage = TestStage.Run;
            CmdLineArgs = e.Args;
            if (Dispatcher.CurrentDispatcher != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (OnReadyState), null);
            }
            else
            {
                fw.LogEvidence ("Dispatcher.CurrentDispatcher was null");
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
            fw.LogEvidence ("Shutting Down App");
            fw.Close();
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
                fw.LogEvidence ("Invoking all listeners to the OnTestReady event");
                OnTestReady ();
                OnTestReady = null;
            }
        }


        #region protected members
        protected static FrameworkLoggerWrapper fw;
        #endregion

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
            Console.WriteLine ("P1");
        }

        public void P2 ()
        {
            Console.WriteLine ("P2");
        }
    }
#endif
}    
