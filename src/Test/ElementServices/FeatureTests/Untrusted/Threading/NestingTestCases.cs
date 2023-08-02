// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Win32;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          NestingTestCases
    ******************************************************************************/
    /// <summary>
    /// Main Class that holds the Misc tests for Dispatcher Class
    /// </summary>
    [Test(1, "Threading.Nesting", TestCaseSecurityLevel.PartialTrust, "NestingTestCases")]
    public class NestingTestCases : AvalonTest
    {
        #region Data
        private bool[] _countArray = {false,false,false};
        private static int s_flag = 0;
        private static object s_syncRootClass = new object();
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("SettingFrameExitFalse")]
        [Variation("DoEventsWithInvoke")]

        /******************************************************************************
        * Function:          NestingTestCases Constructor
        ******************************************************************************/
        public NestingTestCases(string arg)
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
                case "SettingFrameExitFalse":
                    SettingFrameExitFalse();
                    break;
                case "DoEventsWithInvoke":
                    DoEventsWithInvoke();
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
        * Function:          SettingFrameExitFalse
        ******************************************************************************/
        public void SettingFrameExitFalse ()
        {
            DispatcherFrame frame = new DispatcherFrame(false);
            
            DispatcherHelper.EnqueueBackgroundCallback(
                (DispatcherOperationCallback)delegate(object o)
                {
                    DispatcherFrame xframe = (DispatcherFrame)o;
                    DispatcherHelper.PushFrame(xframe);
                    return null;
                },frame);


            DispatcherHelper.EnqueueBackgroundCallback(
                (DispatcherOperationCallback)delegate(object o)
                {
                    DispatcherHelper.ShutDown();
                    return null;
                },frame);


            DispatcherHelper.EnqueueBackgroundCallback(
                (DispatcherOperationCallback)delegate(object o)
                {
                    _countArray[0] = true;
                    DispatcherFrame xframe = (DispatcherFrame)o;
                    xframe.Continue = false;
                    return null;
                },frame);

            DispatcherHelper.RunDispatcher();

            if (!_countArray[0])
            {
                GlobalLog.LogEvidence("FAIL: The message loop exists early.");
            }
        }

        /******************************************************************************
        * Function:          DoEventsWithInvoke
        ******************************************************************************/
        ///<summary>
        /// Calling DoEvents from a Invoke on a different thread.
        ///</summary>
        public void DoEventsWithInvoke()
        {
            WorkerThreadTestCase work = new WorkerThreadTestCase(true);
            work.Initialize(
                (ThreadTestCaseCallback) delegate(ThreadCaseInfo info, EventArgs args)
                {
                   
                    GlobalLog.LogStatus(Dispatcher.CurrentDispatcher.GetHashCode().ToString());
                    DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback(Handler1), null);
                },
                null,
                null,
                (ThreadTestCaseCallback) delegate(ThreadCaseInfo info, EventArgs args)
                {
                    WorkerThreadTestCase workerTestCase = (WorkerThreadTestCase)info.Owner;
                    
                    DispatcherHelper.InvokeBackground(workerTestCase.CurrentDispatcher,
                        (DispatcherOperationCallback)delegate(object o)
                        {
                            s_flag++;

                            GlobalLog.LogStatus("Entering DoEvents, Nested Pump");
                            DispatcherHelper.DoEvents(3000, DispatcherPriority.Normal);
                            GlobalLog.LogStatus("Leaving DoEvents, Nested Pump");
                            
                            DispatcherHelper.EnqueueBackgroundCallback(
                                new DispatcherOperationCallback(Handler1), null);

                            s_flag++;

                            return null;
                        },null);
                },
                null);

            // Starting dispatcher thread and worker thread.
            work.Run();
                            

            if (!_countArray[0])
            {
                GlobalLog.LogEvidence("FAIL: BeginInvoke before DoEvents failed");                
            }

            if (!_countArray[1])
            {
                GlobalLog.LogEvidence("FAIL: BeginInvoke during DoEvents failed");                
            }

            if (!_countArray[2])
            {
                GlobalLog.LogEvidence("FAIL: BeginInvoke after DoEvents failed");                
            }            
        }

        /******************************************************************************
        * Function:          Handler1
        ******************************************************************************/
        public object Handler1(object o)
        {

            if (s_flag == 0)
            {
                _countArray[0] = true;

                DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback(Handler1), null);
                return null;
            }

            if (s_flag == 1)
            {
                if (!_countArray[1])
                {
                    lock(s_syncRootClass)
                    {
                        s_flag++;
                    }
                }
                _countArray[1] = true;
                return null;
            }
            
            if (s_flag == 3)
            {
                _countArray[2] = true;
                DispatcherHelper.ShutDown();
                return null;
            }
            return null;
        }   
        #endregion
    }
}
   
