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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Xml;

namespace DRT
{
    // ----------------------------------------------------------------------
    // IContentHost test suite for Text.
    // ----------------------------------------------------------------------
    internal class TextIContentHostSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextIContentHostSuite()
            : base("TextIContentHost")
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
            _text = new TextBlock();

            _contentRoot.Child = _text;
            _contentRoot.Width = _viewSize.Width;
            _contentRoot.Height = _viewSize.Height;
        }

        // ------------------------------------------------------------------
        // Verify empty Text content.
        // ------------------------------------------------------------------
        private void VerifyEmpty()
        {
            DRT.Assert(_text.Inlines.Count == 0, "Empty TextBlock is expected!");
            DRT.Assert(_text.ContentStart.CompareTo(_text.ContentEnd) == 0, "Empty TextBloxk is expected!");
        }

        // ------------------------------------------------------------------
        // Load focusable elements.
        // ------------------------------------------------------------------

        private void LoadHostedElements()
        {
            _testName = "HostedElements";

            _text = LoadFromXaml(this.TestName + ".xaml") as TextBlock;

            DRT.Assert(_text != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _text;
        }

        // ------------------------------------------------------------------
        // Verify focusable elements.
        // ------------------------------------------------------------------

        private void VerifyHostedElements()
        {
            IEnumerator<IInputElement> hostedElements = ((IContentHost)_text).HostedElements;
            DRT.Assert(hostedElements != null);

            DRT.Assert(hostedElements.MoveNext());
            IInputElement element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Span);
            Span para0 = (Span)DRT.FindElementByID("para0");
            DRT.Assert(para0 != null);
            DRT.Assert((Span)element == para0);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text == "Outer text ");

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is InlineUIContainer);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is FlowDocumentScrollViewer);
            FlowDocumentScrollViewer panel1 = (FlowDocumentScrollViewer)DRT.FindElementByID("panel1");
            DRT.Assert(panel1 != null);
            DRT.Assert(element == panel1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" This conviction is the reason"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text == " "); // implicit whitesace between Spans

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Span);
            Span para1 = (Span)DRT.FindElementByID("para1");
            DRT.Assert(para1 != null);
            DRT.Assert((Span)element == para1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("''The thing is"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Bold);
            Bold bold1 = (Bold)DRT.FindElementByID("bold1");
            DRT.Assert(bold1 != null);
            DRT.Assert((Bold)element == bold1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith("you do two things"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" and one of them is wrong"));

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text == " "); // implicit whitesace between Spans

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Span);
            Span para2 = (Span)DRT.FindElementByID("para2");
            DRT.Assert(para2 != null);
            DRT.Assert((Span)element == para2);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is InlineUIContainer);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is TextBox);
            TextBox box1 = (TextBox)DRT.FindElementByID("box1");
            DRT.Assert(box1 != null);
            DRT.Assert((TextBox)element == box1);

            DRT.Assert(hostedElements.MoveNext());
            element = hostedElements.Current as IInputElement;
            DRT.Assert(element is Run);
            DRT.Assert(((Run)element).Text.StartsWith(" Biting into a brioche, Victor chimed in"));

            DRT.Assert(!hostedElements.MoveNext());
        }
        
        // ------------------------------------------------------------------
        // Load Rectangles.
        // ------------------------------------------------------------------

        private void LoadRectangles()
        {
            _testName = "Rectangles";

            _text = LoadFromXaml(this.TestName + ".xaml") as TextBlock;

            DRT.Assert(_text != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _text;
            _text.TextWrapping = TextWrapping.WrapWithOverflow;   
        }

        // ------------------------------------------------------------------
        // Verify rectangles.
        // ------------------------------------------------------------------

        private void VerifyRectangles()
        {
            // NOTE: the assetions about the number of rectangles are based on the hard-coded input in
            // DrtFiles/TextIContentHostRectangles.xaml and for this window size. If there are any changes to
            // the text text or the window size we will have to recalculate

            XmlTextWriter xmlTextWriter = OpenDumpFile(TestXmlFileName, true);

            xmlTextWriter.WriteStartElement("RectangleDump");

            DumpHostedElement("para1", typeof(Span), (IContentHost)_text,DRT.RootElement, xmlTextWriter);
            DumpHostedElement("para2", typeof(Span), (IContentHost)_text,DRT.RootElement, xmlTextWriter);
            DumpHostedElement("bold1", typeof(Bold), (IContentHost)_text,DRT.RootElement, xmlTextWriter);
            DumpHostedElement("lb1", typeof(LineBreak), (IContentHost)_text,DRT.RootElement, xmlTextWriter);

            xmlTextWriter.WriteEndElement();

            xmlTextWriter.Flush();
            xmlTextWriter.Close();
        }

        // Checks coordinates of rectangle
        // NOTE: this check is imprecise. It will truncate the double values to integers. For more exact
        // precision examine the Rect values themselves
        private void VerifyRectangleCoordinates(Rect rect, int x, int y, int width, int height)
        {
            DRT.Assert((int)rect.X == x);
            DRT.Assert((int)rect.Y == y);
            DRT.Assert((int)rect.Width == width);
            DRT.Assert((int)rect.Height == height);
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private TextBlock _text;
        private Size _viewSize;
        private const string xamlFilePrefix = "TextIContentHost";
    }
}
