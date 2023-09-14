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
    /// Verify CommandBinding can be serialized from a UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBinding can be serialized from a UIElement.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingSerializeApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingSerializeApp();
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

            // Create a normal command Binding using this command.
            _sampleCommandBinding = new CommandBinding(SampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + _sampleCommandBinding.ToString());

            // Create variants on command binding
            CommandBinding sampleProtectedCommandBinding = new CommandBinding(SampleCommand);
            CommandBinding samplePublicCommandBinding = new CommandBinding(SampleCommand);

            // Add event handlers.
            _sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(PrivateOnQuery);
            _sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(PrivateOnSample);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + _sampleCommandBinding.ToString());
            sampleProtectedCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(ProtectedOnQuery);
            sampleProtectedCommandBinding.Executed += new ExecutedRoutedEventHandler(ProtectedOnSample);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + sampleProtectedCommandBinding.ToString());
            samplePublicCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(PublicOnQuery);
            samplePublicCommandBinding.Executed += new ExecutedRoutedEventHandler(PublicOnSample);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + samplePublicCommandBinding.ToString());

            // Add this normal Binding to test element
            _rootElement.CommandBindings.Add(_sampleCommandBinding);
            _rootElement.CommandBindings.Add(sampleProtectedCommandBinding);
            _rootElement.CommandBindings.Add(samplePublicCommandBinding);

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

            CoreLogger.LogStatus("Serialized Outer XML: '" + _outerXml + "'");

            bool bContainsCommandBindings = _outerXml.Contains("CommandBindings");
            bool bContainsPublicOnQuery = _outerXml.Contains("PublicOnQuery");
            bool bContainsProtectedOnQuery = _outerXml.Contains("ProtectedOnQuery");
            CoreLogger.LogStatus("Contains CommandBindings? (expect yes)" + bContainsCommandBindings);
            CoreLogger.LogStatus("Contains PublicOnQuery? (expect no)" + bContainsPublicOnQuery);
            CoreLogger.LogStatus("Contains ProtectedOnQuery? (expect no)" + bContainsProtectedOnQuery);

            bool expectContents = bContainsCommandBindings && (!bContainsPublicOnQuery) && (!bContainsProtectedOnQuery); ;
            Assert(expectContents, "Whoops, contents not as expected");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, a command has been invoked.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void PrivateOnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void PrivateOnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, a command has been invoked.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        protected void ProtectedOnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        protected void ProtectedOnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, a command has been invoked.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        public void PublicOnSample(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
            {
                // Log some debugging data
                if (e.RoutedEvent != null)
                {
                    CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
                }

                CoreLogger.LogStatus(" Command:            " + e.Command);
            }
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        public void PublicOnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e != null)
            {
                // Log some debugging data
                if (e.RoutedEvent != null)
                {
                    CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
                }

                CoreLogger.LogStatus(" Command:            " + e.Command);
            }
        }

        /// <summary>
        /// Stores sample command object.
        /// </summary>
        public static RoutedCommand SampleCommand = new RoutedCommand("Sample", typeof(CommandBindingSerializeApp), null);

        /// <summary>
        /// Stores sample command Binding for the sample command.
        /// </summary>
        private CommandBinding _sampleCommandBinding;

        private string _outerXml = "";
    }
}

