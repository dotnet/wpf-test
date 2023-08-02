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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
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
    /// Verify setting FrameworkContentElement Cursor works for content.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkContentElementCursorApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", @"Compile and Verify setting FrameworkContentElement Cursor works for content in HwndSource.")]
        [TestCase("1", @"CoreInput\Cursor", "Browser", @"Compile and Verify setting FrameworkContentElement Cursor works for content in Browser.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", @"Compile and Verify setting FrameworkContentElement Cursor works for content in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkContentElementCursorApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", @"Verify setting FrameworkContentElement Cursor works for content in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", @"Verify setting FrameworkContentElement Cursor works for content in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkContentElementCursorApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct canvas
            Canvas cvs = new Canvas();

            // Construct table inside a textpanel as table is a contentElement.
            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument();
            
            Canvas.SetTop(tp, 0);
            Canvas.SetLeft(tp, 0);

            tp.Height = 100;
            tp.Width = 100;

            Table tbl = new Table();
            tbl.CellSpacing = 5;

            // Construct table body
            TableRowGroup body = new TableRowGroup();

            // Construct panel (child element) to be placed inside content element
            InstrFrameworkPanel panel = new InstrFrameworkPanel();
            panel.Focusable = true;
            panel.Name = "nFrameworkContentElementForceCursor" + DateTime.Now.Ticks;
            panel.Height = 95;
            panel.Width = 95;

            // Add panel to table cell (content element)
            TableCell c = new TableCell();
            c.Blocks.Add(new BlockUIContainer(panel));

            // Set cursor on content element
            _frameworkContentElement = c;
            _frameworkContentElement.Cursor = Cursors.Pen;

            // Add cell to table row
            TableRow r = new TableRow();
            r.Cells.Add(c);

            // Add row to table body.
            body.Rows.Add(r);

            // Our table is ready. Add it to the canvas.
            tbl.RowGroups.Add(body);
            tp.Document.Blocks.Add(tbl);

            //Add textpanel to Canvas.
            cvs.Children.Add(tp);

            // Put the canvas on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            InputCallback[] ops = new InputCallback[] {
                delegate
                {
                    MouseHelper.Move(((Canvas)_rootElement).Children[0]);
                }
            };

            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need the parent cursor to take precedence.

            // expect matching stock cursors
            IntPtr actualCursor = NativeMethods.GetCursor();
            IntPtr expectedCursor = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_PEN);
            CoreLogger.LogStatus("Found cursor: " + actualCursor + ", expected: " + expectedCursor);

            bool eventFound = (expectedCursor == actualCursor);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store content element on our canvas.
        /// </summary>
        private FrameworkContentElement _frameworkContentElement;
    }
}
