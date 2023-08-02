// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Security;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          TestDispatcherSecurityExceptions
    ******************************************************************************/
    /// <summary>
    /// Testing Security Exception that is supposed to be thrown be Dispatcher.
    /// </summary>
    [Test(0, "Threading", TestCaseSecurityLevel.PartialTrust, "TestDispatcherSecurityExceptions", Disabled=true)]
    public class TestDispatcherSecurityExceptions : AvalonTest
    {

        #region Constructor
        /******************************************************************************
        * Function:          TestDispatcherSecurityExceptions Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public TestDispatcherSecurityExceptions()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        ///<summary>
        /// Worker Thread calling Complete on an operation and waiting to be dispatched
        ///</summary>
        TestResult StartTest()
        {
            // Code for Hooks
            
            bool isExceptionThrown = false;
            try
            {
                GetHooks();
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.Hooks didn't throw an exception.");
            }

            // Code for InvokeShutdown

            isExceptionThrown = false;
            try
            {
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.InvokeShutdown didn't throw an exception.");
            }

            // Code for BeginInvokeShutdown
            
            isExceptionThrown = false;
            try
            {
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.BeginInvokeShutdown didn't throw an exception.");
            }

            // Code for UnhandledExceptionFilter
            
            isExceptionThrown = false;
            try
            {
                GetExceptionFilter(true);
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.UnhandledExceptionFilter didn't throw an exception.");
            }

            // Code for UnhandledExceptionFilter

            isExceptionThrown = false;
            try
            {
                GetExceptionFilter(false);
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.UnhandledExceptionFilter didn't throw an exception.");
            }          


            // Code for PushFrame

            isExceptionThrown = false;
            try
            {
                PushFrame();
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.PushFrame didn't throw an exception.");
            }  

            // Code for Run

            isExceptionThrown = false;
            try
            {
                PushFrameOnDispatcher();
            }
            catch(SecurityException)
            {
                isExceptionThrown = true;
            }

            if (!isExceptionThrown)
            {
                localLogTestResult(false, "Dispatcher.Run didn't throw an exception.");
            }  

            //Any test failures will be caught by throwing an Exception during verification.
            if (_passed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion


        #region Private Members

        /******************************************************************************
        * Function:          Prevent test from attempting to log > 1 result.
        ******************************************************************************/
        private bool _passed = true;
        private void localLogTestResult(bool result, string message)
        {
            _passed &= result;
            CoreLogger.LogStatus(message);
        }

        /******************************************************************************
        * Function:          Run
        ******************************************************************************/
        private void PushFrameOnDispatcher()
        {
            Dispatcher.PushFrame(new DispatcherFrame());
        }

        /******************************************************************************
        * Function:          PushFrame
        ******************************************************************************/
        private void PushFrame()
        {
            Dispatcher.Run();
        }

        /******************************************************************************
        * Function:          GetHooks
        ******************************************************************************/
        private void GetHooks()
        {
            DispatcherHooks hooks = Dispatcher.CurrentDispatcher.Hooks;
        }

        /******************************************************************************
        * Function:          GetExceptionFilter
        ******************************************************************************/
        private void GetExceptionFilter(bool addFilter)
        {
            if (addFilter)
            {
                Dispatcher.CurrentDispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(FilterEventHandler);
            }
            else
            {
                Dispatcher.CurrentDispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(FilterEventHandler);
            }                
        }

        /******************************************************************************
        * Function:          FilterEventHandler
        ******************************************************************************/
        private void FilterEventHandler(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {

        }
        #endregion
    }

}


