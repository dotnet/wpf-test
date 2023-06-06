// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for flow direction. 
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
    // Test suite for pagination.
    // ----------------------------------------------------------------------
    internal sealed class InlineFlowDirectionSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal InlineFlowDirectionSuite()
            : base("InlineFlowDirection")
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
                new DrtTest(CreateEmpty),               new DrtTest(VerifyLayoutCreate),
                new DrtTest(SetTextFlowContent),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddFirstPara),              new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo1),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo2),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo3),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo4),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo5),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo6),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextFlowDirectionCombo7),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(SetTextBlockContent),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddTextBlockInlines),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo1),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo2),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo3),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo4),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo5),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo6),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextBlockDirectionCombo7),  new DrtTest(VerifyLayoutFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load simple content.
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "";
            _container = new Border();
            _contentRoot.Child = _container;
        }

        // ------------------------------------------------------------------
        // SetTextFlowContent
        // ------------------------------------------------------------------
        private void SetTextFlowContent()
        {
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument();
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            _container.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // AddFirstPara
        // ------------------------------------------------------------------
        private void AddFirstPara()
        {
            _para1 = new Paragraph();
            _para1.Inlines.Add(new Run("Sturgeon, the fish that produce black caviar, are severely depleted or threatened with extinction almost everywhere in the world, "));
            _iuc1 = new InlineUIContainer();
            _border1 = new Border();
            _border1.Height = 25;
            _border1.Width = 100;
            _border1.Background = Brushes.Red;
            _iuc1.Child = _border1;
            _para1.Inlines.Add(_iuc1);
            _para1.Inlines.Add(new Run("researchers are reporting today. "));
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo1
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo1()
        {
            _para1.FlowDirection = FlowDirection.RightToLeft;
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo2
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo2()
        {
            _border1.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo3
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo3()
        {
            _iuc1.FlowDirection = FlowDirection.LeftToRight;
            _border1.FlowDirection = FlowDirection.RightToLeft;
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo4
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo4()
        {
            _para1.FlowDirection = FlowDirection.LeftToRight;
            _iuc1.FlowDirection = FlowDirection.RightToLeft;
            _border1.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo5
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo5()
        {
            _border1.FlowDirection = FlowDirection.RightToLeft;
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo6
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo6()
        {
            _iuc1.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // TextFlowDirectionCombo7
        // ------------------------------------------------------------------
        private void TextFlowDirectionCombo7()
        {
            _para1.FlowDirection = FlowDirection.RightToLeft;
            _border1.FlowDirection = FlowDirection.LeftToRight;
        }
                
        // ------------------------------------------------------------------
        // SetTextBlockContent
        // ------------------------------------------------------------------
        private void SetTextBlockContent()
        {
            _textBlock = new TextBlock();
            _textBlock.FontSize = 11.0;
            _textBlock.FontFamily = new FontFamily("Tahoma");
            _container.Child = _textBlock;
        }

        // ------------------------------------------------------------------
        // AddTextBlockInlines
        // ------------------------------------------------------------------
        private void AddTextBlockInlines()
        {
            _textBlock.Inlines.Add(new Run("\"I could not recommend that people eat caviar from any wild population of sturgeon,\" said Ellen K. Pikitch, director of the Pew Institute for Ocean Science at the University"));
            _iuc2 = new InlineUIContainer();
            _border2 = new Border();
            _border2.Height = 25;
            _border2.Width = 100;
            _border2.Background = Brushes.Red;
            _iuc2.Child = _border2;
            _textBlock.Inlines.Add(_iuc2);
            _textBlock.Inlines.Add(new Run("of Miami and the lead author of a comprehensive new study in today's issue of the journal Fish and Fisheries."));
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo1
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo1()
        {
            _textBlock.FlowDirection = FlowDirection.RightToLeft;
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo2
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo2()
        {
            _border2.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo3
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo3()
        {
            _iuc2.FlowDirection = FlowDirection.LeftToRight;
            _border2.FlowDirection = FlowDirection.RightToLeft;
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo4
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo4()
        {
            _textBlock.FlowDirection = FlowDirection.LeftToRight;
            _iuc2.FlowDirection = FlowDirection.RightToLeft;
            _border2.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo5
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo5()
        {
            _border2.FlowDirection = FlowDirection.RightToLeft;
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo6
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo6()
        {
            _iuc2.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // TextBlockDirectionCombo7
        // ------------------------------------------------------------------
        private void TextBlockDirectionCombo7()
        {
            _textBlock.FlowDirection = FlowDirection.RightToLeft;
            _border2.FlowDirection = FlowDirection.LeftToRight;
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private FlowDocumentScrollViewer _fdsv;
        private Paragraph _para1;
        private InlineUIContainer _iuc1;
        private Border _border1;
        private TextBlock _textBlock;
        private InlineUIContainer _iuc2;
        private Border _border2;        
        private Border _container;
    }
}
