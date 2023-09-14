// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          CommandManagerInvalidateStatusApp
    ******************************************************************************/
    /// <summary>
    /// Verify CommandManager StatusInvalidated works for element on InvalidateStatus call.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <remarks>
    /// This test app works via command-enabling a regular element during the setup phase.
    /// A more typical scenario is to just use a command-aware element (usually a control) instead of a regular element.
    /// </remarks>
    /// <author>Microsoft</author>
 
    [Test(0, "Commanding.CoreCommanding", TestCaseSecurityLevel.FullTrust, "CommandManagerInvalidateStatusApp")]
    public class CommandManagerInvalidateStatusApp : TestApp
    {
        #region Private Data
        private List<EventArgs> _queryLog = new List<EventArgs>();
        private EventHandler _requerySuggested; // Needed to prevent garbage collection
        private string _hostTypeStr = "";
        #endregion


        #region Constructor

        [Variation("HwndSource")]
        [Variation("Window")]

        /******************************************************************************
        * Function:          CommandManagerInvalidateStatusApp Constructor
        ******************************************************************************/
        public CommandManagerInvalidateStatusApp(string arg)
        {
            GlobalLog.LogStatus("In CommandManagerInvalidateStatusApp constructor");
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
            TestApp app = new CommandManagerInvalidateStatusApp(_hostTypeStr);
            exe.Run(app, "RunTestApp");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
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
            GlobalLog.LogStatus("Constructing root element....");
            InstrPanel panel = new InstrPanel();

            GlobalLog.LogStatus("Attaching input events...");
            panel.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.KeyDown += new KeyEventHandler(OnKey);
            panel.KeyUp += new KeyEventHandler(OnKey);
            panel.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            panel.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButton);

            // Put the test element on the screen
            GlobalLog.LogStatus("Showing window...");
            DisplayMe(panel, 10, 10, 100, 100);

            GlobalLog.LogStatus("Attaching CommandManager events...");
            _requerySuggested = new EventHandler(OnStatusInvalidated);
            CommandManager.RequerySuggested += _requerySuggested;

            return null;
        }

        /******************************************************************************
        * Function:          DoExecute
        ******************************************************************************/
        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender)
        {
            GlobalLog.LogStatus("Suggesting second requery...");
            CommandManager.InvalidateRequerySuggested();

            GlobalLog.LogStatus("Draining queue...");
            DispatcherHelper.DoEvents(DispatcherPriority.ApplicationIdle);
            GlobalLog.LogStatus("Queue drained. Getting ready to validate...");

            base.DoExecute(sender);
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
            GlobalLog.LogStatus("Validating...");

            GlobalLog.LogStatus("StatusInvalidated events (expect 1): " + _queryLog.Count);
            this.Assert(_queryLog.Count == 1, "Incorrect number of StatusInvalidated events");

            this.TestPassed = true;
            GlobalLog.LogStatus("Setting log result to " + this.TestPassed);

            GlobalLog.LogStatus("Validation complete!");
            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          OnStatusInvalidated
        ******************************************************************************/
        /// <summary>
        /// If we are in this handler, someone is suggesting a requery!
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the event.</param>
        private void OnStatusInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus(" [StatusInvalidated]");
            if (sender != null)
            {
                GlobalLog.LogStatus("  Status Invalidated sender Name: " + sender.ToString());
            }

            _queryLog.Add(e);
        }

        /******************************************************************************
        * Function:          OnFocus
        ******************************************************************************/
        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Log some debugging data
            GlobalLog.LogStatus(" [" + e.RoutedEvent.Name + "]");
            GlobalLog.LogStatus("   Hello focusing from: '" + e.OldFocus + "' to '" + e.NewFocus + "'");
        }

        /******************************************************************************
        * Function:          OnMouseButton
        ******************************************************************************/
        /// <summary>
        /// Standard mouse button event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            GlobalLog.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            GlobalLog.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            GlobalLog.LogStatus("   Btn=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);
        }

        /******************************************************************************
        * Function:          OnKey
        ******************************************************************************/
        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnKey(object sender, KeyEventArgs e)
        {
            // Log some debugging data
            GlobalLog.LogStatus(" [" + e.RoutedEvent.Name + "]");
            GlobalLog.LogStatus("   Hello from: '" + e.Key.ToString() + "', " + e.KeyStates.ToString());
        }
        #endregion
    }
}
