// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
     Regression test for Regression_Bug31
     when hosted the down mouse arrow is pressed on a RightToLeft
     FlowDocumentScrollViewer an exception is thrown
     Expected: No exception is thrown
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing regression test.   
    /// </summary>
    [Test(3, "Bottomless", "Regression_Bug31")]
    public class TestRegression_Bug31 : AvalonTest
    {
        private Window _window;
        TextBox _textBox;

        public TestRegression_Bug31()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);           
        }

        /// <summary>
        /// Initialize: setup tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            Status("Initialize ....");

            _window = new Window();
            Grid grid = new Grid();
            grid.FlowDirection = FlowDirection.RightToLeft;
            Canvas canvas = new Canvas();
            _textBox = SetupTextBox();
            canvas.Children.Add(_textBox);
            grid.Children.Add(canvas);
            _window.Content = grid;
            _window.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            try
            {
                _textBox.Focus();

                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Down, true);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Down, false);
                return TestResult.Pass;
            }
            catch (Exception e)
            {
                Status("The Testcase threw an exception indication Regression_Bug2 may have regressed" + e.Message);
                return TestResult.Fail;
            }
        }

        private TextBox SetupTextBox()
        {
            TextBox textBox = new TextBox();
            Canvas.SetLeft(textBox, 20);
            Canvas.SetTop(textBox, 20);
            textBox.Width = 400;
            textBox.Height = 50;
            return textBox;
        }
    }
}
