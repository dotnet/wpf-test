// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextFlow with figures. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextFlow with figures.
    // ----------------------------------------------------------------------
    internal sealed class TextPanelFiguresSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelFiguresSuite() : base("TextPanelFigures")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(LoadContent),   new DrtTest(VerifyLayoutCreate),
                new DrtTest(HCenter),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(HRight),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(VCenter),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(HCenter),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(HLeft),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(VBottom),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(HCenter),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(HRight),        new DrtTest(VerifyLayoutFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load content from xaml file.
        // ------------------------------------------------------------------
        private void LoadContent()
        {
            _testName = "";
            LoadContentFromXaml();
            _fdsv = _contentRoot.Child as FlowDocumentScrollViewer;
            s_figure1 = (Figure)DRT.FindElementByID("Figure1");
            s_figure2 = (Figure)DRT.FindElementByID("Figure2");
            s_figure3 = (Figure)DRT.FindElementByID("Figure3");
            s_figure4 = (Figure)DRT.FindElementByID("Figure4");
            s_figure5 = (Figure)DRT.FindElementByID("Figure5");
        }

        // ------------------------------------------------------------------
        // HLeft
        // ------------------------------------------------------------------
        private void HLeft()
        {
            s_figure1.HorizontalAnchor = FigureHorizontalAnchor.ContentLeft;
            s_figure2.HorizontalAnchor = FigureHorizontalAnchor.ContentLeft;
            s_figure3.HorizontalAnchor = FigureHorizontalAnchor.ColumnLeft;
        }

        // ------------------------------------------------------------------
        // HCenter
        // ------------------------------------------------------------------
        private void HCenter()
        {
            s_figure1.HorizontalAnchor = FigureHorizontalAnchor.ContentCenter;
            s_figure2.HorizontalAnchor = FigureHorizontalAnchor.ContentCenter;
            s_figure3.HorizontalAnchor = FigureHorizontalAnchor.ColumnCenter;
        }

        // ------------------------------------------------------------------
        // HRight
        // ------------------------------------------------------------------
        private void HRight()
        {
            s_figure1.HorizontalAnchor = FigureHorizontalAnchor.ContentRight;
            s_figure2.HorizontalAnchor = FigureHorizontalAnchor.ContentRight;
            s_figure3.HorizontalAnchor = FigureHorizontalAnchor.ColumnRight;
        }

        // ------------------------------------------------------------------
        // VCenter
        // ------------------------------------------------------------------
        private void VCenter()
        {
            s_figure1.VerticalAnchor = FigureVerticalAnchor.ContentCenter;
            s_figure2.VerticalAnchor = FigureVerticalAnchor.ContentCenter;
        }

        // ------------------------------------------------------------------
        // VBottom
        // ------------------------------------------------------------------
        private void VBottom()
        {
            s_figure1.VerticalAnchor = FigureVerticalAnchor.ContentBottom;
            s_figure2.VerticalAnchor = FigureVerticalAnchor.ContentBottom;
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private FlowDocumentScrollViewer _fdsv;
        private static Figure s_figure1;
        private static Figure s_figure2;
        private static Figure s_figure3;
        private static Figure s_figure4;
        private static Figure s_figure5;
    }
}
