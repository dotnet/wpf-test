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
    /// Verify IFrameworkInputElement Focusable for a FrameworkContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkContentElementFocusableApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Focus","HwndSource", @"Compile and Verify IFrameworkInputElement Focusable for a FrameworkContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Focus","Browser", @"Compile and Verify IFrameworkInputElement Focusable for a FrameworkContentElement in Browser.")]
        [TestCase("2",@"CoreInput\Focus","Window", @"Compile and Verify IFrameworkInputElement Focusable for a FrameworkContentElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "FrameworkContentElementFocusableApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Focus","HwndSource",  @"Verify IFrameworkInputElement Focusable for a FrameworkContentElement in HwndSource.")]
        [TestCase("1",@"CoreInput\Focus","Window",  @"Verify IFrameworkInputElement Focusable for a FrameworkContentElement in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkContentElementFocusableApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing window....");


            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();
            host.Focusable = true;
            _frameworkContentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", host);
            _frameworkContentElement.Focusable = true;
            host.AddChild(_frameworkContentElement);
            _frameworkNonFocusableContentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", host);
            _frameworkNonFocusableContentElement.Focusable = false;
            host.AddChild(_frameworkNonFocusableContentElement);

            // Put the test element on the screen
            DisplayMe(host,0, 0, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            // STEP 1
            CoreLogger.LogStatus("Focusing on the parent....");
            _rootElement.Focus();

            // STEP 2
            CoreLogger.LogStatus("Saving parent focus values (on parent)....");
            bool bResult = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element focused? " + bResult);
            _bWasParentFocusedCorrectly = (bResult);

            // STEP 3
            CoreLogger.LogStatus("Focusing on a content element....");
            _frameworkContentElement.Focusable = true;
            bResult = _frameworkContentElement.Focus();
            CoreLogger.LogStatus("Content element focused? "+bResult);
            _bWasChildFocusedCorrectly = (bResult);

            // STEP 4
            CoreLogger.LogStatus("Focusing on a non-focusable content element....");
            bResult = _frameworkNonFocusableContentElement.Focus();
            CoreLogger.LogStatus("Non-focusable Content element focused? "+bResult);
            _bWasNonFocusableChildFocusedCorrectly = (!bResult);

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

            // Note: for this test we need to make sure focus is NOT set to non-focusable content element.

            bool expected = (_bWasChildFocusedCorrectly) && (_bWasParentFocusedCorrectly) && (_bWasNonFocusableChildFocusedCorrectly);
            bool actual = true;
            bool eventFound = (expected == actual);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store content element on our canvas.
        /// </summary>
        private FrameworkContentElement _frameworkContentElement;

        /// <summary>
        /// Store non-focusable content element on our canvas.
        /// </summary>
        private FrameworkContentElement _frameworkNonFocusableContentElement;

        /// <summary>
        /// Were Focusable properties set correctly after focus set to parent?
        /// </summary>
        private bool _bWasParentFocusedCorrectly = false;

        /// <summary>
        /// Were Focusable properties set correctly after focus set to child?
        /// </summary>
        private bool _bWasChildFocusedCorrectly = false;
        
        /// <summary>
        /// Were Focusable properties set correctly after focus set to non-focusable child?
        /// </summary>
        private bool _bWasNonFocusableChildFocusedCorrectly = false;
    }
}
