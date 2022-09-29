// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for viewers FlowDirection tests.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Documents;             // FlowDocument
using System.Windows.Media;                 // Brushes

namespace DRT
{
    /// <summary>
    /// Test suite for viewers FlowDirection tests.
    /// </summary>
    internal class ViewerFlowDirectionTestSuite : LayoutDumpTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal ViewerFlowDirectionTestSuite() :
            base("ViewerFlowDirection")
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
                new DrtTest(FlowDocumentReaderTest),        new DrtTest(DumpCreate),
                new DrtTest(FlowDocumentPageViewerTest),    new DrtTest(DumpAppend),
                new DrtTest(FlowDocumentScrollViewerTest),  new DrtTest(DumpFinalizeAndVerify),
            };
        }

        /// <summary>
        /// Creates FlowDocumentReaders with different combinations of FlowDirection.
        /// </summary>
        private void FlowDocumentReaderTest()
        {
            FlowDocumentReader viewer;
            Grid grid = CreateGrid();

            // Viewer LTR & Document LTR
            viewer = new FlowDocumentReader();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.LeftToRight;
            viewer.Document = CreateFlowDocument(FlowDirection.LeftToRight);
            Grid.SetColumn(viewer, 1);
            Grid.SetRow(viewer, 1);
            grid.Children.Add(viewer);

            // Viewer LTR & Document RTL
            viewer = new FlowDocumentReader();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.LeftToRight;
            viewer.Document = CreateFlowDocument(FlowDirection.RightToLeft);
            Grid.SetColumn(viewer, 3);
            Grid.SetRow(viewer, 1);
            grid.Children.Add(viewer);

            // Viewer RTL & Document LTR
            viewer = new FlowDocumentReader();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.RightToLeft;
            viewer.Document = CreateFlowDocument(FlowDirection.LeftToRight);
            Grid.SetColumn(viewer, 1);
            Grid.SetRow(viewer, 3);
            grid.Children.Add(viewer);

            // Viewer RTL & Document RTL
            viewer = new FlowDocumentReader();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.RightToLeft;
            viewer.Document = CreateFlowDocument(FlowDirection.RightToLeft);
            Grid.SetColumn(viewer, 3);
            Grid.SetRow(viewer, 3);
            grid.Children.Add(viewer);

            this.Root.Child = grid;
        }

        /// <summary>
        /// Creates FlowDocumentPageViewer with different combinations of FlowDirection.
        /// </summary>
        private void FlowDocumentPageViewerTest()
        {
            FlowDocumentPageViewer viewer;
            Grid grid = CreateGrid();

            // Viewer LTR & Document LTR
            viewer = new FlowDocumentPageViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.LeftToRight;
            viewer.Document = CreateFlowDocument(FlowDirection.LeftToRight);
            Grid.SetColumn(viewer, 1);
            Grid.SetRow(viewer, 1);
            grid.Children.Add(viewer);

            // Viewer LTR & Document RTL
            viewer = new FlowDocumentPageViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.LeftToRight;
            viewer.Document = CreateFlowDocument(FlowDirection.RightToLeft);
            Grid.SetColumn(viewer, 3);
            Grid.SetRow(viewer, 1);
            grid.Children.Add(viewer);

            // Viewer RTL & Document LTR
            viewer = new FlowDocumentPageViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.RightToLeft;
            viewer.Document = CreateFlowDocument(FlowDirection.LeftToRight);
            Grid.SetColumn(viewer, 1);
            Grid.SetRow(viewer, 3);
            grid.Children.Add(viewer);

            // Viewer RTL & Document RTL
            viewer = new FlowDocumentPageViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.FlowDirection = FlowDirection.RightToLeft;
            viewer.Document = CreateFlowDocument(FlowDirection.RightToLeft);
            Grid.SetColumn(viewer, 3);
            Grid.SetRow(viewer, 3);
            grid.Children.Add(viewer);

            this.Root.Child = grid;
        }

        /// <summary>
        /// Creates FlowDocumentScrollViewer with different combinations of FlowDirection.
        /// </summary>
        private void FlowDocumentScrollViewerTest()
        {
            FlowDocumentScrollViewer viewer;
            Grid grid = CreateGrid();

            // Viewer LTR & Document LTR
            viewer = new FlowDocumentScrollViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.FlowDirection = FlowDirection.LeftToRight;
            viewer.Document = CreateFlowDocument(FlowDirection.LeftToRight);
            Grid.SetColumn(viewer, 1);
            Grid.SetRow(viewer, 1);
            grid.Children.Add(viewer);

            // Viewer LTR & Document RTL
            viewer = new FlowDocumentScrollViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.FlowDirection = FlowDirection.LeftToRight;
            viewer.Document = CreateFlowDocument(FlowDirection.RightToLeft);
            Grid.SetColumn(viewer, 3);
            Grid.SetRow(viewer, 1);
            grid.Children.Add(viewer);

            // Viewer RTL & Document LTR
            viewer = new FlowDocumentScrollViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.FlowDirection = FlowDirection.RightToLeft;
            viewer.Document = CreateFlowDocument(FlowDirection.LeftToRight);
            Grid.SetColumn(viewer, 1);
            Grid.SetRow(viewer, 3);
            grid.Children.Add(viewer);

            // Viewer RTL & Document RTL
            viewer = new FlowDocumentScrollViewer();
            viewer.FontSize = 11.0;
            viewer.FontFamily = new FontFamily("Tahoma");
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            viewer.FlowDirection = FlowDirection.RightToLeft;
            viewer.Document = CreateFlowDocument(FlowDirection.RightToLeft);
            Grid.SetColumn(viewer, 3);
            Grid.SetRow(viewer, 3);
            grid.Children.Add(viewer);

            this.Root.Child = grid;
        }

        /// <summary>
        /// Create FlowDocument.
        /// </summary>
        private FlowDocument CreateFlowDocument(FlowDirection flowDirection)
        {
            Run run = new Run("This is text this is text this is text this is text this is text this is text. This is text this is text this is text this is text this is text this is text.");
            Paragraph para = new Paragraph(run);
            FlowDocument document = new FlowDocument(para);
            document.ColumnWidth = 10000;
            document.Background = Brushes.Beige;
            document.PagePadding = new Thickness(50, 0, 0, 0);
            document.FlowDirection = flowDirection;
            return document;
        }

        /// <summary>
        /// Create Grid.
        /// </summary>
        private Grid CreateGrid()
        {
            ColumnDefinition colDefinition;
            RowDefinition rowDefinition;
            Grid grid = new Grid();
            grid.Width = 800;
            grid.Background = Brushes.LightBlue;
            // Add columns
            colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(4);
            grid.ColumnDefinitions.Add(colDefinition);
            colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(colDefinition);
            colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(4);
            grid.ColumnDefinitions.Add(colDefinition);
            colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(colDefinition);
            colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(4);
            grid.ColumnDefinitions.Add(colDefinition);
            // Add rows
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(4);
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(150);
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(4);
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(150);
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(4);
            grid.RowDefinitions.Add(rowDefinition);
            return grid;
        }
    }
}
