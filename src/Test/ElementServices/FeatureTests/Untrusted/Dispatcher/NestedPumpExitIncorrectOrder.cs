// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
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


namespace Avalon.Test.Framework.Dispatchers.Nested
{
    /******************************************************************************
    * CLASS:          NestedPumpExitIncorrectOrder
    ******************************************************************************/
    ///<summary>
    ///         Calling nested pump twice and tried to exit on different order
    ///</summary>
    ///<remarks>
    ///     <ol>Scenarios steps:
    ///         <li>Creating 1 context and Enter the context, mouse is on 0,0 coordinates</li>
    ///         <li>Push a frame Frame1</li>
    ///         <li>Push a Frame2</li>
    ///         <li>Exit Frame1</li>
    ///         <li>Exit Frame2</li>
    ///         <li>Validate they exit correctlyy</li>
    ///     </ol>
    ///     <filename>NestedPumpExitIncorrectOrder.cs</filename>
    ///</remarks>
    [Test(1, "Dispatcher.MultipleThreads", "NestedPumpExitIncorrectOrder")]
    [Test(1, "Dispatcher.MultipleThreads", TestCaseSecurityLevel.FullTrust, "NestedPumpExitIncorrectOrder", Timeout=180)]
    public class NestedPumpExitIncorrectOrder : TestCase
    {
        #region Constructor
        private DispatcherFrame _firstFrame = null,_secondFrame = null;
        private bool _firstFrameExit = false,_secondFrameExit = false;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          NestedPumpExitIncorrectOrder Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public NestedPumpExitIncorrectOrder() :base(TestCaseType.ContextEnteringSupport)
        {
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
            MainDispatcher = Dispatcher.CurrentDispatcher; 

            using(CoreLogger.AutoStatus("Posting first Nested pump"))
            {
                MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(FirstPumpNested), MainDispatcher);

            }
            using(CoreLogger.AutoStatus("Dispatcher.Run"))
            {
                Dispatcher.Run();
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private object FirstPumpNested(object argDispatcher)
        {
            Dispatcher dispatcher = argDispatcher as Dispatcher;

            using(CoreLogger.AutoStatus("Posting second Nested pump"))
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(SecondPumpNested), dispatcher);
            }

            _firstFrame = new DispatcherFrame();
            using(CoreLogger.AutoStatus("Push First Frame"))
            {
                Dispatcher.PushFrame(_firstFrame);
            }

            if (!this._firstFrameExit ||  !this._secondFrameExit)
            {
                throw new Microsoft.Test.TestValidationException("First Frame Exit InCorrectly");
            }
            dispatcher.InvokeShutdown();
            return null;
        }



        private object SecondPumpNested(object argDispatcher)
        {
            Dispatcher dispatcher = argDispatcher as Dispatcher;
            
            _secondFrame = new DispatcherFrame();
            using(CoreLogger.AutoStatus("Posting first Nested pump Exit"))
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFirstFrame), dispatcher);

            }
            using(CoreLogger.AutoStatus("Posting second Nested pump Exit"))
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitSecondFrame), dispatcher);
            }

            using(CoreLogger.AutoStatus("Second PushFrame..."))
            {
                Dispatcher.PushFrame(_secondFrame);
            }
            if (!this._firstFrameExit ||  !this._secondFrameExit || this._secondFrame.Continue)
            {
                throw new Microsoft.Test.TestValidationException("Second Frame Exit incorrectly");
            }
            return null;
        }

        private object ExitFirstFrame(object Dispatcher)
        {
            using(CoreLogger.AutoStatus("Exiting FirstDispatcher"))
            {
                _firstFrame.Continue = false;
                _firstFrameExit=true;
            }
            return null;
        }

        object ExitSecondFrame(object Dispatcher)
        {
            using(CoreLogger.AutoStatus("Exiting Second Dispatcher"))
            {

                _secondFrame.Continue = false;
                _secondFrameExit=true;

            }
            return null;
        }
        #endregion
    }
}


