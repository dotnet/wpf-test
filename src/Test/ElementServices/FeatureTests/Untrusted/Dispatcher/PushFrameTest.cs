// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.Framework.Dispatchers
{
    /******************************************************************************
    * CLASS:          PushFrameTest
    ******************************************************************************/
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
    ///</remarks>
    [Test(0, "Dispatcher.PushFrame", TestCaseSecurityLevel.FullTrust , "PushFrameTest")]
    public class PushFrameTest : TestCase
    {
        #region Constructor
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("PushFrame")]
        [Variation("QuitAndPushFrameQuitScenario")]

        /******************************************************************************
        * Function:          PushFrameTest Constructor
        ******************************************************************************/
        public PushFrameTest(string arg) :base(TestCaseType.None)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "PushFrame":
                    PushFrame();
                    break;
                case "QuitAndPushFrameQuitScenario":
                    QuitAndPushFrameQuitScenario();
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
        * Function:          PushFrame
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///  </ol>
        ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
        /// </remarks>
        public void PushFrame()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;  

            GlobalLog.LogStatus("Calling Dispatcher.Quit");

            MainDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

            DispatcherFrame frame = new DispatcherFrame();

            GlobalLog.LogStatus("Posting a validate item to the queues");            
            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback (validate), frame);
            

            GlobalLog.LogStatus("Calling Dispatcher.PushFrame");
            Dispatcher.PushFrame(frame);  

            GlobalLog.LogStatus("End Validation");
            if(IsTestCaseFail)
                throw new Microsoft.Test.TestValidationException("The dispatcher didn't execute the enqueued op");
            
        }

        /******************************************************************************
        * Function:          QuitAndPushFrameQuitScenario
        ******************************************************************************/
        /// <summary>
        /// Quiting the dispatcher from different thread.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///  </ol>
        ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
        /// </remarks>
        public void QuitAndPushFrameQuitScenario()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;  

            GlobalLog.LogStatus("Calling Dispatcher.Quit");

            MainDispatcher.InvokeShutdown();

            DispatcherFrame frame = new DispatcherFrame();
    
            Dispatcher secondDispatcher = Dispatcher.CurrentDispatcher;

            if (MainDispatcher != secondDispatcher)
                throw new Microsoft.Test.TestValidationException("Dispatchers should be the same");

            GlobalLog.LogStatus("Posting a validation item to the queues");
            if (null != secondDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback (validate), null))
            {
                 GlobalLog.LogStatus("The Dispatcher should be shutdown");
            }

            bool expectionThrown = false;
            try
            {
                Dispatcher.PushFrame(frame);  
            }
            catch(InvalidOperationException)
            {
                expectionThrown = true;
            }

            if (!expectionThrown)
            {
                 GlobalLog.LogStatus("Expecting an expection.");
            }
        }        
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          validate
        ******************************************************************************/
        private object validate(object o)
        {
            DispatcherFrame frame = o as DispatcherFrame;

            GlobalLog.LogStatus("The Posted item is dispatched");
            TestCaseFailed = false;
            
            if (frame != null)                
            {
                GlobalLog.LogStatus("Requesting Exit fromt the thread");
                frame.Continue = false;
            }
            return null;
        }
        #endregion
    }
}


