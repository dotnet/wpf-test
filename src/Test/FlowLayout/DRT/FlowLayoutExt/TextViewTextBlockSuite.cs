// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextView associated with TextBlock. 
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
    // Test suite for TextView associated with TextBlock.
    // ----------------------------------------------------------------------
    internal sealed class TextViewTextBlockSuite : TextViewSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextViewTextBlockSuite()
            : base("TextViewTextBlock")
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
                new DrtTest(LoadCaretNavigationEmpty), new DrtTest(VerifyCaretNavigationEmpty),
                new DrtTest(LoadCaretNavigationTwoLines), new DrtTest(VerifyCaretNavigationTwoLines),
                new DrtTest(LoadCaretNavigationBorder), new DrtTest(VerifyCaretNavigationBorder),
                new DrtTest(LoadCaretNavigationInline), new DrtTest(VerifyCaretNavigationInline),
                new DrtTest(LoadCaretNavigationOverEmptyLine), new DrtTest(VerifyCaretNavigationOverEmptyLine),
                new DrtTest(LoadCaretNavigationInternational), new DrtTest(VerifyCaretNavigationInternational),
            };
        }

        // ------------------------------------------------------------------
        // CreateEmpty
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "Empty";

            _textBlock = new TextBlock();

            // Touch TextBlock content to force it to switch from SimpleContent to ComplexContent
            if (_textBlock.Inlines.FirstInline != null)
            {
                DRT.Assert(false, "TextBlock is suposed to be empty");
            }

            _container = new Border();
            _container.Background = Brushes.White;
            _container.Child = _textBlock;
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
                new PositionFromPointTestDesc(new Point(-10,-10), true, LogicalDirection.Forward, 0),
                new PositionFromPointTestDesc(new Point(50,lineHeight/2), true, LogicalDirection.Forward, 0),
                new PositionFromPointTestDesc(new Point(1000, 1000), true, LogicalDirection.Forward, 0),
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
            _textBlock.Text = "Hello World!";
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
            _textBlock.Text = "Hello \r\nWorld!\r\n";
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
            // GetPositionAtNextLine
            PositionAtNextLineTestDesc[] testsGetPositionAtNextLine = new PositionAtNextLineTestDesc[]{
                new PositionAtNextLineTestDesc(1, LogicalDirection.Forward, 15, 3, 16, LogicalDirection.Forward, 15, 2),
                new PositionAtNextLineTestDesc(12, LogicalDirection.Forward, 15, -1, 2, LogicalDirection.Backward, 15, -1),
                new PositionAtNextLineTestDesc(16, LogicalDirection.Forward, 15, -5, 2, LogicalDirection.Backward, 15, -2),
            };
        }

        // ------------------------------------------------------------------
        // Load bidi content to Text.
        // ------------------------------------------------------------------
        private void LoadBidi()
        {
            _testName = "Bidi";
            _textBlock = LoadFromXaml(this.TestName + ".xaml") as TextBlock;
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _textBlock;
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
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationEmpty
        // ------------------------------------------------------------------
        private void LoadCaretNavigationEmpty()
        {
            _testName = "CaretNavigationEmpty";
            _textBlock.Text = "";
        }

        // ------------------------------------------------------------------
        // Verify CaretNavigationEmpty
        // ------------------------------------------------------------------
        private void VerifyCaretNavigationEmpty()
        {
            TextView tv;
            TextContainer tc;
            GetServices(out tc, out tv);
    
            int endOffset = _textBlock.ContentStart.GetOffsetToPosition(_textBlock.ContentEnd);

            NextCaretUnitPositionTestDesc[] testsGetNextCaretUnitPosition = new NextCaretUnitPositionTestDesc[]{
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Forward, LogicalDirection.Forward, 0, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Backward, LogicalDirection.Backward, 0, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(endOffset, LogicalDirection.Forward, LogicalDirection.Forward, endOffset, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(endOffset, LogicalDirection.Backward, LogicalDirection.Backward, endOffset, LogicalDirection.Backward)
            };

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(0, LogicalDirection.Forward, 0, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(0, LogicalDirection.Backward, 0, LogicalDirection.Backward),
                new BackspaceCaretUnitPositionTestDesc(endOffset, LogicalDirection.Forward, endOffset, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(endOffset, LogicalDirection.Backward, endOffset, LogicalDirection.Backward),
            };

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(0, LogicalDirection.Forward, true),
                new IsAtCaretUnitBoundaryTestDesc(endOffset, LogicalDirection.Backward, false),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationTwoLines
        // ------------------------------------------------------------------
        private void LoadCaretNavigationTwoLines()
        {
            _testName = "CaretNavigationTwoLines";
            _textBlock.Text = "As famous bouldering routes go up indoors, they leave trees and crumbly rock behind. Imitation may be the sincerest form of flattery, but can rock gyms really replicate nature?";
            _textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
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

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(116, LogicalDirection.Forward, 115, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(117, LogicalDirection.Forward, 116, LogicalDirection.Backward)
            };

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
            _textBlock = LoadFromXaml(this.TestName + ".xaml") as TextBlock;
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _textBlock;
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
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Forward, LogicalDirection.Forward, 1, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(15, LogicalDirection.Forward, LogicalDirection.Forward, 16, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(16, LogicalDirection.Forward, LogicalDirection.Forward, 17, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(17, LogicalDirection.Forward, LogicalDirection.Backward, 16, LogicalDirection.Forward),
            };

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(17, LogicalDirection.Forward, 16, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(16, LogicalDirection.Forward, 15, LogicalDirection.Forward)
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(15, LogicalDirection.Forward, true),
                new IsAtCaretUnitBoundaryTestDesc(16, LogicalDirection.Backward, true),
            };
            VerifyIsAtCaretUnitBoundary(testsIsAtCaretUnitBoundary, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationInline
        // ------------------------------------------------------------------
        private void LoadCaretNavigationInline()
        {
            _testName = "CaretNavigationInline";
            _textBlock = LoadFromXaml(this.TestName + ".xaml") as TextBlock;
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _textBlock;
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
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Forward, LogicalDirection.Forward, 3, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(1, LogicalDirection.Forward, LogicalDirection.Backward, 1, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(2, LogicalDirection.Forward, LogicalDirection.Backward, 2, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(10, LogicalDirection.Forward, LogicalDirection.Forward, 11, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(11, LogicalDirection.Forward, LogicalDirection.Forward, 15, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(12, LogicalDirection.Forward, LogicalDirection.Forward, 15, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(13, LogicalDirection.Forward, LogicalDirection.Backward, 10, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(12, LogicalDirection.Forward, LogicalDirection.Backward, 10, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(11, LogicalDirection.Forward, LogicalDirection.Backward, 10, LogicalDirection.Forward),
            };
            VerifyNextCaretUnitPosition(testsGetNextCaretUnitPosition, tv);

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(1, LogicalDirection.Forward, 1, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(2, LogicalDirection.Forward, 2, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(13, LogicalDirection.Forward, 10, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(12, LogicalDirection.Forward, 10, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(11, LogicalDirection.Forward, 10, LogicalDirection.Forward),
            };
            VerifyBackspaceCaretUnitPosition(testsGetBackspaceCaretUnitPosition, tv);
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationOverEmptyLine
        // ------------------------------------------------------------------
        private void LoadCaretNavigationOverEmptyLine()
        {
            _testName = "CaretNavigationOverEmptyLine";
            _textBlock = LoadFromXaml(this.TestName + ".xaml") as TextBlock;
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _textBlock;
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
                new NextCaretUnitPositionTestDesc(3, LogicalDirection.Forward, LogicalDirection.Forward, 4, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(4, LogicalDirection.Forward, LogicalDirection.Forward, 5, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(5, LogicalDirection.Forward, LogicalDirection.Forward, 6, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(6, LogicalDirection.Forward, LogicalDirection.Backward, 5, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(5, LogicalDirection.Forward, LogicalDirection.Backward, 4, LogicalDirection.Forward),
            };

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(6, LogicalDirection.Forward, 5, LogicalDirection.Backward),
                new BackspaceCaretUnitPositionTestDesc(5, LogicalDirection.Forward, 4, LogicalDirection.Forward),
            };
        }

        // ------------------------------------------------------------------
        // Load CaretNavigationInternational
        // ------------------------------------------------------------------
        private void LoadCaretNavigationInternational()
        {
            _testName = "CaretNavigationInternational";
            _textBlock = LoadFromXaml(this.TestName + ".xaml") as TextBlock;
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _container.Child = _textBlock;
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
                new NextCaretUnitPositionTestDesc(0, LogicalDirection.Forward, LogicalDirection.Forward, 2, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(2, LogicalDirection.Forward, LogicalDirection.Backward, 0, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(11, LogicalDirection.Forward, LogicalDirection.Forward, 13, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(13, LogicalDirection.Forward, LogicalDirection.Backward, 11, LogicalDirection.Forward),
                new NextCaretUnitPositionTestDesc(22, LogicalDirection.Forward, LogicalDirection.Forward, 24, LogicalDirection.Backward),
                new NextCaretUnitPositionTestDesc(24, LogicalDirection.Backward, LogicalDirection.Backward, 22, LogicalDirection.Forward),
            };

            BackspaceCaretUnitPositionTestDesc[] testsGetBackspaceCaretUnitPosition = new BackspaceCaretUnitPositionTestDesc[]{
                new BackspaceCaretUnitPositionTestDesc(2, LogicalDirection.Forward, 1, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(1, LogicalDirection.Forward, 0, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(13, LogicalDirection.Forward, 12, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(12, LogicalDirection.Forward, 11, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(24, LogicalDirection.Forward, 23, LogicalDirection.Forward),
                new BackspaceCaretUnitPositionTestDesc(23, LogicalDirection.Forward, 22, LogicalDirection.Forward),
            };

            IsAtCaretUnitBoundaryTestDesc[] testsIsAtCaretUnitBoundary = new IsAtCaretUnitBoundaryTestDesc[]{
                new IsAtCaretUnitBoundaryTestDesc(1, LogicalDirection.Forward, false),
                new IsAtCaretUnitBoundaryTestDesc(12, LogicalDirection.Backward, false),
            };
        }

        // ------------------------------------------------------------------
        // Get TextView and TextContainer services.
        // ------------------------------------------------------------------
        private void GetServices(out TextContainer tc, out TextView tv)
        {
            object tcInstance = ((IServiceProvider)_textBlock).GetService(TextContainer.Type);
            DRT.Assert(tcInstance != null, this.TestName + ": TextBlock does not expose ITextContainer service.");
            tc = new TextContainer(tcInstance);
            object tvInstance = ((IServiceProvider)_textBlock).GetService(TextView.Type);
            DRT.Assert(tvInstance != null, this.TestName + ": TextBlock does not expose TextView service.");
            tv = new TextView(tvInstance);
            DRT.Assert(tc.Instance == tv.TextContainer.Instance, this.TestName + ": ITextContainer and ITextView services are out of sync.");
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private Border _container;
        private TextBlock _textBlock;
        private Size _viewSize;
    }
}
