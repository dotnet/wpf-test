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
using System.Windows.Navigation;
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
    /// Verify Keyboard.FocusedElement property is set properly after changing content.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <




    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyboardFocusedPageNavigateApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing content in HwndSource.")]
        [TestCase("1", @"CoreInput\Focus", "Browser", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing content in Browser.")]
        [TestCase("3", @"CoreInput\Focus", "Window", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing content in window.")]
        [TestCase("2", @"CoreInput\Focus", "NavigationWindow", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing content in NavigationWindow.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "KeyboardFocusedPageNavigateApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", @"Verify Keyboard.FocusedElement property is set properly after changing content in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Verify Keyboard.FocusedElement property is set properly after changing content in window.")]
        [TestCase("1", @"CoreInput\Focus", "NavigationWindow", @"Verify Keyboard.FocusedElement property is set properly after changing content in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyboardFocusedPageNavigateApp(), "Run");
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
            cvs.Name = "rootCanvas";

            CoreLogger.LogStatus("Adding element to our main canvas....");
            FrameworkElement panel = new InstrFrameworkPanel();
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;
            panel.Focusable = true;
            cvs.Children.Add(panel);

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
            CoreLogger.LogStatus("Children in canvas: (expect 1) " + cvs.Children.Count);
            Assert(cvs.Children.Count == 1, "Oh no! Incorrect number of elements in tree");

            // STEP 1
            CoreLogger.LogStatus("Focusing on element outside frame...");
            _removedEl = cvs.Children[0];
            bool bFocus = _removedEl.Focus();
            CoreLogger.LogStatus("Focus element succeeded? " + bFocus);

            Assert(Keyboard.FocusedElement == _removedEl, "Item not in focus! (expected framed element) ");

            // STEP 3
            CoreLogger.LogStatus("Changing content to different tree...");
            //_frame.Content = null;
            UIElement newCanvas = ConstructTreeWithFocusableElement(true);
            NavigateTo(newCanvas);
            CoreLogger.LogStatus("Content changed.  rootElement content=" + _rootElement.GetType().ToString());

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Construct simple content control with one child element that may or may not be focusable.
        /// </summary>
        /// <param name="isFocusable">Is this child focusable?</param>
        /// <returns>New control.</returns>
        private UIElement ConstructTreeWithFocusableElement(bool isFocusable)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;

            Button panel = new Button();
            panel.Height = 40;
            panel.Width = 40;
            panel.Focusable = isFocusable;
            sp.Children.Add(panel);

            return sp;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            CoreLogger.LogStatus("Element in focus post-removal: (expect something besides frame element) '" + Keyboard.FocusedElement + "'");
            Assert(Keyboard.FocusedElement != _removedEl, "Unexpected element in focus! (expected null)");

            // Log final test results
            this.TestPassed = true;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private UIElement _removedEl; 
    }
}

