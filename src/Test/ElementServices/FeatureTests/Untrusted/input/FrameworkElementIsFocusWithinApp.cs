// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Threading;
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
    /// Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    public class FrameworkElementIsFocusWithinApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "FrameworkElementIsFocusWithinApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementIsFocusWithinApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing window....");


            // Construct test element with focusability
            Canvas cvs = new Canvas();
            cvs.Focusable = true;

            _panel = new FrameworkElement[] { new InstrFrameworkPanel(), new InstrFrameworkPanel() };
            foreach (FrameworkElement p in _panel)
            {
                // It's necessary to enable each framework element to receive focus.
                CoreLogger.LogStatus("Panel focusable? " + p.Focusable + ". Turning it on...");
                p.Focusable = true;
            }

            // first element (source) - we set focus here
            Canvas.SetTop(_panel[0], 0);
            Canvas.SetLeft(_panel[0], 0);
            _panel[0].Height = 80;
            _panel[0].Width = 40;
            _panel[0].PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButtonDown);

            // second element (target) - we leave focus alone
            Canvas.SetTop(_panel[1], 50);
            Canvas.SetLeft(_panel[1], 50);
            _panel[1].Height = 10;
            _panel[1].Width = 10;

            // Put the test element on the screen
            cvs.Children.Add(_panel[0]);
            cvs.Children.Add(_panel[1]);
            // Construct related Win32 window
            DisplayMe(cvs,0, 0, 100, 100);

            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            //
            // Position mouse for test.
            //
            MouseHelper.Click(_panel[0]);



            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            // STEP 0
            CoreLogger.LogStatus("Clearing capture....");
            bool bCaptured = Mouse.Capture(null);
            CoreLogger.LogStatus(string.Format("[Capture remains after Capture(null): Expect=false; Actual={0}]", bCaptured));

            // STEP 1
            CoreLogger.LogStatus("Saving startup focus values (on startup)....");
            Debug.Assert(_rootElement != null, "root element is not null");

            bool bRootElementFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus(string.Format("[Root element focused on startup: Expect=false; Actual={0}]", bRootElementFocused));

            bool bRootElementFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bool bFocusedChildFocusWithin = _panel[0].IsKeyboardFocusWithin;
            bool bNonfocusedChildFocusWithin = _panel[1].IsKeyboardFocusWithin;

            CoreLogger.LogStatus(
                String.Format("[Focus within parent: Expect=true; Actual={0}], [Focus within focusedchild: Expect=true Actual={1}], [Focus within nonfocusedchild: Expect=false Actual={2}] (on startup).",
                bRootElementFocusWithin,
                bFocusedChildFocusWithin,
                bNonfocusedChildFocusWithin)
            );

            _bWasStartupFocusedCorrectly = (!bRootElementFocused) && (bRootElementFocusWithin) && (bFocusedChildFocusWithin) && (!bNonfocusedChildFocusWithin);

            // STEP 2
            CoreLogger.LogStatus("Focusing the root test element....");
            _rootElement.Focus();

            // STEP 3
            CoreLogger.LogStatus("Saving root element's focus values....");

            bRootElementFocused = _rootElement.IsKeyboardFocused;
            
            // If _rootElement is the root visual (only in an HwndSource) than it was made a FocusScope and should not recieve focus.
            Visual visualRoot = null;
            PresentationSource source = PresentationSource.FromVisual(_rootElement);
            if (source != null)
            {
                visualRoot = source.RootVisual;
            }

            if (visualRoot != null && _rootElement == visualRoot)
            {
                CoreLogger.LogStatus("Root test element is the window's root visual - element should be a focus scope.");
                CoreLogger.LogStatus(string.Format("[Root test element focused: Expect=false; Actual={0}]", bRootElementFocused));
            }
            else
            {
                CoreLogger.LogStatus("Root test element is not the window's root visual - element should not be a focus scope.");
                CoreLogger.LogStatus(string.Format("[Root test element focused: Expect=true; Actual={0}]", bRootElementFocused));
            }

            bRootElementFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bFocusedChildFocusWithin = _panel[0].IsKeyboardFocusWithin;
            bNonfocusedChildFocusWithin = _panel[1].IsKeyboardFocusWithin;


            if (visualRoot != null && _rootElement == visualRoot)
            {
                CoreLogger.LogStatus("Focus should forward from root to child.");
                CoreLogger.LogStatus(
                    string.Format("[Focus within root: Expect=false Actual={0}],[Focus within focusedchild: Expect=true Actual={1}],[Focus within nonfocusedchild: Expect=false Actual={2}]",
                    bRootElementFocusWithin,
                    bFocusedChildFocusWithin,
                    bNonfocusedChildFocusWithin)
                );

                _bWasParentFocusedCorrectly = (!bRootElementFocused) && (bRootElementFocusWithin) && (bFocusedChildFocusWithin) && (!bNonfocusedChildFocusWithin);
            }
            else
            {
                CoreLogger.LogStatus(
                    string.Format("[Focus within root: Expect=true Actual={0}],[Focus within focusedchild: Expect=false Actual={1}],[Focus within nonfocusedchild: Expect=false Actual={2}]",
                    bRootElementFocusWithin,
                    bFocusedChildFocusWithin,
                    bNonfocusedChildFocusWithin)
                );

                _bWasParentFocusedCorrectly = (bRootElementFocused) && (bRootElementFocusWithin) && (!bFocusedChildFocusWithin) && (!bNonfocusedChildFocusWithin);
            }

            // STEP 4
            CoreLogger.LogStatus("Focusing on a panel....");
            _panel[0].Focus();

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

            // Note: for this test we need to make sure the root does not have focus
            // We also need to make sure that an element within it does have focus.

            bool bFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus(string.Format("[Root element focused: Expect=false; Actual={0}]", bFocused));

            bool bFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bool bFocusWithinFocusedChild = _panel[0].IsKeyboardFocusWithin;
            bool bFocusWithinNonFocusedChild = _panel[1].IsKeyboardFocusWithin;
            CoreLogger.LogStatus(
                String.Format("[Focus within parent: Expect=True Actual={0}],[Focus within focusedchild: Expect=True Actual={1}],[Focus within nonfocusedchild: Expect=False Actual={2}]",
                bFocusWithin,
                bFocusWithinFocusedChild,
                bFocusWithinNonFocusedChild)
            );

            bool expected = (!bFocused) && (bFocusWithin) && (bFocusWithinFocusedChild) && (!bFocusWithinNonFocusedChild) && (_bWasStartupFocusedCorrectly) && (_bWasParentFocusedCorrectly);
            bool actual = true;
            bool eventFound = (expected == actual);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Event handler to run when mouse is down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            // Focus canvas.
            bool bFocus = ((UIElement)sender).Focus();
            CoreLogger.LogStatus(string.Format("[First element focused: Expect=true; Actual={0}]", bFocus));

            e.Handled = true;
        }

        /// <summary>
        /// Store collection of panel elements on our canvas.
        /// </summary>
        private FrameworkElement[] _panel;

        /// <summary>
        /// Were FocusWithin properties set correctly at app startup?
        /// </summary>
        private bool _bWasStartupFocusedCorrectly = false;

        /// <summary>
        /// Were FocusWithin properties set correctly after focus set to parent?
        /// </summary>
        private bool _bWasParentFocusedCorrectly = false;

    }
}
