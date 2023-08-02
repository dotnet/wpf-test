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
    [Test(2, "PropertyTests", "ContextMenu", MethodName = "Run")]
    public class ContextMenuTest : AvalonTest
    {
        private Paragraph _para = new Paragraph();
        private ContextMenu _cm = new ContextMenu();
        private MenuItem _mi = new MenuItem();
        private DateTime _start;
        private Window _w;
        private TestLog _cmOpen;
        private TestLog _cmClose;
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
        public ContextMenuTest(string testName)
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
            _cmOpen = new TestLog("ContextMenu Opening Test");

            _w = new Window();
            _w.Top = 0;
            _w.Left = 0;
            _w.Topmost = true;
            _w.Show();

            _para.Inlines.Add(new Run("basic ContextMenu test"));
            _mi.Header = "One Menu Item";
            _cm.Items.Add(_mi);

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
            tf.Document.Blocks.Add(_para);
            tf.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, null, new Point(-1, -1));

            return TestResult.Pass;
        }

        private TestResult TextBlockTest()
        {
            TextBlock tf = new TextBlock();
            tf.Text = ((Run)(_para.Inlines.FirstInline)).Text;
            tf.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, null, new Point(-1, -1));

            return TestResult.Pass;
        }

        private TestResult FlowDocumentTest()
        {
            FlowDocumentPageViewer root = new FlowDocumentPageViewer();
            FlowDocument tf = new FlowDocument();
            tf.ContextMenu = _cm;
            tf.Blocks.Add(_para);
            (root as IAddChild).AddChild(tf);
            SetupEvents(_w, root, null, tf, new Point(-1, -1));

            return TestResult.Pass;
        }

        private TestResult TableTest()
        {
            FlowDocumentScrollViewer root = new FlowDocumentScrollViewer();
            root.Document = new FlowDocument();
            Table tf = new Table();
            TableRowGroup tblrg = new TableRowGroup();
            TableRow tblrow = new TableRow();
            TableCell tc = new TableCell(_para);
            tblrow.Cells.Add(tc);
            tblrg.Rows.Add(tblrow);
            tf.RowGroups.Add(tblrg);
            tf.ContextMenu = _cm;
            root.Document.Blocks.Add(tf);
            SetupEvents(_w, root, null, tf, new Point(20, 20));

            return TestResult.Pass;
        }

        private TestResult ParagraphTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            tf.Document.Blocks.Add(_para);
            _para.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, _para, new Point(40, 20));

            return TestResult.Pass;
        }

        private TestResult ListTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            List listmain = new List();
            ListItem listItem = new ListItem();
            listItem.Blocks.Add(_para);
            listmain.ListItems.Add(listItem);
            listmain.ContextMenu = _cm;
            listmain.BorderBrush = Brushes.Red;
            listmain.BorderThickness = new Thickness(2);
            tf.Document.Blocks.Add(listmain);
            SetupEvents(_w, tf, tf, listmain, new Point(60, 40));

            return TestResult.Pass;
        }

        private TestResult ListItemTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            List listmain = new List();
            ListItem listItem = new ListItem();
            listItem.Blocks.Add(_para);
            listItem.BorderBrush = Brushes.Red;
            listItem.BorderThickness = new Thickness(2);
            listmain.ListItems.Add(listItem);
            listItem.ContextMenu = _cm;
            tf.Document.Blocks.Add(listmain);
            SetupEvents(_w, tf, tf, listItem, new Point(70, 40));

            return TestResult.Pass;
        }

        private TestResult HyperlinkTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Document.Blocks.Add(p);
            Hyperlink hl = new Hyperlink();
            hl.Inlines.Add(new Run(((Run)(_para.Inlines.FirstInline)).Text));
            p.Inlines.Add(hl);
            hl.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, hl, new Point(40, 30));

            return TestResult.Pass;
        }

        private TestResult FloaterTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Document.Blocks.Add(p);
            Floater fl = new Floater(new Paragraph(new Run("This is a Floater")));
            fl.Background = Brushes.Yellow;
            fl.HorizontalAlignment = HorizontalAlignment.Left;
            fl.Width = 200;
            fl.Padding = new Thickness(50);
            p.Inlines.Add(fl);
            fl.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, fl, new Point(40, 50));

            return TestResult.Pass;
        }

        private TestResult FigureTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Document.Blocks.Add(p);
            Figure fig = new Figure(new Paragraph(new Run("This is a Figure")));
            fig.Background = Brushes.Green;
            fig.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
            fig.Width = new FigureLength(200);
            fig.Height = new FigureLength(150);
            fig.Padding = new Thickness(50);
            p.Inlines.Add(fig);
            fig.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, fig, new Point(60, 80));

            return TestResult.Pass;
        }

        private TestResult SpanTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Document.Blocks.Add(p);
            Bold bld = new Bold(new Run("This is a Span element"));
            bld.FontSize = 45;
            bld.Background = Brushes.Tan;
            p.Inlines.Add(bld);
            bld.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, bld, new Point(60, 40));

            return TestResult.Pass;
        }

        private TestResult BlockUIContainerTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            Canvas can = new Canvas();
            can.Width = 50;
            can.Height = 150;
            can.Background = Brushes.Red;
            BlockUIContainer buc = new BlockUIContainer(can);
            buc.Background = Brushes.Blue;
            tf.Document.Blocks.Add(buc);
            buc.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, buc, new Point(60, 40));

            return TestResult.Pass;
        }

        private TestResult InlineUIContainerTest()
        {
            FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            Paragraph p = new Paragraph();
            tf.Document.Blocks.Add(p);
            Canvas can = new Canvas();
            can.Width = 300;
            can.Height = 50;
            can.Background = Brushes.Orange;
            InlineUIContainer iuc = new InlineUIContainer(can);
            iuc.Background = Brushes.Black;
            p.Inlines.Add(iuc);
            iuc.ContextMenu = _cm;
            SetupEvents(_w, tf, tf, iuc, new Point(60, 40));

            return TestResult.Pass;
        }

        private void SetupEvents(Window w, FrameworkElement root, FrameworkElement fe, FrameworkContentElement fce, Point placement)
        {
            w.Content = root;

            if (fce != null)
            {
                fce.ContextMenuOpening += new ContextMenuEventHandler(cm_ContextMenuOpening);
                fce.ContextMenuClosing += new ContextMenuEventHandler(cm_ContextMenuClosing);
                SummonContextMenu(placement);               
            }
            else
            {
                if (fe != null)
                {
                    fe.ContextMenuOpening += new ContextMenuEventHandler(cm_ContextMenuOpening);
                    fe.ContextMenuClosing += new ContextMenuEventHandler(cm_ContextMenuClosing);
                    SummonContextMenu(placement);                   
                }
            }

            WaitFor(waitTime);
            UserInput.MouseLeftClickCenter(w);
            WaitFor(waitTime);
        }

        private void cm_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            _cmClose.Result = TestResult.Pass;
            _cmClose.Close();

            TimerCalc(sender, e);
        }

        private void cm_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            _cmOpen.Result = TestResult.Pass;
            _cmOpen.Close();
            
            _cmClose = new TestLog("ContextMenu Closing Test");            
            _start = DateTime.Now;          
        }

        private void TimerCalc(object sender, EventArgs e)
        {
            TestLog cmTime = new TestLog("ContextMenu Time Test");
            TimeSpan ts = DateTime.Now - _start;
            cmTime.LogStatus("Time Elapsed between opening and closing ContextMenu: " + ts.Seconds + "." + ts.Milliseconds + "s");
            if (ts.Seconds > 10)
            {
                cmTime.Result = TestResult.Fail;
                cmTime.LogEvidence("Too much time elapsed between opening and closing of ContextMenu");
            }
            else
            {
                cmTime.Result = TestResult.Pass;               
            }
            cmTime.Close();
        }

        private void SummonContextMenu(Point placement)
        {
            if (placement.X == -1)
            {
                UserInput.MouseButton(_w, 100, 100, "RightDown");
                UserInput.MouseButton(_w, 100, 100, "RightUp");
            }
            else
            {
                UserInput.MouseButton(_w, (int)placement.X, (int)placement.Y, "RightDown");
                UserInput.MouseButton(_w, (int)placement.X, (int)placement.Y, "RightUp");
            }
        }
    }
}
