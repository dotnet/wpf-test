// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.CoreInput
{
    /******************************************************************************
    * CLASS:          UIElementIsHitTestVisibleApp
    ******************************************************************************/
    /// <summary>
    /// Verify IInputElement IsHitTestVisible set works for UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for core input.
    /// </description>
    /// <author>Microsoft</author>
 
    [Test(0, "Input.InputManager", TestCaseSecurityLevel.FullTrust, "UIElementIsHitTestVisibleApp")]
    public class UIElementIsHitTestVisibleApp : TestApp
    {
        #region Private Data
        private bool _bExceptionThrown = false;         //Stores results of exception.
        private ArrayList _eventLog = new ArrayList();  //Stores events raised.
        private string _hostTypeStr = "";
        #endregion


        #region Constructor

        [Variation("HwndSource")]
        [Variation("Window")]
        [Variation("NavigationWindow")]
        [Variation("WindowsFormSource")]

        /******************************************************************************
        * Function:          UIElementIsHitTestVisibleApp Constructor
        ******************************************************************************/
        public UIElementIsHitTestVisibleApp(string arg)
        {
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
            TestApp app = new UIElementIsHitTestVisibleApp(_hostTypeStr);
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
            CoreLogger.LogStatus("Constructing element....");
            UIElement p = new InstrPanel();
            p.IsHitTestVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsHitTestVisibleChange);

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
            this.Assert(_rootElement.IsHitTestVisible, "Element is not HitTestVisible (it should be)");

            CoreLogger.LogStatus("Setting IsHitTestVisibleness for bare element....");
            try
            {
                // This should succeed -- a bare UIElement now supports IsHitTestVisibleness.
                _rootElement.IsHitTestVisible = false;
                CoreLogger.LogStatus("IsHitTestVisible is set!");
            }
            catch (InvalidOperationException e)
            {
                // {"Not allowed to call the base implementation of IsHitTestVisible."}
                CoreLogger.LogStatus(".. expected exception:\n" + e.ToString());
                _bExceptionThrown = true;
            }

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
            
            // For this test we need the exception to not have been thrown 
            // We also expect the element to have been disabled (from automatic IsHitTestVisible).
            // We also expect 1 event to be raised.

            CoreLogger.LogStatus("Exception (expect none): " + _bExceptionThrown);
            CoreLogger.LogStatus("Event count  (expect 1): " + _eventLog.Count);

            bool bIsHitTestVisible = _rootElement.IsHitTestVisible;
            CoreLogger.LogStatus("IsHitTestVisible?   (expect not): " + bIsHitTestVisible);

            bool actual = (!_bExceptionThrown) && (_eventLog.Count == 1) && (!bIsHitTestVisible);
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


        #region Public and Protected Members
        /******************************************************************************
        * Function:          OnIsHitTestVisibleChange
        ******************************************************************************/
        /// <summary>
        /// Standard IsHitTestVisible event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnIsHitTestVisibleChange(object sender, DependencyPropertyChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [DependencyPropertyChanged: " + args.Property.Name + "]");
            CoreLogger.LogStatus("   Hello changing from: '" + args.OldValue.ToString() + "' to '" + args.NewValue.ToString() + "'");
        }
        #endregion
    }
}
