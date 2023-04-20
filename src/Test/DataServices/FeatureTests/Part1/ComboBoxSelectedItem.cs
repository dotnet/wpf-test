// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;
using System.Threading; 
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where ComboBox: selected item is not showed
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ComboBoxSelectedItem")]
    public class ComboBoxSelectedItem : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ComboBox _myComboBox;
        
        #endregion

        #region Constructors

        public ComboBoxSelectedItem()
            : base(@"ComboBoxSelectedItem.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myComboBox = (ComboBox)RootElement.FindName("myComboBox");

            if (_myStackPanel == null || _myComboBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            Util.WaitForItemsControlPopulation(_myComboBox, 30);            
            
            // Select second item in ComboBox.         
            _myComboBox.SelectedIndex = 1;
            WaitForPriority(DispatcherPriority.SystemIdle);

            // Grab the visual text.
            ContentPresenter contentPresenter = Util.GetSelectionBox(_myComboBox);
            TextBlock textBlock = (TextBlock)VisualTreeHelper.GetChild(contentPresenter, 0);

            // Verify 
            if (textBlock.Text != "Dog")
            {
                LogComment("ComboBox was not updated correctly.");
                LogComment("Actual Text: " + textBlock.Text);
                return TestResult.Fail;
            }

            if (_myComboBox.SelectedIndex != 1)
            {
                LogComment("ComboBox was not updated correctly. Incorrect SelectedIndex.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}
