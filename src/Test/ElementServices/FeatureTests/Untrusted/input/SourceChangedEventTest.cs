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
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;

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
    /// Verify basic behavior of PresentationSource.SourceChanged event.
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class SourceChangedEventTest : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("ExeStub", "HwndSource", "0", @"CoreInput\SourceChangedEvent", TestCaseSecurityLevel.FullTrust, "Verify basic behavior of PresentationSource.SourceChanged event.")]
        [TestCaseContainerAttribute("ExeStub", "All,-HwndSource", "2", @"CoreInput\SourceChangedEvent", TestCaseSecurityLevel.FullTrust, "Verify basic behavior of PresentationSource.SourceChanged event.")]
        [TestCaseContainerAttribute("TestApplicationStub", "All", "2", @"CoreInput\SourceChangedEvent", TestCaseSecurityLevel.FullTrust, "Verify basic behavior of PresentationSource.SourceChanged event.")]
        public void LaunchTest()
        {
            Run();
        }   

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct child in the ether.
            _child = new StackPanel();
            _child.Background = Brushes.Green;

            // Construct and display intial tree.
            // Border (Red)
            //   - StackPanel (LightBlue)
            _parent = new StackPanel();
            _parent.Background = Brushes.LightBlue;

            _root = new Border();
            _root.BorderThickness = new Thickness(2);
            _root.BorderBrush = Brushes.Red;
            _root.Child = _parent;

            // Put the test element on the screen
            DisplayMe(_root, 10, 10, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute the test.
        /// </summary>
        protected override object DoExecute(object o)
        {
            PresentationSource source = Interop.PresentationSourceFromVisual(_parent);
            SourceChangedEventArgs eventArgs = null;

            PresentationSource.AddSourceChangedHandler(
                _child,
                (SourceChangedEventHandler)delegate(object sender, SourceChangedEventArgs args)
                {
                    eventArgs = args;
                });

            //
            // Public SourceChangedEventArgs API:
            // OldSource
            // NewSource
            // Element
            // OldParent
            //

            // 
            // Note: Due to a product 


            // Add child, verify SourceChanged event.
            eventArgs = null;
            _parent.Children.Add(_child);

            if (eventArgs == null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldSource != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.NewSource != source) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.Element != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldParent != null) throw new Microsoft.Test.TestValidationException("FAILED");


            // Remove child, verify SourceChanged event.
            eventArgs = null;
            _parent.Children.Remove(_child);

            if (eventArgs == null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldSource != source) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.NewSource != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.Element != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldParent != null) throw new Microsoft.Test.TestValidationException("FAILED");


            // Add child again, remove parent this time, verify SourceChanged event.
            _parent.Children.Add(_child);

            eventArgs = null;
            _root.Child = null;

            if (eventArgs == null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldSource != source) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.NewSource != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.Element != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldParent != null) throw new Microsoft.Test.TestValidationException("FAILED");


            // Add parent back, verify SourceChanged event.

            eventArgs = null;
            _root.Child = _parent;

            if (eventArgs == null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldSource != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.NewSource != source) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.Element != null) throw new Microsoft.Test.TestValidationException("FAILED");
            if (eventArgs.OldParent != null) throw new Microsoft.Test.TestValidationException("FAILED");

            base.DoExecute(o);

            return null;
        }

        /// <summary>
        /// All validation is done in DoExecute() for this test.
        /// </summary>
        public override object DoValidate(object sender)
        {
            this.TestPassed = true;
            return null;
        }

        private Border _root = null;
        private Panel _parent = null;
        private Panel _child = null;
    }
}
