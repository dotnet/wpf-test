// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where OneWayToSource binding updates Target Property to 'UnsetValue' instead of property default when no Fallback value is specified
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "OneWayToSourcePriorityBinding")]
    public class OneWayToSourcePriorityBinding : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _myTextBox;
        private Label _myLabel;
        
        #endregion

        #region Constructors

        public OneWayToSourcePriorityBinding()
            : base(@"OneWayToSourcePriorityBinding.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myTextBox = (TextBox)RootElement.FindName("myTextBox");
            _myLabel = (Label)RootElement.FindName("myLabel");

            if (_myStackPanel == null || _myTextBox == null || _myLabel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         

            // Verify 
            if (_myTextBox.Text != _myLabel.FontSize.ToString())
            {
                LogComment("TextBox text was not updated correctly.");
                LogComment("Expected: " + _myLabel.FontSize.ToString());
                LogComment("Actual: " + _myTextBox.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}
