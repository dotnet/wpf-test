// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading; 
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Binding ElementName resolves to wrong namescope when using hierarchical data binding
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingElementName", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class BindingElementName : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private TreeView _myTreeView;
        
        #endregion

        #region Constructors

        public BindingElementName()
            : base(@"BindingElementName.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myTreeView = (TreeView)RootElement.FindName("myTreeView");
            
            if (_myStackPanel == null || _myTreeView == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && ((_myTreeView.Items.Count == 0) || (_myTreeView.Items.IsEmpty)))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            // Grab the header and one of the items. Measure TexTbox size.
            TreeViewItem tvHeader = (TreeViewItem)_myTreeView.ItemContainerGenerator.ContainerFromItem(_myTreeView.Items[0]);
            tvHeader.IsExpanded = true;

            WaitForPriority(DispatcherPriority.SystemIdle);         

            TextBlock textBlockHeader = (TextBlock)Util.FindVisualByType(typeof(TextBlock), tvHeader, false);            
            TreeViewItem tvContent = (TreeViewItem)tvHeader.ItemContainerGenerator.ContainerFromItem(tvHeader.Items[0]);
            TextBlock textBlockContent = (TextBlock)Util.FindVisualByType(typeof(TextBlock), tvContent, false);


            // Verify if the sizes are different.
            if (textBlockContent.Text == textBlockHeader.Text)
            {
                LogComment("Header and content Length were the same. Should be different.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
}
