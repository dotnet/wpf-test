// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
     Regression test for Regression_Bug1
*/
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
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
    [Test(0, "TextBlock", "TextBlockReflowRegression", Versions = "4.7.2+")]
    class TextBlockReflowRegression : AvalonTest
    {
        private Grid _grid;
        private string _testName;
        private Window _window;

        [Variation("Regression_Bug53")]
        public TextBlockReflowRegression(string testName)
            : base()
        {
            this._testName = testName;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestReflow);
        }

        #region Test Steps
        /// <summary>Initialize: setup tests</summary>
        /// <returns>TestResult</returns>
        TestResult Initialize()
        {
            _window = new Window();

            if (_testName == "Regression_Bug53")
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

        /// <summary>TestReflow: Move text pointers within each of the TextBlocks
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult TestReflow()
        {
            var textBlocks = FindVisualChildren<TextBlock>(_grid);
            foreach(var textBlock in textBlocks)
            {
                if (textBlock.Inlines.Count == 1)
                {
                    var inline = textBlock.Inlines.FirstInline;

                    // Get a TextPointer to the end of a line that is specified relative to the current text pointer
                    // Add 1 to get the beginning of the line following the one count is requesting
                    var nextLineStart = inline.ElementStart.GetLineStartPosition(1);

                    if (nextLineStart != null)
                    {
                        // Get a text pointer
                        // This causes the application to crash, and there is no way to catch the exception.
                        var nextPos = nextLineStart.GetNextInsertionPosition(LogicalDirection.Backward);
                    }
                }
            }

            // if we reach here without crashing, test passes
            return TestResult.Pass;
        }
        #endregion

        private FrameworkElement Content()
        {
            if (_testName == "Regression_Bug53")
            {
                _grid = new Grid();

                for (int i=0; i<4; ++i)
                {
                    _grid.ColumnDefinitions.Add(new ColumnDefinition{ Width = new GridLength(115) });
                }
                _grid.RowDefinitions.Add(new RowDefinition{ Height = GridLength.Auto });
                _grid.RowDefinitions.Add(new RowDefinition{ Height = new GridLength(1, GridUnitType.Star) });

                for (int i=0; i<4; ++i)
                {
                    TextBlock tb = new TextBlock{ Text = "ABCDE IAATA Corp." };
                    Grid.SetColumn(tb, i);
                    tb.FontFamily = new FontFamily("Arial");
                    tb.FontSize = (i<3) ? (13 + i) : (15 + i);
                    tb.TextWrapping = TextWrapping.Wrap;
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    _grid.Children.Add(tb);
                }

                return _grid;
            }
            else
            {
                TestLog.Current.LogEvidence("Did not find a test to run.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}

