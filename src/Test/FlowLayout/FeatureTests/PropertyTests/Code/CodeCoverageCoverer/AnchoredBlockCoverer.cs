// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - AnchoredBlock", MethodName = "Run")]
    public class AnchoredBlockCoverer : AvalonTest
    {
        private FlowDocumentScrollViewer _tf;
        private Figure _fig;
        private Floater _flt;
        private Window _w1;

        public AnchoredBlockCoverer()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);            
        }

        private TestResult Initialize()
        {            
            _w1 = new Window();
            UISetup(_w1, out _tf, out _fig, out _flt);
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            GetSetProperties(_tf, _fig, _flt);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private void UISetup(Window w1, out FlowDocumentScrollViewer tf, out Figure fig, out Floater flt)
        {
            tf = new FlowDocumentScrollViewer();
            tf.Document = new FlowDocument();
            fig = new Figure(new Paragraph());
            flt = new Floater(new Paragraph());
            Paragraph p = new Paragraph();
            p.Inlines.Add(fig);
            p.Inlines.Add(flt);
            tf.Document.Blocks.Add(p);
            w1.Content = tf;
            w1.Show();
        }

        private void GetSetProperties(FlowDocumentScrollViewer tf, Figure fig, Floater flt)
        {            
            Brush brush = fig.BorderBrush;
            Thickness thik = fig.BorderThickness;
            FlowDirection fld = fig.FlowDirection;
            double lh = fig.LineHeight;
            thik = fig.Margin;
            thik = fig.Padding;
            TextAlignment ta = fig.TextAlignment;

            thik = new Thickness(10);
            fig.BorderBrush = brush;
            fig.BorderThickness = thik;
            fig.FlowDirection = fld;
            fig.LineHeight = lh;
            fig.Margin = thik;
            fig.Padding = thik;
            fig.TextAlignment = ta;

            fig.HorizontalOffset = 20;
            fig.VerticalOffset = 20;
            fig.WrapDirection = WrapDirection.Left;
            fig.CanDelayPlacement = false;
            fig.Height = new FigureLength();
            fig.Width = new FigureLength();

            flt.HorizontalAlignment = HorizontalAlignment.Center;
            flt.Width = 200;

            Figure fig1 = new Figure(null);
            Floater flt1 = new Floater(null);
        }
    }

}
