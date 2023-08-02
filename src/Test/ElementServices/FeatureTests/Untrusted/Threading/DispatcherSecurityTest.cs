// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.Security;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test;


namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          DispatcherSecurityTests
    ******************************************************************************/
    [Test(0, "Threading.Security", TestCaseSecurityLevel.PartialTrust, "DispatcherSecurityTests")]
    public class DispatcherSecurityTests : AvalonTest
    {
        #region Constructor
        /******************************************************************************
        * Function:          DispatcherSecurityTests Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatcherSecurityTests()
        {
            RunSteps += new TestStep(ShutDownMitigateThreat);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          MultipleAppdomain
        ******************************************************************************/
        ///<summary>
        /// Nesting Dispatcher.FromThread calls into different AppDomain.
        ///</summary>
        TestResult ShutDownMitigateThreat()
        {
            bool exceptionThrown = false;

            GlobalLog.LogStatus("Testing BeginInvokeShutdown not allowed on PT");
            
            try
            {
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
            }
            catch(SecurityException)
            {
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                GlobalLog.LogEvidence("FAIL: BeginInvokeShutDown didn't throw a security exception");
            }

            GlobalLog.LogStatus("Testing InvokeShutdown not allowed on PT");
            exceptionThrown = false;
            
            try
            {
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
            catch(SecurityException)
            {
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                GlobalLog.LogEvidence("FAIL: InvokeShutDown didn't throw a security exception");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}

