// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*                                                                                   *
*  Description:                                                                     *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*************************************************************************************
*/

#define use_tools
#undef throw_on_error
#undef stop_on_error
#undef createbaselineimage
#define createfailureimage
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Globalization;
using System.Windows.Resources;
using System.Reflection;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;



/****************************************************************\
*                                                                *
* Main.cs                                                        *
*                                                                *
* Main Entry point to App, and a bunch of helpers, specifically  *
* the Bag class.                                                 *
*                                                                *
*                                                                *
*                                                                *
\****************************************************************/

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class LaunchPageFunctions
    {
        public static void Run (string[] args)
        {
            try
            {
                if (args.Length < 1 || args == null)
                {
                    NavigationHelper.Fail ("Wrong Usage.");
                    Usage ();
                    return;
                }

                int i;

                for (i = 0; i < args.Length; ++i)
                {
                    if (args[i].IndexOf ("/test:") == 0)
                    {
                        break;
                    }
                }

                if (i >= args.Length)
                {
                    NavigationHelper.Fail ("Wrong Usage.");
                    Usage ();
                    return;
                }

                PageFunctionTestApp app = new PageFunctionTestApp ();
                app.TestSelector = args;

                app.Run();
            }
            catch (Exception err)
            {
                NavigationHelper.Fail(String.Format ("Exception: {0}\nMessage: {1}\nStack:\n{2}", err.GetType ().ToString (), err.Message, err.StackTrace));
                throw;
            }
        }

        public static void Usage ()
        {
            NavigationHelper.Output ("Usage:");
            NavigationHelper.Output ("  CodePageFunctionTestApp /test:<testname> <optional args>");
        }
    }

    public partial class PageFunctionTestApp : BaseTestNavApp
    {

        protected int stage;
        private static PageFunctionTestApp s_currentPageFunctionTestApp = null;

        //hashtable to keep track of test events.
        private Hashtable _testEventsHT;
        public Hashtable TestEventsHT
        {
            set
            {
                _testEventsHT = value;
            }
            get
            {
                return _testEventsHT;
            }
        }

        private string [] _testselector;
        public string [] TestSelector
        {
            set
            {
                _testselector  = value;
            }
            get
            {
                return _testselector;
            }
        }


        public static Application CurrentApp
        {
            get
            {
                return Application.Current;
            }
        }

        public static PageFunctionTestApp CurrentPFTestApp
        {
            get
            {
                return s_currentPageFunctionTestApp;
            }
        }

        public PageFunctionTestApp ()
        {
            NavigationHelper.Output ("Creating PageFunction test app");
            stage = 0;
            TestEventsHT = new Hashtable (5);
            if (s_currentPageFunctionTestApp == null)
                s_currentPageFunctionTestApp = this;
            else
                throw new InvalidOperationException("Tried to create multiple PageFunctionTestApps");
        }

        public void Run()
        {
            int i;

            // DETERMINE the args in both cases: standalone or browser
            if (TestSelector == null)
            {
                NavigationHelper.Fail("Incorrect params passed to test. Giving Up.");
                LaunchPageFunctions.Usage();
            }
            
            if (TestSelector.Length == 0)
            {
                // this happens if you are running in the browser,
                // or if incorrect params were passed to the app
                NavigationHelper.Output("Test is not specified. Checking to see if running in browser.");
            }

            // Find the TEST to run
            for (i = 0; i < TestSelector.Length; ++i)
            {
                if (TestSelector[i].IndexOf ("/test:") == 0)
                {
                    break;
                }
            }

            if (i >= TestSelector.Length)
            {
                NavigationHelper.Fail ("Wrong Usage.");
                LaunchPageFunctions.Usage ();
                return;
            }
            // End of finding test to run

            NavigationHelper.Output ("Test is: " + TestSelector);
            // NavigationHelper.Output ("Test is: " + args[i]);

            switch (TestSelector[i].Substring(6).ToLower(CultureInfo.InvariantCulture))
            {
                case "basicpftest":
                    StandardSetup ();
                    BasicPFTestSetup ();
                    break;

                case "childpftest" :
                    StandardSetup ();
                    ChildPFDisplayTest ();
                    break;

                case "getsetchildproperty":
                    StandardSetup ();
                    ChildGetSetTestPF ();
                    break;

                case "getsetchildpropertymultiple":
                    StandardSetup ();
                    Test_ChildMultipleGetSetPF ();
                    break;

                case "parentstateonchildnavigation":
                    StandardSetup ();
                    Test_ParentStateOnChildNavigation ();
                    break;

                case "keepalive":
                    StandardSetup ();
                    Test_KeepAlive ();
                    break;

                case "removefromjournaltest":
                case "removefromjournaltestfalse":
                    StandardSetup ();
                    Test_RemoveFromJournalTestFalse ();
                    break;

                case "removefromjournaltesttrue":
                    StandardSetup ();
                    Test_RemoveFromJournalTestTrue ();
                    break;

                case "backtest":
                    StandardSetup ();
                    Test_BackTest ();
                    break;

                case "forwardtest":
                case "fwdtest":
                    StandardSetup ();
                    Test_FwdTest ();
                    break;

                case "multiback":
                case "multifwd":
                case "navtofinishedpf":
                    StandardSetup();
                    Test_BackFwdTestFinishedChild();
                    break;


                case "markuppfbasic":
                    //StandardSetup ();
                    Test_MarkupPF ();
                    break;

                case "startmethodbasic":
                    StandardSetup ();
                    Test_StartMethodBasic ();
                    break;

                case "startmethodchildfinish":
                    StandardSetup();
                    Test_StartMethodChildFinish ();
                    break;

                case "startmethodhistorynavback":
                    StandardSetup ();
                    Test_StartMethodHistoryNavBack ();
                    break;

                case "startmethodhistorynavfwd":
                    StandardSetup ();
                    Test_StartMethodHistoryNavFwd ();
                    break;

                case "uilessbasic":
                    StandardSetup ();
                    Test_UILessBasic ();
                    break;

                case "uilessjournal":
                    StandardSetup ();
                    Test_UILessJournalFocus();
                    break;

                // due to brk change, the following need to be changed
                //case "ijnldata.save":
                //    StandardSetup();
                //    VerifySaveCalledTest();
                //    break;
                //
                //case "ijnldata.load":
                //    //does not work
                //    break;
                // end due to brk change, the following need to be changed

                case "retval":
                    TestDiffPFTypes();
                    break;

                case "multiremove":
                    break;

                case "multifinish":
                    // 
                    break;

                default:
                    NavigationHelper.Fail ("Unknown test.");
                    break;

            }
//            base.OnStartup (e);
        }

        private void StandardSetup ()
        {
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                NavigationHelper.Output("Creating a main navigationwindow...");
                NavigationWindow nw = new NavigationWindow();
                nw.Show();
            }
        }
    }

}
