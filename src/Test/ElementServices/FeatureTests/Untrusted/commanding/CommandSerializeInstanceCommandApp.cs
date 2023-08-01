// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
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
    /// Verify dynamic instance of RoutedCommand can be serialized from a UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <




    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify dynamic instance of RoutedCommand can be serialized from a UIElement.")]
    [TestCasePriority("3")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CommandSerializeInstanceCommandApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {

            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandSerializeInstanceCommandApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

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

            // Set up command with no default Binding

            // 
            _sampleCustomCommand = new RoutedCommand("MySample", this.GetType(), null);
            CoreLogger.LogStatus("Command (custom) constructed: Command=" + _sampleCustomCommand.Name);

            _sampleCommand = ApplicationCommands.New;
            CoreLogger.LogStatus("Command (library) constructed: Command=" + _sampleCommand.Name);

            return null;
        }

        /// <summary>
        /// Sample command property.
        /// </summary>
        /// <remarks>
        /// Needed for command serialization to work.
        /// </remarks>
        public static readonly DependencyProperty SampleCommandProperty = DependencyProperty.Register("SampleCommand", typeof(RoutedCommand), typeof(CommandSerializeInstanceCommandApp));

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing...");

            // Add this command to test element
            _rootElement.SetValue(SampleCommandProperty, _sampleCommand);

            // Serialize!
            _outerXml = SerializationHelper.SerializeObjectTree(_rootElement);

            // Add different command to test element
            _rootElement.SetValue(SampleCommandProperty, _sampleCustomCommand);

            // Serialize!
            _outerCustomXml = SerializationHelper.SerializeObjectTree(_rootElement);

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

            // For this test we are checking if our serialized outer XML contains the name of our command.

            CoreLogger.LogStatus("= Serialized Outer XML (should contain 'New'):\n'" + _outerXml + "'");
            CoreLogger.LogStatus("= Serialized Outer XML (should contain 'MySample'):\n'" + _outerCustomXml + "'");

            bool actual = (!_outerXml.Contains("\"New\"") && !_outerCustomXml.Contains("\"MySample\""));
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
        /// Stores sample custom command object.
        /// </summary>
        private RoutedCommand _sampleCustomCommand;

        private string _outerXml = "";

        private string _outerCustomXml = "";
    }
}
