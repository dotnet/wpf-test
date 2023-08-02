// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Logging;
using System.Windows.Media;
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>TextBlock</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing Textblock and Text Decorations.
    /// </description>
    /// </summary>
    [Test(2, "TextBlock", "TextDecorationTest", Variables="VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    public class TextDecorationTest : AvalonTest
    {
        #region Test case members
        
        private Window _w;
        private TextBlock _tb2;
        private string _masterName;       
        
        #endregion

        #region Constructor
      
        [Variation("TextBlock_TextDecorations_Baseline")]
        [Variation("TextBlock_TextDecorations_Overline")]
        [Variation("TextBlock_TextDecorations_Underline")]
        [Variation("TextBlock_TextDecorations_Strikethrough")]
        [Variation("TextBlock_TextDecorations_ChangeForeground")]
        [Variation("TextBlock_TextDecorations_NonInheritance")]
        public TextDecorationTest(string masterName)
            : base()
        {
            this._masterName = masterName;            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VisualVerify);
        }

        #endregion

        #region Test Steps
       
        /// <summary>
        /// Initialize: Setup test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {                  
            _w = new Window();
            _w.SizeToContent = SizeToContent.WidthAndHeight;
            _w.Content = content();
            if (_w.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)_w.Content).Height = 564;
                ((FrameworkElement)_w.Content).Width = 784;
            }
            _w.Show();
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private UIElement content()
        {           
            StackPanel sp = new StackPanel();
            sp.Background = Brushes.LightBlue;
            TextBlock tb = new TextBlock();
            tb.FontFamily = new FontFamily("Tahoma");
            tb.FontSize = 12;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Text = "Indian Prime Minister Manmohan Singh is due to hold talks with US President George W Bush on the opening day of a landmark visit to the country. The two leaders are expected to focus on terrorism, trade, investment and collaboration on technoLogy";
            
            _tb2 = new TextBlock();
            _tb2.FontFamily = new FontFamily("Tahoma");
            _tb2.FontSize = 12;
            _tb2.Text = "The two leaders are expected to focus on terrorism, trade, investment and collaboration on technoLogy.";
            sp.Children.Add(tb);
            sp.Children.Add(_tb2);
            return sp;
        }

  /******************************************************************************
  * Function:          RunTests
  ******************************************************************************/
        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>

        TestResult RunTests()
        {                      
            // Basic Test - Run through all scenarios of TextDecorations and compare
            //TextDecorations.Baseline
            //TextDecorations.OverLine
            //TextDecorations.Strikethrough
            //TextDecorations.Underline
            _tb2.TextDecorations = TextDecorations.Baseline;
            CommonFunctionality.FlushDispatcher();
            _tb2.TextDecorations = TextDecorations.OverLine;
            CommonFunctionality.FlushDispatcher();
            _tb2.TextDecorations = TextDecorations.Baseline;                       
            if (_masterName == "TextBlock_TextDecorations_Baseline")
            {                
                return TestResult.Pass;                             
            }            

            _tb2.TextDecorations = TextDecorations.OverLine;                      
            if (_masterName == "TextBlock_TextDecorations_Overline")
            {               
                return TestResult.Pass;
            }            

            _tb2.TextDecorations = TextDecorations.Underline;                        
            if (_masterName == "TextBlock_TextDecorations_Underline")
            {               
                return TestResult.Pass;
            }            

            _tb2.TextDecorations = TextDecorations.Strikethrough;                    
            if (_masterName == "TextBlock_TextDecorations_Strikethrough")
            {               
                return TestResult.Pass;
            }           

            // color - changing color of element changes color of decoration
            _tb2.Foreground = Brushes.Green;                       
            if (_masterName == "TextBlock_TextDecorations_ChangeForeground")
            {               
                return TestResult.Pass;
            }           

            //Inheritances - if you add elements to the TextBlock the textdecorations are persisted
            new Run("Nassa Nassa Nassa Nassa Nassa Nassa", _tb2.ContentEnd);           
            if (_masterName == "TextBlock_TextDecorations_NonInheritance")
            {                
                return TestResult.Pass;
            }
            
            Log.LogEvidence("Did not find a test variation that we expected.");
            return TestResult.Fail;             
        }

        TestResult VisualVerify()
        {
            WaitFor(1000);
            VScanCommon vscan = new VScanCommon(_w, this, _masterName);
            bool result = vscan.CompareImage();

            if (result)
            {
                LogComment("TextBlock TextDecorations test Passed");
                return TestResult.Pass;
            }
            else
            {
                LogComment("TextBlock TextDecorations test Failed");
                return TestResult.Fail;
            }  
        }

        #endregion
    }
}
