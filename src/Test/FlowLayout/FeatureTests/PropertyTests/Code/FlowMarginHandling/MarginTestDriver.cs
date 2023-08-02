// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "FlowMarginHandling", MethodName = "Run")]
    public class MarginTestDriver: AvalonTest
    {
        private double _errMargin = 2.0;
        private Window _w;
        private DocumentPage _dpage;

        [Variation("Paragraph")]
        [Variation("TextBlock")]
        [Variation("BlockUIContainer")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Figure")]
        [Variation("Floater")]
        [Variation("Section")]
        [Variation("Table")]   
        public MarginTestDriver(string testName)
        {           
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
           
            switch (testName)
            {
                case "Paragraph":
                    {
                        RunSteps += new TestStep(TestParagraphMargins);
                        break;
                    }

                case "TextBlock":
                    {
                        RunSteps += new TestStep(TestTextBlockMargins);
                        break;
                    }

                case "BlockUIContainer":
                    {
                        RunSteps += new TestStep(TestBlockUIContainerMargins);
                        break;
                    }

                case "FlowDocumentScrollViewer":
                    {
                        RunSteps += new TestStep(TestFlowDocumentScrollViewerMargins);
                        break;
                    }

                case "Figure":
                    {
                        RunSteps += new TestStep(TestFigureMargins);
                        break;
                    }

                case "Floater":
                    {
                        RunSteps += new TestStep(TestFloaterMargins);
                        break;
                    }
                case "Section":
                    {
                        RunSteps += new TestStep(TestSectionMargins);
                        break;
                    }

                case "Table":
                    {
                        RunSteps += new TestStep(TestTableMargins);
                        break;
                    }
            }         
        }
       
        private TestResult Initialize()
        {
            _w = new Window();
            StreamReader sr = new StreamReader("Margins_Paragraph.xaml");
            _w.Content = XamlReader.Load(sr.BaseStream);
            _w.Height = 700;
            _w.Width = 800;            
            _w.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            FlowDocumentPageViewer spv = _w.Content as FlowDocumentPageViewer;
            CommonFunctionality.FlushDispatcher();
            
            DocumentPaginator dpaginator = spv.Document.DocumentPaginator;
            _dpage = dpaginator.GetPage(0);
                        
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private TestResult TestParagraphMargins()
        {
            TestLog.Current.LogStatus("Testing Margin Collapsing b/w Paragraphs");
            Paragraph p1 = LogicalTreeHelper.FindLogicalNode(_w, "Para1") as Paragraph;
            Paragraph p2 = LogicalTreeHelper.FindLogicalNode(_w, "Para2") as Paragraph;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(p1);
            ReadOnlyCollection<Rect> r2 = (_dpage as IContentHost).GetRectangles(p2);

            // Check p1 Top Margin
            if (r1[0].Top > p1.Margin.Top)
            {
                TestLog.Current.LogEvidence("Top Margin for First Paragraph is not collapsed. Observed value = " + r1[0].Top);
                TestLog.Current.Result = TestResult.Fail;
            }

            // Calculate Expected Value for Collapse
            double expectedValue = CalcExpectedValueTopBottom(p1, p2);
            if (Math.Abs(r2[0].Top - r1[0].Bottom) != expectedValue)
            {
                TestLog.Current.LogEvidence("Margin Expected = " + expectedValue + ", Observed = " + Math.Abs(r2[0].Top - r1[0].Bottom));
                TestLog.Current.Result = TestResult.Fail;
            }

            // Check Left Values as well
            if (p1.Margin.Left == 0)
            { // no margin left, then the left margin should be same as top margin for paragraph
                if (r1[0].Left != r1[0].Top)
                {
                    TestLog.Current.LogEvidence("Left Margin Expected = " + r1[0].Top + ", Observed = " + r1[0].Left);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }

        private TestResult TestTextBlockMargins()
        {
            TestLog.Current.LogStatus("Testing Margin on TextBlock");
            Paragraph p3 = LogicalTreeHelper.FindLogicalNode(_w, "Para3") as Paragraph;
            TextBlock tb1 = LogicalTreeHelper.FindLogicalNode(_w, "TextBlock1") as TextBlock;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(p3);            
            
            GeneralTransform t = tb1.TransformToAncestor(_dpage.Visual);
            Rect trRect = t.TransformBounds(tb1.ContentStart.GetCharacterRect(LogicalDirection.Forward));

            // Check textBlock Top Margin
            double expectedValue = tb1.Margin.Top;
            if (Math.Abs(trRect.Top - r1[0].Top - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for TextBlock is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - trRect.Top));
                TestLog.Current.Result = TestResult.Fail;
            }

            // Check textBlock Left Margin
            expectedValue = tb1.Margin.Left;
            if (Math.Abs(trRect.Left - r1[0].Left - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for TextBlock is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - trRect.Top));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestBlockUIContainerMargins()
        {
            TestLog.Current.LogStatus("Testing Margins on BlockUIContainer");
            BlockUIContainer buc = LogicalTreeHelper.FindLogicalNode(_w, "BUC1") as BlockUIContainer;
            Image img = LogicalTreeHelper.FindLogicalNode(_w, "Image1") as Image;
            TextBlock tb1 = LogicalTreeHelper.FindLogicalNode(_w, "TextBlock2") as TextBlock;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(buc);

            MatrixTransform t = img.TransformToAncestor(_dpage.Visual) as MatrixTransform;
            Rect trRect = t.TransformBounds(new Rect(new Point(0, 0), img.RenderSize));
            
            // Check textBlock Top Margin
            double expectedValue = img.Margin.Top;
            if (Math.Abs(trRect.Top - r1[0].Top - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for TextBlock is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - trRect.Top));
                TestLog.Current.Result = TestResult.Fail;
            }

            //// Check textBlock Left Margin
            expectedValue = img.Margin.Left;
            if (Math.Abs(trRect.Left - r1[0].Left - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for TextBlock is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - trRect.Top));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestFlowDocumentScrollViewerMargins()
        {
            TestLog.Current.LogStatus("Testing Margin on FlowDocumentScrollViewer");
            Paragraph p4 = LogicalTreeHelper.FindLogicalNode(_w, "Para4") as Paragraph;
            Paragraph pInside = LogicalTreeHelper.FindLogicalNode(_w, "ParaInside") as Paragraph;
            FlowDocumentScrollViewer tf1 = LogicalTreeHelper.FindLogicalNode(_w, "TextFlow1") as FlowDocumentScrollViewer;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(p4);
            
            GeneralTransform t = tf1.TransformToAncestor(_dpage.Visual);            
            Rect trRect = t.TransformBounds(tf1.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward));
            Rect trRectBottom = t.TransformBounds(tf1.Document.ContentEnd.GetCharacterRect(LogicalDirection.Forward));

            FlowDocument fInside = tf1.Document as FlowDocument;
            DocumentPaginator dPaginatorInside = (fInside as IDocumentPaginatorSource).DocumentPaginator;
            DocumentPage dPageInside = dPaginatorInside.GetPage(0);

            ReadOnlyCollection<Rect> r2 = (dPageInside as IContentHost).GetRectangles(pInside);

            // Check FlowDocumentScrollViewer Top Margin 
            double expectedValue = tf1.Margin.Top + p4.Padding.Top + 1; // 1 is for BorderThickness
            if (Math.Abs(trRect.Top - r1[0].Top - r2[0].Top - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for FlowDocumentScrollViewer is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - trRect.Top));
                TestLog.Current.Result = TestResult.Fail;
            }

            // Check FlowDocumentScrollViewer Left Margin
            expectedValue = tf1.Margin.Left + p4.Padding.Left + 1; // 1 is for BorderThickness
            if (Math.Abs(trRect.Left - r1[0].Left - r2[0].Left - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Left Margin for FlowDocumentScrollViewer is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(trRect.Left - r1[0].Left));
                TestLog.Current.Result = TestResult.Fail;
            }

            // Check FlowDocumentScrollViewer Bottom Margin
            expectedValue = tf1.Margin.Bottom + p4.Padding.Bottom + 1;
            if (Math.Abs(r1[0].Bottom - trRectBottom.Bottom - r2[0].Top - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Bottom Margin for FlowDocumentScrollViewer is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Bottom - trRect.Bottom));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestFigureMargins()
        {
            TestLog.Current.LogStatus("Testing Margin on Figure");
            Paragraph p5 = LogicalTreeHelper.FindLogicalNode(_w, "Para5") as Paragraph;
            Run rn1 = LogicalTreeHelper.FindLogicalNode(_w, "Run1") as Run;
            Figure f1 = LogicalTreeHelper.FindLogicalNode(_w, "Fig1") as Figure;
            Figure f2 = LogicalTreeHelper.FindLogicalNode(_w, "Fig2") as Figure;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(p5);
            ReadOnlyCollection<Rect> r2 = (_dpage as IContentHost).GetRectangles(f1);
            ReadOnlyCollection<Rect> r3 = (_dpage as IContentHost).GetRectangles(rn1);
            ReadOnlyCollection<Rect> r4 = (_dpage as IContentHost).GetRectangles(f2);
            
            // Calculate Expected Value for Figure Top
            double expectedValue = f1.Margin.Top + f1.Padding.Top; 
            if (Math.Abs(r2[0].Top - r1[0].Top - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for Figure is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - r2[0].Top));
                TestLog.Current.Result = TestResult.Fail;
            }

            // left margin
            expectedValue = f1.Margin.Left + f1.Padding.Left; 
            if (Math.Abs(r2[0].Left - r1[0].Left - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Left Margin for Figure is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r2[0].Left - r1[0].Left));
                TestLog.Current.Result = TestResult.Fail;
            }

            // bottom margin
            expectedValue = f1.Margin.Bottom + f1.Padding.Bottom;
            if (Math.Abs(r3[0].Top - r2[0].Bottom - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Bottom Margin for Figure is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r3[0].Top - r2[0].Bottom));
                TestLog.Current.Result = TestResult.Fail;
            }

            // right margin
            expectedValue = f2.Margin.Right + f2.Padding.Right;
            if (Math.Abs(r1[0].Right - r4[0].Right - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Right Margin for Figure is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Right - r4[0].Right));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestFloaterMargins()
        {
            TestLog.Current.LogStatus("Testing Margin on Floater");
            Paragraph p5 = LogicalTreeHelper.FindLogicalNode(_w, "Para6") as Paragraph;
            Run rn1 = LogicalTreeHelper.FindLogicalNode(_w, "Run2") as Run;
            Floater f1 = LogicalTreeHelper.FindLogicalNode(_w, "Float1") as Floater;
            
            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(p5);
            ReadOnlyCollection<Rect> r2 = (_dpage as IContentHost).GetRectangles(f1);
            ReadOnlyCollection<Rect> r3 = (_dpage as IContentHost).GetRectangles(rn1);
            
            // Calculate Expected Value for Floater Top
            double expectedValue = f1.Margin.Top + f1.Padding.Top;
            if (Math.Abs(r2[0].Top - r1[0].Top - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for Floater is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - r2[0].Top));
                TestLog.Current.Result = TestResult.Fail;
            }

            // left margin
            expectedValue = f1.Margin.Left + f1.Padding.Left;
            if (Math.Abs(r2[0].Left - r1[0].Left - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Left Margin for Floater is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r2[0].Left - r1[0].Left));
                TestLog.Current.Result = TestResult.Fail;
            }

            // right margin
            expectedValue = f1.Margin.Right + f1.Padding.Right + 2; // border
            if (Math.Abs(r3[0].Left - r2[0].Right - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Right Margin for Figure is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Right - r3[0].Left));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestSectionMargins()
        {
            TestLog.Current.LogStatus("Testing Margin on Sections");
            Section sec1 = LogicalTreeHelper.FindLogicalNode(_w, "Sec1") as Section;
            Paragraph p3 = LogicalTreeHelper.FindLogicalNode(_w, "Para6") as Paragraph;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(sec1);
            ReadOnlyCollection<Rect> r3 = (_dpage as IContentHost).GetRectangles(p3);

            // Test Left Margin
            double expectedValue = sec1.Margin.Left;
            if (Math.Abs(r1[0].Left - _dpage.ContentBox.Left -  expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Left Margin for Section is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Left - _dpage.ContentBox.Left));
                TestLog.Current.Result = TestResult.Fail;
            }            

            // Test Top Margin
            expectedValue = sec1.Margin.Top;
            if (Math.Abs(r1[0].Top - r3[0].Bottom - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for Section is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - r3[0].Bottom));
                TestLog.Current.Result = TestResult.Fail;
            }      

            // Test Right Margin
            expectedValue = sec1.Margin.Right;
            if (Math.Abs(_dpage.ContentBox.Right - r1[0].Right  - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Right Margin for Section is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(_dpage.ContentBox.Right - r1[0].Right));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult TestTableMargins()
        {
            TestLog.Current.LogStatus("Testing Margin on Table");
            Table tbl1 = LogicalTreeHelper.FindLogicalNode(_w, "Table1") as Table;
            Section p3 = LogicalTreeHelper.FindLogicalNode(_w, "Sec1") as Section;

            ReadOnlyCollection<Rect> r1 = (_dpage as IContentHost).GetRectangles(tbl1);
            ReadOnlyCollection<Rect> r3 = (_dpage as IContentHost).GetRectangles(p3);

            // Test Left Margin
            double expectedValue = tbl1.Margin.Left;
            if (Math.Abs(r1[0].Left - _dpage.ContentBox.Left - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Left Margin for Table is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Left - _dpage.ContentBox.Left));
                TestLog.Current.Result = TestResult.Fail;
            }

            // Test Top Margin
            expectedValue = tbl1.Margin.Top;
            if (Math.Abs(r1[0].Top - r3[0].Bottom - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for Table is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - r3[0].Bottom));
                TestLog.Current.Result = TestResult.Fail;
            }

            // Test Right Margin
            expectedValue = tbl1.Margin.Right;
            if (Math.Abs(_dpage.ContentBox.Right - r1[0].Right - expectedValue) > _errMargin)
            {
                TestLog.Current.LogEvidence("Top Margin for Table is incorrect. Expected = " + expectedValue + ", Observed value = " + Math.Abs(r1[0].Top - r3[0].Bottom));
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }
        
        private double CalcExpectedValueTopBottom(Block p1, Block p2)
        {
            double p1Bottom = p1.Margin.Bottom;
            double p2Top = p2.Margin.Top;

            if (Math.Abs(p2Top - p1Bottom) > 0)
            {
                if (p2Top > p1Bottom)
                {
                    return p2Top;
                }
                else
                {
                    return p1Bottom;
                }
            }
            else
            {
                return p1Bottom;
            }            
        }
    }
}
