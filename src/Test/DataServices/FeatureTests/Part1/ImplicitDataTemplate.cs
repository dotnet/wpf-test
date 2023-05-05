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
    ///  Regression coverage for bug where Implicit DataTemplate not re-evaluated after changing XML content
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ImplicitDataTemplate")]
    public class ImplicitDataTemplate : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListBox _myListBox;
        private TextBlock _artistTextBlock;
        private TextBlock _titleTextBlock;
        private ContentControl _myContentControl;

        #endregion

        #region Constructors

        public ImplicitDataTemplate()
            : base(@"ImplicitDataTemplate.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListBox = (ListBox)RootElement.FindName("myListBox");
            _myContentControl = (ContentControl)RootElement.FindName("myContentControl");

            if (_myStackPanel == null || _myListBox == null || _myContentControl == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            Util.WaitForItemsControlPopulation(_myListBox, 30);
            WaitForPriority(DispatcherPriority.Render);  
       
            // Select first item.
            _myListBox.SelectedIndex = 0;
            WaitForPriority(DispatcherPriority.ApplicationIdle);  

            // Now select second item.
            _myListBox.SelectedIndex = 1;
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            

            // Read value for Artist & Title
            _titleTextBlock = (TextBlock)Util.FindVisualByType(typeof(TextBlock), _myContentControl, false, 1);
            _artistTextBlock = (TextBlock)Util.FindVisualByType(typeof(TextBlock), _myContentControl, false, 3);

            // Verify that its reported correctly.            
            if (_titleTextBlock.Text != "Rubber Soul" || _artistTextBlock.Text != "Beatles")
            {
                LogComment("Second item was not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
        
    #region Helper Classes
    
    #endregion
}
