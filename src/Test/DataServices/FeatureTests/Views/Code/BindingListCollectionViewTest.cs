// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Provides coverage for BindingListCollectionView.
	/// </description>
    /// <relatedTasks>

    /// </relatedTasks>
	/// <relatedBugs>


    /// </relatedBugs>
	/// </summary>
    [Test(2, "Views", "BindingListCollectionViewTest")]
	public class BindingListCollectionViewTest : AvalonTest
	{
		private BindingListCollectionView _blcv;
		private DataTable _myTable;
		private DataView _myView;

		public BindingListCollectionViewTest()
		{
			InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestIEnumerable);
            RunSteps += new TestStep(TestIComparer);
            RunSteps += new TestStep(TestICurrentItem);
            RunSteps += new TestStep(TestSort);
            RunSteps += new TestStep(TestCustomFilter);
            RunSteps += new TestStep(TestFilter);
            RunSteps += new TestStep(TestPassesFilter);
            RunSteps += new TestStep(TestContains);
            RunSteps += new TestStep(TestIndexOf);
		}

		private TestResult Setup()
		{
			Status("Setup");

			// Create one DataTable with one column.
			_myTable = new DataTable("myTable");
			DataColumn colItem = new DataColumn("item", Type.GetType("System.String"));
			_myTable.Columns.Add(colItem);
			// Add five items.
			DataRow NewRow;
			for (int i = 0; i < 5; i++)
			{
				NewRow = _myTable.NewRow();
				NewRow["item"] = "Item " + i;
                _myTable.Rows.Add(NewRow);
			}

            // this is used when adding 2 SortDescriptions
            _myTable.Columns.Add("otherItem");
            _myTable.Rows[0]["otherItem"] = "B";
            _myTable.Rows[1]["otherItem"] = "A";
            _myTable.Rows[2]["otherItem"] = "B";
            _myTable.Rows[3]["otherItem"] = "A";
            _myTable.Rows[4]["otherItem"] = "B";

            //string firstItem = (string)myTable.Rows[0]["item"]; // this is "item 0"

			// DataView implements IBindingList
			_myView = new DataView(_myTable);

			_blcv = CollectionViewSource.GetDefaultView(_myView) as BindingListCollectionView;

			return TestResult.Pass;
		}

		private TestResult TestIEnumerable()
		{
			Status("TestIEnumerable");

			int count1 = 0;
			foreach (object obj in _blcv)
			{
				count1++;
			}

			int count2 = _blcv.Count;

			if (count2 != 5)
			{
				LogComment("Fail - Count should be 5, instead it is " + count2);
				return TestResult.Fail;
			}

			if (count1 != count2)
			{
				LogComment("Fail - count1 and count2 should be the same");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult TestIComparer()
		{
			Status("TestIComparer");

			int res1 = ((IComparer)_blcv).Compare(_blcv.GetItemAt(0), _blcv.GetItemAt(1));
			if(res1 != -1)
			{
				LogComment("Fail - TestResult of Compare should be -1, instead it is " + res1);
				return TestResult.Fail;
			}
			int res2 = ((IComparer)_blcv).Compare(_blcv.GetItemAt(1), _blcv.GetItemAt(0));
			if (res2 != 1)
			{
				LogComment("Fail - TestResult of Compare should be 1, instead it is " + res2);
				return TestResult.Fail;
			}
			int res3 = ((IComparer)_blcv).Compare(_blcv.GetItemAt(0), _blcv.GetItemAt(0));
			if (res3 != 0)
			{
				LogComment("Fail - TestResult of Compare should be 0, instead it is " + res3);
				return TestResult.Fail;
			}
			int res4 = ((IComparer)_blcv).Compare(null, _blcv.GetItemAt(0));
			if (res4 != -1)
			{
				LogComment("Fail - TestResult of Compare should be -1, instead it is " + res4);
				return TestResult.Fail;
			}
			int res5 = ((IComparer)_blcv).Compare(null, "hello");
			if (res5 != 0)
			{
				LogComment("Fail - TestResult of Compare should be 0, instead it is " + res5);
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult TestICurrentItem()
		{
			Status("TestICurrentItem");

			// CurrentChanging and CurrentChanged event handlers
			CurrentChangingVerifier ccv = new CurrentChangingVerifier(_blcv);
			CurrentChangedVerifier ccdv = new CurrentChangedVerifier(_blcv);

			_blcv.MoveCurrentToNext();
			ccv.Verify(_blcv, 5, _blcv.GetItemAt(0));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(1));

			_blcv.MoveCurrentToFirst();
			ccv.Verify(_blcv, 5, _blcv.GetItemAt(1));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(0));

			_blcv.MoveCurrentToLast();
			ccv.Verify(_blcv, 5, _blcv.GetItemAt(0));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(4));

			_blcv.MoveCurrentToPrevious();
			ccv.Verify(_blcv, 5, _blcv.GetItemAt(4));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(3));

			_blcv.MoveCurrentTo(_blcv.GetItemAt(2));
			ccv.Verify(_blcv, 5, _blcv.GetItemAt(3));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(2));

            _blcv.MoveCurrentToLast();
            _blcv.MoveCurrentToNext();
            if (!_blcv.IsCurrentAfterLast)
            {
                LogComment("Fail - IsCurrentAfterLast should be true but it is false");
                return TestResult.Fail;
            }

            ccv.Verify(_blcv, 5, _blcv.GetItemAt(4));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(4));

            _blcv.MoveCurrentToFirst();
            _blcv.MoveCurrentToPrevious();
            if (!_blcv.IsCurrentBeforeFirst)
            {
                LogComment("Fail - IsCurrentBeforeFirst should be true but it is false");
                return TestResult.Fail;
            }

            ccv.Verify(_blcv, 5, _blcv.GetItemAt(0));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(0));

			int position = 1;
			_blcv.MoveCurrentToPosition(position);

			if (_blcv.CurrentPosition != position)
			{
				LogComment("Fail - Position:" + position + " CurrentPosition:" + _blcv.CurrentPosition);
				return TestResult.Fail;
			}

			ccv.Verify(_blcv, 5, _blcv.GetItemAt(0));
			ccdv.Verify(_blcv, 5, _blcv.GetItemAt(position));

			ccv.RemoveEventHandler(_blcv);
			ccdv.RemoveEventHandler(_blcv);

			return TestResult.Pass;
		}

		private TestResult TestSort()
		{
			Status("TestSort");

            // Sort on 1 property
			Collection<SortDescription> sortCollection1 = new Collection<SortDescription>();
			sortCollection1.Add(new SortDescription("item", ListSortDirection.Descending));

            if (_blcv.CanSort != true)
            {
                LogComment("Fail - CanSort should be true but it is false");
                return TestResult.Fail;
            }

            _blcv.SortDescriptions.Add(new SortDescription("item", ListSortDirection.Descending));

			if (!CompareSortDescriptionCollection(sortCollection1, _blcv.SortDescriptions))
			{
				LogComment("Fail - Sorting was not properly set");
				return TestResult.Fail;
			}

            string[] expectedItems = new string[] { "Item 4", "Item 3", "Item 2", "Item 1", "Item 0" };
            if (!VerifyItems(expectedItems)) { return TestResult.Fail; }

            // Sort on 2 properties
            _blcv.SortDescriptions.Clear();
            _blcv.SortDescriptions.Add(new SortDescription("otherItem", ListSortDirection.Ascending));
            _blcv.SortDescriptions.Add(new SortDescription("item", ListSortDirection.Ascending));

            string[] expectedItems2 = new string[] { "Item 1", "Item 3", "Item 0", "Item 2", "Item 4" };
            if (!VerifyItems(expectedItems2)) { return TestResult.Fail; }

            _blcv.SortDescriptions.Clear();

			return TestResult.Pass;
		}

        private TestResult TestCustomFilter()
        {
            Status("TestCustomFilter");

            if (_blcv.CanCustomFilter != true)
            {
                LogComment("Fail - CanCustomFilter should be true but it is false");
                return TestResult.Fail;
            }
            _blcv.CustomFilter = "item <> 'Item 4'";
            string[] expectedItems = new string[] { "Item 0", "Item 1", "Item 2", "Item 3" };
            if (!VerifyItems(expectedItems)) { return TestResult.Fail; }

            _blcv.CustomFilter = null;
            return TestResult.Pass;
        }

        private TestResult TestFilter()
        {
            Status("TestFilter");

            if (_blcv.CanFilter != false)
            {
                LogComment("Fail - CanFilter should be true but it is false");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestPassesFilter()
        {
            Status("TestPassesFilter");

            _blcv.CustomFilter = "item <> 'Item 4'";
            if (_blcv.PassesFilter(_blcv.GetItemAt(0)) != true)
            {
                LogComment("Fail - PassesFilter should be true but it is false");
                return TestResult.Fail;
            }
            if (_blcv.PassesFilter("hello") != false)
            {
                LogComment("Fail - PassesFilter should be false but it is true");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

		private TestResult TestContains()
		{
			Status("TestContains");

			if (!_blcv.Contains(_myView[0]))
			{
				LogComment("Fail - blcv should contain myView[0]");
				return TestResult.Fail;
			}
			if (_blcv.Contains("hello"))
			{
				LogComment("Fail - blcv should not contain 'hello'");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult TestIndexOf()
		{
			Status("TestIndexOf");

			if (_blcv.IndexOf(_myView[1]) != 1)
			{
				LogComment("Fail - Index of item should be 1");
				return TestResult.Fail;
			}

			if (_blcv.IndexOf(null) != -1)
			{
				LogComment("Fail - Index of null should be -1");
				return TestResult.Fail;
			}

			return TestResult.Pass;
        }

        #region Aux Methods
        private bool CompareSortDescriptionCollection(IList<SortDescription> expected, IList<SortDescription> actual)
        {
            if (   (expected == null) || (actual == null)
                || (expected.Count != actual.Count))
                return false;

            for (int i = 0; i < expected.Count; i++)
            {
                // .Equals started failing because the IsSealed property is different
                if (((SortDescription)expected[i]).PropertyName != ((SortDescription)actual[i]).PropertyName)
                {
                    return false;
                }
                if (((SortDescription)expected[i]).Direction != ((SortDescription)actual[i]).Direction)
                {
                    return false;
                }
            }
            return true;
        }

        private bool VerifyItems(string[] expectedItems)
        {
            int expectedCount = expectedItems.Length;
            int actualCount = _blcv.Count;

            if (expectedCount != actualCount)
            {
                LogComment("Fail - Expected count: " + expectedCount + ". Actual: " + actualCount);
                return false;
            }

            for (int i = 0; i < actualCount; i++)
            {
                DataRowView drv = (DataRowView)(_blcv.GetItemAt(i));
                string actualItemName = (string)(drv["item"]);
                if (expectedItems[i] != actualItemName)
                {
                    LogComment("Fail - Expected item name: " + expectedItems[i] + ". Actual: " + actualItemName);
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
