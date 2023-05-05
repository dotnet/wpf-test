// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System.Data;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Diagnostics;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Tests a few ADO scenarios, as test followup for some bugs:
    /// Scenario 1: Changing an item in the DataTable does not cause the UI to update.
    /// Scenario 2: Deletes selected item of ListBox bound to DataTable. Makes sure there is no
    /// confusion with indexes after that.
    /// Scenario 3: Scenario involving sorting and selecting different items used to cause app to crash.
    /// Scenario 4: Scenario involving CompositeCollection, ADO.NET data, sorting and selection.
	/// </description>
	/// </summary>





    [Test(1, "Controls", "ADOScenarios")]
	public class ADOScenarios : XamlTest
	{
        Page _page;

        public ADOScenarios()
            : base(@"ADOScenarios.xaml")
		{
            InitializeSteps += new TestStep(Setup);

            RunSteps += new TestStep(UpdateSourceItem);

            RunSteps += new TestStep(DeleteSelectedItem);
            RunSteps += new TestStep(DeleteLastItem);

            RunSteps += new TestStep(SortCrash);

            RunSteps += new TestStep(DoubleSelection);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);
            _page = (Page)(this.Window.Content);
            return TestResult.Pass;
        }

        private TestResult UpdateSourceItem()
        {
            Status("UpdateSourceItem");

            StackPanel sp1 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(this.Window, "sp1"));
            TextBlock txt1 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(sp1, "txt1"));

            DataTable myTable = new DataTable("myTable");
            DataColumn col1 = new DataColumn("item1", Type.GetType("System.String"));
            DataColumn col2 = new DataColumn("item2", Type.GetType("System.String"));
            myTable.Columns.Add(col1);
            myTable.Columns.Add(col2);
            DataRow row1 = myTable.NewRow();
            row1["item1"] = "content 1";
            row1["item2"] = "content 2";
            myTable.Rows.Add(row1);
            this.Window.DataContext = myTable;

            ((DataTable)sp1.DataContext).Rows[0][0] = "foo";

            WaitForPriority(DispatcherPriority.SystemIdle);

            Util.AssertEquals(txt1.Text, "foo");

            return TestResult.Pass;
        }

        private TestResult DeleteSelectedItem()
        {
            Status("DeleteSelectedItem");

            ListBox lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.Window, "lb1"));
            TextBlock txt2 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(this.Window, "txt2"));

            DataRowView drv = lb1.SelectedItem as DataRowView;
            DataRow currentRow = drv.Row as DataRow;
            currentRow.Delete();

            WaitForPriority(DispatcherPriority.SystemIdle);

            Util.AssertEquals(txt2.Text, "Redmond");

            return TestResult.Pass;
        }

        private TestResult DeleteLastItem()
        {
            Status("DeleteLastItem");

            ListBox lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.Window, "lb1"));
            DataTable dt = (DataTable)(_page.Resources["placesDataTable"]);
            int count = lb1.Items.Count;
            object lastItem = dt.Rows[count - 1];
            dt.Rows.RemoveAt(count - 1);

            int index = lb1.Items.IndexOf(lastItem);

            Util.AssertEquals(-1, index);

            return TestResult.Pass;
        }

        private TestResult SortCrash()
        {
            Status("SortCrash");

            // select second item
            ListBox lb2 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.Window, "lb2"));
            lb2.SelectedIndex = 1;
            // add new item
            DataTable dt = (DataTable)(_page.Resources["placesDataTable"]);
            DataRow row1 = dt.NewRow();
            row1[0] = "New Place";
            row1[1] = "New State";
            dt.Rows.Add(row1);
            // sort the data
            DataView dv = dt.DefaultView;
            dv.Sort = "Name DESC";
            // select item added
            lb2.SelectedIndex = lb2.Items.Count - 1;
            // select second item
            lb2.SelectedIndex = 1;
            // select item added
            lb2.SelectedIndex = lb2.Items.Count - 1;

            // ** No crash **

            return TestResult.Pass;
        }

        private TestResult DoubleSelection()
        {
            Status("DoubleSelection");

            // setup
            PlacesDataTable src2 = (PlacesDataTable)(this.RootElement.Resources["src2"]);
            ListBox lb3 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.Window, "lb3"));

            // select Kirkland in the first collection
            lb3.SelectedIndex = 3;

            // select Kirkland in the second collection
            lb3.SelectedIndex = 14;

            // sort collection twice
            BindingListCollectionView blcv = (BindingListCollectionView)(CollectionViewSource.GetDefaultView(src2));
            blcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            blcv.SortDescriptions.Clear();
            blcv.SortDescriptions.Add(new SortDescription("State", ListSortDirection.Ascending));

            if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
            {
                // Before the ADO StickyRow fix the behavior was that after
                // sorting San Jose became selected. The testcase was expecting
                // this flawed behavior.
                Util.AssertEquals(lb3.SelectedIndex, 14);
            }
            else
            {
                // After the ADO StickyRow fix, Kirkland remains selected after
                // sorting. After sorting it is located at index 20.
                Util.AssertEquals(lb3.SelectedIndex, 20);
            }

            // select Kirkland in the first collection
            lb3.SelectedIndex = 3;

            // verify only Kirkland is selected (San Jose is not!)
            Util.AssertEquals(lb3.SelectedIndex, 3);
            Util.AssertEquals(lb3.SelectedItems.Count, 1);

            return TestResult.Pass;
        }
	}
}
