// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
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
    * CLASS:          TestExecutionContextinAvalon
    ******************************************************************************/
    /// <summary>
    /// BVTs for testing Supressing the executioncontext in Avalon
    /// </summary>
    [Test(1, "Threading", TestCaseSecurityLevel.FullTrust, "TestExecutionContextinAvalon")]
    public class TestExecutionContextinAvalon : AvalonTest
    {
        #region Constructor
        /******************************************************************************
        * Function:          TestExecutionContextinAvalon Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public TestExecutionContextinAvalon()
        {
            RunSteps += new TestStep(SuppressExecutionContextFlow);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          SuppressExecutionContextFlow
        ******************************************************************************/
        /// <summary>
        /// Calling begininvoke on a suppress ExecutionContext environment.
        /// </summary>
        TestResult SuppressExecutionContextFlow()
        {
            CallbackResult callbackResult = null;

            using(ExecutionContext.SuppressFlow())
            {
                callbackResult = DispatcherHelper.EnqueueBackgroundCallback((DispatcherOperationCallback) delegate(object o)
                    {             
                        return 1;
                    }, null);            
            }

            DispatcherHelper.ShutDownBackground();

            Dispatcher.Run();                    

            if (callbackResult == null)
            {
                GlobalLog.LogEvidence("FAIL: The BeginInvoke failed.");
                return TestResult.Fail;
            }

            if (((int)callbackResult.Result) != 1)
            {
                GlobalLog.LogEvidence("FAIL: The BeginInvoke was not executed property.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }       
        #endregion


        #region Private Members
        private void SupressShutdown()
        {
            using (ExecutionContext.SuppressFlow())
            {
                DispatcherHelper.ShutDown();
            }
        }
        #endregion
    }
}

