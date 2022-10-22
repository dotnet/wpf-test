// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for FlowDocumentPageViewer navigation scenarios.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // VisualTreeHelper
using System.Windows.Navigation;            // NavigationWindow

namespace DRT
{
    /// <summary>
    /// Test suite for FlowDocumentPageViewer navigation scenarios.
    /// </summary>
    internal class PageViewerNavTestSuite : ViewerNavTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal PageViewerNavTestSuite():
            base("PageViewerNavigation")
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

            DataTemplate dt = new DataTemplate();
            dt.DataType = typeof(FlowDocument);
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(FlowDocumentPageViewer));
            fef.SetValue(FlowDocumentPageViewer.DocumentProperty, new System.Windows.Data.Binding());
            fef.SetValue(FlowDocumentPageViewer.FontSizeProperty, 11.0);
            fef.SetValue(FlowDocumentPageViewer.FontFamilyProperty, new FontFamily("Tahoma"));
            fef.SetValue(FlowDocumentPageViewer.NameProperty, "FlowDocumentPageViewer");
            dt.VisualTree = fef;
            _navWindow.Resources.Add(new DataTemplateKey(typeof(FlowDocument)), dt);
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
                if (_viewer != null)
                {
                    DRT.Assert(_viewer == coreParent, "FlowDocumentPageViewer instantiated through DataTemplate is changing.");
                }
                else
                {
                    _viewer = coreParent as FlowDocumentPageViewer;
                    DRT.Assert(_viewer != null, "Expecting FlowDocumentPageViewer as core parent of the document, got {0}.", coreParent.GetType().ToString());
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


        private FlowDocumentPageViewer _viewer;
    }
}