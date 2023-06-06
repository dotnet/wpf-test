// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for document viewers test suites.
//

using System;                               // string
using System.Collections.ObjectModel;       // ReadOnlyCollection
using System.Globalization;                 // CultureInfo
using System.Reflection;                    // PropertyInfo, BindingFlags
using System.Xml;                           // XmlTextWriter
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // Control
using System.Windows.Controls.Primitives;   // DocumentPageView
using System.Windows.Documents;             // DocumentPage
using System.Windows.Input;                 // NavigationCommands

namespace DRT
{
    /// <summary>
    /// Common functionality for document viewers test suites.
    /// </summary>
    internal abstract class ViewerTestSuite : DumpTestSuite
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static ViewerTestSuite()
        {
            s_piCurrentViewer = typeof(FlowDocumentReader).GetProperty("CurrentViewer", BindingFlags.Instance | BindingFlags.NonPublic);
            s_piRenderScope = typeof(FlowDocumentScrollViewer).GetProperty("RenderScope", BindingFlags.Instance | BindingFlags.NonPublic);
            Type typeFlowDocumentView = DrtFlowBase.FrameworkAssembly.GetType("MS.Internal.Documents.FlowDocumentView");
            s_piDocumentPage = typeFlowDocumentView.GetProperty("DocumentPage", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        protected ViewerTestSuite(string suiteName) :
            base(suiteName)
        {
        }

        /// <summary>
        /// Document viewer instance.
        /// </summary>
        protected abstract Control Viewer { get; }

        /// <summary>
        /// Execute NextPage command.
        /// </summary>
        protected void NextPageCommand()
        {
            NavigationCommands.NextPage.Execute(null, this.Viewer);
        }

        /// <summary>
        /// Execute PreviousPage command.
        /// </summary>
        protected void PreviousPageCommand()
        {
            NavigationCommands.PreviousPage.Execute(null, this.Viewer);
        }

        /// <summary>
        /// Execute FirstPage command.
        /// </summary>
        protected void FirstPageCommand()
        {
            NavigationCommands.FirstPage.Execute(null, this.Viewer);
        }

        /// <summary>
        /// Execute LastPage command.
        /// </summary>
        protected void LastPageCommand()
        {
            NavigationCommands.LastPage.Execute(null, this.Viewer);
        }

        /// <summary>
        /// Execute GoToPage command.
        /// </summary>
        protected void GoToPageCommand(int pageNumber)
        {
            NavigationCommands.GoToPage.Execute(pageNumber, this.Viewer);
        }

        /// <summary>
        /// Execute IncreaseZoom command.
        /// </summary>
        protected void IncreaseZoomCommand()
        {
            NavigationCommands.IncreaseZoom.Execute(null, this.Viewer);
        }

        /// <summary>
        /// Execute DecreaseZoom command.
        /// </summary>
        protected void DecreaseZoomCommand()
        {
            NavigationCommands.DecreaseZoom.Execute(null, this.Viewer);
        }

        /// <summary>
        /// Dump content to XmlTextWriter.
        /// </summary>
        protected override void DumpContent(XmlTextWriter writer)
        {
            // Write root element
            writer.WriteStartElement("ViewerDump");

            // Dump viewr information
            if (this.Viewer != null)
            {
                DumpViewer(writer, this.Viewer);
            }

            // Write end root
            writer.WriteEndElement();
            writer.WriteRaw("\r\n");  // required for bbpack
        }

        /// <summary>
        /// Helper to invoke a hyperlink.
        /// </summary>
        protected void InvokeHyperlink(Hyperlink hyperlink)
        {
            if (hyperlink != null)
            {
                hyperlink.DoClick();
            }
        }

        /// <summary>
        /// Dump state of document viewer to XmlTextWriter.
        /// </summary>
        private void DumpViewer(XmlTextWriter writer, Control viewer)
        {
            writer.WriteStartElement(viewer.GetType().Name);

            if (viewer is FlowDocumentPageViewer)
            {
                DumpPageViewer(writer, (FlowDocumentPageViewer)viewer);
            }
            else if (viewer is FlowDocumentScrollViewer)
            {
                DumpScrollViewer(writer, (FlowDocumentScrollViewer)viewer);
            }
            else if (viewer is FlowDocumentReader)
            {
                DumpReaderViewer(writer, (FlowDocumentReader)viewer);
            }
            else if (viewer is DocumentViewerBase)
            {
                DumpDocumentViewerBase(writer, (DocumentViewerBase)viewer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump state of FlowDocumentPageViewer to XmlTextWriter.
        /// </summary>
        private void DumpPageViewer(XmlTextWriter writer, FlowDocumentPageViewer viewer)
        {
            // Document properties
            writer.WriteStartElement("Document");
            writer.WriteAttributeString("Type", (viewer.Document != null) ? ((object)viewer.Document).GetType().FullName : "(null)");
            writer.WriteAttributeString("PageCount", viewer.PageCount.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Viewer state
            writer.WriteStartElement("State");
            writer.WriteAttributeString("MasterPageNumber", viewer.MasterPageNumber.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanGoToPreviousPage", viewer.CanGoToPreviousPage.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanGoToNextPage", viewer.CanGoToNextPage.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Zoom state
            writer.WriteStartElement("Zoom");
            writer.WriteAttributeString("Zoom", viewer.Zoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MinZoom", viewer.MinZoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MaxZoom", viewer.MaxZoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ZoomIncrement", viewer.ZoomIncrement.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanIncreaseZoom", viewer.CanIncreaseZoom.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanDecreaseZoom", viewer.CanDecreaseZoom.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // PageViews
            ReadOnlyCollection<DocumentPageView> pageViews = viewer.PageViews;
            writer.WriteStartElement("PageViews");
            writer.WriteAttributeString("Count", pageViews.Count.ToString(CultureInfo.InvariantCulture));
            for (int i = 0; i < pageViews.Count; i++)
            {
                DumpDocumentPageView(writer, pageViews[i]);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump state of FlowDocumentScrollViewer to XmlTextWriter.
        /// </summary>
        private void DumpScrollViewer(XmlTextWriter writer, FlowDocumentScrollViewer viewer)
        {
            // Document properties
            writer.WriteStartElement("Document");
            writer.WriteAttributeString("Type", (viewer.Document != null) ? viewer.Document.GetType().FullName : "(null)");
            writer.WriteEndElement();

            // Viewer state
            writer.WriteStartElement("State");
            writer.WriteAttributeString("IsSelectionEnabled", viewer.IsSelectionEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("IsToolBarVisible", viewer.IsToolBarVisible.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("HorizontalScrollBarVisibility", viewer.HorizontalScrollBarVisibility.ToString());
            writer.WriteAttributeString("VerticalScrollBarVisibility", viewer.VerticalScrollBarVisibility.ToString());
            writer.WriteEndElement();

            // Zoom state
            writer.WriteStartElement("Zoom");
            writer.WriteAttributeString("Zoom", viewer.Zoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MinZoom", viewer.MinZoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MaxZoom", viewer.MaxZoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ZoomIncrement", viewer.ZoomIncrement.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanIncreaseZoom", viewer.CanIncreaseZoom.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanDecreaseZoom", viewer.CanDecreaseZoom.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // FlowDocumentView
            object pageView = s_piRenderScope.GetValue(viewer, null);
            if (pageView != null)
            {
                DumpFlowDocumentView(writer, pageView);
            }
        }

        /// <summary>
        /// Dump state of FlowDocumentReader to XmlTextWriter.
        /// </summary>
        private void DumpReaderViewer(XmlTextWriter writer, FlowDocumentReader viewer)
        {
            // Document properties
            writer.WriteStartElement("Document");
            writer.WriteAttributeString("Type", (viewer.Document != null) ? viewer.Document.GetType().FullName : "(null)");
            writer.WriteAttributeString("PageCount", viewer.PageCount.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Viewer state
            writer.WriteStartElement("State");
            writer.WriteAttributeString("PageNumber", viewer.PageNumber.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanGoToPreviousPage", viewer.CanGoToPreviousPage.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanGoToNextPage", viewer.CanGoToNextPage.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("IsFindEnabled", viewer.IsFindEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("IsPrintEnabled", viewer.IsPrintEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Zoom state
            writer.WriteStartElement("Zoom");
            writer.WriteAttributeString("Zoom", viewer.Zoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MinZoom", viewer.MinZoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MaxZoom", viewer.MaxZoom.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ZoomIncrement", viewer.ZoomIncrement.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanIncreaseZoom", viewer.CanIncreaseZoom.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanDecreaseZoom", viewer.CanDecreaseZoom.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Viewing mode
            writer.WriteStartElement("Mode");
            writer.WriteAttributeString("ViewingMode", viewer.ViewingMode.ToString());
            writer.WriteAttributeString("IsPageViewEnabled", viewer.IsPageViewEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("IsTwoPageViewEnabled", viewer.IsTwoPageViewEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("IsScrollViewEnabled", viewer.IsScrollViewEnabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Embedded view
            writer.WriteStartElement("CurrentViewer");
            Control currentViewer = s_piCurrentViewer.GetValue(viewer, null) as Control;
            if (currentViewer != null)
            {
                DumpViewer(writer, currentViewer);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump state of  to XmlTextWriter.
        /// </summary>
        private void DumpDocumentViewerBase(XmlTextWriter writer, DocumentViewerBase viewer)
        {
            // Document properties
            writer.WriteStartElement("Document");
            writer.WriteAttributeString("Type", (viewer.Document != null) ? ((object)viewer.Document).GetType().FullName : "(null)");
            writer.WriteAttributeString("PageCount", viewer.PageCount.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Viewer state
            writer.WriteStartElement("State");
            writer.WriteAttributeString("MasterPageNumber", viewer.MasterPageNumber.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanGoToPreviousPage", viewer.CanGoToPreviousPage.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CanGoToNextPage", viewer.CanGoToNextPage.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // PageViews
            ReadOnlyCollection<DocumentPageView> pageViews = viewer.PageViews;
            writer.WriteStartElement("PageViews");
            writer.WriteAttributeString("Count", pageViews.Count.ToString(CultureInfo.InvariantCulture));
            for (int i = 0; i < pageViews.Count; i++)
            {
                DumpDocumentPageView(writer, pageViews[i]);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump state of DocumentPageView to XmlTextWriter.
        /// </summary>
        private void DumpDocumentPageView(XmlTextWriter writer, DocumentPageView pageView)
        {
            writer.WriteStartElement(pageView.GetType().Name);
            writer.WriteAttributeString("PageNumber", pageView.PageNumber.ToString(CultureInfo.InvariantCulture));

            // DocumentPage
            DocumentPage page = pageView.DocumentPage;
            if (page != null)
            {
                DumpDocumentPage(writer, page);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump state of FlowDocumentView to XmlTextWriter.
        /// </summary>
        private void DumpFlowDocumentView(XmlTextWriter writer, object pageView)
        {
            writer.WriteStartElement(pageView.GetType().Name);

            // IScrollInfo
            IScrollInfo isi = pageView as IScrollInfo;
            if (isi != null)
            {
                writer.WriteStartElement("IScrollInfo");
                writer.WriteAttributeString("CanVerticallyScroll", isi.CanVerticallyScroll.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("CanHorizontallyScroll", isi.CanHorizontallyScroll.ToString(CultureInfo.InvariantCulture));
                DumpSize(writer, "Extent", new Size(isi.ExtentWidth, isi.ExtentHeight));
                DumpSize(writer, "Viewport", new Size(isi.ViewportWidth, isi.ViewportHeight));
                DumpPoint(writer, "Offset", new Point(isi.HorizontalOffset, isi.VerticalOffset));
                writer.WriteEndElement();
            }

            // DocumentPage
            DocumentPage page = s_piDocumentPage.GetValue(pageView, null) as DocumentPage;
            if (page != null)
            {
                DumpDocumentPage(writer, page);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump state of DocumentPage to XmlTextWriter.
        /// </summary>
        private void DumpDocumentPage(XmlTextWriter writer, DocumentPage page)
        {
            writer.WriteStartElement(page.GetType().Name);
            DumpSize(writer, "Size", page.Size);
            DumpRect(writer, "BleedBox", page.BleedBox);
            DumpRect(writer, "ContentBox", page.ContentBox);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump size.
        /// </summary>
        internal static void DumpSize(XmlTextWriter writer, string tagName, Size size)
        {
            writer.WriteStartElement(tagName);
            writer.WriteAttributeString("Width", size.Width.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Height", size.Height.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump point.
        /// </summary>
        internal static void DumpPoint(XmlTextWriter writer, string tagName, Point point)
        {
            writer.WriteStartElement(tagName);
            writer.WriteAttributeString("Left", point.X.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Top", point.Y.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        /// <summary>
        /// Dump rectangle.
        /// </summary>
        internal static void DumpRect(XmlTextWriter writer, string tagName, Rect rect)
        {
            writer.WriteStartElement(tagName);
            writer.WriteAttributeString("Left", rect.Left.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Top", rect.Top.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Width", rect.Width.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Height", rect.Height.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        private static PropertyInfo s_piRenderScope;
        private static PropertyInfo s_piDocumentPage;
        private static PropertyInfo s_piCurrentViewer;
    }
}
