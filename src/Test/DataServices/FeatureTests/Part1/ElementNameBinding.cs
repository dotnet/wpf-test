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
using System.Windows.Media;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where ElementName binding fails on (or under) UIElement in an ItemsCollection
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ElementNameBinding")]
    public class ElementNameBinding : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListBox _myListBox;
        
        #endregion

        #region Constructors

        public ElementNameBinding()
            : base(@"ElementNameBinding.xaml")
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

            if (_myStackPanel == null || _myListBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Grab the second element in the listbox  
            TextBlock textBlock = (TextBlock)_myListBox.Items[1];            

            // Verify if a value is correctly assigned
            if (textBlock.Text != "Hello")
            {
                LogComment("Value is not correctly assigned to listbox");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class MyCollection : ObservableCollection<FrameworkElement>
    {
    }

    #endregion
}
