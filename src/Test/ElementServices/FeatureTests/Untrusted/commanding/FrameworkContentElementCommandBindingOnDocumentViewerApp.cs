// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CanExecute and Executed bubble on framework content elements contained by document viewer.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkContentElementCommandBindingOnDocumentViewerApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"Commanding\CommandBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify CanExecute and Executed bubble on framework content elements contained by document viewer in HwndSource.")]
        [TestCase("2", @"Commanding\CommandBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify CanExecute and Executed bubble on framework content elements contained by document viewer in Browser.")]
        [TestCase("3", @"Commanding\CommandBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify CanExecute and Executed bubble on framework content elements contained by document viewer in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "FrameworkContentElementCommandBindingOnDocumentViewerApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\CommandBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify CanExecute and Executed bubble on framework content elements contained by document viewer in HwndSource.")]
        [TestCase("2", @"Commanding\CommandBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify CanExecute and Executed bubble on framework content elements contained by document viewer in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkContentElementCommandBindingOnDocumentViewerApp(), "Run");

        }
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="arg">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object arg)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct document viewer for tree
            DocumentViewer f = new DocumentViewer();
            f.Background = Brushes.Pink;
            Canvas.SetTop(f, 0.0);
            Canvas.SetLeft(f, 0.0);
            f.Width = 200;
            f.Height = 200;

            // Construct content element to have command invoked against
            _bold = new Bold(new Run("BoldText"));

            // Construct text flow for our content element
            Paragraph p = new Paragraph(_bold);
            //TextFlow tb = new TextFlow(p);
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.Document = new FlowDocument();
            fdsv.Document.Blocks.Add(p);

            // Construct page to hold text flow
            FixedPage page = new FixedPage();
            page.Background = Brushes.Pink;
            page.Children.Add(fdsv);

            // Construct page content from page
            PageContent content = new PageContent();
            content.BeginInit();
            ((IAddChild)content).AddChild(page);
            content.EndInit();

            // Construct document to view page content in
            FixedDocument doc = new FixedDocument();
            doc.Pages.Add(content);

            // Attach document to viewer
            f.Document = doc;

            // Construct tree
            Canvas frameCanvas = new Canvas();

            // Add doc viewer to tree
            frameCanvas.Children.Add(f);

            // Add event handlers
            CommandBinding sampleCommandBinding = new CommandBinding(_routedCmd, OnSample, OnQuery);
            frameCanvas.CommandBindings.Add(sampleCommandBinding);

            // Put the test element on the screen
            DisplayMe(frameCanvas, 10, 10, 200, 200);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing command on framed element...");
            _routedCmd.Execute(null, _bold);

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

            // Note: for this test we are concerned about whether events fire for each execution
            CoreLogger.LogStatus("CanExecute Events found: (expect 1) " + _queryLog.Count);
            CoreLogger.LogStatus("Execute Events found: (expect 1) " + _commandLog.Count);

            bool actual = (_commandLog.Count == 1) && (_queryLog.Count == 1);
            bool expected = true;

            bool result = (actual == expected);
            CoreLogger.LogStatus("Setting log result to " + result);
            this.TestPassed = result;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            _queryLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            CoreLogger.LogStatus("  Status:            " + e.CanExecute);

            if (sender != null)
            {
                CoreLogger.LogStatus(" command sender Name: " + sender.ToString());
            }

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            _commandLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                CoreLogger.LogStatus(" command sender Name: " + sender.ToString());
            }
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        private List<CanExecuteRoutedEventArgs> _queryLog = new List<CanExecuteRoutedEventArgs>();

        private Inline _bold;

        private RoutedCommand _routedCmd = new RoutedCommand();
    }
}
