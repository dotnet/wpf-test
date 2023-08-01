// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;

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
    * CLASS:          DispatcherFromThreadTests
    ******************************************************************************/
    /// <summary>
    /// Main Class that holds the BVTS for Dispatcher Class
    /// </summary>

    // [DISABLE WHILE PORTING]
    // [Test(0, "Threading", TestCaseSecurityLevel.FullTrust, "DispatcherFromThreadTests")]
    public class DispatcherFromThreadTests : AvalonTest
    {
        #region Private Data
        private static bool s_executed = false;
        private static AppDomain s_domain;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          DispatcherFromThreadTests Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatcherFromThreadTests()
        {
            RunSteps += new TestStep(MultipleAppdomain);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          MultipleAppdomain
        ******************************************************************************/
        ///<summary>
        /// Nesting Dispatcher.FromThread calls into different AppDomain.
        ///</summary>
        TestResult MultipleAppdomain()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, 
                (DispatcherOperationCallback)delegate(object o)
            {
                // .NET Core 3.0 There is no concept of AppDomain isolation in .NET Core, changing this code to execute
                // without isolation. Verify that this code works as intended.
                s_domain = AppDomain.CreateDomain("TestDomain");
                s_domain.SetData("domain", AppDomain.CurrentDomain);
                // _domain.DoCallBack(delegate
                // {
                    Dispatcher di = Dispatcher.FromThread(Thread.CurrentThread);
                    if (di != null)
                    {
                        CoreLogger.LogTestResult(false,"The dispatcher should not pass the AppDomain boundary.");
                    }

                    di = Dispatcher.CurrentDispatcher;
                    if (di == null)
                    {
                        CoreLogger.LogTestResult(false,"The dispatcher was not create correctly.");
                    }

                    Dispatcher diTwo = Dispatcher.FromThread(Thread.CurrentThread);
                    if (di != diTwo)
                    {
                        CoreLogger.LogTestResult(false,"The dispatcher must be the same.");
                    }

                    DispatcherHelper.ShutDownBackground();
                    Dispatcher.Run();

                // });
                s_executed = true;
                return null;
            }, null);


            DispatcherHelper.ShutDownBackground();
            Dispatcher.Run();

            if (!s_executed)
            {
                CoreLogger.LogTestResult(false,"The test was not executed");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}



