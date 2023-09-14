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
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify InputBinding can be serialized from a UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify InputBinding can be serialized from a UIElement.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class InputBindingSerializeApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new InputBindingSerializeApp();
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

            // Create a normal Input Binding using this command.
            _sampleInputBinding = new KeyBinding(MediaCommands.Stop, new KeyGesture(Key.F8));
            CoreLogger.LogStatus("Input Binding constructed: InputBinding=" + _sampleInputBinding.ToString());

            // Create variants on binding
            InputBinding sampleMouseBinding = new MouseBinding(MediaCommands.Play, new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt));
            InputBinding sampleNullKeyBinding = new KeyBinding();
            InputBinding sampleNullMouseBinding = new MouseBinding();

            // Add these bindings to test element
            _rootElement.InputBindings.Add(_sampleInputBinding);
            _rootElement.InputBindings.Add(sampleMouseBinding);
            _rootElement.InputBindings.Add(sampleNullKeyBinding);
            _rootElement.InputBindings.Add(sampleNullMouseBinding);

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

            // For this test we are checking if our serialized outer XML contains a InputBindings tag or attribute.

            CoreLogger.LogStatus("Serialized Outer XML: '" + _outerXml + "'");

            bool bContainsInputBindings = _outerXml.Contains("InputBindings");
            bool bContainsStop = _outerXml.Contains("Stop");
            bool bContainsPlay = _outerXml.Contains("Play");
            CoreLogger.LogStatus("Contains InputBindings? (expect yes)" + bContainsInputBindings);
            CoreLogger.LogStatus("Contains Stop? (expect yes)" + bContainsStop);
            CoreLogger.LogStatus("Contains Play? (expect yes)" + bContainsPlay);

            bool expectContents = bContainsInputBindings && bContainsStop && bContainsPlay;
            Assert(expectContents, "Whoops, contents not as expected");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Stores sample command object.
        /// </summary>
        public static RoutedCommand SampleCommand = new RoutedCommand("Sample", typeof(InputBindingSerializeApp), null);

        /// <summary>
        /// Stores sample Input Binding for the sample command.
        /// </summary>
        private InputBinding _sampleInputBinding;

        private string _outerXml = "";
    }
}

