// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.Framework.Dispatchers
{
    /******************************************************************************
    * CLASS:          BasicInvoking
    ******************************************************************************/
    /// <summary>
    /// BVT-level tests for new BeginInvoke/Invoke overloads introduced in .Net3.5 SP1.
    /// Verification consists of:
    ///     a. Checking whether or not the Deleate was invoked.
    ///     b. Checking that an object is correctly passed to the delegate.
    ///     c. Checking that BeginInvoke / Invoke return the correct value.  (In most cases,
    ///        this is set to be an object that was passed in to the delegate.)
    /// Note that some of these tests run in .Net3.5 SP1 only (specified in the Test attribute,
    /// and some of those require System.Windows.Presentation.dll.
    /// </summary>
    [Test(0, "Dispatcher.UnitTests", TestCaseSecurityLevel.FullTrust, "BasicInvoking")]
    public class BasicInvoking : WindowTest
    {
        #region Private Data
        private static DispatcherSignalHelper s_signalHelper;
        private bool                _delegateInvoked         = false;
        private DispatcherOperation _dispatcherOperation     = null;
        private object              _actualDispatcherOp      = null;
        private object              _expectedDispatcherOp    = null;
        private TextBox             _textBox                 = new TextBox();
        private object              _obj1                    = "x";
        private object              _obj2                    = "y";
        private delegate void       VoidDelegate();
        private System.Action       _actionCallback;
        private string              _testName                = "";
        #endregion


        #region Constructor
        [Variation("BeginInvoke1")]
        [Variation("BeginInvoke2")]
        [Variation("BeginInvoke3")]
        [Variation("BeginInvoke4", Versions="AH")]
        [Variation("BeginInvoke5", Versions="AH")]
        [Variation("BeginInvoke6", Versions="AH")]
        [Variation("BeginInvoke7", Versions="AH")]
        [Variation("Invoke1")]
        [Variation("Invoke2")]
        [Variation("Invoke3")]
        [Variation("Invoke4")]
        [Variation("Invoke5")]
        [Variation("Invoke6")]
        [Variation("Invoke7", Versions="AH")]
        [Variation("Invoke8", Versions="AH")]
        [Variation("Invoke9", Versions="AH")]
        [Variation("Invoke10", Versions="AH")]
        [Variation("Invoke11", Versions="AH")]
        [Variation("Invoke12", Versions="AH")]
        [Variation("Invoke13", Versions="AH")]
        [Variation("Invoke14", Versions="AH")]

        /******************************************************************************
        * Function:          BasicInvoking Constructor
        ******************************************************************************/
        public BasicInvoking(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
            RunSteps += new TestStep(FinishTest);
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
                case "BeginInvoke1":
                    BeginInvoke1();
                    break;
                case "BeginInvoke2":
                    BeginInvoke2();
                    break;
                case "BeginInvoke3":
                    BeginInvoke3();
                    break;
                case "BeginInvoke4":
                    BeginInvoke4();
                    break;
                case "BeginInvoke5":
                    BeginInvoke5();
                    break;
                case "BeginInvoke6":
                    BeginInvoke6();
                    break;
                case "BeginInvoke7":
                    BeginInvoke7();
                    break;
                case "Invoke1":
                    Invoke1();
                    break;
                case "Invoke2":
                    Invoke2();
                    break;
                case "Invoke3":
                    Invoke3();
                    break;
                case "Invoke4":
                    Invoke4();
                    break;
                case "Invoke5":
                    Invoke5();
                    break;
                case "Invoke6":
                    Invoke6();
                    break;
                case "Invoke7":
                    Invoke7();
                    break;
                case "Invoke8":
                    Invoke8();
                    break;
                case "Invoke9":
                    Invoke9();
                    break;
                case "Invoke10":
                    Invoke10();
                    break;
                case "Invoke11":
                    Invoke11();
                    break;
                case "Invoke12":
                    Invoke12();
                    break;
                case "Invoke13":
                    Invoke13();
                    break;
                case "Invoke14":
                    Invoke14();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          BeginInvoke1
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #1:  BeginInvoke(DispatcherPriority,Delegate).
        /// </summary>
        public void BeginInvoke1()
        {
            SetupBeginInvokeTest();
            _expectedDispatcherOp = null;  //In this case, the delegate does not return an object.

            _dispatcherOperation = _textBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new VoidDelegate(delegate()
                {
                    _delegateInvoked = true;
                    s_signalHelper.Signal("Finished", TestResult.Ignore);
                }));
        }

        /******************************************************************************
        * Function:          BeginInvoke2
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #2:  BeginInvoke(DispatcherPriority,Delegate,Object).
        /// </summary>
        public void BeginInvoke2()
        {
            SetupBeginInvokeTest();
            _expectedDispatcherOp = _obj1;

            _dispatcherOperation = _textBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object o1)
                {
                    _delegateInvoked = true;
                    s_signalHelper.Signal("Finished", TestResult.Ignore);
                    return o1;
                },
                _obj1);
        }

        /******************************************************************************
        * Function:          BeginInvoke3
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #3:  BeginInvoke(DispatcherPriority,Delegate,Object,Object[]).
        /// </summary>
        public void BeginInvoke3()
        {
            SetupBeginInvokeTest();
            _expectedDispatcherOp = _obj1;

            BeginInvokeCallback invokeCallback = new BeginInvokeCallback(BeginInvokeHandler);

            _dispatcherOperation =  _textBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, invokeCallback, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          BeginInvoke4
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #4:  BeginInvoke(Delegate,Object[]).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void BeginInvoke4()
        {
            SetupBeginInvokeTest();
            _expectedDispatcherOp = _obj1;

            BeginInvokeCallback invokeCallback = new BeginInvokeCallback(BeginInvokeHandler);

            _dispatcherOperation = _textBox.Dispatcher.BeginInvoke(invokeCallback, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          BeginInvoke5
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #5:  BeginInvoke(Delegate,DispatcherPriority,Object[]).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void BeginInvoke5()
        {
            SetupBeginInvokeTest();
            _expectedDispatcherOp = _obj1;

            BeginInvokeCallback invokeCallback = new BeginInvokeCallback(BeginInvokeHandler);

            _dispatcherOperation = _textBox.Dispatcher.BeginInvoke(invokeCallback, DispatcherPriority.Normal, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          BeginInvoke6
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #6:  BeginInvoke(Dispatcher,Action).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void BeginInvoke6()
        {
            SetupBeginInvokeTest();
            SetupActionDelegate("BeginInvoke");
            _expectedDispatcherOp = null;  //In this case, the delegate does not return an object.

            _dispatcherOperation = DispatcherExtensions.BeginInvoke(_textBox.Dispatcher, _actionCallback);
        }

        /******************************************************************************
        * Function:          BeginInvoke7
        ******************************************************************************/
        /// <summary>
        /// BeginInvoke #6:  BeginInvoke(Dispatcher,Action,DispatcherPriority).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void BeginInvoke7()
        {
            SetupBeginInvokeTest();
            SetupActionDelegate("BeginInvoke");
            _expectedDispatcherOp = null;  //In this case, the delegate does not return an object.

            _dispatcherOperation = DispatcherExtensions.BeginInvoke(_textBox.Dispatcher, _actionCallback,DispatcherPriority.Normal);
        }

        /******************************************************************************
        * Function:          Invoke1
        ******************************************************************************/
        /// <summary>
        /// Invoke #1:  Invoke(DispatcherPriority,Delegate).
        /// </summary>
        public void Invoke1()
        {
            _expectedDispatcherOp = null;

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(DispatcherPriority.Normal,
                new VoidDelegate(delegate()
                {
                    _delegateInvoked = true;
                }));
        }

        /******************************************************************************
        * Function:          Invoke2
        ******************************************************************************/
        /// <summary>
        /// Invoke #2:  Invoke(DispatcherPriority,Delegate,Object).
        /// </summary>
        public void Invoke2()
        {
            _expectedDispatcherOp = _obj1;

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(DispatcherPriority.Normal,
                (DispatcherOperationCallback)delegate(object o1)
                {
                    _delegateInvoked = true;
                    return o1;
                }
                ,_obj1);
        }


        /******************************************************************************
        * Function:          Invoke3
        ******************************************************************************/
        /// <summary>
        /// Invoke #3:  Invoke(DispatcherPriority,TimeSpan,Delegate).
        /// </summary>
        public void Invoke3()
        {
            _expectedDispatcherOp = null;

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(DispatcherPriority.Normal,
                new TimeSpan(0,0,1)
                ,new VoidDelegate(delegate()
                {
                    _delegateInvoked = true;
                }));
        }

        /******************************************************************************
        * Function:          Invoke4
        ******************************************************************************/
        /// <summary>
        /// Invoke #4:  Invoke(DispatcherPriority,Delegate,Object,Object[]).
        /// </summary>
        public void Invoke4()
        {
            _expectedDispatcherOp = _obj1;

            InvokeCallback invokeCallback = new InvokeCallback(InvokeHandler);

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(DispatcherPriority.Normal , invokeCallback, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          Invoke5
        ******************************************************************************/
        /// <summary>
        /// Invoke #5:  Invoke(DispatcherPriority,TimeSpan,Delegate,Object).
        /// </summary>
        public void Invoke5()
        {
            _expectedDispatcherOp = _obj1;

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(DispatcherPriority.Normal,
                new TimeSpan(0,0,1),
                (DispatcherOperationCallback)delegate(object o1)
                {
                    _delegateInvoked = true;
                    return o1;
                },
                _obj1);
        }

        /******************************************************************************
        * Function:          Invoke6
        ******************************************************************************/
        /// <summary>
        /// Invoke #6:  Invoke(DispatcherPriority,TimeSpan,Delegate,Object,Object[]).
        /// </summary>
        public void Invoke6()
        {
            _expectedDispatcherOp = _obj1;

            InvokeCallback invokeCallback = new InvokeCallback(InvokeHandler);

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(DispatcherPriority.Normal ,new TimeSpan(0,0,1), invokeCallback, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          Invoke7
        ******************************************************************************/
        /// <summary>
        /// Invoke #7:  Invoke(Delegate,Object[]).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke7()
        {
            _expectedDispatcherOp = _obj1;

            InvokeCallback invokeCallback = new InvokeCallback(InvokeHandler);

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(invokeCallback, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          Invoke8
        ******************************************************************************/
        /// <summary>
        /// Invoke #8:  Invoke(Delegate,TimeSpan,Object[]).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke8()
        {
            _expectedDispatcherOp = _obj1;

            InvokeCallback invokeCallback = new InvokeCallback(InvokeHandler);

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(invokeCallback, new TimeSpan(0,0,1),_obj1, _obj2);
        }

        /******************************************************************************
        * Function:          Invoke9
        ******************************************************************************/
        /// <summary>
        /// Invoke #9:  Invoke(Delegate,DispatcherPriority,Object[]).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke9()
        {
            _expectedDispatcherOp = _obj1;

            InvokeCallback invokeCallback = new InvokeCallback(InvokeHandler);

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(invokeCallback, DispatcherPriority.Normal, _obj1, _obj2);
        }

        /******************************************************************************
        * Function:          Invoke10
        ******************************************************************************/
        /// <summary>
        /// Invoke #10:  Invoke(Delegate,TimeSpan,DispatcherPriority,Object[]).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke10()
        {
            _expectedDispatcherOp = _obj1;

            InvokeCallback invokeCallback = new InvokeCallback(InvokeHandler);

            _actualDispatcherOp = _textBox.Dispatcher.Invoke(invokeCallback, new TimeSpan(0,0,1), DispatcherPriority.Normal, _obj1, _obj2);
        }


        /******************************************************************************
        * Function:          Invoke11
        ******************************************************************************/
        /// <summary>
        /// Invoke #11:  Invoke(Dispatcher,Action).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke11()
        {
            SetupActionDelegate("Invoke");
            _expectedDispatcherOp = null;  //In this case, the delegate returns void.

            DispatcherExtensions.Invoke(_textBox.Dispatcher, _actionCallback);
        }

        /******************************************************************************
        * Function:          Invoke12
        ******************************************************************************/
        /// <summary>
        /// Invoke #11:  Invoke(Dispatcher,Action,TimeSpan).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke12()
        {
            SetupActionDelegate("Invoke");
            _expectedDispatcherOp = null;  //In this case, the delegate returns void.

            DispatcherExtensions.Invoke(_textBox.Dispatcher, _actionCallback, new TimeSpan(0,0,1));
        }

        /******************************************************************************
        * Function:          Invoke13
        ******************************************************************************/
        /// <summary>
        /// Invoke #11:  Invoke(Dispatcher,Action,DispatcherPriority).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke13()
        {
            SetupActionDelegate("Invoke");
            _expectedDispatcherOp = null;  //In this case, the delegate returns void.

            DispatcherExtensions.Invoke(_textBox.Dispatcher, _actionCallback, DispatcherPriority.Normal);
        }

        /******************************************************************************
        * Function:          Invoke14
        ******************************************************************************/
        /// <summary>
        /// Invoke #11:  Invoke(Dispatcher,Action,TimeSpan,DispatcherPriority).  This is in .Net3.5 SP1 only.
        /// </summary>
        public void Invoke14()
        {
            SetupActionDelegate("Invoke");
            _expectedDispatcherOp = null;  //In this case, the delegate returns void.

            DispatcherExtensions.Invoke(_textBox.Dispatcher, _actionCallback, new TimeSpan(0,0,1), DispatcherPriority.Normal);
        }

        /******************************************************************************
        * Function:          BeginInvokeCallback
        ******************************************************************************/
        /// <summary>
        /// Delegate used by some of the BeginInvoke test cases.
        /// </summary>
        public delegate object BeginInvokeCallback(object obj1, object obj2);
        public object BeginInvokeHandler(object obj1, object obj2)
        {
            _delegateInvoked = true;
            s_signalHelper.Signal("Finished", TestResult.Ignore);
            return obj1;
        }

        /******************************************************************************
        * Function:          InvokeCallback
        ******************************************************************************/
        /// <summary>
        /// Delegate used by some of the Invoke test cases.
        /// </summary>
        public delegate object InvokeCallback(object obj1, object obj2);
        public object InvokeHandler(object obj1, object obj2)
        {
            _delegateInvoked = true;
            return obj1;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          SetupBeginInvokeTest
        ******************************************************************************/
        /// <summary>
        /// SetupBeginInvokeTest: A DispatcherSignalHelper is created to control execution of the asynchronous BeginInvoke tests.
        /// </summary>
        private void SetupBeginInvokeTest()
        {
            s_signalHelper = new DispatcherSignalHelper();
        }

        /******************************************************************************
        * Function:          SetupActionDelegate
        ******************************************************************************/
        /// <summary>
        /// SetupActionDelegate: Create a delegate for use in DispatcherExtensions scenarios.
        /// </summary>
        private void SetupActionDelegate(string method)
        {
            _actionCallback = delegate()
            {
                _delegateInvoked = true;

                if (method == "BeginInvoke")
                {
                    s_signalHelper.Signal("Finished", TestResult.Ignore);
                }
            };
        }

        /******************************************************************************
        * Function:          FinishTest
        ******************************************************************************/
        /// <summary>
        /// FinishTest: Pass or fail the test case.
        /// </summary>
        TestResult FinishTest()
        {
            //If a BeginInvoke is being tested, wait for the asynchronous delegate to be invoked before continuing.
            if (s_signalHelper != null)
            {
                s_signalHelper.WaitForSignal("Finished");

                _actualDispatcherOp = _dispatcherOperation.Result;
            }

            GlobalLog.LogEvidence("--------------- RESULTS ---------------");
            GlobalLog.LogEvidence("Actual DispatcherOperation:      " + _actualDispatcherOp);
            GlobalLog.LogEvidence("Expected DispatcherOperation:    " + _expectedDispatcherOp);
            GlobalLog.LogEvidence("Delegate invoked:                " + _delegateInvoked);

            if (_delegateInvoked && (_actualDispatcherOp == _expectedDispatcherOp))
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
