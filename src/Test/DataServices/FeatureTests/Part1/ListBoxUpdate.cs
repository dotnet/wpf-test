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
using System.Data;
using System.IO;
using System.Windows.Media;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Listbox not updated properly when binding to a DataTable using an integer index
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ListBoxUpdate")]
    public class ListBoxUpdate : XamlTest
    {
        #region Private Data
        
        private TextBox _myTextBox;
        private Button _myButton;
        private ListView _detailListView;
        
        #endregion

        #region Constructors

        public ListBoxUpdate()
            : base(@"ListBoxUpdate.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _detailListView = (ListView)RootElement.FindName("detailListView");
            _myTextBox = (TextBox) RootElement.FindName("myTextBox");
            _myButton = (Button)RootElement.FindName("myButton");

            if (_detailListView == null || _myTextBox == null || _myButton == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            // Setup the DataView
            DataTable table = new DataTable();              
            table.Columns.Add("Name");
            table.Columns.Add("Place");
            table.Columns.Add("Animal");
            table.Rows.Add("John", "New York", "Godzilla");            

            DataView view = new DataView(table);            
            Window.DataContext = view;                 

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);           

            // Focus on TextBox and modify text.
            _myTextBox.Focus();
            _myTextBox.Text = "New Information";
            _myButton.Focus();

            WaitForPriority(DispatcherPriority.Render);           
                        
            ListViewItem listViewItem = (ListViewItem) _detailListView.ItemContainerGenerator.ContainerFromItem(_detailListView.Items[0]);
            GridViewRowPresenter gridViewRowPresenter = (GridViewRowPresenter)Util.FindVisualByType(typeof(GridViewRowPresenter), listViewItem, false);
          
            TextBlock textBlock = (TextBlock)VisualTreeHelper.GetChild(gridViewRowPresenter, 0);            

            // Verify if the listview got updated correctly.
            if (textBlock.Text != "New Information")
            {
                LogComment("ListView with DataView binding was not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
}