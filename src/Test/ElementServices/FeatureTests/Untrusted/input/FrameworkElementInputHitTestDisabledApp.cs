// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify FrameworkElement InputHitTest works for disabled FrameworkElement in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for core input.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementInputHitTestDisabledApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\InputManager", "HwndSource", @"Compile and Verify FrameworkElement InputHitTest works for disabled FrameworkElement in HwndSource.")]
        [TestCase("2", @"CoreInput\InputManager", "Browser", @"Compile and Verify FrameworkElement InputHitTest works for disabled FrameworkElement in Browser.")]
        [TestCase("2", @"CoreInput\InputManager", "Window", @"Compile and Verify FrameworkElement InputHitTest works for disabled FrameworkElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementInputHitTestDisabledApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\InputManager", "HwndSource", @"Verify FrameworkElement InputHitTest works for disabled FrameworkElement in HwndSource.")]
        [TestCase("1", @"CoreInput\InputManager", "Window", @"Verify FrameworkElement InputHitTest works for disabled FrameworkElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementInputHitTestDisabledApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct tree for this window.
            Canvas cvs = new Canvas();
            cvs.Name = "Root";
            _rootElement = cvs;

            DisplayMe(_rootElement, 10, 10, 100, 100);

            // Construct test element 
            _panel = new InstrFrameworkPanel[] { new InstrFrameworkPanel(), new InstrFrameworkPanel() };

            // first element (first hit)
            Canvas.SetTop(_panel[0], 0);
            Canvas.SetLeft(_panel[0], 0);
            _panel[0].Height = 50;
            _panel[0].Width = 50;
            _panel[0].Name = "Child1";

            // second element (second hit, slight overlay)
            Canvas.SetTop(_panel[1], 25);
            Canvas.SetLeft(_panel[1], 25);
            _panel[1].Height = 50;
            _panel[1].Width = 50;
            _panel[1].Name = "Child2";

            cvs.Children.Add(_panel[0]);
            cvs.Children.Add(_panel[1]);

            CoreLogger.LogStatus("Setting enabledness....");
            _panel[0].IsEnabled = false;
            _panel[1].IsEnabled = false;
            _rootElement.IsEnabled = false;

            CoreLogger.LogStatus("root size: "+_rootElement.RenderSize.ToString());


            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("child1 size: " + _panel[0].RenderSize.ToString());
            CoreLogger.LogStatus("child2 size: " + _panel[1].RenderSize.ToString());

            Point ptNotOver = new Point(360, -360);

            CoreLogger.LogStatus("Getting invalid input hit element.... (all elements disabled) ");
            _invalidHitEl = _rootElement.InputHitTest(ptNotOver);
            ShowInputHitTestResult(_invalidHitEl, "false");

            Point ptOverBoth = new Point(38, 38);

            CoreLogger.LogStatus("Getting valid input hit element over both....  (all elements disabled) ");
            _hitElDisabledRoot = _rootElement.InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElDisabledRoot, "false");

            CoreLogger.LogStatus("Enabling root...");
            _rootElement.IsEnabled = true;

            CoreLogger.LogStatus("Getting valid input hit element over both....  (root element enabled) ");
            _hitElRoot = _rootElement.InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElRoot, "Root");
            _hitElRootFromPanel = _panel[0].InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElRootFromPanel, "false");

            CoreLogger.LogStatus("Enabling first child element...");
            _panel[0].IsEnabled = true;
            _panel[1].IsEnabled = false;

            CoreLogger.LogStatus("Getting valid input hit element over both....  (root and one child element enabled) ");
            _hitElChild1Both = _rootElement.InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElChild1Both, "Root");
            _hitElChild1BothFromPanel = _panel[0].InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElChild1BothFromPanel, "Child1");
            _hitElChild1BothFromPanel2 = _panel[1].InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElChild1BothFromPanel2, "false");

            Point ptOverChild1Only = new Point(12, 12);
            Point ptOverChild2Only = new Point(62, 62);

            CoreLogger.LogStatus("Getting valid input hit element over one child only....  (root and one child element enabled) ");
            _hitElChild1Only = _rootElement.InputHitTest(ptOverChild1Only);
            ShowInputHitTestResult(_hitElChild1Only, "Child1");
            _hitElChild2Only = _rootElement.InputHitTest(ptOverChild2Only);
            ShowInputHitTestResult(_hitElChild2Only, "Root");

            CoreLogger.LogStatus("Enabling other child element...");
            _panel[0].IsEnabled = false;
            _panel[1].IsEnabled = true;

            CoreLogger.LogStatus("Getting valid input hit element over one child only....  (root and other child element enabled) ");
            _hitElChild1OnlyPanel2Enabled = _rootElement.InputHitTest(ptOverChild1Only);
            ShowInputHitTestResult(_hitElChild1OnlyPanel2Enabled, "Root");
            _hitElChild2OnlyPanel2Enabled = _rootElement.InputHitTest(ptOverChild2Only);
            ShowInputHitTestResult(_hitElChild2OnlyPanel2Enabled, "Child2");

            CoreLogger.LogStatus("Enabling both child elements...");
            _panel[0].IsEnabled = true;
            _panel[1].IsEnabled = true;

            CoreLogger.LogStatus("Getting valid input hit element over both....  (root and both child elements enabled) ");
            // This should return some element.
            _hitElChild2 = _rootElement.InputHitTest(ptOverBoth);
            ShowInputHitTestResult(_hitElChild2, "Child2");

            base.DoExecute(arg);
            return null;
        }

        private void ShowInputHitTestResult(IInputElement e, string expectedResult)
        {
            CoreLogger.LogStatus(" Valid hit element? (expect " + expectedResult + ") " + (e != null));
            if (e != null)
            {
                CoreLogger.LogStatus("  Hit element: " + e.GetType().ToString());
                if (e is FrameworkElement)
                {
                    CoreLogger.LogStatus("     Name: " + ((FrameworkElement)e).Name);
                }
            }
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            bool actual = (_invalidHitEl == null) &&
                          (_hitElDisabledRoot == null) &&

                          (_hitElRoot == _rootElement) &&
                          (_hitElRootFromPanel == null) &&

                          (_hitElChild1Both == _rootElement) &&
                          (_hitElChild1BothFromPanel == _panel[0]) &&
                          (_hitElChild1BothFromPanel2 == null) &&
                          (_hitElChild1OnlyPanel2Enabled == _rootElement) &&
                          (_hitElChild2OnlyPanel2Enabled == _panel[1]) &&
                          (_hitElChild2 == _panel[1])
                          ;
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private InstrFrameworkPanel[] _panel;

        /// <summary>
        /// Stores results of InputHitTest.
        /// </summary>
        private IInputElement _hitElDisabledRoot = null;
        private IInputElement _invalidHitEl = null;
        private IInputElement _hitElChild1Both = null;
        private IInputElement _hitElChild1BothFromPanel = null;
        private IInputElement _hitElChild1BothFromPanel2 = null;
        private IInputElement _hitElChild1Only = null;
        private IInputElement _hitElChild2Only = null;
        private IInputElement _hitElChild1OnlyPanel2Enabled = null;
        private IInputElement _hitElChild2OnlyPanel2Enabled = null;
        private IInputElement _hitElChild2 = null;
        private IInputElement _hitElRoot = null;
        private IInputElement _hitElRootFromPanel = null;
    }
}
