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
using System.Windows.Markup;
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
    /// Verify UIElement InputHitTest works for content-host element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for core input.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementInputHitTestContentHostApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\InputManager", "HwndSource", @"Compile and Verify UIElement InputHitTest works for content-host element in window in HwndSource.")]
        [TestCase("2", @"CoreInput\InputManager", "Browser", @"Compile and Verify UIElement InputHitTest works for content-host element in window in Browser.")]
        [TestCase("3", @"CoreInput\InputManager", "Window", @"Compile and Verify UIElement InputHitTest works for content-host element in window in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementInputHitTestContentHostApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\InputManager", "HwndSource", @"Verify UIElement InputHitTest works for content-host element in window in HwndSource.")]
        [TestCase("2", @"CoreInput\InputManager", "Window", @"Verify UIElement InputHitTest works for content-host element in window in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementInputHitTestContentHostApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct test element
            _host = new InstrContentPanelHost();

            // Layout test element
            _host.Height = 90.00;
            _host.Width = 90.00;
            Canvas.SetLeft(_host, 1.00);
            Canvas.SetTop(_host, 1.00);

            // Construct child element
            ContentElement contentEl = new InstrContentPanel("rootLeaf", "Sample", _host);
            _host.AddChild(contentEl);

            // Add everything to the visual tree
            Canvas cvs = new Canvas();
            cvs.Children.Add(_host);
            _rootElement = cvs;

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 90, 90);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Getting input hit element....");

            MouseHelper.Move(_rootElement);
            Point p = Mouse.GetPosition(_rootElement);
            CoreLogger.LogStatus("Found point: "+p);
            Point validPoint = p;
            Point invalidPoint = new Point(p.X+100, p.Y+100);

            // This should not return any element.
            _hitEl = ((UIElement)_host).InputHitTest(validPoint);
            // This should not return any element.
            _invalidHitEl = ((UIElement)_host).InputHitTest(invalidPoint);

            // This should return our content element.
            _hitContentEl = ((IContentHost)_host).InputHitTest(validPoint);
            // This should still return our content element. 
           
            _invalidHitContentEl = ((IContentHost)_host).InputHitTest(invalidPoint);

            base.DoExecute(arg);
            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need the API to return the element we expected.
            // NOTE: This element should be a content element!

            CoreLogger.LogStatus("Invalid,valid hit host elements? (should be no,yes) " + (_invalidHitEl != null) + "," + (_hitEl != null));
            if (_hitEl != null)
            {
                CoreLogger.LogStatus(" Hit element: " + _hitEl.GetType().ToString());
            }

            CoreLogger.LogStatus("Invalid,valid hit content elements? (should be yes,yes) " + (_invalidHitContentEl != null) + "," + (_hitContentEl != null));
            if (_hitContentEl != null)
            {
                CoreLogger.LogStatus(" Hit element: " + _hitContentEl.GetType().ToString());
            }

            bool actual = (_invalidHitEl == null) &&
                          (_hitEl != null) &&
                          (_hitEl.GetType() == typeof(InstrContentPanel)) &&
                          (_invalidHitContentEl != null) &&
                          (_invalidHitContentEl.GetType() == typeof(InstrContentPanel)) &&
                          (_hitContentEl != null) &&
                          (_hitContentEl.GetType() == typeof(InstrContentPanel));
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Stores results of InputHitTest.
        /// </summary>
        private IInputElement _hitEl = null;
        private IInputElement _invalidHitEl = null;
        private IInputElement _hitContentEl = null;
        private IInputElement _invalidHitContentEl = null;

        private InstrContentPanelHost _host = null;
    }
}
