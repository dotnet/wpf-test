// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections;

namespace DRT
{
    // Regression tests for the TextContainer.
    internal class TextContainerSuite : DrtTestSuite
    {
        //  Constructors

        #region Constructors

        // Creates a new instance.
        internal TextContainerSuite() : base("TextContainer")
        {
        }

        #endregion Constructors
 
        //  Public Methods
 
        #region Public Methods

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] { new DrtTest(TextEditTests),
                                   new DrtTest(CollectionTests),
                                   new DrtTest(TextElementCtorTests),
                                 };
        }

        #endregion Public Methods

        //  Private Methods
 
        #region Private Methods

        // Verifies adding/removing text from FlowDocuments.
        private void TextEditTests()
        {
            const string paragraphText =
                    "The story which follows was first written out in Paris " +
                    "during the Peace Conference, from notes jotted daily on " +
                    "the march, strengthened by some reports sent to my chiefs in " +
                    "Cairo. Afterwards, in the autumn of 1919, this first draft " +
                    "and some of the notes were lost. It seemed to me historically " +
                    "needful to reproduce the tale, as perhaps no one but myself " +
                    "in Feisal's army had thought of writing down at the time what " +
                    "we felt, what we hoped, what we tried. So it was built again " +
                    "with heavy repugnance in London in the winter of 1919-20 from " +
                    "memory and my surviving notes. The record of events was not " +
                    "dulled in me and perhaps few actual mistakes crept in - " +
                    "except in details of dates or numbers - but the outlines and " +
                    "significance of things had lost edge in the haze of new interestz.";

            int i;
            int j;
            int k;
            FlowDocument document = new FlowDocument();

            Run run = new Run("");
            document.Blocks.Add(new Paragraph(run));

            for (i = 0; i < 10000; i++)
            {
                run.ContentStart.InsertTextInRun("0123456789");
            }
            DRT.Assert(run.ContentStart.GetTextRunLength(LogicalDirection.Forward) == 100000);

            char []text = new char[100000];
            run.ContentStart.GetTextInRun(LogicalDirection.Forward, text, 0, 100000);

            for (j = 0; j < 10000; j += 10)
            {
                for (k = 0; k < 10; k++)
                {
                    DRT.Assert(text[j + k] == k + 0x30, "Wrong character returned!");
                }
            }

            document = new FlowDocument();
            for (i = 0; i < 1000; i++)
            {
                Paragraph paragraph = new Paragraph(new Run(paragraphText));
                document.Blocks.Add(paragraph);
            }

            for (i = 0; i < 9; i++)
            {
                TextPointer p1 = document.ContentStart.GetPositionAtOffset(23);
                TextPointer p2 = p1.GetPositionAtOffset(100 * paragraphText.Length);
                new TextRange(p1, p2).Text = "";
            }
        }

        // Tests on the collection apis.
        private void CollectionTests()
        {
            int i;

            // Test TextElementCollection.AddRange
 
            Paragraph paragraph = new Paragraph();

            paragraph.Inlines.AddRange(new Inline[] { new Run(), new Run(), new Run() });
            paragraph.Inlines.AddRange(new Inline[] { new Span(), new Span(), new Span() });

            for (i = 0; i < 3; i++)
            {
                DRT.Assert(((IList)paragraph.Inlines)[i] is Run, "Missing expected Run!");
            }
            for (; i < 6; i++)
            {
                DRT.Assert(((IList)paragraph.Inlines)[i] is Span, "Missing expected Span!");
            }

            // TextElementCollection.IList

            IList list = paragraph.Inlines;

            for (i = 0; i < list.Count; i++)
            {
                Inline inline = (Inline)list[i];
                list[i] = inline;
            }

            i = list.Add(new Run());
            DRT.Assert(i == 6, "Bad return value from IList.Add! (0)");
            Run run = new Run();
            i = list.Add(run);
            DRT.Assert(i == 7, "Bad return value from IList.Add! (1)");
            run.Text = "foo"; // Flush the TextElementCollectin cache.
            run = new Run();
            i = list.Add(run);
            DRT.Assert(i == 8, "Bad return value from IList.Add! (2)");

            i = list.IndexOf(run);
            DRT.Assert(i == 8, "Bad run index! (0)");

            Run run2 = new Run();
            list.Insert(4, run2);
            i = list.IndexOf(run2);
            DRT.Assert(i == 4, "Bad run index! (1)");
            i = list.IndexOf(run);
            DRT.Assert(i == 9, "Bad run index! (2)");

            list.RemoveAt(4);
            i = list.IndexOf(run2);
            DRT.Assert(i == -1, "Bad run index! (3)");

            list.Clear();
            DRT.Assert(list.Count == 0, "Bad list count!");

            list.Insert(0, new Run());
            list.Insert(1, new Run());

            paragraph.Inlines.AddRange(new Inline[] { new Run(), new Run(), new Run() });
            run = new Run();
            paragraph.Inlines.Add(run);
            paragraph.Inlines.AddRange(new Inline[] { new Span(), new Span(), new Span() });
            DRT.Assert(paragraph.Inlines.Contains(run), "Missing Run!");
            paragraph.Inlines.Remove(run);
            DRT.Assert(!paragraph.Inlines.Contains(run), "Extra Run!");
        }

        // Tests the TextElement ctors.
        private void TextElementCtorTests()
        {
            // <Paragraph><Run/></Paragraph>
            FlowDocument flowDocument = new FlowDocument();

            string xml;

            xml = DrtEditing.GetTextRangeXml(new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd));

            // <Paragraph><Run>0123456789</Run><Run/></Paragraph>
            new Run("0123456789", flowDocument.ContentStart);

            xml = DrtEditing.GetTextRangeXml(new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd));

            // <Paragraph><Run>012</Run><IUIC/><Run>3456789</Run></Paragraph>
            TextPointer position = flowDocument.ContentStart.GetPositionAtOffset(5, LogicalDirection.Forward);
            new InlineUIContainer(new Image(), position);

            xml = DrtEditing.GetTextRangeXml(new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd));

            // <Paragraph><Run>012</Run><HyperLink><IUIC/><Run>3456</Run></HyperLink><Run>789</Run></Paragraph>
            TextPointer start = flowDocument.ContentStart.GetPositionAtOffset(5, LogicalDirection.Forward);
            TextPointer end = flowDocument.ContentStart.GetPositionAtOffset(14, LogicalDirection.Forward);
            NewHyperlink(start, end);

            xml = DrtEditing.GetTextRangeXml(new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd));

            // Test negative case for span ctor when HL parent cannot be split
            bool thrown = false;
            try
            {
                start = flowDocument.ContentStart.GetPositionAtOffset(3, LogicalDirection.Forward);
                end = flowDocument.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
                NewHyperlink(start, end);
            }
            catch (InvalidOperationException)
            {
                thrown = true;
            }
            if (!thrown)
            {
                throw new Exception("Span ctor did not throw exception for splitting hyperlink ancestor");
            }

            // <Paragraph><Run>0</Run><HyperLink><Run>12</Run><IUIC/><Run>3456</Run></HyperLink><Run>789</Run></Paragraph>
            start = flowDocument.ContentStart.GetPositionAtOffset(3, LogicalDirection.Forward);
            end = flowDocument.ContentStart.GetPositionAtOffset(18, LogicalDirection.Forward);
            NewHyperlink(start, end);

            xml = DrtEditing.GetTextRangeXml(new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd));
            DrtEditing.AssertEqualXml(/*actual*/xml, /*expected*/"<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" TextAlignment=\"Justify\" LineHeight=\"Auto\" IsHyphenationEnabled=\"False\" xml:lang=\"en-us\" FlowDirection=\"LeftToRight\" NumberSubstitution.CultureSource=\"Text\" NumberSubstitution.Substitution=\"AsCulture\" FontFamily=\"Georgia\" FontStyle=\"Normal\" FontWeight=\"Normal\" FontStretch=\"Normal\" FontSize=\"16\" Foreground=\"#FF000000\" Typography.StandardLigatures=\"True\" Typography.ContextualLigatures=\"True\" Typography.DiscretionaryLigatures=\"False\" Typography.HistoricalLigatures=\"False\" Typography.AnnotationAlternates=\"0\" Typography.ContextualAlternates=\"True\" Typography.HistoricalForms=\"False\" Typography.Kerning=\"True\" Typography.CapitalSpacing=\"False\" Typography.CaseSensitiveForms=\"False\" Typography.StylisticSet1=\"False\" Typography.StylisticSet2=\"False\" Typography.StylisticSet3=\"False\" Typography.StylisticSet4=\"False\" Typography.StylisticSet5=\"False\" Typography.StylisticSet6=\"False\" Typography.StylisticSet7=\"False\" Typography.StylisticSet8=\"False\" Typography.StylisticSet9=\"False\" Typography.StylisticSet10=\"False\" Typography.StylisticSet11=\"False\" Typography.StylisticSet12=\"False\" Typography.StylisticSet13=\"False\" Typography.StylisticSet14=\"False\" Typography.StylisticSet15=\"False\" Typography.StylisticSet16=\"False\" Typography.StylisticSet17=\"False\" Typography.StylisticSet18=\"False\" Typography.StylisticSet19=\"False\" Typography.StylisticSet20=\"False\" Typography.Fraction=\"Normal\" Typography.SlashedZero=\"False\" Typography.MathematicalGreek=\"False\" Typography.EastAsianExpertForms=\"False\" Typography.Variants=\"Normal\" Typography.Capitals=\"Normal\" Typography.NumeralStyle=\"Normal\" Typography.NumeralAlignment=\"Normal\" Typography.EastAsianWidths=\"Normal\" Typography.EastAsianLanguage=\"Normal\" Typography.StandardSwashes=\"0\" Typography.ContextualSwashes=\"0\" Typography.StylisticAlternates=\"0\"><Paragraph><Run>0</Run><Hyperlink Foreground=\"#FF0000FF\" TextDecorations=\"Underline\"><Run>12</Run><Run> </Run><Run>3456</Run></Hyperlink><Run>789</Run></Paragraph></Section>", "Mismatch in serialized xaml");
        }

        void NewHyperlink(TextPointer start, TextPointer end)
        {
            // Hyperlink uses a system color.  Override with Blue to make serialization comparision predictable.
            Hyperlink hl = new Hyperlink(start, end);
            hl.Foreground = System.Windows.Media.Brushes.Blue;
        }

        #endregion Private Methods
    }
}