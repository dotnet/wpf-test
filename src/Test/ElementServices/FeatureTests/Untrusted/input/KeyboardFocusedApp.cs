// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
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
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Keyboard.FocusedElement property is set properly after various actions.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyboardFocusedApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Compile and Verify Keyboard.FocusedElement property is set properly after various actions in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", @"Compile and Verify Keyboard.FocusedElement property is set properly after various actions in Browser.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Compile and Verify Keyboard.FocusedElement property is set properly after various actions in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "KeyboardFocusedApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", @"Verify Keyboard.FocusedElement property is set properly after various actions in HwndSource.")]
        [TestCase("1", @"CoreInput\Focus", "Window", @"Verify Keyboard.FocusedElement property is set properly after various actions in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyboardFocusedApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing tree....");
            Canvas cvs = new Canvas();
            cvs.Focusable = true;

            FrameworkElement panel = new InstrFrameworkPanel();
            panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;
            panel.Focusable = true;

            FrameworkElement panel2 = new InstrFrameworkPanel();
            panel2.Name = "focusbtn" + DateTime.Now.Ticks;
            Canvas.SetTop(panel2, 50);
            Canvas.SetLeft(panel2, 50);
            panel2.Height = 40;
            panel2.Width = 40;
            panel2.Focusable = true;

            cvs.Children.Add(panel);
            cvs.Children.Add(panel2);

            _rootElement = cvs;

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Canvas cvs = (Canvas)(_rootElement);
            CoreLogger.LogStatus("Children in canvas: (expect 2) " + cvs.Children.Count);
            this.Assert(cvs.Children.Count == 2, "Oh no! Incorrect number of elements in tree");

            // STEP 0
            CoreLogger.LogStatus("Focusing on the parent....");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set? (expect true) " + bFocus);
            CoreLogger.LogStatus("Startup focused element: " + InputHelper.GetFocusedElement());

            // STEP 1
            CoreLogger.LogStatus("Focusing on all elements...");
            cvs.Children[0].Focus();
            cvs.Children[1].Focus();
            DispatcherHelper.DoEventsPastInput();

            CoreLogger.LogStatus("Element in focus pre-removal: (expect non-null el) '" + InputHelper.GetFocusedElement() + "'");
            this.Assert(InputHelper.GetFocusedElement() == cvs.Children[1], "Item not in focus!");

            // STEP 2
            CoreLogger.LogStatus("Removing element...");
            _removedEl = (UIElement)(cvs.Children[1]);
            cvs.Children.Remove(_removedEl);

            base.DoExecute(arg);
            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            Canvas cvs = (Canvas)(_rootElement);
            CoreLogger.LogStatus("Children in canvas: (expect 1) " + cvs.Children.Count);
            this.Assert(cvs.Children.Count == 1, "Unexpected element present!");

            CoreLogger.LogStatus("Element in focus post-removal: (expect something other than removed element) '" + InputHelper.GetFocusedElement() + "'");
            this.Assert(InputHelper.GetFocusedElement() != _removedEl, "Removed element still has focus!");

            CoreLogger.LogStatus("Focusing to null....");
            IInputElement e = Keyboard.Focus(null);
            DispatcherHelper.DoEventsPastInput();

            CoreLogger.LogStatus("Element focused? (expect same element) " + e);
            this.Assert(InputHelper.GetFocusedElement() == e, "Removed element still has focus!");

            // Log final test results
            this.TestPassed = true;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private UIElement _removedEl = null;
    }
}

