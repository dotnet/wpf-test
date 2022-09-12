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
    internal class DocumentPeerTestSuite : UIAutoTestSuite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal DocumentPeerTestSuite() :
            base("DocumentAutomationPeer")
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
                new DrtTest(LoadDocument),                  new DrtTest(VerifyLoadDocument),
                new DrtTest(RangeFromPoint),                new DrtTest(VerifyRangeFromPoint),
            };
        }

        /// <summary>
        /// LoadDocument
        /// </summary>
        private void LoadDocument()
        {
            _document = LoadFromXaml("FlowDocumentTOC.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Failed to load 'FlowDocumentTOC.xaml'.");
            _document.ColumnWidth = double.MaxValue; // Force one column
            _viewer = new FlowDocumentReader();
            _viewer.Document = _document;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// VerifyLoadDocument
        /// </summary>
        private void VerifyLoadDocument()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();

            // GetPattern
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // GetBoundingRectangle
            Rect boundingRect = documentPeer.GetBoundingRectangle();
            DRT.Assert(boundingRect != Rect.Empty, "GetBoundingRectangle failed: empty bounding rectangle.");

            // GetClickablePoint
            Point point = documentPeer.GetClickablePoint();
            DRT.Assert(boundingRect.Contains(point), "GetClickablePoint failed.");

            // IsOffscreen
            bool isOffscreen = documentPeer.IsOffscreen();
            DRT.Assert(!isOffscreen, "IsOffscreen failed: document is offscreen.");

            // GetClassName
            string className = documentPeer.GetClassName();
            DRT.Assert(className == "Document", "GetClassName failed.");

            // GetAutomationControlType
            AutomationControlType controlType = documentPeer.GetAutomationControlType();
            DRT.Assert(controlType == AutomationControlType.Document, "GetAutomationControlType failed.");

            // IsContentElement
            bool isContentElement = documentPeer.IsContentElement();
            DRT.Assert(isContentElement, "IsContentElement failed.");

            // IsControlElement
            bool isControlElement = documentPeer.IsControlElement();
            DRT.Assert(isControlElement, "IsControlElement failed.");

            // GetParent
            AutomationPeer parent = documentPeer.GetParent();
            DRT.Assert(parent != null, "GetParent failed.");

            // GetParent
            List<AutomationPeer> children = documentPeer.GetChildren();
            DRT.Assert(children == null, "GetChildren failed.");
        }

        /// <summary>
        /// RangeFromPoint
        /// </summary>
        private void RangeFromPoint()
        {
        }

        /// <summary>
        /// VerifyRangeFromPoint
        /// </summary>
        private void VerifyRangeFromPoint()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();

            // Get TextPattern
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // GetClickablePoint
            Rect boundingRect = documentPeer.GetBoundingRectangle();
            Point point = documentPeer.GetClickablePoint();
            DRT.Assert(boundingRect.Contains(point), "GetClickablePoint failed.");

            // Get RangeFromPoint
            ITextRangeProvider textRangeProvider = textProvider.RangeFromPoint(point);
            DRT.Assert(textRangeProvider != null, "ITextProvider.RangeFromPoint failed.");
        }

        /// <summary>
        /// Gets AutomationPeer for FlowDocument and verifies appropriate connection to the viewer.
        /// </summary>
        private DocumentAutomationPeer GetDocumentAutomationPeer()
        {
            FlowDocumentReaderAutomationPeer viewerPeer = CreateAutomationPeer(_viewer) as FlowDocumentReaderAutomationPeer;
            DRT.Assert(viewerPeer != null, "Failed to retrieve AutomationPeer for FlowDocumentReader.");
            bool connected = EnsureConnected(viewerPeer);
            DRT.Assert(connected, "Failed to associated FlowDocumentReaderAutomationPeer with the current Hwnd.");
            DocumentAutomationPeer documentPeer = CreateAutomationPeer(_document) as DocumentAutomationPeer;
            DRT.Assert(documentPeer != null, "Failed to retrieve AutomationPeer for FlowDocument.");
            List<AutomationPeer> children = viewerPeer.GetChildren();
            DRT.Assert(children.Count > 0 && children[children.Count-1] == documentPeer, "AutomationPeers for FlowDocument and FlowDocumentReader.Document are not matching.");
            AutomationPeer parentPeer = documentPeer.GetParent();
            DRT.Assert(viewerPeer == parentPeer, "AutomationPeers for FlowDocument and FlowDocumentReader.Document are not matching.");
            return documentPeer;
        }

        FlowDocumentReader _viewer;     // Viewer for FlowDocument
        FlowDocument _document;         // FlowDocument
    }
}
