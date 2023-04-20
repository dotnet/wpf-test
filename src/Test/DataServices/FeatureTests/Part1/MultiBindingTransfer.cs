// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
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
    ///  Regression coverage for bug where MultiBinding attempts transfer before child bindings are ready 
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "MultiBindingTransfer")]
    public class MultiBindingTransfer : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        
        #endregion

        #region Constructors

        public MultiBindingTransfer()
            : base(@"MultiBindingTransfer.xaml")
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            
            if (_myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            WaitForPriority(DispatcherPriority.Render);

            // A regression would cause a failure resulting in the app crashing
            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class MyItem
    {
        public object TypedData { get; set; }
        public MyItem(int k) { TypedData = k; }
    }

    public class MyData : ObservableCollection<MyItem>
    {
        public MyData()
        {
            Add(new MyItem(1));
            Add(new MyItem(2));
            Add(new MyItem(3));
        }
    }

    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int x = (int)values[0];
            bool b = (bool)values[1];
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
        }
    }

    #endregion
}