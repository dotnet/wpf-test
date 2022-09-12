// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for FlowDocumentReader basic scenarios.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentReader
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // Fonts

namespace DRT
{
    /// <summary>
    /// Test suite for FlowDocumentReader basic scenarios.
    /// </summary>
    internal class ReaderViewerTestSuite : ViewerTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal ReaderViewerTestSuite():
            base("ReaderViewer")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(NoDocument),                    new DrtTest(DumpCreate),
                new DrtTest(LoadFlowDocument),              new DrtTest(DumpAppend),
                new DrtTest(NextPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(GoToPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(LastPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(PreviousPageCommand),           new DrtTest(DumpAppend),
                new DrtTest(FirstPageCommand),              new DrtTest(DumpAppend),
                new DrtTest(SwitchViewingModeCommand),      new DrtTest(DumpAppend),
                new DrtTest(SwitchViewingModeCommand),      new DrtTest(DumpAppend),
                new DrtTest(SwitchViewingModeCommand),      new DrtTest(DumpAppend),
                new DrtTest(SwitchToScrollMode),            new DrtTest(DumpAppend),
                new DrtTest(DisableAndSwitchToPageMode),    new DrtTest(DumpAppend),
                new DrtTest(EnableAndSwitchToPageMode),     new DrtTest(DumpAppend),
                new DrtTest(DisableAndSwitchToTwoPageMode), new DrtTest(DumpAppend),
                new DrtTest(EnableAndSwitchToTwoPageMode),  new DrtTest(DumpAppend),
                new DrtTest(DisableAndSwitchToScrollMode),  new DrtTest(DumpAppend),
                new DrtTest(EnableAndSwitchToScrollMode),   new DrtTest(DumpAppend),
                new DrtTest(DisablePageModeAndSwitch),      new DrtTest(DumpAppend),
                new DrtTest(DisableScrollModeAndSwitch),    new DrtTest(DumpAppend),
                new DrtTest(EnablePageModeAndSwitch),       new DrtTest(DumpAppend),
                new DrtTest(EnableScrollModeAndSwitch),     new DrtTest(DumpAppend),
                new DrtTest(BringIntoView1),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView2),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView3),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView4),                new DrtTest(DumpAppend),
                new DrtTest(UnloadDocument),                new DrtTest(DumpFinalizeAndVerify),
            };
        }

        /// <summary>
        /// Document viewer instance.
        /// </summary>
        protected override Control Viewer { get { return _viewer; } }

        /// <summary>
        /// Creates FlowDocumentReader with no document attached to it.
        /// </summary>
        private void NoDocument()
        {
            _viewer = new FlowDocumentReader();
            _viewer.FontSize = 11.0;
            _viewer.FontFamily = new FontFamily("Tahoma");
            _document = null;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Load FlowDocument.
        /// </summary>
        private void LoadFlowDocument()
        {
            _document = LoadFromXaml("FlowDocumentComplex.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Cannot load 'FlowDocumentComplex.xaml'.");
            _viewer.Document = _document;
        }

        /// <summary>
        /// Go to page number 5.
        /// </summary>
        private void GoToPageCommand()
        {
            GoToPageCommand(5);
        }

        /// <summary>
        /// SwitchViewingModeCommand
        /// </summary>
        private void SwitchViewingModeCommand()
        {
            FlowDocumentReader.SwitchViewingModeCommand.Execute(null, _viewer);
        }

        /// <summary>
        /// SwitchToScrollMode
        /// </summary>
        private void SwitchToScrollMode()
        {
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.Scroll, _viewer);
        }

        /// <summary>
        /// DisableAndSwitchToPageMode
        /// </summary>
        private void DisableAndSwitchToPageMode()
        {
            _viewer.IsPageViewEnabled = false;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.Page, _viewer);
        }

        /// <summary>
        /// EnableAndSwitchToPageMode
        /// </summary>
        private void EnableAndSwitchToPageMode()
        {
            _viewer.IsPageViewEnabled = true;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.Page, _viewer);
        }

        /// <summary>
        /// DisableAndSwitchToTwoPageMode
        /// </summary>
        private void DisableAndSwitchToTwoPageMode()
        {
            _viewer.IsTwoPageViewEnabled = false;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.TwoPage, _viewer);
        }

        /// <summary>
        /// EnableAndSwitchToTwoPageMode
        /// </summary>
        private void EnableAndSwitchToTwoPageMode()
        {
            _viewer.IsTwoPageViewEnabled = true;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.TwoPage, _viewer);
        }

        /// <summary>
        /// DisableAndSwitchToScrollMode
        /// </summary>
        private void DisableAndSwitchToScrollMode()
        {
            _viewer.IsScrollViewEnabled = false;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.Scroll, _viewer);
        }

        /// <summary>
        /// EnableAndSwitchToScrollMode
        /// </summary>
        private void EnableAndSwitchToScrollMode()
        {
            _viewer.IsScrollViewEnabled = true;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.Scroll, _viewer);
        }

        /// <summary>
        /// DisablePageModeAndSwitch
        /// </summary>
        private void DisablePageModeAndSwitch()
        {
            _viewer.IsPageViewEnabled = false;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(null, _viewer);
        }

        /// <summary>
        /// DisableScrollModeAndSwitch
        /// </summary>
        private void DisableScrollModeAndSwitch()
        {
            _viewer.IsScrollViewEnabled = false;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(null, _viewer);
        }

        /// <summary>
        /// EnablePageModeAndSwitch
        /// </summary>
        private void EnablePageModeAndSwitch()
        {
            _viewer.IsPageViewEnabled = true;
            FlowDocumentReader.SwitchViewingModeCommand.Execute(null, _viewer);
        }

        /// <summary>
        /// EnableScrollModeAndSwitch
        /// </summary>
        private void EnableScrollModeAndSwitch()
        {
            _viewer.IsScrollViewEnabled = true;
            FlowDocumentReader.SwitchViewingModeCommand.Execute("bogus mode", _viewer);
        }

        /// <summary>
        /// Bring paragraph into view.
        /// </summary>
        private void BringIntoView1()
        {
            TextElement element = DRT.FindElementByID("sixteenth", _viewer) as TextElement;
            DRT.Assert(element != null, "Cannot find element with Name='sixteenth'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Bring outer UIElement into view.
        /// </summary>
        private void BringIntoView2()
        {
            FrameworkElement element = DRT.FindElementByID("outer", _viewer) as FrameworkElement;
            DRT.Assert(element != null, "Cannot find element with Name='outer'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Bring paragraph into view.
        /// </summary>
        private void BringIntoView3()
        {
            TextElement element = DRT.FindElementByID("nineteenth", _viewer) as TextElement;
            DRT.Assert(element != null, "Cannot find element with Name='nineteenth'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Bring inner UIElement into view.
        /// </summary>
        private void BringIntoView4()
        {
            FrameworkElement element = DRT.FindElementByID("inner", _viewer) as FrameworkElement;
            DRT.Assert(element != null, "Cannot find element with Name='inner'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Unload document.
        /// </summary>
        private void UnloadDocument()
        {
            _document = null;
            _viewer.Document = null;
        }

        private FlowDocumentReader _viewer;
        private FlowDocument _document;
    }
}
