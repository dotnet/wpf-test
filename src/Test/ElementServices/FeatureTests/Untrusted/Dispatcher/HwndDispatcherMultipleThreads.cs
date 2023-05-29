// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.Framework.Dispatchers
{
    ///<summary>
    /// This test was never ported from (or implemented in?) v1.
    ///</summary>
    ///<remarks>
    ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
    ///</remarks>


    [Test(0, "Dispatcher.MultipleThreads", "HwndDispatcherMultipleThreads", Disabled=true)]
    public class HwndDispatcherMultipleThreads : TestCase
    {
        #region Private Data
        private static Thread s_mainThread;
        private static HwndDispatcher s_dispatcher;
        private AutoResetEvent _ev = new AutoResetEvent(false);
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("QuitFromDifferentThread")]
        [Variation("AbortingThread")]

        /******************************************************************************
        * Function:          HwndDispatcherMultipleThreads Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndDispatcherMultipleThreads(string arg) :base(TestCaseType.None)
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
                case "QuitFromDifferentThread":
                    QuitFromDifferentThread();
                    break;
                case "AbortingThread":
                    AbortingThread();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /// <summary>
        /// Quiting the dispatcher from different thread.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///  </ol>
        ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
        /// </remarks>
        public void QuitFromDifferentThread()
        {
            // DISABLING THIS DUE 

            
            s_dispatcher = new HwndDispatcher();

            CoreLogger.LogStatus("Creating new Thread");
            ThreadStart start = new ThreadStart(workThread);
            Thread thread = new Thread(start);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();
            s_dispatcher.Run();

            CoreLogger.LogStatus("Test Case pass becuase the Dispatcher was stopped on a diff thread");

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }

        /// <summary>
        /// Aborting the dispatcher on a different thread
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///  </ol>
        ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
        /// </remarks>
        public void AbortingThread()
        {
            // DISABLING THIS DUE 
            
            ThreadStart start = new ThreadStart(dispatcherThread);
            Thread thread = new Thread(start);

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            _ev.WaitOne();

            Thread.Sleep(1000);
            CoreLogger.LogStatus("Calling Thread.Abort on the dispatcher Thread");
            thread.Abort();

            int count = 0;
            

            while (thread.ThreadState != ThreadState.Aborted && count < 4)
            {
                Thread.Sleep(500);
                
            }

            if (thread.ThreadState != ThreadState.Aborted)
                throw new Microsoft.Test.TestValidationException("Thread was not aborted");
        }
        #endregion


        #region Private Members
        private void workThread()
        {
            Thread.Sleep(1000);
            CoreLogger.LogStatus("Calling Quit on the Dispatcher from a different Thread.");
            s_dispatcher.Quit();
            
        }

        private void dispatcherThread()
        {
            s_dispatcher = new HwndDispatcher();
            CoreLogger.LogStatus("Creating new Thread");
            s_mainThread = Thread.CurrentThread;
            _ev.Set();
            s_dispatcher.Run();
        }
        #endregion
    }
}





