// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextFlow with multi-column layout. 
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
    // Test suite for TextFlow with multi-column layout.
    // ----------------------------------------------------------------------
    internal sealed class TextPanelColumnsSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelColumnsSuite() : base("TextPanelColumns")
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
                new DrtTest(LoadContent),       new DrtTest(VerifyLayoutCreate),
                new DrtTest(Count),             new DrtTest(VerifyLayoutAppend),
                new DrtTest(BalancingOn),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(Width),             new DrtTest(VerifyLayoutAppend),
                new DrtTest(PolicyStrict),      new DrtTest(VerifyLayoutAppend),
                //new DrtTest(SpaceBetween),    new DrtTest(VerifyLayoutAppend),    // default
                new DrtTest(SpaceStart),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(SpaceEnd),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(PolicyFlexible),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(SpaceStart),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(SpaceBetween),      new DrtTest(VerifyLayoutAppend),
                new DrtTest(Gap),               new DrtTest(VerifyLayoutAppend),
                new DrtTest(Rule),              new DrtTest(VerifyLayoutAppend),
                new DrtTest(ColChange),         new DrtTest(VerifyLayoutAppend),
                //new DrtTest(Breaks),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(KeepWithNextTest),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(KeepTogetherTest),  new DrtTest(VerifyLayoutAppend),
                //new DrtTest(WidowsAndOrphans),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(Width),             new DrtTest(VerifyLayoutAppend),
                new DrtTest(PolicyStrict),      new DrtTest(VerifyLayoutAppend),
                //new DrtTest(SpaceBetween),    new DrtTest(VerifyLayoutAppend),    // default
                new DrtTest(SpaceStart),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(SpaceEnd),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(PolicyFlexible),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(SpaceStart),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(SpaceBetween),      new DrtTest(VerifyLayoutAppend),
                new DrtTest(Gap),               new DrtTest(VerifyLayoutAppend),
                new DrtTest(Rule),              new DrtTest(VerifyLayoutAppend),
                new DrtTest(ColChange),         new DrtTest(VerifyLayoutAppend),
                //new DrtTest(Breaks),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(KeepWithNextTest),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(KeepTogetherTest),  new DrtTest(VerifyLayoutFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load content from xaml file.
        // ------------------------------------------------------------------
        private void LoadContent()
        {
            _testName = "";
            LoadContentFromXaml();
            _textPanel = _contentRoot.Child as TextFlow;
            _block1 = (Paragraph)DRT.FindElementByID("Block1");
            _block2 = (Paragraph)DRT.FindElementByID("Block2");
            _block3 = (Paragraph)DRT.FindElementByID("Block3");
        }

        // ------------------------------------------------------------------
        // Count
        // ------------------------------------------------------------------
        private void Count()
        {
            _textPanel.ColumnCount = 2;
        }

        // ------------------------------------------------------------------
        // BalancingOn
        // ------------------------------------------------------------------
        private void BalancingOn()
        {
            _textPanel.BalanceColumns = true;
        }

        // ------------------------------------------------------------------
        // Width
        // ------------------------------------------------------------------
        private void Width()
        {
            _textPanel.ColumnWidth = 382.5; // 45%
        }

        // ------------------------------------------------------------------
        // PolicyStrict
        // ------------------------------------------------------------------
        private void PolicyStrict()
        {
            _textPanel.FlexibleColumnWidth = false;
        }

        // ------------------------------------------------------------------
        // PolicyFlexible
        // ------------------------------------------------------------------
        private void PolicyFlexible()
        {
            _textPanel.FlexibleColumnWidth = true;
        }

        // ------------------------------------------------------------------
        // SpaceStart
        // ------------------------------------------------------------------
        private void SpaceStart()
        {
            _textPanel.ExcessColumnSpaceDistribution = ColumnSpaceDistribution.Left;
        }

        // ------------------------------------------------------------------
        // SpaceEnd
        // ------------------------------------------------------------------
        private void SpaceEnd()
        {
            _textPanel.ExcessColumnSpaceDistribution = ColumnSpaceDistribution.Right;
        }

        // ------------------------------------------------------------------
        // SpaceBetween
        // ------------------------------------------------------------------
        private void SpaceBetween()
        {
            _textPanel.ExcessColumnSpaceDistribution = ColumnSpaceDistribution.Between;
        }

        // ------------------------------------------------------------------
        // Gap
        // ------------------------------------------------------------------
        private void Gap()
        {
            _textPanel.ColumnGap = 42.5; // 5%
        }

        // ------------------------------------------------------------------
        // Rule
        // ------------------------------------------------------------------
        private void Rule()
        {
            _textPanel.ColumnRuleWidth = 2;
            _textPanel.ColumnRuleBrush = Brushes.Khaki;
        }

        // ------------------------------------------------------------------
        // ColChange
        // ------------------------------------------------------------------
        private void ColChange()
        {
            _textPanel.ColumnWidth = 238; // 28%
            _textPanel.ColumnCount = 5;
            _textPanel.ColumnRuleBrush = Brushes.Black;
            _textPanel.ColumnRuleWidth = 1;
        }
/*
        // ------------------------------------------------------------------
        // Breaks
        // ------------------------------------------------------------------
        private void Breaks()
        {
            _block1.SetValue(TextFlow.ColumnBreakBeforeProperty, true);
            _block2.SetValue(TextFlow.PageBreakBeforeProperty, true);
        }
*/
        // ------------------------------------------------------------------
        // KeepWithNextTest
        // ------------------------------------------------------------------
        private void KeepWithNextTest()
        {
            //_block1.SetValue(TextFlow.ColumnBreakBeforeProperty, false);
            //_block2.SetValue(TextFlow.PageBreakBeforeProperty, false);
            _block3.KeepWithNext = true;
        }

        // ------------------------------------------------------------------
        // KeepTogetherTest
        // ------------------------------------------------------------------
        private void KeepTogetherTest()
        {
            _block3.KeepWithNext = false;
            _block2.KeepTogether = true;
        }
/*
        // ------------------------------------------------------------------
        // WidowsAndOrphans
        // ------------------------------------------------------------------
        private void WidowsAndOrphans()
        {
            _textPanel.ColumnCount = 2;
            TextFlow.SetOrphans(_textPanel, 2);
            TextFlow.SetWidows(_textPanel, 2);
            // Clear local properties
            _textPanel.ClearValue(TextFlow.ColumnWidthProperty);
            _textPanel.ClearValue(TextFlow.FlexibleColumnWidthProperty);
            _textPanel.ClearValue(TextFlow.ExcessColumnSpaceDistributionProperty);
            _textPanel.ClearValue(TextFlow.ColumnGapProperty);
            _textPanel.ClearValue(TextFlow.ColumnRuleWidthProperty);
            _textPanel.ClearValue(TextFlow.ColumnRuleBrushProperty);
            _block1.ClearValue(TextFlow.ColumnBreakBeforeProperty);
            _block2.ClearValue(TextFlow.PageBreakBeforeProperty);
            _block2.ClearValue(TextFlow.KeepTogetherProperty);
            _block3.ClearValue(TextFlow.KeepWithNextProperty);
        }
*/

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private TextFlow _textPanel;
        private Paragraph _block1;
        private Paragraph _block2;
        private Paragraph _block3;
    }
}
