// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test cases for IContentHost functions implemented in Text
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // IContentHost test suite for Text.
    // ----------------------------------------------------------------------
    internal class TextPanelIContentHostSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelIContentHostSuite()
            : base("TextPanelIContentHost")
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
                new DrtTest(CreateEmpty),
                new DrtTest(VerifyEmpty),
                new DrtTest(LoadHostedElements),                  
                new DrtTest(VerifyHostedElements),

                // Creates own xml file by resetting test name
                new DrtTest(LoadRectangles),                      new DrtTest(VerifyLayoutCreate),
                new DrtTest(VerifyRectangles),                    new DrtTest(VerifyLayoutFinalize),
                };
        }

        // ------------------------------------------------------------------
        // Create initial element tree.
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph());
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);

            _contentRoot.Child = _fdsv;
            _contentRoot.Width = _viewSize.Width;
            _contentRoot.Height = _viewSize.Height;
        }

        // ------------------------------------------------------------------
        // Verify empty Text content.
        // ------------------------------------------------------------------
        private void VerifyEmpty()
        {
            DRT.Assert(new TextRange(_fdsv.Document.ContentStart, _fdsv.Document.ContentEnd).Text == "\r\n", "Empty TextFlow expected!");
        }

        // ------------------------------------------------------------------
        // Load focusable elements.
        // ------------------------------------------------------------------

        private void LoadHostedElements()
        {
            _testName = "HostedElements";

            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;

            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify focusable elements.
        // ------------------------------------------------------------------

        private void VerifyHostedElements()
        {
            IContentHost ich = LayoutSuite.GetIContentHost(_fdsv, DRT);
            IEnumerator<IInputElement> hostedElements = ich.HostedElements;
            DRT.Assert(hostedElements != null);

            DRT.Assert(hostedElements.MoveNext());
            IInputElement element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Paragraph);
            Paragraph para0 = (Paragraph)DRT.FindElementByID("para0");
            DRT.Assert(para0 != null);
            DRT.Assert((Paragraph)element == para0);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("Early one day I met Pichet Ong"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is InlineUIContainer);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is TextBlock);
            TextBlock text1 = (TextBlock)DRT.FindElementByID("text1");
            DRT.Assert(text1 != null);
            DRT.Assert((TextBlock)element == text1);
            DRT.Assert(((TextBlock)element).Text.StartsWith("A good point"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Paragraph);
            Paragraph para1 = (Paragraph)DRT.FindElementByID("para1");
            DRT.Assert(para1 != null);
            DRT.Assert((Paragraph)element == para1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("To that end"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Bold);
            Bold bold1 = (Bold)DRT.FindElementByID("bold1");
            DRT.Assert(bold1 != null);
            DRT.Assert((Bold)element == bold1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("\"I've done all kinds of coffee recipes"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" he said."));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Paragraph);
            Paragraph para2 = (Paragraph)DRT.FindElementByID("para2");
            DRT.Assert(para2 != null);
            DRT.Assert((Paragraph)element == para2);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is InlineUIContainer);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is TextBox);
            TextBox box1 = (TextBox)DRT.FindElementByID("box1");
            DRT.Assert(box1 != null);
            DRT.Assert((TextBox)element == box1);
            DRT.Assert(((TextBox)element).Text.StartsWith("\"Traditionally you'd find tapioca in sweet soups,\""));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" \"I thought about other coffee beverage desserts"));

            DRT.Assert(!hostedElements.MoveNext());
        }

        // ------------------------------------------------------------------
        // Load Rectangles.
        // ------------------------------------------------------------------

        private void LoadRectangles()
        {
            _testName = "Rectangles";

            _fdsv = LoadFromXaml(this.TestName + ".xaml") as FlowDocumentScrollViewer;

            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify rectangles.
        // ------------------------------------------------------------------

        private void VerifyRectangles()
        {
            // NOTE: the assetions about the number of rectangles are based on the hard-coded input in
            // DrtFiles/TextPanelIContentHostRectangles.xaml and for this window size. If there are any changes to
            // the text text or the window size we will have to recalculate
            IContentHost ich = LayoutSuite.GetIContentHost(_fdsv, DRT);

            XmlTextWriter xmlTextWriter = OpenDumpFile(TestXmlFileName, true);

            xmlTextWriter.WriteStartElement("RectangleDump");

            DumpHostedElement("para1", typeof(Paragraph), ich, DRT.RootElement, xmlTextWriter);
            DumpHostedElement("para2", typeof(Paragraph), ich, DRT.RootElement, xmlTextWriter);
            DumpHostedElement("bold1", typeof(Bold), ich, DRT.RootElement, xmlTextWriter);
            DumpHostedElement("lb1",   typeof(LineBreak), ich, DRT.RootElement, xmlTextWriter);
            DumpHostedElement("list1", typeof(System.Windows.Documents.List), ich, DRT.RootElement, xmlTextWriter);
            DumpHostedElement("li1",   typeof(ListItem), ich,  DRT.RootElement, xmlTextWriter);
            DumpHostedElement("figure1", typeof(Figure), ich,  DRT.RootElement, xmlTextWriter);
            DumpHostedElement("floater1", typeof(Floater), ich,DRT.RootElement, xmlTextWriter);
            DumpHostedElement("table1", typeof(Table), ich,    DRT.RootElement, xmlTextWriter);
            DumpHostedElement("cell1", typeof(TableCell), ich, DRT.RootElement, xmlTextWriter);
            DumpHostedElement("cell2", typeof(TableCell), ich, DRT.RootElement, xmlTextWriter);

            xmlTextWriter.WriteEndElement();

            xmlTextWriter.Flush();
            xmlTextWriter.Close();
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private FlowDocumentScrollViewer _fdsv;
        private Size _viewSize;
        private const string xamlFilePrefix = "TextPanelIContentHost";
    }
}
