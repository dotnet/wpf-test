// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

    
namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          DispatcherMiscTest
    ******************************************************************************/
    /// <summary>
    /// Main Class that holds the Misc tests for Dispatcher Class
    /// </summary>
    [Test(0, "Threading", TestCaseSecurityLevel.FullTrust, "DispatcherMiscTest")]
    public class DispatcherMiscTest : TestCaseBase
    {
        #region Private Data
        private Delegate _del;
        private static AutoResetEvent s_ev = new AutoResetEvent(false);
        private static AutoResetEvent s_evFinal = new AutoResetEvent(false);        
        private static DispatcherOperation s_op = null;
        private static bool s_called = false;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("BeginInvokeMiddleofHwndMessageOnlyWindowDispose")]
        [Variation("ValidateAllDispatcherPriorities")]

        /******************************************************************************
        * Function:          DispatcherMiscTest Constructor
        ******************************************************************************/
        public DispatcherMiscTest(string arg)
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
                case "BeginInvokeMiddleofHwndMessageOnlyWindowDispose":
                    BeginInvokeMiddleofHwndMessageOnlyWindowDispose();
                    break;
                case "ValidateAllDispatcherPriorities":
                    ValidateAllDispatcherPriorities();
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
        * Function:          BeginInvokeMiddleofHwndMessageOnlyWindowDispose
        ******************************************************************************/
        ///<summary>
        /// Validate that if we are on the middle of disposing our internal HWND and another thread begininvoke doesnt crash.
        ///</summary>
        public void BeginInvokeMiddleofHwndMessageOnlyWindowDispose()
        {
            _del = DispatcherHelper.AddHookToDispatcherMessageOnlyWindow(typeof(DispatcherMiscTest).GetMethod("SleepMiddleDispose"));

            object[] args = {s_ev, Dispatcher.CurrentDispatcher, s_evFinal};

            // Making another thread to call BeginInvoke on the middle of disposing.

            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                object[] arg = (object[])o;

                AutoResetEvent ev = (AutoResetEvent)arg[0];
                ev.WaitOne();                
                
                s_op = ((Dispatcher)arg[1]).BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback) delegate
                    {
                        CoreLogger.LogTestResult(false, "This should not be called.");
                        return null;
                    }, null);

                AutoResetEvent evFinal = (AutoResetEvent)arg[2];
                evFinal.Set();               


            }, args);
            
            DispatcherHelper.ShutDown();

            if (s_op == null)
            {
                CoreLogger.LogTestResult(false, "The Operation should be enqueued.");                
            }
            else if (s_op.Status != DispatcherOperationStatus.Aborted)
            {
                CoreLogger.LogTestResult(false, "The Operation should be aborted.");                
            }
            if (_del == null)
            {
                CoreLogger.LogTestResult(false, "?");                
            }

            if (!s_called)
            {
                CoreLogger.LogTestResult(false, "The test didn't run properly. Missing the WM_DESTROY call.");                
            }
        }

        /******************************************************************************
        * Function:          ValidateAllDispatcherPriorities
        ******************************************************************************/
        ///<summary>
        /// Validates all the DispatcherPriorities against Dispatcher.ValidatePriority
        ///</summary>
        public void ValidateAllDispatcherPriorities()
        {
            DispatcherPriority[] priorities = Microsoft.Test.Threading.DispatcherPriorityHelper.GetAllDispatcherPriorities();

            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] == DispatcherPriority.Invalid)
                {
                    if (!IsInvalidEnumExceptionThrown(priorities[i]))
                    {
                        CoreLogger.LogTestResult(false,"Expecting an exception if you passed Invalid DispatcherPriority.");
                    }
                }
                else
                {
                    if (IsInvalidEnumExceptionThrown(priorities[i]))
                    {
                        CoreLogger.LogTestResult(false,"Not expecting an exception.");
                    }
                }
            }            
        }

        /******************************************************************************
        * Function:          SleepMiddleDispose
        ******************************************************************************/
        ///<summary>
        /// Validate correct arguments to the BeginInvoke and Invoke methods.
        ///</summary>
        static public IntPtr SleepMiddleDispose(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (NativeConstants.WM_DESTROY == msg)
            {
                s_ev.Set();
                CoreLogger.LogStatus("Win32  Message: " + msg.ToString());
                s_evFinal.WaitOne();
                CoreLogger.LogStatus("Win32 Message: " + msg.ToString());
                s_called = true;
            }

            handled = false;
            return IntPtr.Zero;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          IsInvalidEnumExceptionThrown
        ******************************************************************************/
        private bool IsInvalidEnumExceptionThrown(DispatcherPriority priority)
        {
            bool exceptionThrow = false;
            
            try
            {
                Dispatcher.ValidatePriority(priority,"bla");
            }
            catch(InvalidEnumArgumentException)
            {
                exceptionThrow = true;
            }

            return exceptionThrow;
        }
        #endregion
    }
}
