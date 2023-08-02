// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "PropertyStyling", MethodName = "Run")]
    class PropertyStylingTest : AvalonTest
    {
        private Window _w;
        private ushort _numContentLines = 0;
        private int _testNum;

        [Variation("Bottomless", 0)]
        [Variation("Bottomless", 1)]
        [Variation("Paginated", 0)]
        [Variation("Paginated", 1)]
        public PropertyStylingTest(string testName, int testNum)
        {           
             InitializeSteps += new TestStep(Initialize);
             CleanUpSteps += new TestStep(CleanUp);
             this._testNum = testNum;

             switch (testName)
             {
                 case "Bottomless":
                     RunSteps += new TestStep(BottomlessPropertyStylingTest);
                     break;

                 case "Paginated":
                     RunSteps += new TestStep(PaginatedPropertyStylingTest);
                     break;
             }
        }
       
        private TestResult Initialize()
        {
            _w = new Window();
            _w.Height = 560;
            _w.Width = 940;
            _w.Left = 0;
            _w.Top = 0;
            
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private TestResult BottomlessPropertyStylingTest()
        {
            StreamReader strReaderSW = strReaderSW = new StreamReader("PropertyStylingTextFlow.xaml");
            _w.Content = XamlReader.Load(strReaderSW.BaseStream);
            FlowDocumentScrollViewer tf = (FlowDocumentScrollViewer)LogicalTreeHelper.FindLogicalNode((DependencyObject)_w.Content, "TextFlow");
            _w.Show();
            
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            APITests(tf);
           
            return TestResult.Pass;
        }

        private TestResult PaginatedPropertyStylingTest()
        {
            StreamReader strReaderSW = strReaderSW = new StreamReader("PropertyStylingFlowDocument.xaml");
            _w.Content = XamlReader.Load(strReaderSW.BaseStream);
            FlowDocument fd = (FlowDocument)LogicalTreeHelper.FindLogicalNode((DependencyObject)_w.Content, "FlowDocument");
            _w.Show();

            APITests(fd);
            
            return TestResult.Pass;
        }
    
        private void APITests(FlowDocumentScrollViewer tf)
        {
            Style theStyle = new Style(typeof(FlowDocument));
            theStyle.Setters.Add(new Setter(FlowDocument.LineHeightProperty, (double)20));
            tf.Document.SetValue(FlowDocument.StyleProperty, theStyle);
            CommonFunctionality.FlushDispatcher();

            theStyle = new Style(typeof(FlowDocument), theStyle);
            theStyle.Setters.Add(new Setter(FlowDocument.TextAlignmentProperty, TextAlignment.Center));
            tf.Document.Style = new Style(typeof(FlowDocument), theStyle);
            CommonFunctionality.FlushDispatcher();

            switch (_testNum)
            {
                case 0:
                    TestLineHeight(tf, tf.Document.LineHeight);
                    break;
                case 1:
                    TestTextAlignment(tf);
                    break;
            }          
        }

        private void APITests(FlowDocument fd)
        {
            Style theStyle = new Style(typeof(FlowDocument));
            theStyle.Setters.Add(new Setter(FlowDocument.LineHeightProperty, (double)20));
            fd.SetValue(FlowDocument.StyleProperty, theStyle);
            CommonFunctionality.FlushDispatcher();            

            theStyle = new Style(typeof(FlowDocument), theStyle);
            theStyle.Setters.Add(new Setter(FlowDocument.TextAlignmentProperty, TextAlignment.Center));
            fd.Style = new Style(typeof(FlowDocument), theStyle);
            CommonFunctionality.FlushDispatcher();

            switch (_testNum)
            {
                case 0:
                    TestLineHeight(fd, fd.LineHeight);
                    break;

                case 1:
                    TestTextAlignment(fd);
                    break;
            }          
        }

        #region TEXTALIGNMENT TEST
        private void TestTextAlignment(FlowDocument fd)
        {
            WalkObjTree(fd, 1, 1);
        }

        private void TestTextAlignment(FlowDocumentScrollViewer tf)
        {
            WalkObjTree(tf, 1, 0);
        }        

        private void ProcessTextAlignment(TextParagraphResultW paragraph)
        {
            uint i = 0;
            double errorMargin = 6;

            foreach (LineResultW line in paragraph.Lines)
            {
                i++;
                Rect rectLeft = line.StartPosition.GetCharacterRect(LogicalDirection.Forward);
                Rect rectRight = line.EndPosition.GetCharacterRect(LogicalDirection.Backward);
                if (Math.Abs(line.LayoutBox.Right - rectRight.Right) < Math.Abs(rectLeft.Left - line.LayoutBox.Left) - errorMargin)
                {
                    TestLog.Current.Result = TestResult.Fail;
                    TestLog.Current.LogStatus("TextAlignment of FlowDocumentScrollViewer invalid value: Line " + i + " Top = " +
                        line.LayoutBox.Top + " and Line " + i + " rectLeft = " + rectLeft.Left);
                }
            }
        }
        #endregion

        #region LINEHEIGHT TEST
        private void TestLineHeight(FlowDocument fd, double specifiedLineHeight)
        {
            WalkObjTree(fd, 0, 1);
        }

        private void TestLineHeight(FlowDocumentScrollViewer tf, double specifiedLineHeight)
        {
            WalkObjTree(tf, 0, 0);
        }

        private void ProcessLines(TextParagraphResultW paragraph, double specifiedLineHeight)
        {
            uint i = 0;
            double compareAgainst = 0.0, errorMargin = 0.1;

            foreach (LineResultW line in paragraph.Lines)
            {
                _numContentLines++;
                if (double.IsNaN(specifiedLineHeight) || double.IsInfinity(specifiedLineHeight))
                {
                    if (i == 0)
                    {
                        compareAgainst = line.LayoutBox.Bottom - line.LayoutBox.Top;
                    }
                }
                else
                {
                    compareAgainst = specifiedLineHeight;
                }

                i++;

                if (Math.Abs(line.LayoutBox.Bottom - line.LayoutBox.Top - compareAgainst) > errorMargin)
                {
                    TestLog.Current.Result = TestResult.Fail;
                    TestLog.Current.LogStatus("LineHeight of TextBlock invalid value: Line " + i + " Top = " +
                        line.LayoutBox.Top + " and Line " + i + " Bottom = " + line.LayoutBox.Bottom);
                }
            }
        }
        #endregion 

        #region COMMON TREE WALK 
        private void WalkObjTree(DependencyObject dobj, ushort iTestWhat, ushort rootElement)
        {
            FlowDocument fd = null;
            FlowDocumentScrollViewer tf = null;
            TextDocumentViewW tdv = null;            
            switch (rootElement)
            {
                case 0:                    
                    tf = (FlowDocumentScrollViewer)dobj;
                    DocumentPageTextViewW dptv = DocumentPageTextViewW.FromIServiceProvider(tf);
                    tdv = dptv.TextDocumentViewW;
                    break;
                case 1:
                    fd = (FlowDocument)dobj;
                    tdv = TextDocumentViewW.FromIServiceProvider((((IDocumentPaginatorSource)fd).DocumentPaginator).GetPage(0) as IServiceProvider);
                    break;
            }

            if (tdv == null) return;            
            
            foreach (ColumnResultW columns in tdv.Columns)
            {
                foreach (ParagraphResultW paragraph in columns.Paragraphs)
                {
                    if (paragraph is ContainerParagraphResultW)
                    {
                        ContainerParagraphResultW containerparagraph = paragraph as ContainerParagraphResultW;
                        foreach (ContainerParagraphResultW cparagraph in containerparagraph.Paragraphs)
                        {
                            foreach (ParagraphResultW pW in cparagraph.Paragraphs)
                            {
                                if (pW is TextParagraphResultW)
                                {
                                    TestProperty(dobj, iTestWhat, rootElement, (TextParagraphResultW)pW);
                                }
                            }
                        }
                    }
                    else if (paragraph is TextParagraphResultW)
                    {
                        TestProperty(dobj, iTestWhat, rootElement, paragraph as TextParagraphResultW);
                    }
                }
            }
        }
        #endregion

        private void TestProperty(DependencyObject dobj, ushort iTestWhat, ushort rootElement, TextParagraphResultW textparagraph)
        {
            double lineHeight = 0;
            switch (rootElement)
            {
                case 0:
                    lineHeight = ((FlowDocumentScrollViewer)dobj).Document.LineHeight;
                    break;
                case 1:
                    lineHeight = ((FlowDocument)dobj).LineHeight;
                    break;
            }
            
            switch (iTestWhat)
            {
                case 0:
                    ProcessLines(textparagraph, lineHeight);
                    break;
                case 1:
                    ProcessTextAlignment(textparagraph);
                    break;
            }
        }
    }
}
