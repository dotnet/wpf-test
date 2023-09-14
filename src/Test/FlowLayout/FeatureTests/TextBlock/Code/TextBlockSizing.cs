// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// TextBlock sizing
//
// Verifying widths
// of TextBlock Elements with the following properties:
// LineHeight, Width, MinWidth, and MaxWidth
//
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>TextBlock</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing Text sizing.
    /// </description>
    /// </summary>
    [Test(0, "TextBlock", "TextBlockAddDelete")]
    public class TextBlockSizing : AvalonTest
    {
        private Window _testWin;        
        private Canvas _canvas;
        private TextBlock _text;
        private TextBlock _obj1,_obj2,_obj3,_obj4,_obj5,_obj6;

        #region Constructor
     
        public TextBlockSizing()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: Setup test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            Status("Initialize ....");

            _testWin = new Window();
            Border border = new Border();

            border.Background = new SolidColorBrush(Colors.Beige);
            _canvas = new Canvas();
            _canvas.Height = border.Height = 300;
            _canvas.Width = border.Width = 500;

            // TextBlock tree starts here
            _text = new TextBlock();
            _text.TextWrapping = TextWrapping.WrapWithOverflow;
            _text.Text = "Line height is: ";

            Span sp1;
            InlineUIContainer iuc1;

            _text.Inlines.Add(sp1 = new Span());
            sp1.Inlines.Add(iuc1 = new InlineUIContainer());
            _obj1 = new TextBlock();
            iuc1.Child = _obj1;

            Run rn1;
            _text.Inlines.Add(rn1 = new Run(" & line stacking strategy = "));

            Span sp2;
            InlineUIContainer iuc2;
            _text.Inlines.Add(sp2 = new Span());
            sp2.Inlines.Add(iuc2 = new InlineUIContainer());
            _obj2 = new TextBlock();
            iuc2.Child = _obj2;

            Run rn2;
            _obj1.Inlines.Add(rn2 = new Run("10pt"));

            // obj1 is 50px
            _obj1.Width = 50;
            Run rn3;
            _obj2.Inlines.Add(rn3 = new Run("MaxHeight"));

            // obj2 is 140px
            _obj2.Width = 140;
            _text.FontSize = 10.0 * 96.0 / 72.0; // 10pt

            // TextBlock LineHeight is 10pt, which is ((10*96/72) pixels)
            _text.LineHeight = 10 * 96.0 / 72.0;
            _obj1.FontFamily = new FontFamily("Comic Sans Seriff");
            _obj1.FontSize = 12.0 * 96.0 / 72.0; // 16pt
            _obj1.Foreground = Brushes.Red;
            _obj1.FontWeight = FontWeights.Bold;
            _obj2.FontFamily = new FontFamily("Times New Roman");
            _obj2.Foreground = Brushes.Red;
            _obj2.FontWeight = FontWeights.Bold;

            _text.TextWrapping = TextWrapping.WrapWithOverflow;

            Run rn4;
            _text.Inlines.Add(rn4 = new Run(" Line height is: "));

            Span sp3;
            InlineUIContainer iuc3;
            _text.Inlines.Add(sp3 = new Span());
            sp3.Inlines.Add(iuc3 = new InlineUIContainer());
            _obj3 = new TextBlock();
            iuc3.Child = _obj3;

            Run rn5;
            _text.Inlines.Add(rn5 = new Run(" & Line stacking strategy = "));

            Span sp4;
            InlineUIContainer iuc4;
            _text.Inlines.Add(sp4 = new Span());
            sp4.Inlines.Add(iuc4 = new InlineUIContainer());
            _obj4 = new TextBlock();
            iuc4.Child = _obj4;

            Run rn6;
            _obj3.Inlines.Add(rn6 = new Run("10pt"));

            // obj3 is 50px
            _obj3.Width = 50;
            Run rn7;
            _obj4.Inlines.Add(rn7 = new Run("objLineHeight"));

            // obj4 is 150px
            _obj4.Width = 150;
            _text.FontSize = 12.0 * 96.0 / 72.0; // 12pt

            // TextBlock LineHeight is 10pt, which is ((10*96/72) pixels)
            _text.LineHeight = 10 * 96.0 / 72.0;
            _obj3.FontFamily = new FontFamily("Times New Roman");
            _obj3.FontSize = 16.0 * 96.0 / 72.0; // 16pt
            _obj3.Foreground = Brushes.Red;
            _obj3.FontWeight = FontWeights.Bold;
            _obj4.FontFamily = new FontFamily("Comic Sans Seriff");
            _obj4.Foreground = Brushes.Red;
            _obj4.FontWeight = FontWeights.Bold;
            _obj4.FontSize = 10.0 * 96.0 / 72.0; // 10pt
            _text.TextTrimming = TextTrimming.CharacterEllipsis;

            Span sp5;
            InlineUIContainer iuc5;
            _text.Inlines.Add(sp5 = new Span());
            sp5.Inlines.Add(iuc5 = new InlineUIContainer());
            _obj5 = new TextBlock();
            iuc5.Child = _obj5;

            Span sp6;
            InlineUIContainer iuc6;
            _text.Inlines.Add(sp6 = new Span());
            sp6.Inlines.Add(iuc6 = new InlineUIContainer());
            _obj6 = new TextBlock();
            iuc6.Child = _obj6;

            // obj5 is at least 200px
            _obj5.MinWidth = 200;

            Run rn8;
            _obj5.Inlines.Add(rn8 = new Run("MinWidth is set at 400px in this 500px Panel. So it should be at least halfway to the border"));

            _obj5.Foreground = Brushes.Navy;
            _obj5.FontSize = 9.0 * 96.0 / 72.0; //9pt

            // obj6 is at most 300px
            _obj6.MaxWidth = 300;
            _obj6.Foreground = Brushes.Red;
            _obj6.FontSize = (9.0 * 96.0 / 72.0); //9pt

            Run rn9;
            _obj6.Inlines.Add(rn9 = new Run("MaxWidth is set at 300px so this will be cut off before you can read the rest"));
            _canvas.Children.Add(_text);
            border.Child = _canvas;
            _testWin.Content = border;
            _testWin.Width = 600;
            _testWin.Height = 800;
            _testWin.Top = 0;
            _testWin.Left = 0;
            _testWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

       
        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            LogComment("STARTING TEST VERIFICATION...");
            LogComment("Comparing Widths of TextBlock Elements...");

            // Verify obj1 is 50px
            if (_obj1.RenderSize.Width != 50)
            {                
                LogComment("Obj1 Width is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }

            LogComment("Obj1 Width is good...");
            LogComment("Now comparing obj2 Width...");

            // Verify obj2 Width is 140px
            if (_obj2.RenderSize.Width != 140)
            {                
                LogComment("obj2 Width is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }

            LogComment("Obj2 Width is good...");
            LogComment("Now comparing obj3 Width...");

            // Verify obj3 Width is 50px
            if (_obj3.RenderSize.Width != 50)
            {                
                LogComment("obj3 Width is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }

            LogComment("Obj3 Width is good...");
            LogComment("Now comparing obj4 Width...");

            // Verify obj4 Width is 150px
            if (_obj4.RenderSize.Width != 150)
            {                
                LogComment("obj4 Width is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }

            LogComment("Obj4 Width is good...");
            LogComment("Now comparing obj5 Width...");

            // Verify obj5 Width is at least 200px
            if (_obj5.RenderSize.Width < 200)
            {                
                LogComment("obj5 Width is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }

            LogComment("Obj5 Width is good...");
            LogComment("Now comparing obj6 Width...");

            // Verify obj6 Width is at most 300px
            if (_obj6.DesiredSize.Width > 300)
            {                
                LogComment("obj6 Width is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }

            LogComment("Obj6 Width is good...");
            LogComment("Now comparing TextBlock LineHeight...");

            // Verify TextBlock LineHeight is 10pt, which is ((10*96/72) pixels)
            if (_text.LineHeight != 10 * 96.0 / 72.0)
            {                
                LogComment("text LineHeight is incorrect.");
                LogComment("TEST CASE FAILED.");

                return TestResult.Fail;
            }
            else // TextBlock Passes
            {                
                LogComment("Text sizes are correct!");
                LogComment("TEST CASE PASSED.");

                return TestResult.Pass;
            }
        }
        #endregion
    }
}


