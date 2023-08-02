// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.CoreInput
{
    /******************************************************************************
    * CLASS:          UIElementInputHitTestVisibleFalseApp
    ******************************************************************************/
    /// <summary>
    /// Verify UIElement InputHitTest works for element with IsHitTestVisible turned off.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for core input.
    /// </description>
    /// <author>Microsoft</author>
 
    [Test(0, "Input.InputManager", TestCaseSecurityLevel.FullTrust, "UIElementInputHitTestVisibleFalseApp")]
    public class UIElementInputHitTestVisibleFalseApp : TestApp
    {
        #region Private Data
        private IInputElement _hitEl = null;  //Stores results of InputHitTest.
        private string _hostTypeStr = "";
        #endregion


        #region Constructor

        [Variation("HwndSource")]
        [Variation("Window")]
        [Variation("NavigationWindow")]
        [Variation("WindowsFormSource")]

        /******************************************************************************
        * Function:          UIElementInputHitTestVisibleFalseApp Constructor
        ******************************************************************************/
        public UIElementInputHitTestVisibleFalseApp(string arg)
        {
            GlobalLog.LogStatus("In UIElementInputHitTestVisibleFalseApp constructor");
            _hostTypeStr = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), _hostTypeStr);

            ExeStubContainerCore exe = new ExeStubContainerCore();
            TestApp app = new UIElementInputHitTestVisibleFalseApp(_hostTypeStr);
            exe.Run(app, "RunTestApp");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          DoSetup
        ******************************************************************************/
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct test element, add event handling
            InstrPanel p = new InstrPanel();

            // Put the test element on the screen
            DisplayMe(p, 10, 10, 100, 100);

            return null;
        }

        /******************************************************************************
        * Function:          DoExecute
        ******************************************************************************/
        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Getting input hit element....");
            _hitEl = _rootElement;

            CoreLogger.LogStatus("Element hit test visible? (expect yes) " + _rootElement.IsHitTestVisible);
            this.Assert(_rootElement.IsHitTestVisible, "Whoops, element is not hit test visible!");

            CoreLogger.LogStatus("Turning off input hit visibility....");
            _rootElement.IsHitTestVisible = false;


            CoreLogger.LogStatus("Getting input hit element again....");
            MouseHelper.Move(_rootElement);
            Point p = Mouse.GetPosition(_rootElement);
            CoreLogger.LogStatus("Found point: "+p);

            // This should return no element.
            _hitEl = _rootElement.InputHitTest(p);

            base.DoExecute(arg);
            return null;
        }

        /******************************************************************************
        * Function:          DoValidate
        ******************************************************************************/
        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.BeginVariation(_hostTypeStr);
            CoreLogger.LogStatus("Validating...");

            // For this test we need the API to return the element we expected.
            CoreLogger.LogStatus("valid hit elements? (expect no) " + (_hitEl != null));
            if (_hitEl != null)
            {
                CoreLogger.LogStatus(" Hit element: " + _hitEl.GetType().ToString());
            }

            bool actual = (_hitEl == null);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");
            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }
        #endregion
    }
}
