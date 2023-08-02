// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
     Regression test for Part2 Regression_Bug27
*/
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>TextBlock</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>.net 4.5 regression test.</description>
    /// </summary>
    [Test(0, "TextBlock", "TextBlockTrimmingAndWrappingRegression", Versions = "4.5.1+")]
    class TextBlockTrimmingAndWrappingRegression : AvalonTest
    {
        private TextBlock _tb;
        private string _testName;
        private Window _window;
        private double _tbHeight = System.Double.MinValue;

        [Variation("Part2Regression_Bug27")]
        public TextBlockTrimmingAndWrappingRegression(string testName)
            : base()
        {
            this._testName = testName;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(MeasureTextBlock);
        }

        #region Test Steps
        /// <summary>Initialize: setup tests</summary>
        /// <returns>TestResult</returns>
        TestResult Initialize()
        {
            _window = new Window();

            if (_testName == "Part2Regression_Bug27")
            {
                _window.Width = 525;
                _window.Height = 500;
            }
            else
            {
                TestLog.Current.LogEvidence("Did not find a test to run.");
                TestLog.Current.Result = TestResult.Fail;
            }

            _window.Content = Content();
            _window.Show();
            _window.Focus();

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        /// <summary>MeasureTextBlock: Verify that only 1 line of TextBlock is visible when
        /// TextWrapping.NoWrap and TextTrimming.CharacterEllipsis are set on a TextBlock
        /// containing a long string of text.</summary>
        /// <returns>TestResult</returns>
        private TestResult MeasureTextBlock()
        {
            WaitFor(2000);
            _tbHeight = _tb.ActualHeight;
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            if (_tbHeight < 0 || _tbHeight == System.Double.MinValue)
            {
                TestLog.Current.LogEvidence("ERROR - TextBlock has negative height: TextBlock.ActualHeight = " + _tbHeight);
                return TestResult.Fail;
            }
            else if (_tbHeight <= 8 || _tbHeight >= 15)
            {
                TestLog.Current.LogEvidence("ERROR - TextBlock height is not 1 line tall: TextBlock.ActualHeight = " + _tbHeight);
                return TestResult.Fail;
            }
            else
            {
                // WIN 8, 1024*768: ActualHeight = 10.64
                // WIN7 SP1, 1152*864 = ActualHeight = 10.64
                TestLog.Current.LogEvidence("TextBlock.ActualHeight = " + _tbHeight);
                return TestResult.Pass;
            }
        }
        #endregion

        private FrameworkElement Content()
        {
            if (_testName == "Part2Regression_Bug27")
            {
                StackPanel sp = new StackPanel();
                _tb = new TextBlock();

                string str = "q ";
                for (int i = 0; i < 20000; i++)
                {
                    str += "qwetre ";
                }

                _tb.Text = str;
                _tb.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                _tb.FontSize = 8;
                _tb.FontWeight = FontWeights.Bold;
                _tb.TextWrapping = TextWrapping.NoWrap;
                _tb.TextTrimming = TextTrimming.CharacterEllipsis;

                ((IAddChild)sp).AddChild(_tb);
                return sp;
            }
            else
            {
                TestLog.Current.LogEvidence("Did not find a test to run.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return null;
        }
    }
}
