// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: TextPattern test for FlowDocument.
//

using System;                               // string
using System.Collections.Generic;           // List<T>
using System.Windows;                       // DependencyObject
using System.Windows.Automation;            // ControlType
using System.Windows.Automation.Peers;      // FlowDocumentReaderAutomationPeer
using System.Windows.Automation.Provider;   // ITextProvider
using System.Windows.Automation.Text;       // TextPatternRangeEndpoint
using System.Windows.Controls;              // FlowDocumentReader
using System.Windows.Documents;             // FlowDocument
using System.Windows.Media;                 // Brushes

namespace DRT
{
    /// <summary>
    /// TextPattern test for FlowDocument.
    /// </summary>
    internal class FlowDocTextPatternTestSuite : UIAutoTestSuite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal FlowDocTextPatternTestSuite() :
            base("FlowDocTextPattern")
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
                new DrtTest(EmptyDocument),                 new DrtTest(VerifyEmptyDocument),
                new DrtTest(SingleParagraph),               new DrtTest(VerifySingleParagraph),
                new DrtTest(AppendUIContainer),             new DrtTest(VerifyAppendUIContainer),
                new DrtTest(AppendTable),                   new DrtTest(VerifyAppendTable),
                new DrtTest(GridPattern),                   new DrtTest(VerifyGridPattern),
                new DrtTest(TextRangeProviderBasic),        new DrtTest(VerifyTextRangeProviderBasic),
                new DrtTest(MoveEndpointByRange),           new DrtTest(VerifyMoveEndpointByRange),
                new DrtTest(LoadDocument),                  new DrtTest(VerifyLoadDocument),
                new DrtTest(MoveByCharacter),               new DrtTest(VerifyMoveByCharacter),
                new DrtTest(MoveByWord),                    new DrtTest(VerifyMoveByWord),
                new DrtTest(MoveByLine),                    new DrtTest(VerifyMoveByLine),
                new DrtTest(MoveByFormat),                  new DrtTest(VerifyMoveByFormat),
                new DrtTest(MoveEndpoint),                  new DrtTest(VerifyMoveEndpoint),
                new DrtTest(MoveByParagraph),               new DrtTest(VerifyMoveByParagraph),
                new DrtTest(MoveEndpointByCharacter),       new DrtTest(VerifyMoveEndpointByCharacter),
                new DrtTest(MoveEndpointByWord),            new DrtTest(VerifyMoveEndpointByWord),
                new DrtTest(MoveEndpointByLine),            new DrtTest(VerifyMoveEndpointByLine),
            };
        }

        //-------------------------------------------------------------------
        //
        //  Test/Verification Methods
        //
        //-------------------------------------------------------------------

        #region Test/Verification Methods

        /// <summary>
        /// EmptyDocument
        /// </summary>
        private void EmptyDocument()
        {
            _viewer = new FlowDocumentReader();
            _document = new FlowDocument();
            _document.ColumnWidth = double.MaxValue; // Force one column
            _viewer.Document = _document;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// VerifyEmptyDocument
        /// </summary>
        private void VerifyEmptyDocument()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                // DocumentRange
                VerifyDocumentRange(textProvider, _document.ContentStart, _document.ContentEnd);
                // Selection
                VerifyTextSelection(textProvider, _document.ContentStart, true);
                // VisibleRanges
                VerifyVisibleRanges(textProvider, _document.ContentStart, _document.ContentEnd, true);
            }
        }

        /// <summary>
        /// SingleParagraph
        /// </summary>
        private void SingleParagraph()
        {
            Run run;

            // Hyperlink
            run = new Run("Hyperlink");
            run.Name = "HRun";
            Hyperlink hyperlink = new Hyperlink(run);
            hyperlink.Name = "Hyperlink";

            // InlineUIContainer 1
            Button button = new Button();
            button.Content = "Button";
            button.Name = "Button";
            InlineUIContainer uiContainer1 = new InlineUIContainer(button);

            // InlineUIContainer 2
            Border border = new Border();
            border.Width = 100;
            border.Height = 20;
            border.Background = Brushes.Green;
            border.Name = "Border";
            InlineUIContainer uiContainer2 = new InlineUIContainer(border);

            // Paragraph
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(run = new Run("one"));
            run.Name = "Run1";
            paragraph.Inlines.Add(hyperlink);
            paragraph.Inlines.Add(run = new Run("two"));
            run.Name = "Run2";
            paragraph.Inlines.Add(uiContainer1);
            paragraph.Inlines.Add(run = new Run("three"));
            run.Name = "Run3";
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(run = new Run("four"));
            run.Name = "Run4";
            paragraph.Inlines.Add(uiContainer2);
            paragraph.Inlines.Add(run = new Run("five"));
            run.Name = "Run5";

            _document.Blocks.Add(paragraph);
        }

        /// <summary>
        /// VerifySingleParagraph
        /// </summary>
        private void VerifySingleParagraph()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                // DocumentRange
                VerifyDocumentRange(textProvider, _document.ContentStart, _document.ContentEnd);
                // Selection
                VerifyTextSelection(textProvider, _document.ContentStart, true);
                // VisibleRanges
                VerifyVisibleRanges(textProvider, _document.ContentStart, _document.ContentEnd, true);

                // Verify document children.
                List<DependencyObject> children = new List<DependencyObject>();
                FindAndAddChild(children, "Hyperlink");
                FindAndAddChild(children, "Button");
                if (children.Count > 0)
                {
                    VerifyDocumentRangeChildren(textProvider, children);
                }
            }
        }

        /// <summary>
        /// AppendUIContainer
        /// </summary>
        private void AppendUIContainer()
        {
            // BlockUIContainer
            Button button = new Button();
            button.Content = "BlockButton";
            button.Name = "BlockButton";
            button.Height = 50;
            BlockUIContainer uiContainer = new BlockUIContainer(button);

            _document.Blocks.Add(uiContainer);
        }

        /// <summary>
        /// AppendUIContainer
        /// </summary>
        private void VerifyAppendUIContainer()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                // DocumentRange
                VerifyDocumentRange(textProvider, _document.ContentStart, _document.ContentEnd);
                // Selection
                VerifyTextSelection(textProvider, _document.ContentStart, true);
                // VisibleRanges
                VerifyVisibleRanges(textProvider, _document.ContentStart, _document.ContentEnd, true);

                // Verify document children.
                List<DependencyObject> children = new List<DependencyObject>();
                FindAndAddChild(children, "Hyperlink");
                FindAndAddChild(children, "Button");
                FindAndAddChild(children, "BlockButton");
                if (children.Count > 0)
                {
                    VerifyDocumentRangeChildren(textProvider, children);
                }
            }
        }

        /// <summary>
        /// AppendTable
        /// </summary>
        private void AppendTable()
        {
            TableRow row;
            TableCell cell;
            Paragraph paragraph;

            TableRowGroup rowGroup = new TableRowGroup();
            // Row 1
            row = new TableRow();
            rowGroup.Rows.Add(row);
            // Cell 1.1
            paragraph = new Paragraph(new Run("Cell 1.1"));
            cell = new TableCell(paragraph);
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            cell.Name = "Cell11";
            row.Cells.Add(cell);
            // Cell 1.2
            Hyperlink hyperlink = new Hyperlink(new Run("Hyperlink"));
            hyperlink.Name = "TableHyperlink";
            paragraph = new Paragraph(new Run("Cell 1.2"));
            paragraph.Inlines.Add(hyperlink);
            cell = new TableCell(paragraph);
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            cell.Name = "Cell12";
            row.Cells.Add(cell);
            // Row 2
            row = new TableRow();
            rowGroup.Rows.Add(row);
            // Cell 2.1
            paragraph = new Paragraph(new Run("Cell 2.1"));
            cell = new TableCell(paragraph);
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            cell.Name = "Cell21";
            row.Cells.Add(cell);
            // Cell 2.2
            Button button = new Button();
            button.Content = "Button";
            button.Name = "TableButton";
            paragraph = new Paragraph(new Run("Cell 2.2"));
            paragraph.Inlines.Add(new InlineUIContainer(button));
            cell = new TableCell(paragraph);
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            cell.Name = "Cell22";
            row.Cells.Add(cell);

            Table table = new Table();
            table.RowGroups.Add(rowGroup);
            table.Name = "Table";

            _document.Blocks.InsertAfter(_document.Blocks.FirstBlock, table);
        }

        /// <summary>
        /// VerifyAppendTable
        /// </summary>
        private void VerifyAppendTable()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                // DocumentRange
                VerifyDocumentRange(textProvider, _document.ContentStart, _document.ContentEnd);
                // Selection
                VerifyTextSelection(textProvider, _document.ContentStart, true);
                // VisibleRanges
                VerifyVisibleRanges(textProvider, _document.ContentStart, _document.ContentEnd, true);

                // Verify document children.
                List<DependencyObject> children = new List<DependencyObject>();
                FindAndAddChild(children, "Hyperlink");
                FindAndAddChild(children, "Button");
                FindAndAddChild(children, "Table");
                FindAndAddChild(children, "BlockButton");
                if (children.Count > 0)
                {
                    VerifyDocumentRangeChildren(textProvider, children);
                }
            }
        }

        /// <summary>
        /// GridPattern
        /// </summary>
        private void GridPattern()
        {
        }

        /// <summary>
        /// VerifyGridPattern
        /// </summary>
        private void VerifyGridPattern()
        {
            ITextRangeProvider textRange = null;
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                textRange = textProvider.DocumentRange;
                DRT.Assert(textRange != null, "ITextProvider.DocumentRange is null.");
            }

            if (textRange != null)
            {
                IRawElementProviderSimple[] rawChildren = textRange.GetChildren();
                DRT.Assert(rawChildren != null && rawChildren.Length == 4, "DocumentRange.GetChildren: expecting {0} children, got {1}.", 4, rawChildren != null ? rawChildren.Length : 0);
                if (rawChildren != null && rawChildren.Length == 4)
                {
                    IRawElementProviderSimple tableRaw = rawChildren[2];
                    TableAutomationPeer tablePeer = GetPeerFromRawElement(tableRaw) as TableAutomationPeer;
                    DRT.Assert(tablePeer != null, "Cannot retrieve TableAutomationPeer from DocumentRange.Children collection.");
                    if (tablePeer != null)
                    {
                        // Property checks
                        AutomationControlType controlType = tablePeer.GetAutomationControlType();
                        DRT.Assert(controlType == AutomationControlType.Table, "Table.GetControlType failed: expecting {0}, got {1}.", AutomationControlType.Table.ToString(), controlType.ToString());

                        // Grid pattern
                        IGridProvider gridProvider = tablePeer.GetPattern(PatternInterface.Grid) as IGridProvider;
                        DRT.Assert(gridProvider != null, "Cannot retrieve IGridProvider from TableAutomationPeer.");
                        if (gridProvider != null)
                        {
                            DRT.Assert(gridProvider.ColumnCount == 2, "IGridProvider.ColumnCount: expecting {0} column(s), got {1}.", 2, gridProvider.ColumnCount);
                            DRT.Assert(gridProvider.RowCount == 2, "IGridProvider.RowCount: expecting {0} row(s), got {1}.", 2, gridProvider.RowCount);

                            VerifyTableCell(tablePeer, gridProvider, 0, 0, 1, 1);
                            VerifyTableCell(tablePeer, gridProvider, 0, 1, 1, 1);
                            VerifyTableCell(tablePeer, gridProvider, 1, 0, 1, 1);
                            VerifyTableCell(tablePeer, gridProvider, 1, 1, 1, 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// TextRangeProviderBasic
        /// </summary>
        private void TextRangeProviderBasic()
        {
        }

        /// <summary>
        /// VerifyTextRangeProviderBasic
        /// </summary>
        private void VerifyTextRangeProviderBasic()
        {
            ITextRangeProvider documentRange = null;
            ITextProvider textProvider = null;

            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                documentRange = textProvider.DocumentRange;
                DRT.Assert(documentRange != null, "ITextProvider.DocumentRange is null.");
            }

            if (documentRange != null && textProvider != null)
            {
                // FindText / GetText
                ITextRangeProvider range = documentRange.FindText("Cell", false, false);
                DRT.Assert(range != null, "ITextRageProvider.FindText failed to find 'Cell'.");
                if (range == null) { return; }
                string text = range.GetText(int.MaxValue);
                DRT.Assert(text == "Cell", "ITextRageProvider.GetText failed.");

                // GetEnclosingElement
                IRawElementProviderSimple rawElement = range.GetEnclosingElement();
                DRT.Assert(rawElement != null, "ITextRageProvider.GetEnclosingElement returned null.");
                if (rawElement != null)
                {
                    DependencyObject tableCell = DRT.FindElementByID("Cell11", _document);
                    DRT.Assert(tableCell != null, "Failed to find '{0}' in FlowDocument.", "Cell11");
                    if (tableCell != null)
                    {
                        VerifyChildAutomationPeer("TableCell", rawElement, tableCell);
                        VerifyChildTextRange("TableCell", textProvider, rawElement, tableCell);
                    }
                }

                // MoveEndpointByRange
                range.MoveEndpointByRange(TextPatternRangeEndpoint.End, documentRange, TextPatternRangeEndpoint.End);
                range = range.FindText("Cell 1.2", false, false);
                DRT.Assert(range != null, "ITextRageProvider.FindText failed to find 'Cell12'.");
                if (range == null) { return; }

                // ExpandToEnclosingUnit
                range.ExpandToEnclosingUnit(TextUnit.Paragraph);

                // TableCell children
                rawElement = range.GetEnclosingElement();
                DRT.Assert(rawElement != null, "ITextRageProvider.GetEnclosingElement returned null.");
                if (rawElement != null)
                {
                    DependencyObject tableCell = DRT.FindElementByID("Cell12", _document);
                    DRT.Assert(tableCell != null, "Failed to find '{0}' in FlowDocument.", "Cell12");
                    if (tableCell != null)
                    {
                        VerifyChildAutomationPeer("TableCell", rawElement, tableCell);
                        VerifyChildTextRange("TableCell", textProvider, rawElement, tableCell);
                    }

                    // Verify TableCell children.
                    List<DependencyObject> children = new List<DependencyObject>();
                    FindAndAddChild(children, "TableHyperlink");
                    if (children.Count > 0)
                    {
                        VerifyTextRangeChildren("TableCell", textProvider, range, children);
                    }
                }

                // MoveEndpointByUnit
                int count = range.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Character, -1);
                DRT.Assert(count == -1, "ITextRangeProvider.MoveEndpointByUnit failed. Expecting -1, got {0}.", count);

                // Table children
                rawElement = range.GetEnclosingElement();
                DRT.Assert(rawElement != null, "ITextRageProvider.GetEnclosingElement returned null.");
                if (rawElement != null)
                {
                    TableAutomationPeer tablePeer = GetPeerFromRawElement(rawElement) as TableAutomationPeer;
                    DRT.Assert(tablePeer != null, "ITextRageProvider.GetEnclosingElement failed. Expecting raw element for Table.");
                    if (tablePeer != null)
                    {
                        List<AutomationPeer> peers = tablePeer.GetChildren();
                        DRT.Assert(peers != null, "AutomationPeer.GetChildren() returned null for Table.");
                        if (peers != null)
                        {
                            List<DependencyObject> children = new List<DependencyObject>();
                            FindAndAddChild(children, "Cell11");
                            FindAndAddChild(children, "Cell12");
                            FindAndAddChild(children, "Cell21");
                            FindAndAddChild(children, "Cell22");
                            DRT.Assert(peers.Count == children.Count, "AutomationPeer.GetChildren() failed. Expecting element count {0}, got {1}.", children.Count, peers.Count);
                            if (peers.Count == children.Count)
                            {
                                for (int i = 0; i < children.Count; i++)
                                {
                                    VerifyChildAutomationPeer("Table", peers[i], children[i]);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// MoveEndpointByRange
        /// </summary>
        private void MoveEndpointByRange()
        {
        }

        /// <summary>
        /// VerifyMoveEndpointByRange
        /// </summary>
        private void VerifyMoveEndpointByRange()
        {
            int result;
            ITextRangeProvider textRangeProvider;
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();

            // Get TextPattern
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider docRangeProvider = textProvider.DocumentRange;
            DRT.Assert(docRangeProvider != null, "ITextProvider.DocumentRange failed.");

            // MoveEndpointByRange : Endpoint.End to Endpoint.Start
            textRangeProvider = docRangeProvider.Clone();
            DRT.Assert(textRangeProvider != null, "ITextRangeProvider.Clone failed.");
            textRangeProvider.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRangeProvider, TextPatternRangeEndpoint.Start);
            result = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.End, docRangeProvider, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // MoveEndpointByRange : Endpoint.Start to Endpoint.End
            textRangeProvider = docRangeProvider.Clone();
            DRT.Assert(textRangeProvider != null, "ITextRangeProvider.Clone failed.");
            textRangeProvider.MoveEndpointByRange(TextPatternRangeEndpoint.Start, docRangeProvider, TextPatternRangeEndpoint.End);
            result = textRangeProvider.CompareEndpoints(TextPatternRangeEndpoint.Start, docRangeProvider, TextPatternRangeEndpoint.End);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.Start to Endpoint.End.");
        }

        /// <summary>
        /// LoadDocument
        /// </summary>
        private void LoadDocument()
        {
            _document = LoadFromXaml("FlowDocumentPlain.xaml") as FlowDocument;
            _viewer.Document = _document;
        }

        /// <summary>
        /// VerifyLoadDocument
        /// </summary>
        private void VerifyLoadDocument()
        {
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                // DocumentRange
                VerifyDocumentRange(textProvider, _document.ContentStart, _document.ContentEnd);
                // Selection
                VerifyTextSelection(textProvider, _document.ContentStart, true);
            }
        }

        /// <summary>
        /// MoveByCharacter
        /// </summary>
        private void MoveByCharacter()
        {
        }

        /// <summary>
        /// VerifyMoveByCharacter
        /// </summary>
        private void VerifyMoveByCharacter()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnit(trp, TextUnit.Character,  1,  1, "a");
            MoveToNextUnit(trp, TextUnit.Character,  5,  5, " ");
            MoveToNextUnit(trp, TextUnit.Character,  1,  1, "t");
            MoveToNextUnit(trp, TextUnit.Character, 10, 10, " ");
            MoveToNextUnit(trp, TextUnit.Character,  1,  1, "A");
            MoveToNextUnit(trp, TextUnit.Character, 30, 30, "s");
            MoveToNextUnit(trp, TextUnit.Character,  1,  1, "\r\n");
            MoveToNextUnit(trp, TextUnit.Character,  1,  1, "P");

            // Move backward
            MoveToNextUnit(trp, TextUnit.Character,  -1,  -1, "\r\n");
            MoveToNextUnit(trp, TextUnit.Character,  -1,  -1, "s");
            MoveToNextUnit(trp, TextUnit.Character, -30, -30, "A");
            MoveToNextUnit(trp, TextUnit.Character,  -1,  -1, " ");
            MoveToNextUnit(trp, TextUnit.Character, -10, -10, "t");
            MoveToNextUnit(trp, TextUnit.Character,  -1,  -1, " ");
            MoveToNextUnit(trp, TextUnit.Character,  -6,  -6, "S");
            MoveToNextUnit(trp, TextUnit.Character,  -1,   0, "S");
        }
        
        /// <summary>
        /// MoveEndpointByCharacter
        /// </summary>
        private void MoveEndpointByCharacter()
        {
        }

        /// <summary>
        /// VerifyMoveEndpointByCharacter
        /// </summary>
        private void VerifyMoveEndpointByCharacter()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  1,  1, "a");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  5,  5, " ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  1,  1, "t");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character, 10, 10, " ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  1,  1, "A");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character, 30, 30, "s");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  1,  1, "\r\n");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  1,  1, "P");

            // Move backward
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  -1,  -1, "\r\n");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  -1,  -1, "s");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character, -30, -30, "A");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  -1,  -1, " ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character, -10, -10, "t");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  -1,  -1, " ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  -6,  -6, "S");
            MoveToNextUnitByEndpoint(trp, TextUnit.Character,  -1,   0, "S");
        }

        /// <summary>
        /// MoveByWord
        /// </summary>
        private void MoveByWord()
        {
        }

        /// <summary>
        /// VerifyMoveByWord
        /// </summary>
        private void VerifyMoveByWord()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "topic ");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "from ");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "Aero ");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "User ");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "Experience ");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "Guidelines");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "\r\n");
            MoveToNextUnit(trp, TextUnit.Word, 1, 1, "Picking ");

            // Move backward
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "\r\n");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "Guidelines");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "Experience ");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "User ");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "Aero ");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "from ");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "topic ");
            MoveToNextUnit(trp, TextUnit.Word, -1, -1, "Sample ");
            MoveToNextUnit(trp, TextUnit.Word, -1,  0, "Sample ");
        }

        /// <summary>
        /// MoveEndpointByWord
        /// </summary>
        private void MoveEndpointByWord()
        {
        }

        /// <summary>
        /// VerifyMoveEndpointByWord
        /// </summary>
        private void VerifyMoveEndpointByWord()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "topic ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "from ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "Aero ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "User ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "Experience ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "Guidelines");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "\r\n");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, 1, 1, "Picking ");

            // Move backward
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "\r\n");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "Guidelines");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "Experience ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "User ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "Aero ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "from ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "topic ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1, -1, "Sample ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Word, -1,  0, "Sample ");
        }

        /// <summary>
        /// MoveByLine
        /// </summary>
        private void MoveByLine()
        {
        }

        /// <summary>
        /// VerifyMoveByLine
        /// </summary>
        private void VerifyMoveByLine()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnit(trp, TextUnit.Line, 1, 1, "Picking the Right Degree of ");
            MoveToNextUnit(trp, TextUnit.Line, 1, 1, "Control for User Interfaces");
            MoveToNextUnit(trp, TextUnit.Line, 1, 1, "Microsoft User Experience Group");
            MoveToNextUnit(trp, TextUnit.Line, 1, 1, "October 2003");

            // Move backward
            MoveToNextUnit(trp, TextUnit.Line, -1, -1, "Microsoft User Experience Group");
            MoveToNextUnit(trp, TextUnit.Line, -1, -1, "Control for User Interfaces");
            MoveToNextUnit(trp, TextUnit.Line, -1, -1, "Picking the Right Degree of ");
            MoveToNextUnit(trp, TextUnit.Line, -1, -1, "Sample topic from Aero User Experience Guidelines");
            MoveToNextUnit(trp, TextUnit.Line, -1,  0, "Sample topic from Aero User Experience Guidelines");
        }

        /// <summary>
        /// MoveEndpointByLine
        /// </summary>
        private void MoveEndpointByLine()
        {
        }

        /// <summary>
        /// VerifyMoveEndpointByLine
        /// </summary>
        private void VerifyMoveEndpointByLine()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, 1, 1, "Picking the Right Degree of ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, 1, 1, "Control for User Interfaces");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, 1, 1, "Microsoft User Experience Group");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, 1, 1, "October 2003");

            // Move backward
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, -1, -1, "Microsoft User Experience Group");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, -1, -1, "Control for User Interfaces");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, -1, -1, "Picking the Right Degree of ");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, -1, -1, "Sample topic from Aero User Experience Guidelines");
            MoveToNextUnitByEndpoint(trp, TextUnit.Line, -1,  0, "Sample topic from Aero User Experience Guidelines");
        }

        /// <summary>
        /// MoveByFormat
        /// </summary>
        private void MoveByFormat()
        {
        }

        /// <summary>
        /// VerifyMoveByFormat
        /// </summary>
        private void VerifyMoveByFormat()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnit(trp, TextUnit.Format, 1, 1, "Aero User Experience Guidelines\r\n");
            MoveToNextUnit(trp, TextUnit.Format, 1, 1, "Picking the Right Degree of Control for User Interfaces\r\n");
            MoveToNextUnit(trp, TextUnit.Format, 1, 1, "Microsoft User Experience Group\r\n");
            MoveToNextUnit(trp, TextUnit.Format, 1, 1, "October 2003\r\n");

            // Move backward
            MoveToNextUnit(trp, TextUnit.Format, -1, -1, "Microsoft User Experience Group\r\n");
            MoveToNextUnit(trp, TextUnit.Format, -1, -1, "Picking the Right Degree of Control for User Interfaces\r\n");
            MoveToNextUnit(trp, TextUnit.Format, -1, -1, "Aero User Experience Guidelines\r\n");
            MoveToNextUnit(trp, TextUnit.Format, -1, -1, "Sample topic from ");
            MoveToNextUnit(trp, TextUnit.Format, -1,  0, "Sample topic from ");
        }

        /// <summary>
        /// MoveByParagraph
        /// </summary>
        private void MoveByParagraph()
        {
        }

        /// <summary>
        /// VerifyMoveByParagraph
        /// </summary>
        private void VerifyMoveByParagraph()
        {
            int result;

            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // Move range to document start.
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            result = trp.CompareEndpoints(TextPatternRangeEndpoint.End, trp, TextPatternRangeEndpoint.Start);
            DRT.Assert(result == 0, "ITextRangeProvider.MoveEndpointByRange failed to move Endpoint.End to Endpoint.Start.");

            // Move forward
            MoveToNextUnit(trp, TextUnit.Paragraph, 1, 1, "Picking the Right Degree of Control for User Interfaces");
            MoveToNextUnit(trp, TextUnit.Paragraph, 1, 1, "Microsoft User Experience Group\r\nOctober 2003");
            MoveToNextUnit(trp, TextUnit.Paragraph, 1, 1, "Picking the Right Degree of Control for User Interfaces");

            // Move backward
            MoveToNextUnit(trp, TextUnit.Paragraph, -1, -1, "Microsoft User Experience Group\r\nOctober 2003");
            MoveToNextUnit(trp, TextUnit.Paragraph, -1, -1, "Picking the Right Degree of Control for User Interfaces");
            MoveToNextUnit(trp, TextUnit.Paragraph, -1, -1, "Sample topic from Aero User Experience Guidelines");
            MoveToNextUnit(trp, TextUnit.Paragraph, -1,  0, "Sample topic from Aero User Experience Guidelines");
        }

        /// <summary>
        /// MoveEndpoint
        /// </summary>
        private void MoveEndpoint()
        {
        }

        /// <summary>
        /// VerifyMoveEndpoint
        /// </summary>
        private void VerifyMoveEndpoint()
        {
            // Get TextPattern
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider tp = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(tp != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");

            // Get DocumentRange
            ITextRangeProvider docRange = tp.DocumentRange;
            ITextRangeProvider trp = tp.DocumentRange;
            DRT.Assert(trp != null, "ITextProvider.DocumentRange failed.");

            // TextUnit.Word
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Word, -1, -1, "topic ");
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Word, 1, 1, "topic ");

            // TextUnit.Line
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Line, -1, -1, "Sample topic from Aero User Experience Guidelines");
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Line, 1, 1, "Sample topic from Aero User Experience Guidelines");

            // TextUnit.Paragraph
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Paragraph, -1, -1, "Sample topic from Aero User Experience Guidelines");
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Paragraph, 1, 1, "Sample topic from Aero User Experience Guidelines");

            // TextUnit.Format
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Format, -1, -1, "Sample topic from ");
            trp.MoveEndpointByRange(TextPatternRangeEndpoint.End, docRange, TextPatternRangeEndpoint.Start);
            MoveToNextUnitBoundary(trp, TextUnit.Format, 1, 1, "Sample topic from ");
        }

        #endregion Test/Verification Methods

        //-------------------------------------------------------------------
        //
        //  Helpers
        //
        //-------------------------------------------------------------------

        #region Helpers

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

        /// <summary>
        /// Gets document range as ITextRangeProvider.
        /// </summary>
        private ITextRangeProvider GetDocumentRange()
        {
            ITextRangeProvider documentRange = null;
            DocumentAutomationPeer documentPeer = GetDocumentAutomationPeer();
            ITextProvider textProvider = documentPeer.GetPattern(PatternInterface.Text) as ITextProvider;
            DRT.Assert(textProvider != null, "Failed to retrieve TextPattern from DocumentAutomationPeer.");
            if (textProvider != null)
            {
                documentRange = textProvider.DocumentRange;
                DRT.Assert(documentRange != null, "ITextProvider.DocumentRange is null.");
            }
            return documentRange;
        }

        /// <summary>
        /// Finds element with given name and adds it to children collection.
        /// </summary>
        /// <param name="children">Collection of children.</param>
        /// <param name="name">Name of element to add.</param>
        private void FindAndAddChild(List<DependencyObject> children, string name)
        {
            DependencyObject child = DRT.FindElementByID(name, _document);
            DRT.Assert(child != null, "Failed to find '{0}' in FlowDocument.", name);
            if (child != null)
            {
                children.Add(child);
            }
        }

        /// <summary>
        /// Helper to move range to the next unit using ITextRangeProvider.Move.
        /// </summary>
        private void MoveToNextUnit(ITextRangeProvider trp, TextUnit textUnit, int count, int moved, string expectedText)
        {
            int result;
            string text;
            // Move endpoint to new unit boundary
            result = trp.Move(textUnit, count);
            DRT.Assert(result == moved, "ITextRangeProvider.Move failed to move by requested number of units.");
            // Verify content of the range.
            text = trp.GetText(-1);
            DRT.Assert(text == expectedText, "MoveToNextUnit failed. Expecting '{0}', got '{1}'.", expectedText, text);
        }

        /// <summary>
        /// Helper to move range to the next unit using ITextRangeProvider.MoveEndpointByUnit.
        /// </summary>
        private void MoveToNextUnitByEndpoint(ITextRangeProvider trp, TextUnit textUnit, int count, int moved, string expectedText)
        {
            int result;
            string text;

            // Move Start endpoint to new unit boundary
            result = trp.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, textUnit, count);
            DRT.Assert(result == moved, "ITextRangeProvider.MoveEndpointByUnit failed to move Endpoint to new unit boundary.");
            // Expand the range to cover entire unit.
            trp.ExpandToEnclosingUnit(textUnit);
            // Verify content of the range.
            text = trp.GetText(-1);
            DRT.Assert(text == expectedText, "MoveToNextUnitByEndpoint failed. Expecting '{0}', got '{1}'.", expectedText, text);
        }

        /// <summary>
        /// Helper to move range to the next unit using ITextRangeProvider.MoveEndpointByUnit.
        /// </summary>
        private void MoveToNextUnitBoundary(ITextRangeProvider trp, TextUnit textUnit, int count, int moved, string expectedText)
        {
            int result;
            string text;

            // Move the range to initial starting position
            result = trp.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Character, 10);
            DRT.Assert(result == 10, "ITextRangeProvider.MoveEndpointByUnit failed to move Endpoint to TextUnit.Character boundary.");
            // Expand the range to TextUnit.Character.
            trp.ExpandToEnclosingUnit(TextUnit.Character);
            // Verify content of the range.
            text = trp.GetText(-1);
            DRT.Assert(text == "i", "MoveToNextUnitByEndpoint failed. Expecting '{0}', got '{1}'.", "p", text);

            // Move the range to unit boundary
            if (count > 0)
            {
                result = trp.MoveEndpointByUnit(TextPatternRangeEndpoint.End, textUnit, count);
            }
            else
            {
                result = trp.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, textUnit, count);
            }
            DRT.Assert(result == moved, "ITextRangeProvider.MoveEndpointByUnit failed to move Endpoint to new unit boundary.");
            // Expand the range to cover entire unit.
            trp.ExpandToEnclosingUnit(textUnit);
            // Verify content of the range.
            text = trp.GetText(-1);
            DRT.Assert(text == expectedText, "MoveToNextUnitByEndpoint failed. Expecting '{0}', got '{1}'.", expectedText, text);
        }

        /// <summary>
        /// Verify proper expose of TableCell inside Table.
        /// </summary>
        private void VerifyTableCell(TableAutomationPeer tablePeer, IGridProvider gridProvider, int col, int row, int colSpan, int rowSpan)
        {
            IRawElementProviderSimple cellRaw = gridProvider.GetItem(row, col);
            DRT.Assert(cellRaw != null, "IGridProvider.GetItem({0},{1}) returned 'null'.", col, row);
            if (cellRaw != null)
            {
                TableCellAutomationPeer cellPeer = GetPeerFromRawElement(cellRaw) as TableCellAutomationPeer;
                DRT.Assert(cellPeer != null, "Cannot retrieve TableCellAutomationPeer from IRawElementProviderSimple.");
                if (cellPeer != null)
                {
                    // Properties
                    AutomationControlType controlType = cellPeer.GetAutomationControlType();
                    DRT.Assert(controlType == AutomationControlType.Custom, "TableCell.GetControlType failed: expecting {0}, got {1}.", AutomationControlType.Custom.ToString(), controlType.ToString());
                    string locControlType = cellPeer.GetLocalizedControlType();
                    DRT.Assert(locControlType == "cell", "TableCell.GetLocalizedControlType failed: expecting {0}, got {1}.", "cell", locControlType);

                    // GridItem 
                    IGridItemProvider gridItemProvider = cellPeer.GetPattern(PatternInterface.GridItem) as IGridItemProvider;
                    DRT.Assert(gridItemProvider != null, "Cannot retrieve IGridItemProvider from TableCellAutomationPeer.");
                    if (gridItemProvider != null)
                    {
                        DRT.Assert(gridItemProvider.Column == col, "IGridItemProvider.Column: expecting {0}, got {1}.", col, gridItemProvider.Column);
                        DRT.Assert(gridItemProvider.Row == row, "IGridItemProvider.Row: expecting {0}, got {1}.", row, gridItemProvider.Row);
                        DRT.Assert(gridItemProvider.ColumnSpan == colSpan, "IGridItemProvider.ColumnSpan: expecting {0}, got {1}.", colSpan, gridItemProvider.ColumnSpan);
                        DRT.Assert(gridItemProvider.RowSpan == rowSpan, "IGridItemProvider.RowSpan: expecting {0}, got {1}.", rowSpan, gridItemProvider.RowSpan);
                        TableAutomationPeer newTablePeer = GetPeerFromRawElement(gridItemProvider.ContainingGrid) as TableAutomationPeer;
                        DRT.Assert(newTablePeer == tablePeer, "IGridItemProvider.ContainingGrid failed to return Table element.");
                    }
                }
            }
        }

        #endregion Helpers

        private FlowDocumentReader _viewer;     // Viewer for FlowDocument
        private FlowDocument _document;         // FlowDocument
    }
}
