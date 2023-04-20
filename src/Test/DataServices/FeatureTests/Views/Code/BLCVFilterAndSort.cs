// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Data;
using System.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
	/// <description>
    /// This tests filtering and sorting BindingListCollectionView when the source is a bound to a DataTable, is a
    /// DataView or is a BindingListT.
    /// </description>
	/// <relatedTasks>

    /// </relatedTasks>
	/// </summary>
    [Test(1, "Views", "BLCVFilterAndSort")]
    public class BLCVFilterAndSort : XamlTest
    {
        ListBox _lb1;
        ListBox _lb2;
        ListBox _lb3;

        public BLCVFilterAndSort() : base(@"BLCVFilterAndSort.xaml")
		{
            InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(BoundToDataTable);
            RunSteps += new TestStep(SourceIsDataView);
            RunSteps += new TestStep(SourceBindingListT);
            RunSteps += new TestStep(SourceBindingListTSortThrows);
            RunSteps += new TestStep(SourceBindingListTCustomFilterThrows);
        }

        TestResult Setup()
        {
            _lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb1"));
            _lb2 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb2"));
            _lb3 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb3"));
            return TestResult.Pass;
        }

        TestResult BoundToDataTable()
        {
            Status("BoundToDataTable");

            PlacesDataTable placesDataTable = new PlacesDataTable();
            Binding b = new Binding();
            b.Source = placesDataTable;
            _lb1.SetBinding(ItemsControl.ItemsSourceProperty, b);
            if (!VerifyFiltering(_lb1)) { return TestResult.Fail; }
            if (!VerifySorting(_lb1)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult SourceIsDataView()
        {
            Status("SourceIsDataView");

            PlacesDataTable placesDataTable = new PlacesDataTable();
            DataView dataView = placesDataTable.DefaultView;
            _lb2.ItemsSource = dataView;
            if (!VerifyFiltering(_lb2)) { return TestResult.Fail; }
            if (!VerifySorting(_lb2)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult SourceBindingListT()
        {
            Status("SourceBindingListT");

            Places places = new Places();
            BindingList<Place> bindingListPlace = new BindingList<Place>(places);
            _lb3.ItemsSource = bindingListPlace;
            BindingListCollectionView blcv = (BindingListCollectionView)(CollectionViewSource.GetDefaultView(_lb3.ItemsSource));
            if (blcv.CanCustomFilter != false)
            {
                LogComment("Fail - CanCustomFilter should be false but it is true");
                return TestResult.Fail;
            }
            if (blcv.CanSort != false)
            {
                LogComment("Fail - CanSort should be false but it is true");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult SourceBindingListTSortThrows()
        {
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));

            BindingListCollectionView blcv = (BindingListCollectionView)(CollectionViewSource.GetDefaultView(_lb3.ItemsSource));
            blcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            return TestResult.Fail;
        }

        TestResult SourceBindingListTCustomFilterThrows()
        {
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));

            BindingListCollectionView blcv = (BindingListCollectionView)(CollectionViewSource.GetDefaultView(_lb3.ItemsSource));
            blcv.CustomFilter = "State <> 'WA'";
            return TestResult.Fail;
        }

        #region aux methods
        bool VerifyFiltering(ListBox lb)
        {
            BindingListCollectionView blcv = (BindingListCollectionView)(CollectionViewSource.GetDefaultView(lb.ItemsSource));
            if (blcv.CanCustomFilter != true)
            {
                LogComment("Fail - CanCustomFilter should be true but is false");
                return false;
            }

            blcv.CustomFilter = "State <> 'WA'";
            string[] expectedItems1 = new string[] { "Portland", "San Francisco", "Los Angeles", "San Diego", "San Jose", "Santa Ana"};
            if (!VerifyItems(lb, expectedItems1)) { return false; }

            blcv.CustomFilter = null;
            string[] expectedItems2 = new string[] { "Seattle", "Redmond", "Bellevue", "Kirkland", "Portland", "San Francisco", "Los Angeles", "San Diego", "San Jose", "Santa Ana", "Bellingham" };
            if (!VerifyItems(lb, expectedItems2)) { return false; }

            return true;
        }

        bool VerifySorting(ListBox lb)
        {
            BindingListCollectionView blcv = (BindingListCollectionView)(CollectionViewSource.GetDefaultView(lb.ItemsSource));
            if (blcv.CanSort != true)
            {
                LogComment("Fail - CanSort should be true but is false");
                return false;
            }

            blcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            string[] expectedItems1 = new string[] { "Seattle", "Santa Ana", "San Jose", "San Francisco", "San Diego", "Redmond", "Portland", "Los Angeles", "Kirkland", "Bellingham", "Bellevue" };
            if (!VerifyItems(lb, expectedItems1)) { return false; }

            blcv.SortDescriptions.Clear();
            string[] expectedItems2 = new string[] { "Seattle", "Redmond", "Bellevue", "Kirkland", "Portland", "San Francisco", "Los Angeles", "San Diego", "San Jose", "Santa Ana", "Bellingham" };
            if (!VerifyItems(lb, expectedItems2)) { return false; }

            return true;
        }

        bool VerifyItems(ListBox lb, string[] expectedItems)
        {
            int expectedCount = expectedItems.Length;
            int actualCount = lb.Items.Count;
            if (expectedCount != actualCount)
            {
                LogComment("Fail - Expected number of items in list box: " + expectedCount + ". Actual: " + actualCount);
                return false;
            }

            for (int i = 0; i < expectedCount; i++)
            {
                DataRowView row = (DataRowView)(lb.Items[i]);
                string placeName = (string)(row["Name"]);
                if (expectedItems[i] != placeName)
                {
                    LogComment("Fail - Expected item: " + expectedItems[i] + ". Actual: " + ((Place)(lb.Items[i])).Name);
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
