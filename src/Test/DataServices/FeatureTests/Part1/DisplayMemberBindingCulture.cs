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
    ///  Regression coverage for bug where GridViewColumn.DisplayMemberBinding ignores culture and always uses en-US instead
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "DisplayMemberBindingCulture")]
    public class DisplayMemberBindingCulture : XamlTest
    {
        #region Private Data
        
        private StackPanel _myStackPanel;
        private ListView _myListView;
        private string _expectedDateColumnOne;
        private string _expectedDateColumnTwo;
        private string _actualDateColumnOne;
        private string _actualDateColumnTwo;
        
        #endregion

        #region Constructors

        public DisplayMemberBindingCulture()
            : base(@"DisplayMemberBindingCulture.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListView = (ListView)RootElement.FindName("myListView");

            if (_myStackPanel == null || _myListView == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);           

            DateTime testDateTime = DateTime.Parse("2008-12-21");

            _expectedDateColumnOne = testDateTime.ToString("d", new CultureInfo("de-DE"));
            _expectedDateColumnTwo = testDateTime.ToString("D", new CultureInfo("de-DE"));

            // Grab Text in First & Second Columns (ignore unwanted chars)
            ListViewItem lvi = (ListViewItem)_myListView.ItemContainerGenerator.ContainerFromItem(_myListView.Items[0]);
            GridViewRowPresenter gvrp = (GridViewRowPresenter)Util.FindVisualByType(typeof(GridViewRowPresenter), lvi, false);

            TextBlock tb1 = (TextBlock)VisualTreeHelper.GetChild(gvrp, 0);
            TextBlock tb2 = (TextBlock)VisualTreeHelper.GetChild(gvrp, 1);

            _actualDateColumnOne = tb1.Text.Substring(0, _expectedDateColumnOne.Length);
            _actualDateColumnTwo = tb2.Text.Substring(0, _expectedDateColumnTwo.Length);

            // Verify the culture of the date.
            if (_actualDateColumnOne != _expectedDateColumnOne)
            {
                LogComment("Column One Dates dont match.");
                LogComment("Expected: " + _expectedDateColumnOne);
                LogComment("Actual: " + _actualDateColumnOne);
                return TestResult.Fail;
            }

            if (_actualDateColumnTwo != _expectedDateColumnTwo)
            {
                LogComment("Column Two Dates dont match.");
                LogComment("Expected: " + _expectedDateColumnTwo);
                LogComment("Actual: " + _actualDateColumnTwo);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}
