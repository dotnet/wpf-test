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
using System.Windows.Markup;
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
    /// Verify IInputElement IsEnabled set works for ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for core input.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <

    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementIsEnabledApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\InputManager","HwndSource","1",@"Compile and Verify IInputElement IsEnabled set fails for ContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\InputManager","Browser","1",@"Compile and Verify IInputElement IsEnabled set fails for ContentElement in Browser.")]
        [TestCase("2",@"CoreInput\InputManager","Window","1",@"Compile and Verify IInputElement IsEnabled set fails for ContentElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementIsEnabledApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\InputManager","HwndSource","1",@"Verify IInputElement IsEnabled set fails for ContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\InputManager","Window","1",@"Verify IInputElement IsEnabled set fails for ContentElement in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementIsEnabledApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing element....");

            // Construct test element
            InstrContentPanelHost host = new InstrContentPanelHost();

            // Construct child element
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);
            _contentElement.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChange);

            // Add it to test element.
            host.AddChild(_contentElement);

            _rootElement = host;

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Setting enabledness for bare element....");
            try
            {
                // This should succeed -- a bare ContentElement now supports enabledness.
                _contentElement.IsEnabled = false;
                CoreLogger.LogStatus("Enabled is set!");
            }
            catch (InvalidOperationException e)
            {
                // {"Not allowed to call the base implementation of IsEnabled."}
                CoreLogger.LogStatus(".. expected exception:\n" + e.ToString());
                _bExceptionThrown = true;
            }

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

            // For this test we need the exception to not have been thrown 
            // We also expect the element to have been disabled (from automatic enabled).
            // We also expect 1 event to be raised.

            CoreLogger.LogStatus("Exception (expect none): " + _bExceptionThrown);
            CoreLogger.LogStatus("Event count  (expect 1): " + _eventLog.Count);

            bool bIsEnabled = _contentElement.IsEnabled;
            CoreLogger.LogStatus("Enabled?   (expect not): " + bIsEnabled);

            bool actual = (!_bExceptionThrown) && (_eventLog.Count == 1) && (!bIsEnabled);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard enabled event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnIsEnabledChange(object sender, DependencyPropertyChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [DependencyPropertyChanged: " + args.Property.Name + "]");
            CoreLogger.LogStatus("   Hello changing from: '" + args.OldValue.ToString() + "' to '" + args.NewValue.ToString() + "'");
        }

        /// <summary>
        /// Stores results of exception.
        /// </summary>
        private bool _bExceptionThrown = false;

        /// <summary>
        /// Stores events raised.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        private InstrContentPanel _contentElement;
    }
}
