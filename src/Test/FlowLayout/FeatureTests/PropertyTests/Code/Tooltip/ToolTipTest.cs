// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "ToolTip", MethodName = "Run")]
    public class ToolTipTest : AvalonTest
    {
        private Window _w;
        private Paragraph _para = new Paragraph();
        private ToolTip _ttp = new ToolTip();
        private DateTime _start;
        private TestLog _tipOpen;
        private TestLog _tipClose;
        private const int waitTime = 3000;

        [Variation("TextBlock")]
        [Variation("FlowDocument")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Table")]
        [Variation("Paragraph")]
        [Variation("List")]
        [Variation("ListItem")]
        [Variation("Hyperlink")]
        [Variation("Floater")]
        [Variation("Figure")]
        [Variation("Span")]
        [Variation("BlockUIContainer")]
        [Variation("InlineUIContainer")]
        public ToolTipTest(string testName)
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);

            switch (testName)
            {
                case "TextBlock":
                    RunSteps += new TestStep(TextBlockTest);
                    break;

                case "FlowDocumentScrollViewer":
                    RunSteps += new TestStep(FlowDocumentScrollViewerTest);
                    break;

                case "FlowDocument":
                    RunSteps += new TestStep(FlowDocumentTest);
                    break;

                case "Table":
                    RunSteps += new TestStep(TableTest);
                    break;

                case "Paragraph":
                    RunSteps += new TestStep(ParagraphTest);
                    break;

                case "List":
                    RunSteps += new TestStep(ListTest);
                    break;

                case "ListItem":
                    RunSteps += new TestStep(ListItemTest);
                    break;

                case "Hyperlink":
                    RunSteps += new TestStep(HyperlinkTest);
                    break;

                case "Floater":
                    RunSteps += new TestStep(FloaterTest);
                    break;

                case "Figure":
                    RunSteps += new TestStep(FigureTest);
                    break;

                case "Span":
                    RunSteps += new TestStep(SpanTest);
                    break;

                case "BlockUIContainer":
                    RunSteps += new TestStep(BlockUIContainerTest);
                    break;

                case "InlineUIContainer":
                    RunSteps += new TestStep(InlineUIContainerTest);
                    break;
            }
        }

        private TestResult Initialize()
        {
            _tipOpen = new TestLog("ToolTip Opening Test");

            _w = new Window();
            _w.Top = 0;
            _w.Left = 0;
            _w.Topmost = true;
            _w.Show();

            _para.Inlines.Add(new Run("basic tooltip test"));
            _ttp.Content = "This is the ToolTip Test";

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private TestResult FlowDocumentScrollViewerTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            tf.ToolTip = _ttp;
            tf.Document.Blocks.Add(_para);
            SetupEvents(_w, tf, tf, null, new Point(-1, -1));

            return TestResult.Pass;
        }

        private TestResult TextBlockTest()
        {
            TextBlock tf = new TextBlock();
            tf.Text = ((Run)_para.Inlines.FirstInline).Text;
            tf.ToolTip = _ttp;
            SetupEvents(_w, tf, tf, null, new Point(-1, -1));

            return TestResult.Pass;
        }

        private TestResult FlowDocumentTest()
        {
            FlowDocumentPageViewer root = new FlowDocumentPageViewer();
            FlowDocument tf = new FlowDocument();
            tf.ToolTip = _ttp;
            tf.Blocks.Add(_para);
            (root as IAddChild).AddChild(tf);
            SetupEvents(_w, root, null, tf, new Point(-1, -1));

            return TestResult.Pass;
        }

        private TestResult TableTest()
        {
            _para = new Paragraph();
            _para.Inlines.Add(new Run("basic tooltip test"));
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument root = fdsv.Document = new FlowDocument();
            Table tf = new Table();
            TableRowGroup tblrg = new TableRowGroup();
            TableRow tblrow = new TableRow();
            TableCell tc = new TableCell(_para);
            tblrow.Cells.Add(tc);
            tblrg.Rows.Add(tblrow);
            tf.RowGroups.Add(tblrg);
            tf.ToolTip = _ttp;
            root.Blocks.Add(tf);
            SetupEvents(_w, fdsv, null, tf, new Point(40, 30));

            return TestResult.Pass;
        }

        private TestResult ParagraphTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            tf.Blocks.Add(_para);
            _para.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, _para, new Point(40, 30));

            return TestResult.Pass;
        }

        private TestResult ListTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            List listmain = new List();
            ListItem listItem = new ListItem();
            listItem.Blocks.Add(_para);
            listmain.ListItems.Add(listItem);
            listmain.ToolTip = _ttp;
            listmain.BorderBrush = Brushes.Red;
            listmain.BorderThickness = new Thickness(2);
            tf.Blocks.Add(listmain);
            SetupEvents(_w, fdsv, fdsv, listmain, new Point(40, 40));

            return TestResult.Pass;
        }

        private TestResult ListItemTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            List listmain = new List();
            ListItem listItem = new ListItem();
            listItem.Blocks.Add(_para);
            listItem.BorderBrush = Brushes.Red;
            listItem.BorderThickness = new Thickness(2);
            listmain.ListItems.Add(listItem);
            listItem.ToolTip = _ttp;
            tf.Blocks.Add(listmain);
            SetupEvents(_w, fdsv, fdsv, listItem, new Point(70, 40));

            return TestResult.Pass;
        }

        private TestResult HyperlinkTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Blocks.Add(p);
            Hyperlink hl = new Hyperlink(new Run("Some Hyperlink"));
            p.Inlines.Add(hl);
            hl.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, hl, new Point(40, 30));

            return TestResult.Pass;
        }

        private TestResult FloaterTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Blocks.Add(p);
            Floater fl = new Floater(new Paragraph(new Run("This is a Floater")));
            fl.Background = Brushes.Yellow;
            fl.HorizontalAlignment = HorizontalAlignment.Left;
            fl.Width = 200;
            fl.Padding = new Thickness(50);
            p.Inlines.Add(fl);
            fl.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, fl, new Point(40, 50));

            return TestResult.Pass;
        }

        private TestResult FigureTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Blocks.Add(p);
            Figure fig = new Figure(new Paragraph(new Run("This is a Figure")));
            fig.Background = Brushes.Green;
            fig.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
            fig.Width = new FigureLength(200);
            fig.Height = new FigureLength(150);
            fig.Padding = new Thickness(50);
            p.Inlines.Add(fig);
            fig.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, fig, new Point(60, 80));

            return TestResult.Pass;
        }

        private TestResult SpanTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Blocks.Add(p);
            Bold bld = new Bold(new Run("This is a Span element"));
            bld.FontSize = 45;
            bld.Background = Brushes.Tan;
            p.Inlines.Add(bld);
            bld.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, bld, new Point(60, 40));

            return TestResult.Pass;
        }

        private TestResult BlockUIContainerTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            Canvas can = new Canvas();
            can.Width = 50;
            can.Height = 150;
            can.Background = Brushes.Red;
            BlockUIContainer buc = new BlockUIContainer(can);
            buc.Background = Brushes.Blue;
            tf.Blocks.Add(buc);
            buc.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, buc, new Point(60, 40));

            return TestResult.Pass;
        }

        private TestResult InlineUIContainerTest()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument tf = fdsv.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Blocks.Add(p);
            Canvas can = new Canvas();
            can.Width = 300;
            can.Height = 50;
            can.Background = Brushes.Orange;
            InlineUIContainer iuc = new InlineUIContainer(can);
            iuc.Background = Brushes.Black;
            p.Inlines.Add(iuc);
            iuc.ToolTip = _ttp;
            SetupEvents(_w, fdsv, fdsv, iuc, new Point(60, 40));

            return TestResult.Pass;
        }

        private void SetupEvents(Window w, FrameworkElement root, FrameworkElement fe, FrameworkContentElement fce, Point placement)
        {
            w.Content = root;

            if (fce != null)
            {
                fce.ToolTipOpening += new ToolTipEventHandler(tp_ToolTipOpening);
                fce.ToolTipClosing += new ToolTipEventHandler(tp_ToolTipClosing);
                SummonToolTip(placement);                
            }
            else
            {
                if (fe != null)
                {
                    fe.ToolTipOpening += new ToolTipEventHandler(tp_ToolTipOpening);
                    fe.ToolTipClosing += new ToolTipEventHandler(tp_ToolTipClosing);
                    SummonToolTip(placement);                  
                }
            }
            WaitFor(waitTime);
            WaitFor(waitTime);
        }

        private void tp_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            _tipClose.Result = TestResult.Pass;
            _tipClose.Close();
            TimerCalc(sender, e);           
        }

        private void tp_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _tipOpen.Result = TestResult.Pass;
            _tipOpen.Close();

            _tipClose = new TestLog("ToolTip Closing Test");
            _start = DateTime.Now;           
        }

        private void TimerCalc(object sender, EventArgs e)
        {
            TestLog tipTime = new TestLog("ToolTip Time Test");

            TimeSpan ts = DateTime.Now - _start;

            tipTime.LogStatus("Time Elapsed between opening and closing Tooltip: " + ts.Seconds + "." + ts.Milliseconds + "s");
            if (ts.Seconds > 10)
            {
                tipTime.Result = TestResult.Fail;
                tipTime.LogEvidence("Too much time elapsed between opening and closing of tooltip");
            }
            else
            {
                tipTime.Result = TestResult.Pass;               
            }
            tipTime.Close();
        }

        private void SummonToolTip(Point placement)
        {
            if (placement.X == -1)
            {
                UserInput.MouseButton(_w, 100, 100, "Move");
            }
            else
            {
                UserInput.MouseButton(_w, (int)placement.X, (int)placement.Y, "Move");
            }
        }
    }
}
