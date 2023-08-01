// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          TestDispatcherProcessingDisabled
    ******************************************************************************/
    /// <summary>
    /// Main Class that holds the Misc tests for Dispatcher Class
    /// </summary>
    [Test(1, "Threading.DisabledDispatcher", TestCaseSecurityLevel.FullTrust, "TestDispatcherProcessingDisabled")]
    public class TestDispatcherProcessingDisabled : AvalonTest
    {
        #region Data
        private static bool s_threadResultPassed = false;
        private static object s_globalSyncRoot = new object();
        private static List<DispatcherProcessingDisabled> s_disabledProList = new List<DispatcherProcessingDisabled>();
        private static List<Dispatcher> s_disabledDispatcherList = new List<Dispatcher>();
        private static ManualResetEvent s_ev = new ManualResetEvent(false);       
        private List<DispatcherProcessingDisabled> _dpd = new List<DispatcherProcessingDisabled>();
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("DisposeDisabledDispatcherTwice")]
        [Variation("DisposeDisabledDispatcherInUsing", Disabled=true)]
        [Variation("CallingDisabledProcessingFromDifferentThread")]

        /******************************************************************************
        * Function:          TestDispatcherProcessingDisabled Constructor
        ******************************************************************************/
        public TestDispatcherProcessingDisabled(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "DisposeDisabledDispatcherTwice":
                    DisposeDisabledDispatcherTwice();
                    break;
                case "DisposeDisabledDispatcherInUsing":
                    DisposeDisabledDispatcherInUsing();
                    break;
                case "CallingDisabledProcessingFromDifferentThread":
                    CallingDisabledProcessingFromDifferentThread();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          DisposeDisabledDispatcherTwice
        ******************************************************************************/
        ///<summary>
        /// Calling twice DisableProcessing.Dispose.
        ///</summary>
        public void DisposeDisabledDispatcherTwice()
        {
            DispatcherProcessingDisabled dpd = Dispatcher.CurrentDispatcher.DisableProcessing();
            dpd.Dispose();
            dpd.Dispose();

            if (DispatcherHelper.IsDispatcherDisabledProcessing())
            {
                CoreLogger.LogTestResult(false,"The ref count should be 0.");
            }
        }

        /******************************************************************************
        * Function:          DisposeDisabledDispatcherInUsing
        ******************************************************************************/
        ///<summary>
        /// Calling  DisableProcessing.Dispose between the using c# statement.
        ///</summary>
        public void DisposeDisabledDispatcherInUsing()
        {
            DispatcherProcessingDisabled dpd = Dispatcher.CurrentDispatcher.DisableProcessing();
            using (dpd)
            {
                dpd.Dispose();
            }

            if (DispatcherHelper.IsDispatcherDisabledProcessing())
            {
                CoreLogger.LogTestResult(false,"The ref count should be 0.");
            }                          
        }
/*
        void ValidateNoReenter()
        {
            if (ValidateExceptionThrown() != 2)
            {
                CoreLogger.LogTestResult(false,
                    "It was possible to reenter (PushFrame or Win32::SendMessage). It doesn't throw a InvalidOperationException.");
            }
        }

        void ValidateReenter()
        {
            if (ValidateExceptionThrown() != 0)
            {
                CoreLogger.LogTestResult(false,
                    "It was not possible to reenter (PushFrame or Win32::SendMessage). It do throw a InvalidOperationException.");
            }
        }


        int ValidateExceptionThrown()
        {
            int count = 0;
            
            try
            {
                DispatcherFrame frame = new DispatcherFrame();
                
                DispatcherHelper.EnterLoop(frame, 
                    delegate(object o, EventArgs args)
                    {
                        
                        DispatcherFrame f = (DispatcherFrame)o;
                        f.Continue = false;
                        
                    }, DispatcherPriority.Normal);
            }
            catch(InvalidOperationException)
            {
                count++;
            }

            try
            {
                DispatcherHelper.SendMessageToDispatcherWndProc(DispatcherHelper.VoidWin32Message,IntPtr.Zero,IntPtr.Zero);
            }
            catch(InvalidOperationException)
            {
                count++;
            }

            return count;
        }
*/

        /******************************************************************************
        * Function:          CallingDisabledProcessingFromDifferentThread
        ******************************************************************************/
        ///<summary>
        /// Validating all the methods on DispatcherProcessingDisabled using single and multiple dispatchers.
        ///</summary>
        public void CallingDisabledProcessingFromDifferentThread()
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            Thread workerThread = new Thread(
                delegate (object o)
                {
                    Dispatcher d = (Dispatcher)o;  
                    try
                    {
                        d.DisableProcessing();
                    }
                    catch(InvalidOperationException)
                    {
                        s_threadResultPassed = true;
                    }                  
                });

            workerThread.Start(dispatcher);
            workerThread.Join();

            if (!s_threadResultPassed)
            {
                CoreLogger.LogTestResult(false,"Calling Dispatcher.DisableProcessing from a different Thread doesn't throw a InvalidOperationException.");
            }

            s_threadResultPassed = false;

            DispatcherProcessingDisabled dpd = dispatcher.DisableProcessing();

            workerThread = new Thread(
                delegate (object o)
                {
                    DispatcherProcessingDisabled dpDisabled = (DispatcherProcessingDisabled)o;  
                    try
                    {
                        dpDisabled.Dispose();
                    }
                    catch(InvalidOperationException)
                    {
                        s_threadResultPassed = true;
                    }                  
                });
            
            workerThread.Start(dpd);
            workerThread.Join();

            if (!s_threadResultPassed)
            {
                CoreLogger.LogTestResult(false,"Calling DispatcherProcessingDisabled.Dispose from a different Thread doesn't throw a InvalidOperationException.");
            }
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          Validating
        ******************************************************************************/
        private void Validating(int i, int j, bool v1, bool v2)
        {

            bool equalValuePassed = v1; //false;
            if (s_disabledProList[i] == s_disabledProList[j])
            {
                equalValuePassed = v2; // true;
            }

            if (!equalValuePassed)
            {
                CoreLogger.LogTestResult(false,"The equal operator didn't work as expected.");
            }

            equalValuePassed = v1; //false;
            if (s_disabledProList[i].Equals(s_disabledProList[j]))
            {
                equalValuePassed = v2;// true;
            }

            if (!equalValuePassed)
            {
                CoreLogger.LogTestResult(false,"The Equals method didn't work as expected.");
            }

            bool notEqualValuePassed = v2; //true;
            if (s_disabledProList[i] != s_disabledProList[j])
            {
                notEqualValuePassed = v1; //false;
            }

            if (!notEqualValuePassed)
            {
                CoreLogger.LogTestResult(false,"The not equal operator didn't work as expected.");
            }   
        }
        #endregion         
    }
}

