// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for FlowDocumentReader navigation scenarios.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentReader
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // VisualTreeHelper
using System.Windows.Navigation;            // NavigationWindow

namespace DRT
{
    /// <summary>
    /// Test suite for FlowDocumentReader navigation scenarios.
    /// </summary>
    internal class ReaderViewerNavTestSuite : ViewerNavTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal ReaderViewerNavTestSuite():
            base("ReaderViewerNavigation")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Document viewer instance.
        /// </summary>
        protected override Control Viewer { get { return _viewer; } }

        /// <summary>
        /// Creates empty NavigationWindow with no content.
        /// </summary>
        protected override void ShowNavWindow()
        {
            base.ShowNavWindow();
            _navWindow.FontSize = 11.0;
            _navWindow.FontFamily = new FontFamily("Tahoma");
        }

        /// <summary>
        /// Update reference to the viewer object.
        /// </summary>
        protected override void UpdateViewer()
        {
            if (_document != null)
            {
                object coreParent = ContentOperations.GetParent(_document);
                DRT.Assert(coreParent != null, "Failed to retrieve core parent of the document.");
                if (coreParent is Control)
                {
                    coreParent = VisualTreeHelper.GetParent((Control)coreParent) as FrameworkElement;
                    if (coreParent != null)
                    {
                        coreParent = ((FrameworkElement)coreParent).TemplatedParent;
                    }
                }
                if (_viewer != null)
                {
                    DRT.Assert(_viewer == coreParent, "FlowDocumentReader instantiated through DataTemplate is changing.");
                }
                else
                {
                    _viewer = coreParent as FlowDocumentReader;
                    DRT.Assert(_viewer != null, "Expecting FlowDocumentReader as core parent of the document, got {0}.", coreParent.GetType().ToString());
                }
            }
            else
            {
                _viewer = null;
            }
        }

        /// <summary>
        /// Resets the viewer document
        /// </summary>
        protected override void ResetDocument()
        {
            _viewer.Document = new FlowDocument();
        }


        /// <summary>
        /// Go to page #5.
        /// </summary>
        protected override void GoToPageCommand()
        {
            NextPageCommand();
            NextPageCommand();
            NextPageCommand();
            NextPageCommand();
        }

        /// <summary>
        /// Custom command.
        /// </summary>
        protected override void CustomViewerCommand()
        {
            _viewer.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
        }

        private FlowDocumentReader _viewer;
    }
}
