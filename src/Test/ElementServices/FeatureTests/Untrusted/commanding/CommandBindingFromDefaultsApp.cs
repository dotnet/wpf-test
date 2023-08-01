// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
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

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CommandBinding can be retrieved from link collection via default bindings.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBinding can be retrieved from link collection via default bindings.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingFromDefaultsApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            
            CoreLogger.LogStatus ("Creating app object...");
            TestApp app = new CommandBindingFromDefaultsApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus ("App object: "+app.ToString());

            CoreLogger.LogStatus ("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus ("App run!");
        }

        private CommandBinding _sampleCommandBinding;

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            
            // Construct test element
            _rootElement = new InstrPanel();

            // Set up default command link with bindings
            CommandBinding defaultSampleCommandBinding = new CommandBinding();
            CoreLogger.LogStatus ("Default RoutedCommand link constructed: CommandBinding="+defaultSampleCommandBinding.ToString());

            // Set up command with this link as a default
            _sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus ("Command constructed: Command="+_sampleCommand.ToString());

            // Create a normal command link using this command.
            _sampleCommandBinding = new CommandBinding(_sampleCommand);
            CoreLogger.LogStatus ("Command link constructed: CommandBinding="+_sampleCommandBinding.ToString());

            // Add this normal link to test element
            _rootElement.CommandBindings.Add(_sampleCommandBinding);
            _saveKeyBinding = new KeyBinding(_sampleCommand, _saveKeyGesture);
            _saveMouseBinding = new MouseBinding(_sampleCommand, _saveMouseGesture);
            _rootElement.InputBindings.Add(_saveKeyBinding);
            _rootElement.InputBindings.Add(_saveMouseBinding);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus ("Executing...");
            
            // Enforce using defaults in our command link.
            //bool wasUseDefaults = _sampleCommandBinding.UseDefaults.
            //CoreLogger.LogStatus ("Previous UseDefaults state = "+wasUseDefaults+". Turning it on....").
            //_sampleCommandBinding.UseDefaults = true.
            
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
            CoreLogger.LogStatus ("Validating...");

            // For this test we are checking if our command link can be retrieved by default bindings.
            // The link does not have any explicit bindings on it,
            // but the command it is linked to does have those bindings in its Defaults collection.
            // So the link references should work, and they should all point to the same link.
            
            CoreLogger.LogStatus ("Links found: "+_rootElement.CommandBindings.Count);

            bool linksFound;
            if (_rootElement.InputBindings.Count == 2)
            {
                bool linkKey = _rootElement.InputBindings.Contains(_saveKeyBinding);
                bool linkMouse = _rootElement.InputBindings.Contains(_saveMouseBinding);

                linksFound = (linkKey ) && (linkMouse );
                CoreLogger.LogStatus ("Key,Mouse default links? "+linksFound);

            }
            else
            {
                linksFound = false;
            }

            bool actual = linksFound;
            bool expected = true;
            bool bResult = (expected == actual);

            CoreLogger.LogStatus ("Setting log result to " + bResult);
            this.TestPassed = bResult;
            
            CoreLogger.LogStatus ("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Stores sample command object.
        /// </summary>
        private RoutedCommand _sampleCommand;

        private KeyGesture _saveKeyGesture = new KeyGesture(Key.B, ModifierKeys.Windows);

        /// <summary>
        /// Key binding to be used as an indexer into the collection of input bindings.
        /// </summary>
        private KeyBinding _saveKeyBinding;

        private MouseGesture _saveMouseGesture = new MouseGesture(MouseAction.WheelClick);

        /// <summary>
        /// Mouse binding to be used as an indexer into the collection of input bindings.
        /// </summary>
        private MouseBinding _saveMouseBinding;
    }
}
