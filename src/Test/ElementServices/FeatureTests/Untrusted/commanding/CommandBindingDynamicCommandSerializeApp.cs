// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify valid InputBindings can be serialized from element.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify valid InputBindings can be serialized from element.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingDynamicCommandSerializeApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingDynamicCommandSerializeApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            // Construct test element
            _rootElement = new InstrPanel();

            // Set up command with this Binding as a default
            _sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + _sampleCommand.ToString());

            // Create a normal command Binding using this command.
            _sampleCommandBinding = new CommandBinding(_sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + _sampleCommandBinding.ToString());

            // Add this normal Binding to test element
            _rootElement.CommandBindings.Add(_sampleCommandBinding);

            // Add some input bindings to test element
            _rootElement.InputBindings.Add(new KeyBinding(_sampleCommand, _saveKeyGesture));
            _rootElement.InputBindings.Add(new MouseBinding(_sampleCommand, _saveMouseGesture));

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus("Executing...");
            
            // Serialize!
            _outerXml = SerializationHelper.SerializeObjectTree(_rootElement);

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

            // For this test we are checking if our serialized outer XML contains a CommandBindings tag or attribute.
            
            CoreLogger.LogStatus("Serialized Outer XML: '"+_outerXml+"'");
            CoreLogger.LogStatus("Contains InputBindings? "+_outerXml.Contains("InputBindings") );
            CoreLogger.LogStatus("Contains MouseBinding value? "+_outerXml.Contains("WheelClick"));
            CoreLogger.LogStatus("Contains KeyBinding value? "+_outerXml.Contains("+B"));

            bool actual = (_outerXml.Contains("InputBindings") && _outerXml.Contains("WheelClick") && _outerXml.Contains("+B"));
            bool expected = true;
            bool bResult = (expected == actual);

            CoreLogger.LogStatus("Setting log result to " + bResult);
            this.TestPassed = bResult;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Stores sample command object.
        /// </summary>
        private RoutedCommand _sampleCommand;

        /// <summary>
        /// Stores sample command Binding for the sample command.
        /// </summary>
        private CommandBinding _sampleCommandBinding;

        /// <summary>
        /// Key binding to be used as an indexer into the collection of command Bindings.
        /// </summary>
        private KeyGesture _saveKeyGesture = new KeyGesture(Key.B, ModifierKeys.Windows);

        /// <summary>
        /// Mouse binding to be used as an indexer into the collection of command Bindings.
        /// </summary>
        private MouseGesture _saveMouseGesture = new MouseGesture(MouseAction.WheelClick);

        private string _outerXml = "";
    }
}
