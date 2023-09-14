// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
     Regression test for Regression_Bug28 and Regression_Bug29    
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
    /// <priority>2</priority>
    /// <description>Orcas Regression test.</description>
    /// </summary>
    [Test(3, "TextBlock", "TextBlockHitTestingAndFocusRegression")]
    class TextBlockHitTestingAndFocusRegression : AvalonTest
    {
        private FrameworkElement _elementToClick;
        private string _testName;
        private Window _window;

        [Variation("Regression_Bug28")]
        [Variation("Regression_Bug29")]
        public TextBlockHitTestingAndFocusRegression(string testName)
            : base()
        {
            this._testName = testName;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ClickElement);
        }

        #region Test Steps
        /// <summary>Initialize: setup tests</summary>
        /// <returns>TestResult</returns>
        TestResult Initialize()
        {
            _window = new Window();

            if (_testName == "Regression_Bug29")
            {
                _window.Width = 894;
                _window.Height = 690;
            }
            else if (_testName == "Regression_Bug28")
            {
                _window.Width = 300;
                _window.Height = 300;
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

        /// <summary>ClickButton: Click a button and see if we crash</summary>
        /// <returns>TestResult</returns>
        private TestResult ClickElement()
        {
            WaitFor(2000);
            Input.Input.MoveToAndClick(_elementToClick);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }
        #endregion

        private FrameworkElement Content()
        {
            if (_testName == "Regression_Bug29")
            {
                StackPanel sp = new StackPanel();
                _elementToClick = new Button();
                TextBlock tb = new TextBlock();

                tb.Text = "520";
                tb.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                tb.FontSize = 8;
                tb.FontWeight = FontWeights.Bold;
                tb.Padding = new Thickness(10);
                tb.TextWrapping = TextWrapping.Wrap;

                ((IAddChild)_elementToClick).AddChild(tb);
                ((IAddChild)sp).AddChild(_elementToClick);
                return sp;
            }
            else if (_testName == "Regression_Bug28")
            {
                _elementToClick = new TestControl();                
                Canvas.SetTop(_elementToClick, 100);
                Canvas.SetLeft(_elementToClick, 30);

                Style controlStyle = new Style(typeof(TestControl));
                ControlTemplate template = new ControlTemplate(typeof(TestControl));
                FrameworkElementFactory tbStyle = new FrameworkElementFactory(typeof(TextBlock), "styleTextBlock");
                tbStyle.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                tbStyle.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Arial"));
                tbStyle.SetValue(TextBlock.FontSizeProperty, 16.0);
                tbStyle.SetValue(TextBlock.TextProperty, "        ..               .    .                  ...    ;     ;;    /");
                template.VisualTree = tbStyle;
                controlStyle.Setters.Add(new Setter(Control.TemplateProperty, template));
                _elementToClick.Style = controlStyle;

                Canvas canvas = new Canvas();
                ((IAddChild)canvas).AddChild(_elementToClick);

                return canvas;
            }
            else
            {
                TestLog.Current.LogEvidence("Did not find a test to run.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return null;
        }
    }

    public partial class TestControl : Control
    {
        /// <summary>
        /// Canvas always measures us without constraint and then calls arrange with our DesiredSize 
        /// which we get from our template which is just a textblock
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Width = 250;
            Size size = base.MeasureOverride(constraint);
            return size;
        }
    }
}
