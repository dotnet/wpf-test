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
    * CLASS:          DispatcherBVTs
    ******************************************************************************/
    ///<summary>
    /// Main Class that holds the BVTS for Dispatcher Class
    ///</summary>
    [Test(0, "Threading.Dispatcher", TestCaseSecurityLevel.PartialTrust, "DispatcherBVTs")]
    public class DispatcherBVTs : TestCaseBase
    {
        #region Constructor
        /******************************************************************************
        * Function:          DispatcherBVTs Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatcherBVTs()
        {
            RunSteps += new TestStep(CurrentDispatcher_ScenarioOne);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// </summary>
        ///<summary>
        /// In a single thread application, the current thread needs to grab the dispatcher, calling Dispatcher.CurrentDispatcher API.
        ///</summary>
        TestResult CurrentDispatcher_ScenarioOne()
        {
            
            this.MainDispatcher = Dispatcher.CurrentDispatcher;

            if (this.MainDispatcher != Dispatcher.CurrentDispatcher)
                throw new Microsoft.Test.TestValidationException("The dispatcher doesn't match with the cached dispatcher");

            DispatcherHelper.EnqueueBackgroundCallback(
                this.MainDispatcher,
                new DispatcherOperationCallback(currentDispatcher_scenarioOne_Handler),
                null);                            

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          currentDispatcher_scenarioOne_Handler
        ******************************************************************************/
        private object currentDispatcher_scenarioOne_Handler(object o)
        {
            if (this.MainDispatcher != Dispatcher.CurrentDispatcher)
                throw new Microsoft.Test.TestValidationException("The dispatcher doesn't match with the cached dispatcher");            

            DispatcherHelper.ShutDown();
            
            return null;
        }
        #endregion
    }
}




