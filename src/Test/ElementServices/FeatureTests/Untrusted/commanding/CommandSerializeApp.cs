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
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify RoutedCommand from ApplicationCommands can be serialized from a UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <







    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify RoutedCommand from ApplicationCommands can be serialized from a UIElement.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CommandSerializeApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandSerializeApp();
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
            _rootElement = new CommandSerializePanel();

            s_sampleCommand = ApplicationCommands.New;
            CoreLogger.LogStatus("Command (library) constructed: Command=" + s_sampleCommand.Name);
            CoreLogger.LogStatus("Command (custom) constructed: Command=" + _sampleCustomCommand.Name);

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

            // Add this command to test element
            ((CommandSerializePanel)(_rootElement)).SampleCommand = s_sampleCommand;

            // Serialize!
            _outerXml = SerializationHelper.SerializeObjectTree(_rootElement);

            // Add different command to test element
            ((CommandSerializePanel)(_rootElement)).SampleCommand = _sampleCustomCommand;

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

            // For this test we are checking if our serialized outer XML contains the name of our command (for a library command),
            // and if it doesn't (for a custom command).

            CoreLogger.LogStatus("Serialized Outer XML (New):\n'" + _outerXml + "'");
            bool bContainsNew = (_outerXml.Contains("\"New\"")) || (_outerXml.Contains(">New<"));
            CoreLogger.LogStatus("Contains New? (expect true) "+bContainsNew);
            
            CoreLogger.LogStatus("Serialized Outer XML (MySample):\n'" + _outerCustomXml + "'");
            bool bContainsMySample = _outerCustomXml.Contains("\"MySample\"");
            CoreLogger.LogStatus("Contains MySample? (expect false) " + bContainsMySample);

            bool actual = bContainsNew && !bContainsMySample;
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
        private static RoutedCommand s_sampleCommand;

        /// <summary>
        /// Stores sample custom command object.
        /// </summary>
        public static RoutedCommand _sampleCustomCommand = new RoutedCommand("MySample", typeof(CommandSerializePanel), null);

        private string _outerXml = "";

        private string _outerCustomXml = "";
    }

    /// <summary>
    /// Element class that is serialized.
    /// </summary>
    public class CommandSerializePanel : InstrPanel
    {
        /// <summary>
        /// Sample RoutedCommand property.
        /// </summary>
        /// <remarks>
        /// Needed for command serialization to work.
        /// A DependencyProperty backs this CLR property.
        /// </remarks>
        public RoutedCommand SampleCommand
        {
            get { return (RoutedCommand)GetValue(SampleCommandProperty); }
            set { SetValue(SampleCommandProperty, value); }
        }

        /// <summary>
        /// Sample command property.
        /// </summary>
        /// <remarks>
        /// Needed for command serialization to work.
        /// </remarks>
        public static readonly DependencyProperty SampleCommandProperty = DependencyProperty.Register("SampleCommand", typeof(RoutedCommand), typeof(CommandSerializePanel));
    }
}

