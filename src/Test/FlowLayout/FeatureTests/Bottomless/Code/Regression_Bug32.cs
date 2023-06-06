// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
     Regression test for Regression_Bug32
     Textbox doesn't render TextFlow.background on text ranges when set in code.  It does work if set in xaml though.  It looks like textrange.apply isn't applying TextFlow.Background
     Expected: TextBoxes background color changes
*/
using System.Windows.Threading;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing regression test.   
    /// </summary>
    [Test(3, "Bottomless", "Regression_Bug32")]
    class TestRegression_Bug32 : AvalonTest
    {               
        private StackPanel _stackPanel;
        private Window _window;
        private RichTextBox _richTextBox;
        private Bitmap _beforeImage = null;
        private Bitmap _afterImage = null;     
        
        public TestRegression_Bug32()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(CaptureWindow);
            RunSteps += new TestStep(ChangeBackground);
            RunSteps += new TestStep(CaptureWindowAndCompare);
        }       
        
        /// <summary>
        /// Initialize: setup tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {                   
            Status("Initialize ....");

            _window = new Window();
            _window.Title = "TextTest!";
            _window.Width = 800;
            _window.Height = 600;

            _stackPanel = new StackPanel();
            _richTextBox = new RichTextBox();
            _richTextBox.Document.Blocks.Add(new Paragraph(new Run("This is some text in a rich text box")));
            _richTextBox.AllowDrop = true;
            _richTextBox.Height = 200;
            _richTextBox.Width = 200;
            _stackPanel.Children.Add(_richTextBox);
            _window.Content = _stackPanel;
            _window.Show();            
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            _window.Focus();
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// CaptureWindow: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult CaptureWindow()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            WaitFor(500);
            _beforeImage = ImageUtility.CaptureElement(_richTextBox);
            return TestResult.Pass;            
        }
       
        /// <summary>
        /// ChangeBackground: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult ChangeBackground()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);            
            TextRange tr = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            tr.ApplyPropertyValue(Paragraph.BackgroundProperty, System.Windows.Media.Brushes.Red);
            WaitFor(500);
            return TestResult.Pass;            
        }
       
        /// <summary>
        /// CaptureWindowAndCompare: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult CaptureWindowAndCompare()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            _afterImage = ImageUtility.CaptureElement(_richTextBox);

            ImageAdapter beforeAdapter = new ImageAdapter(_beforeImage);
            ImageAdapter afterAdapter = new ImageAdapter(_afterImage);

            ImageComparator comparator = new ImageComparator();
            bool matches = comparator.Compare(beforeAdapter, afterAdapter, false);            

            if (!matches)
            {
                Status("Text box visualy reacted to background property change");
                return TestResult.Pass;
            }
            else
            {
                _beforeImage.Save("BeforeBackgroundChange.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                GlobalLog.LogFile("BeforeBackgroundChange.tif");
                _afterImage.Save("AfterBackgroundChange.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                GlobalLog.LogFile("AfterBackgroundChange.tif");
                Status("Text box did not visualy react to background property change");
                Status("TextRange.Apply(TextElement.BackgroundProperty, SolidColorBrush) had no effect on the textbox");
                return TestResult.Fail;
            }                                 
        }       
    }
}
