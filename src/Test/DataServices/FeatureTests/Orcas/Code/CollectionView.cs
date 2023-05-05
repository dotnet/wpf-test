// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests scenarios involving adding/removing items, filtering, sorting,
    /// and selection.
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(0, "Views", "CollectionViewTest", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class CollectionViewTest : WindowTest
    {
        #region Private Data

        private Type _dataSourceType = null;

        #endregion

        #region Constructors

        [Variation(typeof(DataViewViewProvider))]
        [Variation(typeof(IEnumerableINotifyCollectionChangedViewProvider))]
        [Variation(typeof(XLinqViewProvider))]    
        public CollectionViewTest(Type dataType)
        {
            _dataSourceType = dataType;

            RunSteps += new TestStep(AddItem);
            RunSteps += new TestStep(RemoveItemBeforeSelectedItem);
            RunSteps += new TestStep(RemoveItemAfterSelectedItem);
            RunSteps += new TestStep(RemoveSelectedItem);
            RunSteps += new TestStep(RemoveSelectedItemAtEndOfList);

            RunSteps += new TestStep(Sort);
            RunSteps += new TestStep(AddItemToSortedBeforeSelectedItem);
            RunSteps += new TestStep(AddItemToSortedAfterSelectedItem);
            RunSteps += new TestStep(RemoveSort);

            RunSteps += new TestStep(FilterSelectedItemPasses);
            RunSteps += new TestStep(FilterSelectedItemFails);
            RunSteps += new TestStep(AddItemPassesFilter);
            RunSteps += new TestStep(AddItemFailsFilter);
            RunSteps += new TestStep(AddItemFailsFilterThenRemoveFilter);
        }

        #endregion

        #region Private Members

        private TestResult AddItem()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            Place placeToAdd = new Place("Brea", "CA");
            ArrayList listAfterAdding = PlacesList;
            listAfterAdding.Add(placeToAdd);

            dataSourceViewProvider.View.MoveCurrentToPosition(4);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesList, dataSourceViewProvider);

            dataSourceViewProvider.Add(placeToAdd);

            return ValidateView(PlacesList, snappedPlaces, listAfterAdding, dataSourceViewProvider, 4);
        }

        // Each scenario is composed of the following steps:
        // 1. Setup general state. This involves creating the
        //    dataSourceViewProvider, sorting/filtering as appropriate, etc.
        // 2. Set the current item and snap the collection view. Snapping
        //    the collection view is especially relevant to the ADO
        //    sticky rows/index issue. In ADO, values instead of references
        //    are moved during add/remove/sorting/filtering/etc. What this
        //    means is that a reference to a DataRowView can contain different
        //    values before and after a sort, for example. Since WPF's
        //    selection model pays attention to the reference, while the user
        //    may have selected Redmond, WA, after a sort the selection might
        //    now be for Seattle, WA. By snapping these references, we can
        //    validate the values are not changed after the action. 
        // 3. Perform the action the scenario is about.
        // 4. Create the list for the expected collection.
        // 5. Call a validate method which validates the values of the snapped
        //    references, the CollectionView, and currency.

        private TestResult RemoveItemBeforeSelectedItem()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            ArrayList listAfterRemoving = PlacesList;
            listAfterRemoving.RemoveAt(2);

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, listAfterRemoving, dataSourceViewProvider);

            dataSourceViewProvider.Remove(GetItemAtViewIndex(2, dataSourceViewProvider.View));

            return ValidateView(listAfterRemoving, snappedPlaces, listAfterRemoving, dataSourceViewProvider, 4);
        }

        private TestResult RemoveItemAfterSelectedItem()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            ArrayList listAfterRemoving = PlacesList;
            listAfterRemoving.RemoveAt(7);

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, listAfterRemoving, dataSourceViewProvider);

            dataSourceViewProvider.Remove(GetItemAtViewIndex(7, dataSourceViewProvider.View));

            return ValidateView(listAfterRemoving, snappedPlaces, listAfterRemoving, dataSourceViewProvider, 5);
        }

        private TestResult RemoveSelectedItem()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            ArrayList listAfterRemoving = PlacesList;
            listAfterRemoving.RemoveAt(5);

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, listAfterRemoving, dataSourceViewProvider);

            dataSourceViewProvider.Remove(GetItemAtViewIndex(5, dataSourceViewProvider.View));

            return ValidateView(listAfterRemoving, snappedPlaces, listAfterRemoving, dataSourceViewProvider, 5);
        }

        private TestResult RemoveSelectedItemAtEndOfList()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            ArrayList listAfterRemoving = PlacesList;
            listAfterRemoving.RemoveAt(10);

            dataSourceViewProvider.View.MoveCurrentToPosition(10);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, listAfterRemoving, dataSourceViewProvider);

            dataSourceViewProvider.Remove(GetItemAtViewIndex(10, dataSourceViewProvider.View));

            return ValidateView(listAfterRemoving, snappedPlaces, listAfterRemoving, dataSourceViewProvider, 9);
        }

 
        private TestResult Sort()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesList, dataSourceViewProvider);

            dataSourceViewProvider.Sort(new SortDescription("Name", ListSortDirection.Ascending));

            return ValidateView(PlacesList, snappedPlaces, PlacesListAfterSorting, dataSourceViewProvider, 7);
        }

        private TestResult AddItemToSortedBeforeSelectedItem()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            dataSourceViewProvider.Sort(new SortDescription("Name", ListSortDirection.Ascending));

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterSorting, dataSourceViewProvider);

            Place placeToAdd = new Place("Brea", "CA");
            dataSourceViewProvider.Add(placeToAdd);

            ArrayList listAfterAdding = PlacesListAfterSorting;
            listAfterAdding.Insert(2, placeToAdd);

            return ValidateView(PlacesListAfterSorting, snappedPlaces, listAfterAdding, dataSourceViewProvider, 6);
        }

        private TestResult AddItemToSortedAfterSelectedItem()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            dataSourceViewProvider.Sort(new SortDescription("Name", ListSortDirection.Ascending));

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterSorting, dataSourceViewProvider);

            Place placeToAdd = new Place("Walla Walla", "WA");
            dataSourceViewProvider.Add(placeToAdd);

            ArrayList listAfterAdding = PlacesListAfterSorting;
            listAfterAdding.Add(placeToAdd);

            return ValidateView(PlacesListAfterSorting, snappedPlaces, listAfterAdding, dataSourceViewProvider, 5);
        }

        private TestResult RemoveSort()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            dataSourceViewProvider.Sort(new SortDescription("Name", ListSortDirection.Ascending));

            dataSourceViewProvider.View.MoveCurrentToPosition(5);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterSorting, dataSourceViewProvider);

            dataSourceViewProvider.ClearSort();

            return ValidateView(PlacesListAfterSorting, snappedPlaces, PlacesList, dataSourceViewProvider, 1);
        }


        private TestResult FilterSelectedItemPasses()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);

            dataSourceViewProvider.View.MoveCurrentToPosition(10);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterFiltering, dataSourceViewProvider);

            dataSourceViewProvider.Filter("State", FilterOperator.Equal, "WA");

            return ValidateView(PlacesListAfterFiltering, snappedPlaces, PlacesListAfterFiltering, dataSourceViewProvider, 4);
        }

        private TestResult FilterSelectedItemFails()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);

            dataSourceViewProvider.View.MoveCurrentToPosition(9);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterFiltering, dataSourceViewProvider);

            dataSourceViewProvider.Filter("State", FilterOperator.Equal, "WA");

            return ValidateView(PlacesListAfterFiltering, snappedPlaces, PlacesListAfterFiltering, dataSourceViewProvider, 0);
        }

        private TestResult AddItemPassesFilter()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            dataSourceViewProvider.Filter("State", FilterOperator.Equal, "WA");

            dataSourceViewProvider.View.MoveCurrentToPosition(2);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterFiltering, dataSourceViewProvider);

            Place placeToAdd = new Place("Walla Walla", "WA");
            dataSourceViewProvider.Add(placeToAdd);

            ArrayList listAfterAdding = PlacesListAfterFiltering;
            listAfterAdding.Add(placeToAdd);

            return ValidateView(PlacesListAfterFiltering, snappedPlaces, listAfterAdding, dataSourceViewProvider, 2);
        }

        private TestResult AddItemFailsFilter()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            dataSourceViewProvider.Filter("State", FilterOperator.Equal, "WA");

            dataSourceViewProvider.View.MoveCurrentToPosition(2);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterFiltering, dataSourceViewProvider);

            Place placeToAdd = new Place("Brea", "CA");
            dataSourceViewProvider.Add(placeToAdd);

            return ValidateView(PlacesListAfterFiltering, snappedPlaces, PlacesListAfterFiltering, dataSourceViewProvider, 2);
        }

        private TestResult AddItemFailsFilterThenRemoveFilter()
        {
            DataSourceViewProvider dataSourceViewProvider = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, PlacesList);
            dataSourceViewProvider.Filter("State", FilterOperator.Equal, "WA");
            Place placeToAdd = new Place("Brea", "CA");
            dataSourceViewProvider.Add(placeToAdd);

            dataSourceViewProvider.View.MoveCurrentToPosition(4);
            ArrayList snappedPlaces = SnapCollectionView(dataSourceViewProvider.View, PlacesListAfterFiltering, dataSourceViewProvider);

            dataSourceViewProvider.ClearFilter();

            ArrayList listAfterAdding = PlacesList;
            listAfterAdding.Add(placeToAdd);

            return ValidateView(PlacesListAfterFiltering, snappedPlaces, listAfterAdding, dataSourceViewProvider, 10);
        }

        // Private utility methods.
        private bool CompareCollections(IEnumerable primaryForm, IEnumerable nonCanonicalForm, DataSourceViewProvider dataSourceViewProvider)
        {
            if (primaryForm == null)
            {
                throw new ArgumentNullException("primaryForm");
            }
            if (nonCanonicalForm == null)
            {
                throw new ArgumentNullException("nonCanonicalForm");
            }

            bool collectionsAreEqual = true;

            IEnumerator primaryEnumerator = primaryForm.GetEnumerator();
            IEnumerator nonCanonicalEnumerator = nonCanonicalForm.GetEnumerator();

            while (primaryEnumerator.MoveNext())
            {
                if (nonCanonicalEnumerator.MoveNext())
                {
                    if (!dataSourceViewProvider.CompareByValue(primaryEnumerator.Current, nonCanonicalEnumerator.Current))
                    {
                        collectionsAreEqual = false;
                        TestLog.Current.LogEvidence("Items from each collection do not match.", new object[] { primaryEnumerator.Current, nonCanonicalEnumerator.Current });
                        break;
                    }
                }
                else
                {
                    // nonCanonicalEnumerator reached the end of the collection before the other enumerator,
                    // so the collection sizes don't match.
                    collectionsAreEqual = false;
                    TestLog.Current.LogEvidence("The collection sizes are not equal.");
                    break;
                }
            }

            if (collectionsAreEqual && nonCanonicalEnumerator.MoveNext())
            {
                // primaryEnumerator reached the end of the collection before the other enumerator, so the
                // collection sizes don't match.
                collectionsAreEqual = false;
                TestLog.Current.LogEvidence("The collection sizes are not equal.");
            }

            return collectionsAreEqual;
        }

        private ArrayList SnapCollectionView(ICollectionView collectionView, IEnumerable itemsThatRemainInView, DataSourceViewProvider dsvp)
        {
            IEnumerator itemsRemainingEnumerator = itemsThatRemainInView.GetEnumerator();
            itemsRemainingEnumerator.MoveNext();

            ArrayList collectionViewItems = new ArrayList();
            foreach (object item in collectionView)
            {
                if (dsvp.CompareByValue(itemsRemainingEnumerator.Current, item))
                {
                    collectionViewItems.Add(item);
                    if (!itemsRemainingEnumerator.MoveNext())
                    {
                        break;
                    }
                }
            }
            return collectionViewItems;
        }

        private object GetItemAtViewIndex(int index, ICollectionView collectionView)
        {
            IEnumerator collectionViewEnumerator = collectionView.GetEnumerator();
            for (int i = 0; i < index + 1; i++)
            {
                if (!collectionViewEnumerator.MoveNext())
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }
            return collectionViewEnumerator.Current;
        }

        private TestResult ValidateView(ArrayList expectedSnappedView, ArrayList snappedView, ArrayList expectedCollectionView, DataSourceViewProvider dataSourceViewProvider, int expectedViewCurrentPosition)
        {
            TestResult result = TestResult.Pass;

            if (!CompareCollections(expectedSnappedView, snappedView, dataSourceViewProvider))
            {
                result = TestResult.Fail;
                TestLog.Current.LogEvidence("Values of the item references changed after operation.", new object[] { snappedView, expectedSnappedView });
            }

            if (!CompareCollections(expectedCollectionView, dataSourceViewProvider.View, dataSourceViewProvider))
            {
                result = TestResult.Fail;
                TestLog.Current.LogEvidence("The contents of the CollectionView did not match the expected contents.", new object[] { dataSourceViewProvider.View, expectedCollectionView });
            }

            if (dataSourceViewProvider.View.CurrentPosition != expectedViewCurrentPosition)
            {
                result = TestResult.Fail;
                TestLog.Current.LogEvidence("The current position did not match the expected position.", new object[] { dataSourceViewProvider.View.CurrentPosition, expectedViewCurrentPosition });
            }

            if (!dataSourceViewProvider.CompareByValue(expectedCollectionView[expectedViewCurrentPosition], dataSourceViewProvider.View.CurrentItem))
            {
                result = TestResult.Fail;
                TestLog.Current.LogEvidence("The current item did not match the expected item.", new object[] { dataSourceViewProvider.View.CurrentItem, expectedCollectionView[expectedViewCurrentPosition] });
            }

            return result;
        }

        // Provide original, sorted, and filtered versions of Places so that
        // scenarios don't have to keep recreating them themselves.
        private ArrayList PlacesList
        {
            get
            {
                return new ArrayList(new Places());
            }
        }

        private ArrayList PlacesListAfterSorting
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(PlacesList[2]); // Bellevue
                list.Add(PlacesList[10]); // Bellingham
                list.Add(PlacesList[3]); // Kirkland
                list.Add(PlacesList[6]); // Los Angeles
                list.Add(PlacesList[4]); // Portland
                list.Add(PlacesList[1]); // Redmond
                list.Add(PlacesList[7]); // San Diego
                list.Add(PlacesList[5]); // San Francisco
                list.Add(PlacesList[8]); // San Jose
                list.Add(PlacesList[9]); // Santa Ana
                list.Add(PlacesList[0]); // Seattle
                return list;
            }
        }

        private ArrayList PlacesListAfterFiltering
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(PlacesList[0]); // Seattle
                list.Add(PlacesList[1]); // Redmond
                list.Add(PlacesList[2]); // Bellevue
                list.Add(PlacesList[3]); // Kirkland
                list.Add(PlacesList[10]); // Bellingham
                return list;
            }
        }

        #endregion
    }
}
