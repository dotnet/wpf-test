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
    ///  Regression coverage for bug where PreviousData binding does not work well with ListView
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "PreviousData")]
    public class PreviousData : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListView _myListView;
        private string _expectedValue;        
        
        #endregion

        #region Constructors

        public PreviousData()
            : base(@"PreviousData.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListView = (ListView) RootElement.FindName("myListView");

            if (_myStackPanel == null || _myListView == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {            
            WaitForPriority(DispatcherPriority.Background);           
            
            // Grab the appropriate PreviousData Container.
            ListViewItem lvi = (ListViewItem) _myListView.ItemContainerGenerator.ContainerFromItem(_myListView.Items[1]);
            GridViewRowPresenter gvrp = (GridViewRowPresenter)Util.FindVisualByType(typeof(GridViewRowPresenter), lvi, false);
            ContentPresenter cp = (ContentPresenter)VisualTreeHelper.GetChild(gvrp, 0);
            TextBlock tb = (TextBlock)VisualTreeHelper.GetChild(cp, 0);

            _expectedValue = "100";

            if (tb.Text != _expectedValue)
            {
                LogComment("PreviousData Template is not working correctly");
                LogComment("Expected Value: " + _expectedValue);
                LogComment("Actual Value: " + tb.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class NumbersList : ObservableCollection<int>
    {
        public NumbersList()
        {
            this.Add(100);
            this.Add(130);
            this.Add(150);
            this.Add(140);
            this.Add(145);
        }
    }

    #endregion
}