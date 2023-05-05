// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where BindingGroup should handle changes to the DataContext
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingGroupDataContext")]
    public class BindingGroupDataContext : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TextBox _tbName;
        private BindingGroup _bindingGroup;
        
        #endregion

        #region Constructors

        public BindingGroupDataContext()
            : base(@"BindingGroupDataContext.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _tbName = (TextBox) RootElement.FindName("tbName");            

            if (_myStackPanel == null || _tbName == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            _bindingGroup = _myStackPanel.BindingGroup;

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Change the DataContext on a BindingGroup element.
            _tbName.DataContext = new Musician("Chewbacca", Title.Section, 20000);

            // Verify 
            if (_bindingGroup.BindingExpressions.Count != 2)
            {
                LogComment("BindingGroup did not handle change of DataContext.");
                return TestResult.Fail;
            }            

            return TestResult.Pass;
        }

        #endregion
        
    }   
}
