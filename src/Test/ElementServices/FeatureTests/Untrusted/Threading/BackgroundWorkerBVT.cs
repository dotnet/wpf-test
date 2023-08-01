// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
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

    ///<summary>
    /// Using BackgroundWorker API with Avalon Dispatcher.
    ///</summary>
    [Test(0, "Threading", "BackgroundWorkerBVT")]
    public class BackgroundWorkerBVT : AvalonTest
    {
        #region Private Data
        private BackgroundWorker _b = null;
        private static int s_progressCount = 0;
        private static Dispatcher s_dispatcher = null;
        #endregion

        #region Constructor
        /******************************************************************************
        * Function:          BackgroundWorkerBVT Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundWorkerBVT()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        ///<summary>
        /// Validating the Progress and COmpleted Events on Background thread
        /// is executed on the correct thread.
        ///</summary>
        TestResult StartTest()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;
            DispatcherHelper.EnqueueBackgroundCallback(
                (DispatcherOperationCallback)delegate(object o)
                {
                    _b = new BackgroundWorker();
                    _b.DoWork += new DoWorkEventHandler(b_DoWork);
                    _b.WorkerReportsProgress = true;
                    _b.WorkerSupportsCancellation = false;
                    _b.RunWorkerAsync();
                    _b.RunWorkerCompleted += new RunWorkerCompletedEventHandler(b_RunWorkerCompleted);
                    _b.ProgressChanged += new ProgressChangedEventHandler(b_ProgressChanged);

                    return null;
                }, null);

            DispatcherHelper.RunDispatcher();            

            if (s_progressCount != 100)
            {
                CoreLogger.LogTestResult(false,"The progress count is not the expected.  Expected: 100  Actual: " + s_progressCount.ToString());
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private static void b_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CoreLogger.LogStatus("Progress Change called.");
            s_progressCount++;
            Validation(true, "Progress Callback");
        }

        private static void b_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CoreLogger.LogStatus("Completed called.");            
            Validation(true, "Completed Callback");
            DispatcherHelper.ShutDown();
        }

        private static void b_DoWork(object sender, DoWorkEventArgs e)
        {
            CoreLogger.LogStatus("DoWork called.");               
            Validation(false, "Working Callback");
            for (int i = 0; i<100; i++)
            {
                ((BackgroundWorker)sender).ReportProgress(i);                
            }
        }        

        private static void Validation(bool sameDispatcher, string msg)
        {
            bool b = false;
            
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            if (sameDispatcher && s_dispatcher != currentDispatcher)
            {
                b = true;
            }

            if (!sameDispatcher && s_dispatcher == currentDispatcher)
            {
                b = true;
            }

            if (b)
            {
                CoreLogger.LogTestResult(false,"The dispatcher doesn't match on the " + msg);
            }
        }
        #endregion
    }
}


