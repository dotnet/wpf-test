// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Tests for DocumentAutomationPeer.
//

using System;                               // string
using System.Collections.Generic;           // List<T>
using System.Windows;                       // DependencyObject
using System.Windows.Automation.Peers;      // FlowDocumentReaderAutomationPeer
using System.Windows.Automation.Provider;   // ITextProvider
using System.Windows.Automation.Text;       // TextPatternRangeEndpoint
using System.Windows.Controls;              // FlowDocumentReader
using System.Windows.Documents;             // FlowDocument
using System.Windows.Media;                 // Brushes

namespace DRT
{
    /// <summary>
    /// Tests for DocumentAutomationPeer.
    /// </summary>
    internal class ReaderPeerTestSuite : UIAutoTestSuite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal ReaderPeerTestSuite() :
            base("ReaderAutomationPeer")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run
            return new DrtTest[] {
                new DrtTest(Create),                        new DrtTest(VerifyCreate),
                new DrtTest(SwitchToTwoPage),               new DrtTest(VerifySwitchToTwoPage),
                new DrtTest(SwitchToScroll),                new DrtTest(VerifySwitchToScroll),
                new DrtTest(SwitchToPage),                  new DrtTest(VerifySwitchToPage),
                new DrtTest(DisableTwoPage),                new DrtTest(VerifyDisableTwoPage),
            };
        }

        /// <summary>
        /// Create
        /// </summary>
        private void Create()
        {
            _document = LoadFromXaml("FlowDocumentTOC.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Failed to load 'FlowDocumentTOC.xaml'.");
            _document.ColumnWidth = double.MaxValue; // Force one column
            _viewer = new FlowDocumentReader();
            _viewer.Document = _document;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// VerifyCreate
        /// </summary>
        private void VerifyCreate()
        {
            DRT.Assert(_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page, "Expecting Page ViewingMode.");

            FlowDocumentReaderAutomationPeer viewerPeer = GetViewerAutomationPeer();

            // GetClassName
            string className = viewerPeer.GetClassName();
            DRT.Assert(className == "FlowDocumentReader", "GetClassName failed.");

            // GetPattern
            IMultipleViewProvider multiViewProvider = viewerPeer.GetPattern(PatternInterface.MultipleView) as IMultipleViewProvider;
            DRT.Assert(multiViewProvider != null, "Failed to retrieve MultipleViewPattern from FlowDocumentReaderAutomationPeer.");
            VerifyMultipleViewState(multiViewProvider, new int[] { 0, 1, 2 }, 0);
        }

        /// <summary>
        /// SwitchToTwoPage
        /// </summary>
        private void SwitchToTwoPage()
        {
            FlowDocumentReader.SwitchViewingModeCommand.Execute(FlowDocumentReaderViewingMode.TwoPage, _viewer);
        }

        /// <summary>
        /// VerifySwitchToTwoPage
        /// </summary>
        private void VerifySwitchToTwoPage()
        {
            DRT.Assert(_viewer.ViewingMode == FlowDocumentReaderViewingMode.TwoPage, "Expecting TwoPage ViewingMode.");

            FlowDocumentReaderAutomationPeer viewerPeer = GetViewerAutomationPeer();
            IMultipleViewProvider multiViewProvider = viewerPeer.GetPattern(PatternInterface.MultipleView) as IMultipleViewProvider;
            DRT.Assert(multiViewProvider != null, "Failed to retrieve MultipleViewPattern from FlowDocumentReaderAutomationPeer.");
            VerifyMultipleViewState(multiViewProvider, new int[] { 0, 1, 2 }, 1);
        }

        /// <summary>
        /// SwitchToScroll
        /// </summary>
        private void SwitchToScroll()
        {
            FlowDocumentReaderAutomationPeer viewerPeer = GetViewerAutomationPeer();
            IMultipleViewProvider multiViewProvider = viewerPeer.GetPattern(PatternInterface.MultipleView) as IMultipleViewProvider;
            DRT.Assert(multiViewProvider != null, "Failed to retrieve MultipleViewPattern from FlowDocumentReaderAutomationPeer.");
            multiViewProvider.SetCurrentView(2);
        }

        /// <summary>
        /// VerifySwitchToScroll
        /// </summary>
        private void VerifySwitchToScroll()
        {
            DRT.Assert(_viewer.ViewingMode == FlowDocumentReaderViewingMode.Scroll, "Expecting Scroll ViewingMode.");

            FlowDocumentReaderAutomationPeer viewerPeer = GetViewerAutomationPeer();
            IMultipleViewProvider multiViewProvider = viewerPeer.GetPattern(PatternInterface.MultipleView) as IMultipleViewProvider;
            DRT.Assert(multiViewProvider != null, "Failed to retrieve MultipleViewPattern from FlowDocumentReaderAutomationPeer.");
            VerifyMultipleViewState(multiViewProvider, new int[] { 0, 1, 2 }, 2);
        }

        /// <summary>
        /// SwitchToPage
        /// </summary>
        private void SwitchToPage()
        {
            FlowDocumentReader.SwitchViewingModeCommand.Execute(null, _viewer);
        }

        /// <summary>
        /// VerifySwitchToPage
        /// </summary>
        private void VerifySwitchToPage()
        {
            DRT.Assert(_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page, "Expecting Page ViewingMode.");

            FlowDocumentReaderAutomationPeer viewerPeer = GetViewerAutomationPeer();
            IMultipleViewProvider multiViewProvider = viewerPeer.GetPattern(PatternInterface.MultipleView) as IMultipleViewProvider;
            DRT.Assert(multiViewProvider != null, "Failed to retrieve MultipleViewPattern from FlowDocumentReaderAutomationPeer.");
            VerifyMultipleViewState(multiViewProvider, new int[] { 0, 1, 2 }, 0);
        }

        /// <summary>
        /// DisableTwoPage
        /// </summary>
        private void DisableTwoPage()
        {
            _viewer.IsTwoPageViewEnabled = false;
        }

        /// <summary>
        /// VerifyDisableTwoPage
        /// </summary>
        private void VerifyDisableTwoPage()
        {
            DRT.Assert(_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page, "Expecting Page ViewingMode.");

            FlowDocumentReaderAutomationPeer viewerPeer = GetViewerAutomationPeer();
            IMultipleViewProvider multiViewProvider = viewerPeer.GetPattern(PatternInterface.MultipleView) as IMultipleViewProvider;
            DRT.Assert(multiViewProvider != null, "Failed to retrieve MultipleViewPattern from FlowDocumentReaderAutomationPeer.");
            VerifyMultipleViewState(multiViewProvider, new int[] { 0, 2 }, 0);
        }

        /// <summary>
        /// Gets AutomationPeer for viewer.
        /// </summary>
        private FlowDocumentReaderAutomationPeer GetViewerAutomationPeer()
        {
            FlowDocumentReaderAutomationPeer viewerPeer = CreateAutomationPeer(_viewer) as FlowDocumentReaderAutomationPeer;
            DRT.Assert(viewerPeer != null, "Failed to retrieve AutomationPeer for FlowDocumentReader.");
            bool connected = EnsureConnected(viewerPeer);
            DRT.Assert(connected, "Failed to associated FlowDocumentReaderAutomationPeer with the current Hwnd.");
            return viewerPeer;
        }

        /// <summary>
        /// Verify state of IMultipleViewProvider
        /// </summary>
        private void VerifyMultipleViewState(IMultipleViewProvider provider, int[] expectedViews, int expectedCurrentView)
        {
            // GetSupportedViews
            int[] views = provider.GetSupportedViews();
            DRT.Assert(views != null, "IMultipleViewProvider.GetSupportedViews returned null.");
            DRT.Assert(views.Length == expectedViews.Length, "IMultipleViewProvider.GetSupportedViews failed. Expecting {0} views, got {1}.", expectedViews.Length, views.Length);
            for (int i = 0; i < views.Length; i++)
            {
                DRT.Assert(views[0] == 0, "IMultipleViewProvider.GetSupportedViews failed. Expecting id {0} for view #{1}, got {2}.", expectedViews[i], i, views[i]);
            }

            // CurrentView
            int currentView = provider.CurrentView;
            DRT.Assert(currentView == expectedCurrentView, "IMultipleViewProvider.CurrentView failed. Expecting id {0}, got {1}", expectedCurrentView, currentView);

            // GetViewName
            string name = provider.GetViewName(currentView);
            DRT.Assert(!String.IsNullOrEmpty(name), "IMultipleViewProvider.GetViewName returned empty name.");
        }

        FlowDocumentReader _viewer;     // Viewer for FlowDocument
        FlowDocument _document;         // FlowDocument
    }
}
