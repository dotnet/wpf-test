// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections;
using System.Xml.Serialization;

namespace DRT
{
    // Regression tests for textrange serialization.
    internal class TextRangeSerializationSuite : DrtTestSuite
    {
        #region Constructors

        // Creates a new instance.
        internal TextRangeSerializationSuite()
            : base("TextRangeSerialization")
        {
            Contact = "Microsoft";
        }

        #endregion Constructors

        #region Public Methods

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[] { new DrtTest(TestHyperlinkSingle),
                                   new DrtTest(TestHyperlinkPartial),
                                   new DrtTest(TestHyperlinkMixed),
                                   new DrtTest(TestCustomInline),
                                 };
        }

        #endregion Public Methods

        #region Private Methods

        private UIElement CreateTree()
        {
            Canvas canvas = new Canvas();
            _richTextBox = new RichTextBox();
      
            Canvas.SetLeft(_richTextBox, 5);
            Canvas.SetTop(_richTextBox, 5);
            _richTextBox.Width = 200;
            _richTextBox.Height = 200;

            // Set the font for consistency across themes
            _richTextBox.FontFamily = new FontFamily("Tahoma");
            _richTextBox.FontSize = 11.0;

            canvas.Children.Add(_richTextBox);

            return canvas;
        }

        private void TestHyperlinkSingle()
        {
            // Prepare content for a test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Hyperlink Foreground=\"#FFACA899\">link</Hyperlink>c</Paragraph>"));

            // Create text range to serialize
            TextPointer hyperlinkStart = _richTextBox.Document.ContentStart.GetPositionAtOffset(3);
            TextPointer hyperlinkEnd = _richTextBox.Document.ContentStart.GetPositionAtOffset(7);
            TextRange range = new TextRange(hyperlinkStart, hyperlinkEnd);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Hyperlink Foreground=\"#FFACA899\" TextDecorations=\"Underline\"><Run>link</Run></Hyperlink>"), "Mismatch in serialized xaml");
        }

        private void TestHyperlinkPartial()
        {
            // Prepare content for a test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Hyperlink Foreground=\"#FFACA899\">link</Hyperlink>c</Paragraph>"));

            // Create text range to serialize
            TextPointer start = _richTextBox.Document.ContentStart.GetPositionAtOffset(3);
            TextPointer end = _richTextBox.Document.ContentStart.GetPositionAtOffset(5);
            TextRange range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Run>li</Run>"), "Mismatch in serialized xaml");

            // Create text range to serialize
            start = _richTextBox.Document.ContentStart.GetPositionAtOffset(5);
            end = _richTextBox.Document.ContentStart.GetPositionAtOffset(7);
            range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Run>nk</Run>"), "Mismatch in serialized xaml");

            // Create text range to serialize
            start = _richTextBox.Document.ContentStart.GetPositionAtOffset(4);
            end = _richTextBox.Document.ContentStart.GetPositionAtOffset(6);
            range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Run>in</Run>"), "Mismatch in serialized xaml");

            // Create text range to serialize
            start = _richTextBox.Document.ContentStart.GetPositionAtOffset(3);
            end = _richTextBox.Document.ContentStart.GetPositionAtOffset(11);
            range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Hyperlink Foreground=\"#FFACA899\" TextDecorations=\"Underline\"><Run>link</Run></Hyperlink><Run>c</Run>"), "Mismatch in serialized xaml");
        }

        private void TestHyperlinkMixed()
        {
            // Prepare content for a test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Hyperlink Foreground=\"#FFACA899\">link</Hyperlink>c</Paragraph>"));

            // Create text range to serialize
            TextPointer start = _richTextBox.Document.ContentStart.GetPositionAtOffset(3);
            TextPointer end = _richTextBox.Document.ContentStart.GetPositionAtOffset(11);
            TextRange range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Hyperlink Foreground=\"#FFACA899\" TextDecorations=\"Underline\"><Run>link</Run></Hyperlink><Run>c</Run>"), "Mismatch in serialized xaml");

            // Prepare content for a test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph>c<Hyperlink Foreground=\"#FFACA899\">link</Hyperlink></Paragraph>"));

            // Create text range to serialize
            start = _richTextBox.Document.ContentStart.GetPositionAtOffset(2);
            end = _richTextBox.Document.ContentStart.GetPositionAtOffset(10);
            range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), GetClipboardSpanXaml("<Run>c</Run><Hyperlink Foreground=\"#FFACA899\" TextDecorations=\"Underline\"><Run>link</Run></Hyperlink>"), "Mismatch in serialized xaml");
        }

        private void TestCustomInline()
        {
            // Prepare content for a test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><DRT:SmartTag xmlns:DRT=\"clr-namespace:DRT;assembly=DrtEditing\">Tag!</DRT:SmartTag>c</Paragraph>"));

            // Create text range to serialize
            TextPointer start = _richTextBox.Document.ContentStart.GetPositionAtOffset(3);
            TextPointer end = _richTextBox.Document.ContentStart.GetPositionAtOffset(11);
            TextRange range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXmlWithInlines(range), GetClipboardSpanXaml("<DRT:SmartTag xmlns:DRT=\"clr-namespace:DRT;assembly=DrtEditing\"><Run>Tag!</Run></DRT:SmartTag><Run>c</Run>"), "Mismatch in serialized xaml");

            // Prepare content for a test.  Include non-default value for custom property
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph>c<DRT:SmartTag CustomString=\"MyString\" xmlns:DRT=\"clr-namespace:DRT;assembly=DrtEditing\">Tag!</DRT:SmartTag></Paragraph>"));

            // Create text range to serialize
            start = _richTextBox.Document.ContentStart.GetPositionAtOffset(2);
            end = _richTextBox.Document.ContentStart.GetPositionAtOffset(10);
            range = new TextRange(start, end);

            // Check serialiazed range xaml
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXmlWithInlines(range), GetClipboardSpanXaml("<Run>c</Run><DRT:SmartTag xmlns:DRT=\"clr-namespace:DRT;assembly=DrtEditing\" CustomString=\"MyString\"><Run>Tag!</Run></DRT:SmartTag>"), "Mismatch in serialized xaml");

            // Save and load via data object (clipboard).
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Xaml, true);
            UTF8Encoding encoding = new UTF8Encoding();
            DataObject dataObject = new DataObject("MyXaml", encoding.GetString(stream.GetBuffer()));

            string mySerializedXaml = dataObject.GetData("MyXaml") as string;
            MemoryStream stream2 = new MemoryStream();
            StreamWriter xamlStreamWriter = new StreamWriter(stream2);
            xamlStreamWriter.Write(mySerializedXaml);
            xamlStreamWriter.Flush();
            range.Load(stream2, DataFormats.Xaml);

            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXmlWithInlines(range), GetClipboardSpanXaml("<Run>c</Run><DRT:SmartTag xmlns:DRT=\"clr-namespace:DRT;assembly=DrtEditing\" CustomString=\"MyString\"><Run>Tag!</Run></DRT:SmartTag>"), "Mismatch in serialized xaml");
        }

        //  Private helpers
        
        /// <summary>
        /// Loads richtextbox with given content
        /// </summary>
        /// <param name="s">
        /// String specifying xaml content to be loaded
        /// </param>
        private void LoadXamlContent(string s)
        {
            _richTextBox.Document = new FlowDocument(new Paragraph(new Run()));
            TextRange range = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            DrtEditing.SetTextRangeXml(range, s);
        }

        /// <summary>
        /// Creates a text range within another text range or a textbox. Returns a TextRange with the specified length and
        /// offset from the anchor position
        /// </summary>
        /// <param name="anchor">
        /// TextPosition that is used to determine the offset of the text range from its parent. The text range positions are 
        /// calculated with respect to this parameter
        /// </param>
        /// <param name="offset">
        /// Offset for the text range start with respect to the anchor position
        /// </param>
        /// <param name="length">
        /// Length (number of TextPositions) of the text range
        /// </param>
        private TextRange CreateTextRange(TextPointer anchor, int offset, int length)
        {
            TextPointer start = anchor.GetPositionAtOffset(+offset);

            TextPointer end = start.GetPositionAtOffset(+length);

            TextRange range = new TextRange(start, end);
            return range;
        }

        // Wraps FlowDocument content in a standard clipboard span.
        private static string GetClipboardSpanXaml(string content)
        {
            return (
                "<Span xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" xml:lang=\"en-us\" FlowDirection=\"LeftToRight\" NumberSubstitution.CultureSource=\"Text\" NumberSubstitution.Substitution=\"AsCulture\" FontFamily=\"Tahoma\" FontStyle=\"Normal\" FontWeight=\"Normal\" FontStretch=\"Normal\" FontSize=\"11\" Foreground=\"#FF000000\" Typography.StandardLigatures=\"True\" Typography.ContextualLigatures=\"True\" Typography.DiscretionaryLigatures=\"False\" Typography.HistoricalLigatures=\"False\" Typography.AnnotationAlternates=\"0\" Typography.ContextualAlternates=\"True\" Typography.HistoricalForms=\"False\" Typography.Kerning=\"True\" Typography.CapitalSpacing=\"False\" Typography.CaseSensitiveForms=\"False\" Typography.StylisticSet1=\"False\" Typography.StylisticSet2=\"False\" Typography.StylisticSet3=\"False\" Typography.StylisticSet4=\"False\" Typography.StylisticSet5=\"False\" Typography.StylisticSet6=\"False\" Typography.StylisticSet7=\"False\" Typography.StylisticSet8=\"False\" Typography.StylisticSet9=\"False\" Typography.StylisticSet10=\"False\" Typography.StylisticSet11=\"False\" Typography.StylisticSet12=\"False\" Typography.StylisticSet13=\"False\" Typography.StylisticSet14=\"False\" Typography.StylisticSet15=\"False\" Typography.StylisticSet16=\"False\" Typography.StylisticSet17=\"False\" Typography.StylisticSet18=\"False\" Typography.StylisticSet19=\"False\" Typography.StylisticSet20=\"False\" Typography.Fraction=\"Normal\" Typography.SlashedZero=\"False\" Typography.MathematicalGreek=\"False\" Typography.EastAsianExpertForms=\"False\" Typography.Variants=\"Normal\" Typography.Capitals=\"Normal\" Typography.NumeralStyle=\"Normal\" Typography.NumeralAlignment=\"Normal\" Typography.EastAsianWidths=\"Normal\" Typography.EastAsianLanguage=\"Normal\" Typography.StandardSwashes=\"0\" Typography.ContextualSwashes=\"0\" Typography.StylisticAlternates=\"0\" BaselineAlignment=\"Baseline\" TextDecorations=\"\">"
                + content +
                "</Span>");
        }

        // Wraps FlowDocument content in a standard clipboard span.
        private static string GetClipboardSpanWithUnderlineXaml(string content)
        {
            return (
                "<Span xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" xml:lang=\"en-us\" FlowDirection=\"LeftToRight\" NumberSubstitution.CultureSource=\"Text\" NumberSubstitution.Substitution=\"AsCulture\" FontFamily=\"Tahoma\" FontStyle=\"Normal\" FontWeight=\"Normal\" FontStretch=\"Normal\" FontSize=\"11\" Foreground=\"#FFACA899\" Typography.StandardLigatures=\"True\" Typography.ContextualLigatures=\"True\" Typography.DiscretionaryLigatures=\"False\" Typography.HistoricalLigatures=\"False\" Typography.AnnotationAlternates=\"0\" Typography.ContextualAlternates=\"True\" Typography.HistoricalForms=\"False\" Typography.Kerning=\"True\" Typography.CapitalSpacing=\"False\" Typography.CaseSensitiveForms=\"False\" Typography.StylisticSet1=\"False\" Typography.StylisticSet2=\"False\" Typography.StylisticSet3=\"False\" Typography.StylisticSet4=\"False\" Typography.StylisticSet5=\"False\" Typography.StylisticSet6=\"False\" Typography.StylisticSet7=\"False\" Typography.StylisticSet8=\"False\" Typography.StylisticSet9=\"False\" Typography.StylisticSet10=\"False\" Typography.StylisticSet11=\"False\" Typography.StylisticSet12=\"False\" Typography.StylisticSet13=\"False\" Typography.StylisticSet14=\"False\" Typography.StylisticSet15=\"False\" Typography.StylisticSet16=\"False\" Typography.StylisticSet17=\"False\" Typography.StylisticSet18=\"False\" Typography.StylisticSet19=\"False\" Typography.StylisticSet20=\"False\" Typography.Fraction=\"Normal\" Typography.SlashedZero=\"False\" Typography.MathematicalGreek=\"False\" Typography.EastAsianExpertForms=\"False\" Typography.Variants=\"Normal\" Typography.Capitals=\"Normal\" Typography.NumeralStyle=\"Normal\" Typography.NumeralAlignment=\"Normal\" Typography.EastAsianWidths=\"Normal\" Typography.EastAsianLanguage=\"Normal\" Typography.StandardSwashes=\"0\" Typography.ContextualSwashes=\"0\" Typography.StylisticAlternates=\"0\" BaselineAlignment=\"Baseline\" TextDecorations=\"Underline\">"
                + content +
                "</Span>");
        }

        #endregion Private Methods

        #region Private Fields

        private RichTextBox _richTextBox;

        #endregion Private Fields
    }

    [XmlTypeAttribute(Namespace = "clr-namespace:DRT")]
    [TextElementEditingBehaviorAttribute(IsMergeable = false, IsTypographicOnly = false)]
    public class SmartTag : Span
    {
        public SmartTag()
            : base()
        {
        }

        public SmartTag(TextPointer start, TextPointer end)
            : base(start, end)
        {
        }

        public static readonly DependencyProperty CustomStringProperty =
                DependencyProperty.Register(
                        "CustomString", // Property name
                        typeof(string), // Property type
                        typeof(SmartTag), // Property owner
                        new FrameworkPropertyMetadata("default value"));

        public String CustomString
        {
            get
            {
                return (String)GetValue(CustomStringProperty);
            }
            set
            {
                SetValue(CustomStringProperty, value);
            }
        }

        public int CustomInt
        {
            get
            {
                return _customInt;
            }
            set
            {
                _customInt = value;
            }
        }

        private int _customInt;
    }
}