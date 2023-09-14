// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextView associated with TextFlow. 
//
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextView associated with TextFlow.
    // ----------------------------------------------------------------------
    internal sealed class TextViewTextFlowSuite : TextViewSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextViewTextFlowSuite() : base("TextViewTextFlow")
        {
            this.Contact = "Microsoft";
            _viewSize = new Size(700, 500);
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(CreateEmpty),           new DrtTest(VerifyEmpty),
                new DrtTest(LoadSimple),            new DrtTest(VerifySimple),
                new DrtTest(LoadMultiLine),         new DrtTest(VerifyMultiLine),
                new DrtTest(LoadBidi),              new DrtTest(VerifyBidi),
                new DrtTest(LoadParas),             new DrtTest(VerifyParas),
                new DrtTest(LoadCaretNavigationEmpty), new DrtTest(VerifyCaretNavigationEmpty),
                new DrtTest(LoadCaretNavigationTwoLines), new DrtTest(VerifyCaretNavigationTwoLines),
                new DrtTest(LoadCaretNavigationBorder), new DrtTest(VerifyCaretNavigationBorder),
                new DrtTest(LoadCaretNavigationInline), new DrtTest(VerifyCaretNavigationInline),
                new DrtTest(LoadCaretNavigationOverEmptyLine), new DrtTest(VerifyCaretNavigationOverEmptyLine),
                new DrtTest(LoadCaretNavigationInternational), new DrtTest(VerifyCaretNavigationInternational),
                new DrtTest(LoadCaretNavigationFiguresFloaters), new DrtTest(VerifyCaretNavigationFiguresFloaters),
                new DrtTest(LoadCaretNavigationHebrew), new DrtTest(VerifyCaretNavigationHebrew),
            };
        }

        // ------------------------------------------------------------------
        // CreateEmpty
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "Empty";

            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph());
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);

            _container = new Border();
            _container.Background = Brushes.White;
            _container.Child = _fdsv;
            _container.Width = _viewSize.Width;
            _container.Height = _viewSize.Height;

            _contentRoot.Child = _container;
        }

        // ------------------------------------------------------------------
        // Verify TextView for empty content.
        // ------------------------------------------------------------------
        private void VerifyEmpty()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            double lineHeight = tv.GetRectangleFromTextPosition(tc.Start).Height;

            // GetTextPositionFromPoint 
            PositionFromPointTestDesc[] testsGetTextPositionFromPoint = new PositionFromPointTestDesc[]{
                new PositionFromPointTestDesc(new Point(-10,-10), false, LogicalDirection.Forward, -1),
                new PositionFromPointTestDesc(new Point(-10,-10), true, LogicalDirection.Forward, 1),
                new PositionFromPointTestDesc(new Point(50,lineHeight/2), true, LogicalDirection.Forward, 1),
                new PositionFromPointTestDesc(new Point(1000, 1000), true, LogicalDirection.Forward, 1),
            };
            VerifyTextPositionFromPoint(testsGetTextPositionFromPoint, tv);

            // GetPositionAtNextLine
            PositionAtNextLineTestDesc[] testsGetPositionAtNextLine = new PositionAtNextLineTestDesc[]{
                new PositionAtNextLineTestDesc(0, LogicalDirection.Forward, 50, 5, 0, LogicalDirection.Forward, 50, 0),
                new PositionAtNextLineTestDesc(0, LogicalDirection.Backward, 50, 5, 0, LogicalDirection.Backward, 50, 0),
            };
            VerifyPositionAtNextLine(testsGetPositionAtNextLine, tv);
        }

        // ------------------------------------------------------------------
        // LoadSimple
        // ------------------------------------------------------------------
        private void LoadSimple()
        {
            _testName = "Simple";
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph(new Run("Hello World!")));
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // VerifySimple
        // ------------------------------------------------------------------
        private void VerifySimple()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            double lineHeight = tv.GetRectangleFromTextPosition(tc.Start).Height;

            // GetTextPositionFromPoint 
            PositionFromPointTestDesc[] testsGetTextPositionFromPoint = new PositionFromPointTestDesc[]{
                new PositionFromPointTestDesc(new Point(-10,-10), false, LogicalDirection.Forward, -1),
                new PositionFromPointTestDesc(new Point(-10,-10), true, LogicalDirection.Forward, 0),
                new PositionFromPointTestDesc(new Point(25,lineHeight/2), true, LogicalDirection.Forward, 4),
                new PositionFromPointTestDesc(new Point(1000, 1000), true, LogicalDirection.Backward, 12),
            };
            VerifyTextPositionFromPoint(testsGetTextPositionFromPoint, tv);

            // GetPositionAtNextLine
            PositionAtNextLineTestDesc[] testsGetPositionAtNextLine = new PositionAtNextLineTestDesc[]{
                new PositionAtNextLineTestDesc(0, LogicalDirection.Forward, 25, 5, 0, LogicalDirection.Forward, 25, 0),
                new PositionAtNextLineTestDesc(6, LogicalDirection.Forward, 25, 5, 6, LogicalDirection.Forward, 25, 0),
                new PositionAtNextLineTestDesc(12, LogicalDirection.Backward, 25, 5, 12, LogicalDirection.Backward, 25, 0),
                new PositionAtNextLineTestDesc(12, LogicalDirection.Backward, double.NaN, 5, 12, LogicalDirection.Backward, double.NaN, 0),
            };
            VerifyPositionAtNextLine(testsGetPositionAtNextLine, tv);
        }

        // ------------------------------------------------------------------
        // LoadMultiLine
        // ------------------------------------------------------------------
        private void LoadMultiLine()
        {
            _testName = "MultiLine";
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph(new Run("Hello \r\nWorld!\r\n")));
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // VerifyMultiLine
        // ------------------------------------------------------------------
        private void VerifyMultiLine()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            double lineHeight = tv.GetRectangleFromTextPosition(tc.Start).Height;

            // GetTextPositionFromPoint 
            PositionFromPointTestDesc[] testsGetTextPositionFromPoint = new PositionFromPointTestDesc[]{
                new PositionFromPointTestDesc(new Point(200,lineHeight*0.5), true, LogicalDirection.Backward, 6),
                new PositionFromPointTestDesc(new Point(1,lineHeight*1.5), true, LogicalDirection.Forward, 8),
                new PositionFromPointTestDesc(new Point(200,lineHeight*1.5), true, LogicalDirection.Backward, 14),
                new PositionFromPointTestDesc(new Point(1,lineHeight*2.5), true, LogicalDirection.Forward, 16),
            };
            VerifyTextPositionFromPoint(testsGetTextPositionFromPoint, tv);

            // GetPositionAtNextLine
            PositionAtNextLineTestDesc[] testsGetPositionAtNextLine = new PositionAtNextLineTestDesc[]{
                new PositionAtNextLineTestDesc(1, LogicalDirection.Forward, 15, 3, 16, LogicalDirection.Forward, 15, 2),
                new PositionAtNextLineTestDesc(12, LogicalDirection.Forward, 15, -1, 2, LogicalDirection.Backward, 15, -1),
                new PositionAtNextLineTestDesc(16, LogicalDirection.Forward, 15, -5, 2, LogicalDirection.Backward, 15, -2),
            };
            VerifyPositionAtNextLine(testsGetPositionAtNextLine, tv);
        }

        // ------------------------------------------------------------------
        // Load bidi content to TextFlow.
        // ------------------------------------------------------------------
        private void LoadBidi()
        {
            _testName = "Bidi";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // VerifyBidi
        // ------------------------------------------------------------------
        private void VerifyBidi()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            double lineHeight = tv.GetRectangleFromTextPosition(tc.Start).Height;

            // GetTextPositionFromPoint 
            PositionFromPointTestDesc[] testsGetTextPositionFromPoint = new PositionFromPointTestDesc[]{
                new PositionFromPointTestDesc(new Point(0,lineHeight*4.5), true, LogicalDirection.Backward, 52),
                new PositionFromPointTestDesc(new Point(300,lineHeight*4.5), true, LogicalDirection.Backward, 67),
            };
            VerifyTextPositionFromPoint(testsGetTextPositionFromPoint, tv);
        }

        // ------------------------------------------------------------------
        // Load content with paragraphs to TextFlow.
        // ------------------------------------------------------------------
        private void LoadParas()
        {
            _testName = "Paras";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // VerifyParas
        // ------------------------------------------------------------------
        private void VerifyParas()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            // GetPositionAtNextLine
            PositionAtNextLineTestDesc[] testsGetPositionAtNextLine = new PositionAtNextLineTestDesc[]{
                new PositionAtNextLineTestDesc(1, LogicalDirection.Forward, 0, 1, 3, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(3, LogicalDirection.Forward, 0, 1, 15, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(15, LogicalDirection.Forward, 0, 1, 28, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(28, LogicalDirection.Forward, 0, 1, 39, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(39, LogicalDirection.Forward, 0, 1, 51, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(51, LogicalDirection.Forward, 0, 1, 62, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(62, LogicalDirection.Forward, 0, 1, 65, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(65, LogicalDirection.Forward, 0, 1, 73, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(73, LogicalDirection.Forward, 58, 1, 79, LogicalDirection.Backward, 58, 1),
                new PositionAtNextLineTestDesc(79, LogicalDirection.Forward, 0, 1, 83, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(83, LogicalDirection.Forward, 0, 1, 85, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(85, LogicalDirection.Forward, 0, 1, 109, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(109, LogicalDirection.Forward, 0, 1, 114, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(114, LogicalDirection.Forward, 0, 1, 115, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(115, LogicalDirection.Forward, 0, 1, 137, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(137, LogicalDirection.Forward, 0, 1, 159, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(159, LogicalDirection.Forward, 0, 1, 164, LogicalDirection.Forward, 0, 1),
                new PositionAtNextLineTestDesc(164, LogicalDirection.Forward, 0, 1, 164, LogicalDirection.Forward, 0, 0),
                new PositionAtNextLineTestDesc(164, LogicalDirection.Forward, 0, -4, 114, LogicalDirection.Forward, 0, -4),
                new PositionAtNextLineTestDesc(114, LogicalDirection.Forward, 0, -5, 73, LogicalDirection.Forward, 0, -5),
                new PositionAtNextLineTestDesc(73, LogicalDirection.Forward, 0, -3, 51, LogicalDirection.Forward, 0, -3),
                new PositionAtNextLineTestDesc(51, LogicalDirection.Forward, 0, -10, 1, LogicalDirection.Forward, 0, -5),
                new PositionAtNextLineTestDesc(1, LogicalDirection.Forward, 55, 16, 159, LogicalDirection.Forward, 55, 16),
            };
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationEmpty
        // ------------------------------------------------------------------
        private void LoadCaretNavigationEmpty()
        {
            _testName = "CaretNavigationEmpty";
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph());
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationEmpty
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationEmpty()
        {
            TextView tv;
            TextContainer tc;
            GetServices(out tc, out tv);

            int endOffset = _fdsv.Document.ContentStart.GetOffsetToPosition(_fdsv.Document.ContentEnd);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Forward, LogicalDirection.Forward, 0, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Backward, LogicalDirection.Backward, 0, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(endOffset, LogicalDirection.Forward, LogicalDirection.Forward, endOffset, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(endOffset, LogicalDirection.Backward, LogicalDirection.Backward, endOffset, LogicalDirection.Backward)
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(0, LogicalDirection.Forward, 0, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(0, LogicalDirection.Backward, 0, LogicalDirection.Backward),
                new BackspaceCaretUnitPositionTestDesc(endOffset, LogicalDirection.Forward, endOffset, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(endOffset, LogicalDirection.Backward, endOffset, LogicalDirection.Backward),
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(0, LogicalDirection.Forward, true),
                new IsAtCaretUnitBoundaryTestDesc(endOffset, LogicalDirection.Backward, true),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationTwoLines
        // ------------------------------------------------------------------
        private void LoadCaretNavigationTwoLines()
        {
            _testName = "CaretNavigationTwoLines";
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph(new Run("As famous bouldering routes go up indoors, they leave trees and crumbly rock behind. Imitation may be the sincerest form of flattery, but can rock gyms really replicate nature?")));
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationTwoLines
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationTwoLines()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(115, LogicalDirection.Forward, LogicalDirection.Forward, 116, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(116, LogicalDirection.Backward, LogicalDirection.Forward, 117, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(116, LogicalDirection.Forward, LogicalDirection.Backward, 115, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(117, LogicalDirection.Forward, LogicalDirection.Backward, 116, LogicalDirection.Backward)
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(116, LogicalDirection.Forward, 115, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(117, LogicalDirection.Forward, 116, LogicalDirection.Backward)
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(116, LogicalDirection.Forward, true),
                new IsAtCaretUnitBoundaryTestDesc(116, LogicalDirection.Backward, true),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationBorder
        // ------------------------------------------------------------------
        private void LoadCaretNavigationBorder()
        {
            _testName = "CaretNavigationBorder";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationBorder
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationBorder()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(1, LogicalDirection.Forward, LogicalDirection.Forward, 2, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(20, LogicalDirection.Forward, LogicalDirection.Forward, 21, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(21, LogicalDirection.Forward, LogicalDirection.Forward, 22, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(22, LogicalDirection.Forward, LogicalDirection.Backward, 21, LogicalDirection.Forward),
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(22, LogicalDirection.Forward, 21, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(21, LogicalDirection.Forward, 20, LogicalDirection.Forward)
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(20, LogicalDirection.Forward, true),
                new IsAtCaretUnitBoundaryTestDesc(21, LogicalDirection.Backward, true),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationInline
        // ------------------------------------------------------------------
        private void LoadCaretNavigationInline()
        {
            _testName = "CaretNavigationInline";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationInline
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationInline()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(1, LogicalDirection.Forward, LogicalDirection.Forward, 2, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(2, LogicalDirection.Forward, LogicalDirection.Backward, 1, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(11, LogicalDirection.Forward, LogicalDirection.Forward, 12, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(12, LogicalDirection.Forward, LogicalDirection.Forward, 13, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(13, LogicalDirection.Forward, LogicalDirection.Forward, 14, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(14, LogicalDirection.Forward, LogicalDirection.Backward, 13, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(13, LogicalDirection.Forward, LogicalDirection.Backward, 12, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(12, LogicalDirection.Forward, LogicalDirection.Backward, 11, LogicalDirection.Forward),
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(2, LogicalDirection.Forward, 1, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(3, LogicalDirection.Forward, 2, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(14, LogicalDirection.Forward, 13, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(13, LogicalDirection.Forward, 12, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(12, LogicalDirection.Forward, 11, LogicalDirection.Forward),
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationOverEmptyLine
        // ------------------------------------------------------------------
        private void LoadCaretNavigationOverEmptyLine()
        {
            _testName = "CaretNavigationOverEmptyLine";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationOverEmptyLine
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationOverEmptyLine()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(4, LogicalDirection.Forward, LogicalDirection.Forward, 5, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(5, LogicalDirection.Forward, LogicalDirection.Forward, 6, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(6, LogicalDirection.Forward, LogicalDirection.Forward, 7, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(7, LogicalDirection.Forward, LogicalDirection.Backward, 6, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(6, LogicalDirection.Forward, LogicalDirection.Backward, 5, LogicalDirection.Forward),
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(7, LogicalDirection.Forward, 6, LogicalDirection.Backward),
                new BackspaceCaretUnitPositionTestDesc(6, LogicalDirection.Forward, 5, LogicalDirection.Forward),
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationInetrnational
        // ------------------------------------------------------------------
        private void LoadCaretNavigationInternational()
        {
            _testName = "CaretNavigationInternational";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationInternational
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationInternational()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(1, LogicalDirection.Forward, LogicalDirection.Forward, 3, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(3, LogicalDirection.Forward, LogicalDirection.Backward, 1, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(12, LogicalDirection.Forward, LogicalDirection.Forward, 14, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(14, LogicalDirection.Forward, LogicalDirection.Backward, 12, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(23, LogicalDirection.Forward, LogicalDirection.Forward, 25, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(25, LogicalDirection.Backward, LogicalDirection.Backward, 23, LogicalDirection.Forward),
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(3, LogicalDirection.Forward, 2, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(2, LogicalDirection.Forward, 1, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(14, LogicalDirection.Forward, 13, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(13, LogicalDirection.Forward, 12, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(25, LogicalDirection.Forward, 24, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(24, LogicalDirection.Forward, 23, LogicalDirection.Forward),
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(2, LogicalDirection.Forward, false),
                new IsAtCaretUnitBoundaryTestDesc(13, LogicalDirection.Backward, false),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);

        }

        // ------------------------------------------------------------------
        // Load CaretNavigationFiguresFloaters
        // ------------------------------------------------------------------
        private void LoadCaretNavigationFiguresFloaters()
        {
            _testName = "CaretNavigationFiguresFloaters";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationFiguresFloaters
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationFiguresFloaters()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(5, LogicalDirection.Forward, LogicalDirection.Forward, 6, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(8, LogicalDirection.Forward, LogicalDirection.Forward, 9, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(12, LogicalDirection.Forward, LogicalDirection.Forward, 13, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(13, LogicalDirection.Forward, LogicalDirection.Forward, 14, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(16, LogicalDirection.Forward, LogicalDirection.Forward, 17, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(17, LogicalDirection.Backward, LogicalDirection.Forward, 18, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(17, LogicalDirection.Forward, LogicalDirection.Backward, 16, LogicalDirection.Forward),
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);
        }

        // ------------------------------------------------------------------
        // Get TextView and TextContainer services.
        // ------------------------------------------------------------------
        private void GetServices(out TextContainer tc, out TextView tv)
        {
            object tcInstance = ((IServiceProvider)_fdsv).GetService(TextContainer.Type);
            DRT.Assert(tcInstance != null, this.TestName + ": TextFlow does not expose ITextContainer service.");
            tc = new TextContainer(tcInstance);
            object tvInstance = ((IServiceProvider)_fdsv).GetService(TextView.Type);
            DRT.Assert(tvInstance != null, this.TestName + ": TextFlow does not expose TextView service.");
            tv = new TextView(tvInstance);
            DRT.Assert(tc.Instance == tv.TextContainer.Instance, this.TestName + ": ITextContainer and ITextView services are out of sync.");
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationInetrnational
        // ------------------------------------------------------------------
        private void LoadCaretNavigationHebrew()
        {
            _testName = "CaretNavigationHebrew";
            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationInternational
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationHebrew()
        {
            TextContainer tc;
            TextView tv;
            GetServices(out tc, out tv);

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(3, LogicalDirection.Forward, true),
                new IsAtCaretUnitBoundaryTestDesc(4, LogicalDirection.Backward, true),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private Border _container;
        private FlowDocumentScrollViewer _fdsv;
        private Size _viewSize;
    }
}
