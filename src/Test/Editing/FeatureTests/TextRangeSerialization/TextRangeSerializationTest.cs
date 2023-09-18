// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for public API of the TextRange class. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextRangeSerialization/TextRangeSerializationTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using System.Xml;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion

    /// <summary>Abstract base class for all TextRangeSerializationTest code</summary>
    [TestBugs("205, 204, 206")]
    public abstract class TextRangeSerializationTest
    {
        /// <summary>protected ctor</summary>
        protected TextRangeSerializationTest(string testName)
        {
            _result = false;

            _sourceRichTextBox = new RichTextBox();
            _targetRichTextBox = new RichTextBox();

            _testName = testName;
        }

        /// <summary>
        /// Position the start and end Pointers to the desired location
        /// before test starts
        /// By default start points to control.Start position
        /// and end points to control.End position
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected virtual void PreparePointers(TextPointer start, TextPointer end)
        {
        }

        /// <summary>
        /// Children classes override this to fill the content of source RichTextBox.
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected abstract void InitializeSourceTextBox(RichTextBox sourceRichTextBox);

        /// <summary>
        /// Apply content modification to the target TextRange when some special case is needed
        /// e.g. Environment.NewLine is not deserialized as \r\n but \n
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected virtual void PostDeserializeProcessTarget(TextPointer start, TextPointer end)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        protected RichTextBox SourceRichTextBox
        {
            get { return _sourceRichTextBox; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        protected RichTextBox TargetRichTextBox
        {
            get { return _targetRichTextBox; }
        }

        /// <summary>Run this test</summary>
        public void Run(Window mainwindow, SimpleHandler handler)
        {
            string unmatchReason;
            TextPointer start;
            TextPointer end;
            TextPointer targetStart;
            TextPointer targetEnd;
            StackPanel panel;

            panel = new StackPanel();
            panel.Children.Add(_sourceRichTextBox);
            panel.Children.Add(_targetRichTextBox);
            mainwindow.Height = 600;
            mainwindow.Width = 800;
            _sourceRichTextBox.Height = 300;
            _targetRichTextBox.Height = 300;
            mainwindow.Content = panel;

            (new TextRange(_sourceRichTextBox.Document.ContentStart, _sourceRichTextBox.Document.ContentEnd)).Text = String.Empty;
            (new TextRange(_targetRichTextBox.Document.ContentStart, _targetRichTextBox.Document.ContentEnd)).Text = String.Empty;

            InitializeSourceTextBox(_sourceRichTextBox);

            start = _sourceRichTextBox.Document.ContentStart;
            end = _sourceRichTextBox.Document.ContentEnd;

            PreparePointers(start, end);

            // do the round trip serialization and deserialization
            XamlUtils.TextRange_SetXml(new TextRange(_targetRichTextBox.Document.ContentStart, _targetRichTextBox.Document.ContentEnd), XamlUtils.TextRange_GetXml(new TextRange(start, end)));

            targetStart = _targetRichTextBox.Document.ContentStart;
            targetEnd = _targetRichTextBox.Document.ContentEnd;

            PostDeserializeProcessTarget(targetStart, targetEnd);

            // make sure both start and end textpointer are valid
            if ((start.GetTextInRun(LogicalDirection.Forward) == "") &&
                (start.GetNextInsertionPosition(LogicalDirection.Forward) != null))
            {
                start = start.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            if ((end.GetTextInRun(LogicalDirection.Backward) == "") &&
                (end.GetNextInsertionPosition(LogicalDirection.Backward) != null))
            {
                end = end.GetNextInsertionPosition(LogicalDirection.Backward);
            }
            if ((targetStart.GetTextInRun(LogicalDirection.Forward) == "") &&
                (targetStart.GetNextInsertionPosition(LogicalDirection.Forward) != null))
            {
                targetStart = targetStart.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            if ((targetEnd.GetTextInRun(LogicalDirection.Backward) == "") &&
                (targetEnd.GetNextInsertionPosition(LogicalDirection.Backward) != null))
            {
                targetEnd = targetEnd.GetNextInsertionPosition(LogicalDirection.Backward);
            }

            _result = TextTreeTestHelper.CompareTextRangeContents(start,
                end,
                targetStart,
                targetEnd,
                out unmatchReason);


            Logger.Current.Log("sourceRange: {0}", TextRangeDumper.Dump(start, end, false, false));
            Logger.Current.Log("targetRange: {0}", TextRangeDumper.Dump(targetStart, targetEnd, false, false));
            if (_result)
            {
                Logger.Current.Log("{0} passes", _testName);
            }
            else
            {
                Logger.Current.Log("{0} fails", _testName);
                Logger.Current.Log("Reason: {0}", unmatchReason);
                TextRange range = new TextRange(start, end);
                Logger.Current.Log("SourceXaml[" + XamlUtils.TextRange_GetXml(range) + "]");
                range = new TextRange(targetStart, targetEnd);
                Logger.Current.Log("TargetXaml[" + XamlUtils.TextRange_GetXml(range) + "]");
            }

            QueueHelper.Current.QueueDelegate(handler);
        }
        
        /// <summary>
        /// Set the Properties for a textElement
        /// </summary>
        /// <param name="element"></param>
        /// <param name="data"></param>
        protected void SetDPForTextElement(DependencyObject element, DependencyPropertyData[] data)
        {
            bool BugSkip = false;
            for (int i = 0; i < data.Length; i++)
            {
                //start with false;
                BugSkip = false;

                //Regression_Bug204 - Duplicated TextDecorations property is created byTextRange.Save() if both Paragraph and its span child has this property set.
                //Remove the following line afte the bug is fixed..
                //BugSkip = element is Paragraph && data[i].Property == Paragraph.TextDecorationsProperty;

                //if Regression_Bug205 - Many DPs are not serialized by TextRange.Save()
                BugSkip = BugSkip || (element is ListItem && (data[i].Property == ListItem.TextAlignmentProperty || data[i].Property == ListItem.LineHeightProperty));
                BugSkip = BugSkip || (element is Block && (data[i].Property == Block.IsHyphenationEnabledProperty));
                BugSkip = BugSkip || (element is List && (data[i].Property == Block.ClearFloatersProperty));
              
                //Regression_Bug206 - <Hyperlink> in RichTextBox shows the Enabled Foreground color
                BugSkip = BugSkip || (element is Hyperlink && (data[i].Property == TextElement.ForegroundProperty));
                
                if (!BugSkip)
                {                    
                    element.SetValue(data[i].Property, data[i].TestValue);
                }
                else
                {
                    Logger.Current.Log("***** Skipping assigning a test value for " + data[i].Property.ToString() + " property");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool TestResult
        {
            get { return _result; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public string TestName
        {
            get { return _testName; }
        }

        private bool _result;
        private RichTextBox _targetRichTextBox;
        private RichTextBox _sourceRichTextBox;
        private string _testName;
    }

    /// <summary>TextRange covering all text in the TextTree, plain Text only</summary>
    public class TextRangeSerializationTest1 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest1() : base("Plain text - whole text run") { }

        /// <summary>
        /// TextRange covering all text in the TextTree, plain Text only
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("This is a test")));
        }
    }

    /// <summary>TextRange covering partial text in the TextTree, plain Text only</summary>
    public class TextRangeSerializationTest2 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest2() : base("Plain text - partial text run") { }

        /// <summary>
        /// TextRange covering partial text in the TextTree, plain Text only
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("This is a very very long string")));
        }

        /// <summary>
        /// Move start to offset 5 and end to offset -5.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            // we are trying to deserialize part of the text in the TextContainer
            start = start.GetNextInsertionPosition(LogicalDirection.Forward); // set position at flowdocument
            start = start.GetNextInsertionPosition(LogicalDirection.Forward); // set positin at section
            end = end.GetNextInsertionPosition(LogicalDirection.Backward);
            start = start.GetPositionAtOffset(5);
            end = end.GetPositionAtOffset(-1);
        }
    }

    /// <summary>Empty TextRange tests</summary>
    public class TextRangeSerializationTest3 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest3() : base("Empty TextRange") { }

        /// <summary>
        /// Empty TextRange
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
        }
    }

    /// <summary>TextRange with multiple spaces</summary>
    public class TextRangeSerializationTest4 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest4() : base("Plain text - whole text run - multiple spaces") { }

        /// <summary>
        /// TextRange with multiple spaces
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("            ")));
        }
    }

    /// <summary>TextRange covering partial text in the TextTree, plain Text only and TextNode is split/// </summary>
    public class TextRangeSerializationTest5 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest5() : base("Plain text - partial text run with text node split") { }

        /// <summary>
        /// TextRange covering partial text in the TextTree, plain Text only
        /// and TextNode is split.
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("This is a very very long string")));
        }

        /// <summary>
        /// Move start to offset 5 and end to offset -5.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            start = start.GetNextInsertionPosition(LogicalDirection.Forward); // set positin at flowdocument
            start = start.GetNextInsertionPosition(LogicalDirection.Forward); // set positin at flowdocument
            end = end.GetNextInsertionPosition(LogicalDirection.Backward); // set positin at section

            start = start.GetPositionAtOffset(5);
            end = end.GetPositionAtOffset(-5);

            TextTreeTestHelper.SplitTextNode(new TextRange(start, end), 5);
        }
    }

    /// <summary>TextRange with Environment.NewLine</summary>
    public class TextRangeSerializationTest6 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest6() : base("Plain text - Environment.NewLine") { }

        /// <summary>
        /// TextRange with Environment.NewLine
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("This is first line")));
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run(Environment.NewLine)));
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("This is second line")));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PostDeserializeProcessTarget(TextPointer start, TextPointer end)
        {
            TextTreeTestHelper.WalkTextRange(start,
                end,
                null,
                null,
                new TextTreeTestHelper.ProcessTextContextDelegate(OnTextContext),
                null,
                null,
                null,
                null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="textPointer"></param>
        /// <param name="content"></param>
        /// <param name="args"></param>
        private bool OnTextContext(TextPointer textPointer, string content, object[] args)
        {
            Run runElement;

            if (textPointer.Parent is Run)
            {
                runElement = (Run)textPointer.Parent;
            }
            else
            {
                throw new ApplicationException("Parent of a pointer with TextContext should be a Run");
            }

            // replace \n with \r\n so that verification will work
            content = content.Replace("\n", Environment.NewLine);
            runElement.Text = content;
            return false; /* continue navigation */
        }
    }

    /// <summary>Partial, cross multiple paragraph tests</summary>
    public class TextRangeSerializationTest7 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest7() : base("Rich text - Partial, cross multiple paragraph tests") { }

        /// <summary>
        /// Partial, cross multiple paragraph
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            // create and insert empty paragraph
            Paragraph paragraph;
            Paragraph paragraph1;
            Paragraph paragraph2;
            Paragraph paragraph3;
            TextRange textRange;
            LinearGradientBrush linearGradientBrush;

            //empty paragraph already exists

            // create and insert non-empty paragraph
            paragraph = new Paragraph(new Run("This is some text in the paragraph"));
            sourceRichTextBox.Document.Blocks.Add(paragraph);
            _start = paragraph.ContentStart;
            _start = _start.GetPositionAtOffset(5);

            // create another paragraph
            paragraph1 = new Paragraph(new Run("Inner paragraph"));

            textRange = new TextRange(paragraph1.ContentStart, paragraph1.ContentEnd);
            textRange.ApplyPropertyValue(Paragraph.TextIndentProperty, 10.0);

            textRange.ApplyPropertyValue(Block.BorderThicknessProperty, new Thickness(1));
            textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 72.0);
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Gray);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
            textRange.ApplyPropertyValue(Block.PaddingProperty, new Thickness(2));
            linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.Black, 0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
            linearGradientBrush.StartPoint = new Point(0, 0);
            linearGradientBrush.EndPoint = new Point(0.75, 0.75);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, linearGradientBrush);
            sourceRichTextBox.Document.Blocks.Add(paragraph1);

            // create another paragraphs
            paragraph2 = new Paragraph(new Run("One more paragraph"));
            sourceRichTextBox.Document.Blocks.Add(paragraph2);

            // create last paragraph
            paragraph3 = new Paragraph(new Run("last paragraph"));
            sourceRichTextBox.Document.Blocks.Add(paragraph3);
            _end = paragraph3.ContentEnd;
            _end = _end.GetPositionAtOffset(-5);
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            start = _start;
            end = _end;
        }
        private TextPointer _start;
        private TextPointer _end;
    }

    /// <summary>TextRange containing xml reservered characters</summary>
    public class TextRangeSerializationTest8 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest8() : base("Plain text - Xml reserved characters") { }

        /// <summary>
        /// TextRange containing xml reservered characters
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("<?&>\\/")));
        }
    }

    /// <summary>
    /// TextRange with FontFamily, FontSize, FontWeight, TextDecorations changed.
    /// Subscript and Superscript added (PS #Regression_Bug899)
    /// </summary>
    public class TextRangeSerializationTest9 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest9() : base("Rich text - FontFamily, FontSize, FontStyle, FontWeight, TextDecorations, Subscript, Superscript") { }

        /// <summary>
        /// return TextRange with FontFamily, FontSize, FontWeight, TextDecorations changed.
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Paragraph paragraph;
            TextDecorationCollection textDecorations;
            Bold bold;
            Span span;
            Run run;

            span = new Span();
            run = new Run();
            paragraph = new Paragraph(span);

            sourceRichTextBox.Document.Blocks.Clear(); // Remove the default paragraph
            span.Inlines.Clear();
            sourceRichTextBox.Document.Blocks.Add(paragraph);

            run.Text = "Trailing text";
            span.Inlines.Add(run);

            run = new Run("Green text");
            run.Foreground = Brushes.Green;
            span.Inlines.Add(run);

            run = new Run("Red text");
            run.Foreground = Brushes.Red;
            span.Inlines.Add(run);

            run = new Run("Underline text");
            textDecorations = new TextDecorationCollection();

            foreach (TextDecoration td2 in TextDecorations.Underline)
            {
                textDecorations.Add(td2);
            }
            run.TextDecorations = textDecorations;
            span.Inlines.Add(run);

            bold = new Bold(new Run("Superscript text"));
            bold.BaselineAlignment = BaselineAlignment.Superscript;
            span.Inlines.Add(bold);

            bold = new Bold(new Run("Subscript text"));
            bold.BaselineAlignment = BaselineAlignment.Subscript;
            span.Inlines.Add(bold);

            run = new Run("FontStytle changed");
            run.FontStyle = FontStyles.Italic;
            span.Inlines.Add(run);

            run = new Run("FontSize changed");
            run.FontSize = 72.0;
            span.Inlines.Add(run);

            run = new Run("Font Family changed");
            run.FontFamily = new FontFamily("Courier New");
            span.Inlines.Add(run);

            run = new Run("Some text");
            span.Inlines.Add(run);
        }
    }

    /// <summary>TextRange = long string[B]Bold Text[I]Italic Text[/I][/B]</summary>
    public class TextRangeSerializationTest10 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest10() : base("Rich text - nested Bold and Italic") { }

        /// <summary>
        /// TextRange = long string[B]Bold Text[I]Italic Text[/I][/B]
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Bold bold;
            Italic italic;

            sourceRichTextBox.Document.Blocks.Clear(); // Clear default Paragraph
            // get some header text in the TextRange
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("Some long string")));
            // create bold object and append that to the end of TextRange
            bold = new Bold(new Run("Bold Text"));
            ((Paragraph)(sourceRichTextBox.Document.Blocks.LastBlock)).Inlines.Add(bold);
            // create Italic object and append that to the end of the Bold object so this object is nested
            italic = new Italic(new Run("Italic Text"));
            bold.Inlines.Add(italic);
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            // move the pointers to include part of the text
            start = start.GetPositionAtOffset(5);
            end = end.GetPositionAtOffset(-5);
        }
    }

    /// <summary>TextRange Plain text - xaml like text</summary>
    public class TextRangeSerializationTest11 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest11() : base("Plain text - xaml like text") { }

        /// <summary>
        /// TextRange Plain text - xaml like text
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("<TextBox>This is a test</TextBox><CheckBox/>")));
        }
    }

    /// <summary>TextRange Plain text - xaml like text</summary>
    public class TextRangeSerializationTest12 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest12() : base("Plain text - xaml like text") { }

        /// <summary>
        /// TextRange Plain text - xaml like text
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("<Button /><TextBox>This is a test</TextBox><CheckBox>")));
        }
    }

    /// <summary>TextRange containing xml reservered characters</summary>
    public class TextRangeSerializationTest13 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest13() : base("Plain text - Symbol characters test") { }

        /// <summary>
        /// TextRange containing xml reservered characters
        /// </summary>
        /// <param name="sourceRichTextBox">Source richTextBox to have contents filled</param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("!@#$%^&*()_+{{}}[]/><?,.")));
        }
    }

    /// <summary>Test LineBreak element</summary>
    public class TextRangeSerializationTest14 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest14() : base("Rich text - LineBreak  elements nested inside a bold element") { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Bold bold;

            sourceRichTextBox.Document.Blocks.Clear(); // Remove default Paragraph
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("Normal header text")));

            bold = new Bold(new Run("Bold header text"));
            ((Paragraph)(sourceRichTextBox.Document.Blocks.LastBlock)).Inlines.Add(bold);
            new LineBreak(bold.ContentEnd);
            new Run("Bold trailing text", bold.ContentEnd);
            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("normal trailing text")));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            // move the pointers to include part of the text
            start = start.GetPositionAtOffset(5);
            end = end.GetPositionAtOffset(-5);
        }
    }

    /// <summary>TextRange with empty and non-empty paragraphs, and test null TextDecorations</summary>
    public class TextRangeSerializationTest15 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest15() : base("Rich text - TextRange with empty and non-empty paragraphs, and test null TextDecorations") { }

        /// <summary>
        /// TextRange with empty and non-empty paragraphs, and test null TextDecorations
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Paragraph paragraph;

            //empty paragraph already exists.
            //create and insert non-empty paragraph
            paragraph = new Paragraph(new Run("This is some text in the paragraph"));
            new TextRange(paragraph.ContentStart, paragraph.ContentEnd).ApplyPropertyValue(Paragraph.TextDecorationsProperty, null);
            sourceRichTextBox.Document.Blocks.Add(paragraph);
        }
    }

    /// <summary>TextRange with HorizontalAlignment and FlowDirection changed</summary>
    public class TextRangeSerializationTest16 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest16() : base("Rich text - HorizontalAlignment and FlowDirection") { }

        /// <summary>
        /// TextRange with HorizontalAlignment and FlowDirection changed
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Paragraph paragraph;

            paragraph = new Paragraph(new Run("TextAlignment.Left text"));

            sourceRichTextBox.Document.Blocks.Add(paragraph);

            (new TextRange(paragraph.ContentStart, paragraph.ContentEnd)).ApplyPropertyValue(Block.TextAlignmentProperty,
                TextAlignment.Left);

            paragraph = new Paragraph(new Run("TextAlignment.Center text"));

            sourceRichTextBox.Document.Blocks.Add(paragraph);

            (new TextRange(paragraph.ContentStart, paragraph.ContentEnd)).ApplyPropertyValue(Block.TextAlignmentProperty,
                TextAlignment.Center);

            paragraph = new Paragraph(new Run("TextAlignment.Right text"));

            sourceRichTextBox.Document.Blocks.Add(paragraph);

            (new TextRange(paragraph.ContentStart, paragraph.ContentEnd)).ApplyPropertyValue(Block.TextAlignmentProperty,
                TextAlignment.Right);

            paragraph = new Paragraph(new Run("TextAlignment.Justify text"));

            sourceRichTextBox.Document.Blocks.Add(paragraph);

            (new TextRange(paragraph.ContentStart, paragraph.ContentEnd)).ApplyPropertyValue(Block.TextAlignmentProperty,
                TextAlignment.Justify);

            paragraph = new Paragraph(new Run("FlowDirection.RightToLeft text"));

            sourceRichTextBox.Document.Blocks.Add(paragraph);

            (new TextRange(paragraph.ContentStart, paragraph.ContentEnd)).ApplyPropertyValue(Paragraph.FlowDirectionProperty,
                System.Windows.FlowDirection.RightToLeft);

            paragraph = new Paragraph(new Run("FlowDirection.LeftToRight text"));

            sourceRichTextBox.Document.Blocks.Add(paragraph);

            (new TextRange(paragraph.ContentStart, paragraph.ContentEnd)).ApplyPropertyValue(Paragraph.FlowDirectionProperty,
                System.Windows.FlowDirection.LeftToRight);
        }
    }

    /// <summary>TextRange with List and ListItem</summary>
    public class TextRangeSerializationTest17 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest17() : base("Rich text - List and ListItem") { }

        /// <summary>
        /// TextRange with List and ListItem
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            List list1;
            List list2;
            List list3;
            ListItem listItem1;
            ListItem listItem2;
            ListItem listItem3;

            // The structure is visualized as follows:
            //<List>
            //    <ListItem>
            //        <Paragraph>ListItem 1</Paragraph>
            //        <List>
            //            <ListItem><Paragraph>ListItem 2</Paragraph></ListItem>
            //        </List>
            //        <List>
            //            <ListItem><Paragraph>ListItem 3</Paragraph></ListItem>
            //        </List>
            //    </ListItem>
            //</List>

            listItem1 = new ListItem();
            listItem1.Blocks.Add(new Paragraph(new Run("ListItem 1")));
            list1 = new List();
            list1.ListItems.Add(listItem1);
            list1.SetValue(List.MarkerStyleProperty, TextMarkerStyle.Circle);
            sourceRichTextBox.Document.Blocks.Add(list1);

            listItem2 = new ListItem();
            listItem2.Blocks.Add(new Paragraph(new Run("ListItem 2")));
            list2 = new List();
            list2.ListItems.Add(listItem2);
            listItem1.Blocks.Add(list2);

            listItem3 = new ListItem();
            listItem3.Blocks.Add(new Paragraph(new Run("ListItem 3")));
            list3 = new List();
            list3.ListItems.Add(listItem3);
            listItem1.Blocks.Add(list3);
        }
    }

    /// <summary>TextRange with partial List and ListItem (Regression_Bug612)</summary>
    [TestBugs("205")]
    public class TextRangeSerializationTest18 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest18() : base("Rich text - partial List and ListItem") { }

        /// <summary>
        /// TextRange with partial List and ListItem
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            List list1;
            List list2;
            List list3;
            ListItem listItem1;
            ListItem listItem2;
            ListItem listItem3;

            // The structure is visualized as follows:
            // <List>
            //     <ListItem>
            //         <Paragraph>ListE|lementItem 1</Paragraph>
            //         <List>
            //             <ListItem><Paragraph>ListItem 2</Paragraph></ListItem>
            //         </List>
            //     </ListItem>
            // </List>
            // <List>
            //     <ListItem><Paragraph>ListElementI|tem 3</Paragraph></ListItem>
            // </List>

            listItem1 = new ListItem();
            SetDPForTextElement(listItem1, DependencyPropertyData.ListItemPropertyData);
            listItem1.Blocks.Add(new Paragraph(new Run("ListItem 1")));
            list1 = new List();
            SetDPForTextElement(list1, DependencyPropertyData.ListPropertyData);
            list1.ListItems.Add(listItem1);
            list1.SetValue(List.MarkerStyleProperty, TextMarkerStyle.Circle);
            sourceRichTextBox.Document.Blocks.Add(list1);

            listItem2 = new ListItem();
            listItem2.Blocks.Add(new Paragraph(new Run("ListItem 2")));
            list2 = new List();
            list2.ListItems.Add(listItem2);
            listItem1.Blocks.Add(list2);

            listItem3 = new ListItem();
            listItem3.Blocks.Add(new Paragraph(new Run("ListItem 3")));
            list3 = new List();
            list3.ListItems.Add(listItem3);
            sourceRichTextBox.Document.Blocks.Add(list3);

            _start = listItem1.ContentStart;
            _start = _start.GetPositionAtOffset(5);

            _end = listItem3.ContentEnd;
            _end = _end.GetPositionAtOffset(-5);
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            // move the pointers to include part of the text
            start = _start;
            end = _end;
        }

        private TextPointer _start;
        private TextPointer _end;
    }

    /// <summary>TextRange with table and table cell</summary>
    public class TextRangeSerializationTest19 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest19() : base("Rich text - Table and table cells") { }

        /// <summary>
        /// TextRange with List and ListItem
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Table table;
            Table table2;
            TableColumn tableColumn1;
            TableColumn tableColumn2;
            TableColumn tableColumn3;
            TableColumn tableColumn4;
            TableRowGroup tableBody;
            TableRow tableRow1;
            TableRow tableRow2;
            TableCell tableCell;
            TextPointer start;
            TextPointer end;
            List list;
            TextDecorationCollection textDecorations;
            ListItem listItem;

            sourceRichTextBox.Document.Blocks.Add(new Paragraph(new Run("Some header text")));

            table = new Table();
            SetDPForTextElement(table, DependencyPropertyData.TablePropertyData);
            table.BorderThickness = new Thickness(1, 1, 1, 1);

            tableBody = new TableRowGroup();
            SetDPForTextElement(tableBody, DependencyPropertyData.TableRowGroupPropertyData);
            table.RowGroups.Add(tableBody);

            tableColumn1 = new TableColumn();
            SetDPForTextElement(tableColumn1, DependencyPropertyData.TableColumnPropertyData);
            tableColumn1.Width = new GridLength(40);
            table.Columns.Add(tableColumn1);

            tableColumn2 = new TableColumn();
            SetDPForTextElement(tableColumn2, DependencyPropertyData.TableColumnPropertyData);
            tableColumn2.Width = new GridLength(20);
            table.Columns.Add(tableColumn2);

            tableColumn3 = new TableColumn();
            SetDPForTextElement(tableColumn3, DependencyPropertyData.TableColumnPropertyData);
            tableColumn3.Width = new GridLength(30);
            table.Columns.Add(tableColumn3);

            tableColumn4 = new TableColumn();
            SetDPForTextElement(tableColumn4, DependencyPropertyData.TableColumnPropertyData);
            tableColumn4.Width = new GridLength(20);
            table.Columns.Add(tableColumn4);

            tableRow1 = new TableRow();
            SetDPForTextElement(tableRow1, DependencyPropertyData.TableRowPropertyData);
            tableBody.Rows.Add(tableRow1);

            tableRow2 = new TableRow();
            SetDPForTextElement(tableRow2, DependencyPropertyData.TableRowPropertyData);
            tableBody.Rows.Add(tableRow2);

            // another table
            table2 = new Table();
            table2.RowGroups.Add(new TableRowGroup());

            // this cell has another table inside.
            tableCell = new TableCell(table2);
            tableCell.RowSpan = 2;
            tableRow1.Cells.Add(tableCell);

            // this table cell has plain text
            tableCell = new TableCell(new Paragraph(new Run("Plain text cell")));
            tableRow1.Cells.Add(tableCell);

            // this table cell has rich text
            tableCell = new TableCell(new Paragraph(new Run("Some rich text in this table cell")));

            start = tableCell.ContentStart;
            end = tableCell.ContentEnd;
            start = start.GetNextInsertionPosition(LogicalDirection.Forward);
            end = end.GetNextInsertionPosition(LogicalDirection.Backward);
            start = start.GetPositionAtOffset(4);
            end = end.GetPositionAtOffset(15);  // Set end position to an invalid position (end will be null after this statement)

            start = start.GetPositionAtOffset(5);
            end = tableCell.ContentEnd;
            end = end.GetPositionAtOffset(-5);
            new TextRange(start, end).ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);

            tableRow2.Cells.Add(tableCell);

            // rich text with TextDecorations and fontfamily changed
            tableCell = new TableCell(new Paragraph(new Run("Some rich text in this table cell")));
            start = tableCell.ContentStart;
            end = tableCell.ContentStart;
            start = start.GetPositionAtOffset(2);
            end = end.GetPositionAtOffset(4);
            textDecorations = new TextDecorationCollection();

            foreach (TextDecoration td in TextDecorations.Underline)
            {
                textDecorations.Add(td);
            }

            new TextRange(start, end).ApplyPropertyValue(Paragraph.TextDecorationsProperty, textDecorations);
            start = start.GetPositionAtOffset(3);
            end = end.GetPositionAtOffset(5);
            new TextRange(start, end).ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily("Courier New"));
            tableRow1.Cells.Add(tableCell);

            listItem = new ListItem();
            SetDPForTextElement(listItem, DependencyPropertyData.ListItemPropertyData);
            listItem.Blocks.Add(new Paragraph(new Run("ListItem 1")));
            list = new List();
            SetDPForTextElement(list, DependencyPropertyData.ListPropertyData);
            list.ListItems.Add(listItem);
            tableCell = new TableCell(list);
            SetDPForTextElement(tableCell, DependencyPropertyData.TableCellPropertyData);
            tableRow1.Cells.Add(tableCell);

            sourceRichTextBox.Document.Blocks.Add(table);
        }

        /// <summary>
        /// Move start to somewhere inside the leading paragraph
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PreparePointers(TextPointer start, TextPointer end)
        {
            start = start.GetPositionAtOffset(3);
        }

        /// <summary>
        /// This function allows you to move start and end of the target (origianlly at ContentStart
        /// and ContentEnd, before validation happens.
        /// This is needed, for example, when partial paragraph is serialized and de-serialized.
        /// A start tag is created in the target because it is partial serialization / de-serialization
        /// but TextRangeComparer is not aware of this extra start tag and will flag an error on validation
        /// So in this function testers are allowed to workaround the start and end pointers for validation
        /// purpose
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected override void PostDeserializeProcessTarget(TextPointer start, TextPointer end)
        {
            TextPointer pointer;

            // start should be next to a <Paragraph>
            Verifier.Verify(start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart);

            pointer = start;

            // move into the context and verify that this is a paragraph
            pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);

            Verifier.Verify(pointer.Parent.GetType() == typeof(Paragraph));

            // move start to this position, since we need TextRange comparer to ignore this paragraph
            // start tag.
            start = pointer;
        }
    }
   
    //This case need to be fixed.
    ///// <summary>
    ///// Embedded object test - Image, TextBox, RichTextBox, Button,
    ///// Animation, Styled control, databounded control
    ///// </summary>
    //public class TextRangeSerializationTest20 : TextRangeSerializationTest
    //{
    //    /// <summary>
    //    /// Ctor
    //    /// </summary>
    //    public TextRangeSerializationTest20() : base("Embedded object test - Image, TextBox, RichTextBox, Button, Animation, Styled control, databounded control") { }

    //    /// <summary>
    //    /// Custom Inline test
    //    /// </summary>
    //    /// <param name="sourceRichTextBox"></param>
    //    protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
    //    {
    //        Paragraph paragraph;
    //        Image image;
    //        TextBox textBox;
    //        RichTextBox richTextBox;
    //        Button button;
    //        /*
    //        KeyFrameDoubleAnimation animation;
    //        AnimationClockCollection clockCollection;
    //        Duration duration;
    //         */

    //        FrameworkElementFactory fef;
    //        Style style;

    //        TextBlock textBlock;
    //        Binding bind;
    //        TextBox uiStyledTextBox;
    //        TextBox dataBoundTextBox;
    //        Span span;
    //        Run run;

    //        run = new Run("This is a test");
    //        span = new Span(run);
    //        paragraph = new Paragraph(span);

    //        // insert some header text
    //        sourceRichTextBox.Document.Blocks.Clear(); // Remove default Paragraph
    //        sourceRichTextBox.Document.Blocks.Add(paragraph);

    //        // insert an image
    //        image = new Image();
    //        image.Source = BitmapFrame.Create(new Uri("pack://siteoforigin:,,,/test.png", UriKind.RelativeOrAbsolute));
    //        span.Inlines.Add(new InlineUIContainer(image));

    //        // insert a TextBox control
    //        textBox = new TextBox();
    //        textBox.Text = "Inner textbox text";
    //        span.Inlines.Add(new InlineUIContainer(textBox));

    //        // insert a RichTextBox control
    //        richTextBox = new RichTextBox();
    //        richTextBox.Document.Blocks.Add(new Paragraph(new Run("Inner rich textBox")));
    //        span.Inlines.Add(new InlineUIContainer(richTextBox));

    //        // insert a Button
    //        button = new Button();
    //        span.Inlines.Add(new InlineUIContainer(button));

    //        // set up animation and such
    //        /*
    //        duration = new Duration(TimeSpan.FromMilliseconds(500));
    //        animation = new KeyFrameDoubleAnimation();
    //        animation.BeginTime = TimeManager.CurrentGlobalTime;
    //        animation.RepeatBehavior = RepeatBehavior.Forever;
    //        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromPercent(0.0)));
    //        animation.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromPercent(0.5)));
    //        animation.Duration = blinkDuration;
    //        clockCollection = (AnimationClock)TimelineClock.FromTimeline(animation);
    //        */

    //        // Insert a UI styled TextBox
    //        uiStyledTextBox = new TextBox();
    //        style = new Style(typeof(TextBox));
    //        fef = new FrameworkElementFactory(typeof(TextBlock));
    //        ControlTemplate template = new ControlTemplate(typeof(TextBox));
    //        template.VisualTree = fef;
    //        style.Setters.Add(new Setter(TextBox.TemplateProperty, template));
    //        uiStyledTextBox.Style = style;

    //        // Insert a DataBounded TextBox
    //        textBlock = new TextBlock();
    //        dataBoundTextBox = new TextBox();
    //        textBlock.Text = "Data which is binded to the TextBox";
    //        dataBoundTextBox.DataContext = textBlock;
    //        bind = new Binding("Text");
    //        bind.Mode = BindingMode.OneWay;
    //        dataBoundTextBox.SetBinding(TextBox.TextProperty, bind);

    //        // Insert trailing text
    //        span.Inlines.Add(new Run("Some trailing text"));
    //    }
    //}

    /// <summary>TextRange covering many kind of textElement.</summary>
    public class TextRangeSerializationTest21 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest21() : base("TextRange covering many kinds of textElements.") { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {
            Span span ;
            Run run; 
            Paragraph paragraph;

            ////run
            paragraph = new Paragraph();
            run = new Run("This is a test ");
            SetDPForTextElement(run, DependencyPropertyData.RunPropertyData);
            paragraph.Inlines.Add(run);

            //bold
            run = new Run("Bold Text ");
            SetDPForTextElement(run, DependencyPropertyData.RunPropertyData);
            span = new Bold(run);
            SetDPForTextElement(span, DependencyPropertyData.InlinePropertyData);
            paragraph.Inlines.Add(span);

            //Italic 
            run = new Run("Italic Text ");
            SetDPForTextElement(run, DependencyPropertyData.RunPropertyData);
            span = new Italic(run);
            SetDPForTextElement(span, DependencyPropertyData.InlinePropertyData);
            paragraph.Inlines.Add(span);

            //Underline
            run = new Run("Underline Text ");
            SetDPForTextElement(run, DependencyPropertyData.RunPropertyData);
            span = new Underline(run);
            SetDPForTextElement(span, DependencyPropertyData.InlinePropertyData);
            paragraph.Inlines.Add(span);

            //span
            run = new Run("Span Text ");
            SetDPForTextElement(run, DependencyPropertyData.RunPropertyData);
            span = new Span(run);
            SetDPForTextElement(span, DependencyPropertyData.InlinePropertyData);
            paragraph.Inlines.Add(span);

            //Hyperlink
            run = new Run("Hyperlink Text ");
            SetDPForTextElement(run, DependencyPropertyData.RunPropertyData);
            span = new Hyperlink(run);
           
            SetDPForTextElement(span,  DependencyPropertyData.HyperlinkPropertyData);
            paragraph.Inlines.Add(span);
           
            //Set property for The Paragraph.
            SetDPForTextElement(paragraph, DependencyPropertyData.ParagraphPropertyData);
           
            sourceRichTextBox.Document.Blocks.Add(paragraph);          
        }
    }

    /// <summary>TextRange covering globalization specific properties.</summary>
    public class TextRangeSerializationTest22 : TextRangeSerializationTest
    {
        /// <summary>Ctor</summary>
        public TextRangeSerializationTest22() : base("TextRange covering globalization specific properties") { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceRichTextBox"></param>
        protected override void InitializeSourceTextBox(RichTextBox sourceRichTextBox)
        {            
            Run run;
            Paragraph paragraph;

            //Coverage for 
            paragraph = new Paragraph();
            run = new Run("1+2=3");
            run.FlowDirection = FlowDirection.RightToLeft;
            run.Language = XmlLanguage.GetLanguage("ar-sa");            
            run.SetValue(NumberSubstitution.CultureSourceProperty, NumberCultureSource.Override);
            run.SetValue(NumberSubstitution.CultureOverrideProperty, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            run.SetValue(NumberSubstitution.SubstitutionProperty, NumberSubstitutionMethod.Traditional);
            paragraph.Inlines.Add(run);            

            sourceRichTextBox.Document.Blocks.Add(paragraph);
        }
    }

    /// <summary>This test runs TextRangeSerializationTest1 to TextRangeSerializationTest22</summary>
    [Test(0, "TextOM", "TextRangeSerializationTestBVT", MethodParameters = "/TestCaseType:TextRangeSerializationTestBVT", Keywords = "MicroSuite")]
    [TestOwner("Microsoft"), TestTactics("376"), TestBugs("648"), TestTitle("TextRangeSerializationTestBVT"), TestLastUpdatedOn("Aug 9, 2006")]
    public class TextRangeSerializationTestBVT : ManagedCombinatorialTestCase
    {
        TextRangeSerializationTest _test = null;
 
        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _test.Run(MainWindow, new SimpleHandler(CheckResult));             
        }

        private void CheckResult()
        {
            Verifier.Verify(_test.TestResult);
            QueueDelegate(NextCombination);
        }

        /// <summary>
        /// return all the tests for text serialization.
        /// </summary>
        /// <returns></returns>
        public static object[] GetAllRoundTripTests
        {
            get
            {
                Assembly assembly;
                Type[] types;
                ArrayList testList;

                assembly = typeof(TextRangeSerializationTest).Assembly;
                types = assembly.GetTypes();
                testList = new ArrayList();

                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(typeof(TextRangeSerializationTest)))
                    {
                        testList.Add(Activator.CreateInstance(type));
                    }
                }

                return testList.ToArray();
            }
        }
    }

    /// <summary>
    /// Test class for invalid xaml. This test should include some xaml which fail schema validation
    /// </summary>
    [Test(0, "TextOM", "TextRangeSerializationInvalidXamlBVTTest", MethodParameters = "/TestCaseType=TextRangeSerializationInvalidXamlBVTTest")]
    [TestOwner("Microsoft"), TestTitle("TextRangeSerializationInvalidXamlBVTTest "), WindowlessTest(true), TestTactics("378"), TestLastUpdatedOn("Jan 25, 2007")]
    public class TextRangeSerializationInvalidXamlBVTTest : CustomTestCase
    {
        internal struct TestData
        {
            public TestData(string xamlString, string testName, Type expectedExceptionType)
            {
                XamlString = xamlString;
                TestName = testName;
                ExpectedExceptionType = expectedExceptionType;
                TestResult = false;
            }

            public string XamlString;
            public string TestName;
            public Type ExpectedExceptionType;
            public bool TestResult;
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TextRange textRange;
            //string message;
            RichTextBox richTextBox;

            richTextBox = new RichTextBox();
            textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            for (int i = 0; i < _testData.Length; i++)
            {
                Logger.Current.Log("Running test case {0} - {1}", (i + 1).ToString(), _testData[i].TestName);
                Logger.Current.Log("XamlString = {0}", _testData[i].XamlString);
                Logger.Current.Log("Expected exception type = {0}", _testData[i].ExpectedExceptionType.ToString());
                Logger.Current.Log("");
                if (_testData[i].ExpectedExceptionType == null)
                {
                    XamlUtils.TextRange_SetXml(textRange, _testData[i].XamlString);

                    _testData[i].TestResult = true;
                }
                else
                {
                    try
                    {
                        XamlUtils.TextRange_SetXml(textRange, _testData[i].XamlString);
                    }
                    catch (Exception e)
                    {
                        if (_testData[i].ExpectedExceptionType == e.GetType())
                        {
                            _testData[i].TestResult = true;
                        }
                    }
                }
            }

            Logger.Current.Log("******************* Test Result *******************");
            for (int i = 0; i < _testData.Length; i++)
            {
                Logger.Current.Log("Test {0} - " + _testData[i].TestName + ": " + (_testData[i].TestResult ? "passes" : "FAILS"), i + 1);
            }

            Logger.Current.ReportSuccess();
        }

        /// <summary>
        /// TestData array. Each TestData instance consists of the xaml string to be set, and the expected exception to be thrown.
        /// The test is considered a pass if the string is not accepted in serializer and the expected exception is thrown and caught
        /// </summary>
        private TestData[] _testData =
            {
                // orphan / incomplete tags <Bold>
                new TestData("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" Background=\"#FFFFFFFF\" FontFamily=\"Tahoma\" FontSize=\"11px\" Foreground=\"#FF000000\">This is header text<Bold>Bold<Italic>Italic</Italic>This is trailing text</FlowDocument>",
                    "Orphan / incomplete tags <Bold>",
                    typeof(System.Windows.Markup.XamlParseException)),

                // Unbalanced tags <Bold><Italic></Bold></Italic>
                new TestData("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" Background=\"#FFFFFFFF\" FontFamily=\"Tahoma\" FontSize=\"11px\" Foreground=\"#FF000000\">This is header text<Bold>Bold<Italic>Italic</Bold></Italic>This is trailing text</FlowDocument>",
                    "Unbalanced tags <Bold><Italic></Bold></Italic>",
                    typeof(System.Windows.Markup.XamlParseException)),

                // invalid property for TextElement Hello="#FFFFFFFF"
                new TestData("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" Hello=\"#FFFFFFFF\">This is header text</FlowDocument>",
                    "Invalid property for TextElement Hello=\"#FFFFFFFF\"",
                    typeof(System.Windows.Markup.XamlParseException)),

                // invalid value for valid property FontSize="#FFFFFFFF"
                new TestData("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" FontSize=\"#FFFFFFFF\">This is header text</FlowDocument>",
                    "Invalid value for valid property FontSize=\"#FFFFFFFF\"",
                    typeof(System.Windows.Markup.XamlParseException))
            };
    }

    /// <summary>
    /// Test that properties that affect the content through inheritance
    /// are part of the serialized content.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-1: EditingTest.exe /TestCaseType=TextRangeSourceSerialization (~16)
    /// </remarks>
    [Test(1, "TextOM", "TextRangeSourceSerialization", MethodParameters = "/TestCaseType=TextRangeSourceSerialization")]
    [TestOwner("Microsoft"), TestWorkItem("44"), TestTactics("375"), TestBugs("649")]
    public class TextRangeSourceSerialization : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            RichTextBox richTextBox;    // Source rich text box for transfer.
            RichTextBox target;         // Target rich text box for transfer.
            TextPointer inspection;     // TextPointer used to inspect values in target.
            TextRange contentRange;     // TextRange to get content from source.
            object targetValue;         // Value in target content.

            // Set the value on the control.
            richTextBox = new RichTextBox();
            richTextBox.SetValue(_propertyData.Property, _propertyData.TestValue);

            // Set text into the default paragraph
            ((Paragraph)richTextBox.Document.Blocks.FirstBlock).Inlines.Add(new Run("sample text"));

            // Create a range that should have the inherited content.
            TextPointer start = richTextBox.Document.ContentStart;
            start = start.GetPositionAtOffset(2);
            TextPointer end = start;
            end = end.GetPositionAtOffset(2);
            contentRange = new TextRange(start, end);

            // Verify that when applied to a new RichTextBox, the content
            // has the same properties.
            target = new RichTextBox();
            XamlUtils.TextRange_SetXml(new TextRange(target.Document.ContentStart, target.Document.ContentEnd), XamlUtils.TextRange_GetXml(contentRange));

            // Verify that the old property still holds.
            inspection = target.Document.ContentStart;
            while (inspection.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.Text)
            {
                inspection = inspection.GetNextContextPosition(LogicalDirection.Forward);
            }
            targetValue = inspection.Parent.GetValue(_propertyData.Property);

            if (targetValue.ToString() == "Georgia")
            {
                targetValue = new FontFamily("Arial");
            }
            else if (targetValue.ToString() == "16")
            {
                targetValue = 10.0;
            }

            if (!Verifier.AreValuesEqual(targetValue, _propertyData.TestValue))
            {
                Log("Serialized text:\r\n" + XamlUtils.TextRange_GetXml(contentRange));
                throw new Exception("Target value is " + targetValue + ", but expected " + _propertyData.TestValue);
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private data.

        private DependencyPropertyData _propertyData = null;

        #endregion Private data.
    }

    /// <summary>
    /// Test that the plain text converter works with all script types.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-1: EditingTest.exe /TestCaseType=PlainTextConverterScripts (~2s)
    /// </remarks>
    [Test(1, "TextOM", "PlainTextConverterScripts", MethodParameters = "/TestCaseType=PlainTextConverterScripts")]
    [TestOwner("Microsoft"), TestWorkItem("43"), TestTactics("374"), TestLastUpdatedOn(" April 27, 2006")]
    public class PlainTextConverterScripts : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is PasswordBox)
            {
                QueueDelegate(NextCombination);
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);

                _scriptArray = TextScript.Values;
                _count = _scriptArray.Length;
                _count--;

                _controlWrapper.Clear();

                QueueDelegate(ExecuteTrigger);
            }
        }

        private void ExecuteTrigger()
        {
            Log("Script Name: " + _scriptArray[_count].Name);
            _controlWrapper.Text = _scriptArray[_count].Sample;
            QueueDelegate(VerifyTextIsSet);
        }

        private void VerifyTextIsSet()
        {
            string actualText = _controlWrapper.Text;
            string expectedText = (_element is TextBox) ? _scriptArray[_count].Sample : (_scriptArray[_count].Sample + "\r\n");

            Verifier.Verify(actualText == expectedText,
                "Serialized text [" + actualText + "] matches expected text [" + expectedText + "]",
                true);

            _count--;
            if (_count < 0)
            {
                QueueDelegate(NextCombination);
            }
            else
            {
                QueueDelegate(ExecuteTrigger);
            }
        }

        #endregion Main flow.

        #region Private data.

        private UIElementWrapper _controlWrapper;

        private TextScript[] _scriptArray;
        private int _count = 0;

        private TextEditableType _editableType = null;
        private UIElement _element = null;

        #endregion Private data.
    }

    /// <summary>
    /// Test that the plain text converter works with all elemetns.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-1: EditingTest.exe /TestCaseType=PlainTextConverterElements
    /// </remarks>
    [Test(1, "TextOM", "PlainTextConverterElements", MethodParameters = "/TestCaseType=PlainTextConverterElements")]
    [TestOwner("Microsoft"), TestWorkItem("41,42"), TestTactics("373"),
     TestBugs("762,763,764,765")]
    public class PlainTextConverterElements: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            RichTextBox richTextBox;    // Source rich text box for transfer.
            FlowDocument document;      // Document to work with.
            TextRange range;            // Range to extract content from.
            string actualText;          // Plain text version of content.
            string expectedText;        // Expected plain text version of content.
            bool match;                 // Whether the actual and expected texts match.

            // Set the value on the control.
            richTextBox = new RichTextBox();
            document = CreateSampleDocumentForType(_elementType, _elementCount, _textSample);
            richTextBox.Document = document;

            // Convert the RichTextBox contents to plain text.
            range = new TextRange(document.ContentStart, document.ContentEnd);
            actualText = range.Text;
            expectedText = GetExpectedPlainText(range);

            match = actualText == expectedText;

            // NOTE: by having these checks here, we at least test that
            // invoking the converter with these elements does not crash.

            match = match || this._elementType == typeof(Figure);
            match = match || this._elementType == typeof(Floater);

            if (!match)
            {
                Log("Expected text [" + TextUtils.ConvertToSingleLineAnsi(expectedText) + "]");
                Log("Actual text   [" + TextUtils.ConvertToSingleLineAnsi(actualText) + "]");
                Log("XAML          [" + XamlUtils.TextRange_GetXml(range) + "]");
                TextTreeLogger.LogContainer("plaintext-mismatch", document.ContentStart,
                    range.Start, "Range start", range.End, "Range end");
                throw new Exception("Serialized text [" + actualText + "] matches expected text [" + expectedText + "]");
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>Creates a sample document containing the specified type.</summary>
        /// <param name="elementType">Type of element to create in sample document.</param>
        /// <param name="elementCount">Number of sibling elements to create.</param>
        /// <param name="textSample">Text to use when adding content.</param>
        /// <returns>A new FlowDocument instance containing occurences of the specified textElementType.</returns>
        private static FlowDocument CreateSampleDocumentForType(
            Type elementType, int elementCount, string textSample)
        {
            TextElement element;
            FlowDocument document;

            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            // These variables are used as the parent elements of multiple
            // sibling elements.
            Paragraph parentParagraph = null;
            List parentList = null;
            Table parentTable = null;
            TableRowGroup parentTableRowGroup = null;
            TableRow parentTableRow = null;

            document = new FlowDocument();

            for (int i = 0; i < elementCount; i++)
            {
                // Create an element and generate some meaningful content inside.
                element = (TextElement)Activator.CreateInstance(elementType);
                GenerateContent(element, textSample);

                // Insert the element inside the document to be returned,
                // creating any required intermediate elements.
                if (element is Block)
                {
                    document.Blocks.Add((Block)element);
                }
                else if (element is Inline)
                {
                    if (parentParagraph == null)
                    {
                        parentParagraph = new Paragraph();
                        document.Blocks.Add(parentParagraph);
                    }
                    parentParagraph.Inlines.Add((Inline)element);
                }
                else if (element is ListItem)
                {
                    if (parentList == null)
                    {
                        parentList = new List();
                        document.Blocks.Add(parentList);
                    }
                    parentList.ListItems.Add((ListItem)element);
                }
                else if (element is TableRowGroup)
                {
                    if (parentTable == null)
                    {
                        parentTable = new Table();
                        document.Blocks.Add(parentTable);
                    }
                    parentTable.RowGroups.Add((TableRowGroup)element);
                }
                else if (element is TableRow)
                {
                    if (parentTable == null)
                    {
                        parentTable = new Table();
                        parentTableRowGroup = new TableRowGroup();
                        parentTable.RowGroups.Add(parentTableRowGroup);
                        document.Blocks.Add(parentTable);
                    }
                    parentTableRowGroup.Rows.Add((TableRow)element);
                }
                else if (element is TableCell)
                {
                    if (parentTable == null)
                    {
                        parentTable = new Table();
                        parentTableRowGroup = new TableRowGroup();
                        parentTableRow = new TableRow();
                        parentTableRowGroup.Rows.Add(parentTableRow);
                        parentTable.RowGroups.Add(parentTableRowGroup);
                        document.Blocks.Add(parentTable);
                    }
                    parentTableRow.Cells.Add((TableCell)element);
                }
            }

            return document;
        }

        /// <summary>
        /// Generates text inside the element (including all required
        /// intermediate elements).
        /// </summary>
        /// <param name="element">Element to generate content in.</param>
        /// <param name="textSample">Text to use when adding content.</param>
        private static void GenerateContent(TextElement element, string textSample)
        {
            if (element is Run)
            {
                ((Run)element).Text = textSample;
            }
            else if (TextElementType.IsValidChildType(typeof(Run), element.GetType()))
            {
                new Run(textSample, element.ContentStart);
            }
            else if (TextElementType.IsValidChildType(typeof(Paragraph), element.GetType()))
            {
                if (element is ListItem)
                {
                    BlockCollection blocks = ((ListItem)element).Blocks;
                    if (null == blocks.FirstBlock)
                    {
                        blocks.Add(new Paragraph(new Run(textSample)));
                    }
                    else
                    {
                        blocks.InsertBefore(blocks.FirstBlock, new Paragraph(new Run(textSample)));
                    }
                }
                else
                {
                    Logger.Current.Log("Now only ListItems (as an Textelement) contains blocks.");
                }
            }
            else if (element is List)
            {
                ((List)element).ListItems.Add(new ListItem(new Paragraph(new Run(textSample))));
            }
            else if (element is Table)
            {
                TableRowGroup group = new TableRowGroup();
                GenerateContent(group, textSample);
                ((Table)element).RowGroups.Add(group);
            }
            else if (element is TableRowGroup)
            {
                TableRow row = new TableRow();
                GenerateContent(row, textSample);
                ((TableRowGroup)element).Rows.Add(row);
            }
            else if (element is TableRow)
            {
                TableCell cell = new TableCell();
                GenerateContent(cell, textSample);
                ((TableRow)element).Cells.Add(cell);
            }
            else if (element is LineBreak)
            {
                // LineBreak elements must be empty.
            }
            else if (element is InlineUIContainer)
            {
                if (textSample.Length > 0)
                {
                    Button button;
                    button = new Button();
                    button.Content = textSample;
                    ((InlineUIContainer)element).Child = button;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot generate content for element " + element);
            }
        }

        private static string GetExpectedPlainText(TextRange range)
        {
            TextPointer cursor;
            StringBuilder builder;
            char[] textBuffer;
            bool inListItem;

            builder = new StringBuilder();
            cursor = range.Start;
            textBuffer = new char[1];

            inListItem = false;
            while (cursor.CompareTo(range.End) < 0)
            {
                Type elementType;
                DependencyObject element;

                switch (cursor.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.None:
                        throw new InvalidOperationException("Cannot find end pointer.");
                    case TextPointerContext.Text:
                        cursor.GetTextInRun(LogicalDirection.Forward, textBuffer, 0, 1);
                        builder.Append(textBuffer[0]);
                        cursor = cursor.GetPositionAtOffset(1);
                        break;
                    case TextPointerContext.ElementStart:
                        element = cursor.GetAdjacentElement(LogicalDirection.Forward);
                        elementType = element.GetType();
                        if (elementType == typeof(Figure) ||
                            elementType == typeof(Floater))
                        {
                            cursor = ((TextElement)cursor.GetAdjacentElement(LogicalDirection.Forward)).ContentEnd;
                        }
                        else if (elementType == typeof(ListItem))
                        {
                            inListItem = true;
                            builder.Append("\x2022\t");
                        }
                        cursor = cursor.GetPositionAtOffset(1);
                        break;
                    case TextPointerContext.ElementEnd:
                        element = cursor.GetAdjacentElement(LogicalDirection.Forward);
                        elementType = element.GetType();
                        if (elementType == typeof(LineBreak))
                        {
                            builder.Append("\r\n");
                        }
                        else if (elementType == typeof(Paragraph))
                        {
                            if (IsLastParagraphOfCell((Paragraph)element) &&
                                !IsLastParagraphOfRow((Paragraph)element))
                            {
                                builder.Append("\t");
                            }
                            else
                            {
                                builder.Append("\r\n");
                            }
                        }

                        if (!inListItem && elementType == typeof(ListItem))
                        {
                            builder.Insert(0, "\x2022\t");
                        }
                        cursor = cursor.GetPositionAtOffset(1);
                        break;
                    case TextPointerContext.EmbeddedElement:
                        builder.Append(' '); // space is represent for EmbeddedElement
                        cursor = cursor.GetPositionAtOffset(1);
                        break;
                }
            }

            return builder.ToString();
        }

        private static bool IsLastParagraphOfCell(Paragraph paragraph)
        {
            TableCell parent;

            if (paragraph == null)
            {
                throw new ArgumentNullException("paragraph");
            }

            parent = paragraph.Parent as TableCell;
            if (parent == null)
            {
                return false;
            }

            return parent.Blocks.LastBlock == paragraph;
        }

        private static bool IsLastParagraphOfRow(Paragraph paragraph)
        {
            TableCell parent;
            TableRow parentRow;

            if (paragraph == null)
            {
                throw new ArgumentNullException("paragraph");
            }

            parent = paragraph.Parent as TableCell;
            if (parent == null)
            {
                return false;
            }

            parentRow = parent.Parent as TableRow;
            if (parentRow == null)
            {
                return false;
            }

            return parent.Blocks.LastBlock == paragraph &&
                parentRow.Cells[parentRow.Cells.Count - 1] == parent;
        }

        #endregion Helper methods.

        #region Private data.

        /// <summary>The type of element to test.</summary>
        private Type _elementType = null;

        /// <summary>The number of elements to include.</summary>
        private int _elementCount=0;

        /// <summary>Text to use when adding content.</summary>
        private string _textSample=string.Empty;

        #endregion Private data.
    }
}
