// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing Text add and delete.   
    /// </summary>
    [Test(0, "TextBlock", "TextBlockAddDelete")]
    public class TextBlockAddDelete : AvalonTest
    {
        #region Test case members
       
        private Window _testWin;
        private Border _border;
        private TextBlock _text;
        private TextBlock _text2;
        private TextBlock _text3;
        private Bold _bld1;
        private Italic _itl1;
        
        #endregion

        #region Constructor

        public TextBlockAddDelete()
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
            _testWin = new Window();
            _border = new Border();
            _border.Background = new SolidColorBrush(Colors.Beige);
            StackPanel sp = new StackPanel();
            sp.Height = _border.Height = 300;
            sp.Width = _border.Width = 600;
            
            _text = new TextBlock();
            _text2 = new TextBlock();
            _text3 = new TextBlock();
            _text2.Background = Brushes.LightYellow;
            _text3.Background = Brushes.LightGreen;

            //Properties
            _text.FontSize = _text2.FontSize = _text3.FontSize = 22;
            _text.FontFamily = new FontFamily("Tahoma");
            _text2.FontFamily = new FontFamily("Times New Roman");
            _text3.FontFamily = new FontFamily("Georgia");
            _text.TextWrapping = _text2.TextWrapping = _text3.TextWrapping = TextWrapping.WrapWithOverflow;

            Run r = new Run("This should be deleted and should not be overlapped");
            _bld1 = new Bold(new Run("Bold"));
            _itl1 = new Italic(new Run("Italic"));
            Button but1 = new Button();
            but1.Content = "Button";
            _text.Inlines.Add(r);           
            _text.Inlines.Remove(r);
            _text.Inlines.Add(new Run("This is Span #1."));
            _text.Inlines.Add(_bld1);
            _text.Inlines.Add(new Italic(new Run("This is Span #2.")));
            _text2.Inlines.Add("This is Span #3.");
            _text2.Inlines.Add(new Bold(new Run("This is Span #4.")));
            _text2.Inlines.Add(_itl1);
            _text3.Inlines.Add(new Run("This is Span #5."));
            _text3.Inlines.Add(_itl1);
            _text3.Inlines.Add(new Underline(new Run("This is Span #6.")));
            _text3.Inlines.Add("This is Span #7.");
            sp.Children.Add(_text);
            sp.Children.Add(_text2);
            sp.Children.Add(_text3);
            _border.Child = sp;

            _testWin.Content = _border;
            _testWin.Width = 600;
            _testWin.Height = 800;
            _testWin.Top = 0;
            _testWin.Left = 0;
            _testWin.Name = "PropertyDumpRoot";
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
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("Starting tests...");
            _text.Inlines.InsertAfter(_bld1, new Run("Run inserted After"));
            _text.Inlines.InsertBefore(_bld1, new Run("Run inserted Before"));
            _text3.Inlines.Remove(_itl1);
            _text2.Inlines.Clear();

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            testVerification();
            
            return Log.Result;
        }
        #endregion

        private void testVerification()
        {
            LogComment("starting verification...");

            try
            {
                PropertyDumpHelper helper = new PropertyDumpHelper(_border);

                bool result;
                Arguments testArgs = new Arguments(this);

                result = helper.CompareLogShow(testArgs);

                LogComment("test case result is " + result.ToString());
                if (result == true)
                {
                    LogComment("Test case passed!");
                    Log.Result = TestResult.Pass;
                }
                else
                {
                    LogComment("!! PropertyDump Comparision !!");
                    Log.Result = TestResult.Fail;
                }
            }
            catch (System.Xml.XmlException)
            {
                Log.Result = TestResult.Ignore;
            }
        }
    }
}
