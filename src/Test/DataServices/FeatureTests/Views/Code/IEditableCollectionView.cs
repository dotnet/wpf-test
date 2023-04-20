// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
    /// Complete Test Suite for ListCollectionView, BindingListCollectionView, ItemsCollectionView, and ProxyViewProvider
    /// IEditableCollectionView implementations across a variety of data sources.
    /// </description>
    /// </summary>
    [Test(0, "Views", "IEditableCollectionViewTest")]
    public class IEditableCollectionViewTest : WindowTest
    {
        #region Private Data

        private Type _dsvpType = null;
        private static DataSourceViewProvider s_dsvp = null;
        private CollectionView _cv = null;
        private IEditableCollectionView _iecv = null;
        private ListBox _lb = null;

        #endregion

        #region Constructors

        // Parameterless constructor used for testing convenience in VS
        public IEditableCollectionViewTest()
            : this(typeof(DataViewViewProvider))
        {
        }

        [Variation(typeof(DataViewViewProvider))]
        [Variation(typeof(IListViewProvider))]
        [Variation(typeof(BindingListViewProvider<Place>))]
        [Variation(typeof(ObservableCollectionViewProvider<Place>))]
        [Variation(typeof(IListINotifyCollectionChangedViewProvider))]
        [Variation(typeof(ItemsCollectionViewProvider<ObservableCollectionViewProvider<Place>>))]
        // This variation doesn't work in 3.X and has not for a long time.  Disabled for now.
#if TESTBUILD_CLR40
        [Variation(typeof(ProxyViewProvider<ObservableCollectionViewProvider<Place>>))]
#endif
        public IEditableCollectionViewTest(Type dsvpType)
        {
            this._dsvpType = dsvpType;

            // AddNew scenarios
            RunSteps += new TestStep(AddingNewItemsScenario);
            RunSteps += new TestStep(NewItemPosition);

            // Remove scenarios
            RunSteps += new TestStep(RemovingItemsScenario);
            RunSteps += new TestStep(RemoveAtOutOfRangeException);
            RunSteps += new TestStep(RemoveItemNotInCollection);

            // EditItem scenarios
            RunSteps += new TestStep(EditingItemsScenario);
            RunSteps += new TestStep(EditItemNotInCollection);

            // Transaction scenarios
            RunSteps += new TestStep(OverlappingTransactions);
            RunSteps += new TestStep(ChangeViewDuringAddEditException);

            // PlaceHolder scenarios
            RunSteps += new TestStep(PlaceHolderItemScenario);
            RunSteps += new TestStep(PlaceHolderItemCurrency);
            RunSteps += new TestStep(PlaceHolderItemEditRemove);
            RunSteps += new TestStep(PlaceHolderItemMisc);

            // Chicken sink
            RunSteps += new TestStep(ISupportInitializeIEditableObject);
            RunSteps += new TestStep(Misc);
            RunSteps += new TestStep(GroupingScenario);

            // Convenience step for testing in VS
            RunSteps += new TestStep(LastStep);
        }

        #endregion

        #region Private Members

        // Provide original and a sorted/filtered version of Places so that
        // scenarios don't have to keep recreating them themselves.
        private ArrayList PlacesList
        {
            get
            {
                return new ArrayList(new Places());
            }
        }

        private ArrayList PlacesListAfterSortingFilteringGrouping
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(PlacesList[4]); // Portland

                // These are the members of the 'ContainsO' group. (Name contains an 'o')
                list.Add(PlacesList[4]); // Portland
                list.Add(PlacesList[1]); // Redmond
                list.Add(PlacesList[7]); // San Diego
                list.Add(PlacesList[5]); // San Francisco
                list.Add(PlacesList[8]); // San Jose

                list.Add(PlacesList[1]); // Redmond
                list.Add(PlacesList[0]); // Seattle
                list.Add(PlacesList[7]); // San Diego
                list.Add(PlacesList[5]); // San Francisco
                list.Add(PlacesList[8]); // San Jose
                list.Add(PlacesList[9]); // Santa Ana
                return list;
            }
        }

        private void InitializeView(Type dsType)
        {
            s_dsvp = DataSourceViewProvider.CreateDataSourceViewProvider(dsType, PlacesList);
            _iecv = (IEditableCollectionView)s_dsvp.View;
            _cv = (CollectionView)s_dsvp.View;

            _lb = new ListBox();
            _lb.ItemsSource = _cv;

            StackPanel sp = new StackPanel();
            Window.Content = sp;
            sp.Children.Add(_lb);
        }

        private ArrayList AddPlaceHolder(ArrayList list, NewItemPlaceholderPosition position)
        {
            switch (position)
            {
                case NewItemPlaceholderPosition.AtBeginning:
                    list.Insert(0, CollectionView.NewItemPlaceholder);
                    break;
                case NewItemPlaceholderPosition.AtEnd:
                    list.Add(CollectionView.NewItemPlaceholder);
                    break;
                case NewItemPlaceholderPosition.None:
                    break;
                default:
                    break;
            }

            return list;
        }

        /// <summary>
        /// We want to validate that the CollectionView and generated listbox items are in sync, which
        /// verifies that CollectionView events are being raised correctly.
        /// </summary>
        /// <returns>Whether CollectionView and ListBox visuals match.</returns>
        private bool ValidateSync()
        {
            // Since we are dealing with visuals, want to make sure we've settled down.
            WaitForPriority(DispatcherPriority.SystemIdle);

            ArrayList generatedContainers = new ArrayList();
            for (int i = 0; i < _lb.Items.Count; i++)
            {
                generatedContainers.Add(((ContentControl)_lb.ItemContainerGenerator.ContainerFromIndex(i)).Content);
            }

            return ValidateCollection(generatedContainers);
        }

        /// <summary>
        /// Validate the CollectionView against an expected list.
        /// </summary>
        /// <param name="expectedList">List that the CollectionView should match against.</param>
        /// <returns>Whether the collections match or not.</returns>
        private bool ValidateCollection(ArrayList expectedList)
        {
            IEnumerable primaryForm = (IEnumerable)expectedList;
            IEnumerable nonCanonicalForm = (IEnumerable)_iecv;

            IEnumerator primaryEnumerator = primaryForm.GetEnumerator();
            IEnumerator nonCanonicalEnumerator = nonCanonicalForm.GetEnumerator();

            bool collectionsAreEqual = true;
            int i = 0;

            while (primaryEnumerator.MoveNext())
            {
                if (nonCanonicalEnumerator.MoveNext())
                {
                    if (!s_dsvp.CompareByValue(primaryEnumerator.Current, nonCanonicalEnumerator.Current))
                    {
                        collectionsAreEqual = false;
                        TestLog.Current.LogEvidence("Items from each collection do not match.", new object[] { primaryEnumerator.Current, nonCanonicalEnumerator.Current });
                        break;
                    }

                    // We don't trust the indexers, so verify that GetItemAt and IndexOf are returning the expected answers.
                    // (This has found actual bugs)

                    // Validate GetItemAt
                    if (nonCanonicalEnumerator.Current != _cv.GetItemAt(i))
                    {
                        collectionsAreEqual = false;
                        TestLog.Current.LogEvidence("GetItemAt does not match current.", new object[] { nonCanonicalEnumerator.Current, _cv.GetItemAt(i) });
                        break;
                    }

                    // For IndexOf Grouping complicates things because IndexOf gives the first instance of the item, whereas we might be enumerating
                    // over the item at another point in the CV.
                    if (_cv.GroupDescriptions.Count == 0)
                    {
                        if (_cv.IndexOf(nonCanonicalEnumerator.Current) != i)
                        {
                            collectionsAreEqual = false;
                            TestLog.Current.LogEvidence("IndexOf does not match current.", new object[] { nonCanonicalEnumerator.Current, _cv.IndexOf(nonCanonicalEnumerator.Current) });
                            break;
                        }
                    }
                    // So under grouping we can verify that the index IndexOf gives us does contain the Current item.
                    else
                    {
                        if (_cv.GetItemAt(_cv.IndexOf(nonCanonicalEnumerator.Current)) != nonCanonicalEnumerator.Current)
                        {
                            collectionsAreEqual = false;
                            TestLog.Current.LogEvidence("The item located at IndexOf does not match current.", new object[] { nonCanonicalEnumerator.Current, _cv.IndexOf(nonCanonicalEnumerator.Current), _cv.GetItemAt(_cv.IndexOf(nonCanonicalEnumerator.Current)) });
                            break;
                        }
                    }

                    i++;
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

            if (_cv.Count != i)
            {
                collectionsAreEqual = false;
                TestLog.Current.LogEvidence("Count wrong", new object[] { _cv.Count, i });
            }

            return collectionsAreEqual;
        }

        private bool DoubleCheckCount(int expectedCount)
        {
            if (_cv.Count != expectedCount) return false;

            // In addition to verifying count, we double-check by counting ourselves
            int numIterated = 0;
            foreach (object o in _cv)
            {
                numIterated++;
            }
            if (numIterated != expectedCount) return false;

            return true;
        }

        //--- AddNew scenarios

        private TestResult AddingNewItemsScenario()
        {
            InitializeView(_dsvpType);

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            if (_iecv.CanAddNew)
            {
                // Verify initial state is nothing being added currently
                if (_iecv.IsAddingNew) return TestResult.Fail;
                if (_iecv.CurrentAddItem != null) return TestResult.Fail;

                // Add new item
                object newItem = _iecv.AddNew();

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Verify a new item was created (technically that it is at least not null) and that AddNew() return value and CurrentAddItem match
                if (newItem == null) return TestResult.Fail;
                if (!_iecv.IsAddingNew) return TestResult.Fail;
                if (newItem != _iecv.CurrentAddItem) return TestResult.Fail;

                // Verify new item is at the end of the collection
                if (_cv.GetItemAt(_cv.Count - 1) != newItem) return TestResult.Fail;

                // Verify currency moved to the new item
                if (_cv.CurrentItem != newItem) return TestResult.Fail;

                // Verify IsAddingNew is true
                if (!_iecv.IsAddingNew) return TestResult.Fail;

                // Call CommitNew
                _iecv.CommitNew();

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Item should remain at end, currency should remain on the item
                if (_cv.GetItemAt(_cv.Count - 1) != newItem || newItem != _cv.CurrentItem)
                    return TestResult.Fail;

                // Verify IsAddingNew is false, and CurrentAddItem is null
                if (_iecv.IsAddingNew) return TestResult.Fail;
                if (_iecv.CurrentAddItem != null) return TestResult.Fail;

                // Verify item is part of collection
                if (!_cv.Contains(newItem)) return TestResult.Fail;

                // Add a new item
                object secondNewItem = _iecv.AddNew();

                // Cancel
                _iecv.CancelNew();

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Cancelled item should be removed from collection
                if (_cv.Contains(secondNewItem)) return TestResult.Fail;

                // currency should go back to the previous item
                if (_cv.CurrentItem != newItem || _cv.CurrentPosition != _cv.IndexOf(newItem))
                    return TestResult.Fail;

                // IsAddingNew should become false
                if (_iecv.IsAddingNew) return TestResult.Fail;
                if (_iecv.CurrentAddItem != null) return TestResult.Fail;
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iecv.AddNew(); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        private TestResult NewItemPosition()
        {
            InitializeView(_dsvpType);

            // Scenario tested when PlaceHolder position was None, so this needs to cover AtBeginning and AtEnd
            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;
            _iecv.AddNew();

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            // The placeholder is at position 0, so new item should be at position 1
            if (_cv.IndexOf(_iecv.CurrentAddItem) != 1) return TestResult.Fail;

            _iecv.CancelNew();

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            _iecv.AddNew();

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            // The placeholder is at position Count - 1, so new item should be at position count - 2
            if (_cv.IndexOf(_iecv.CurrentAddItem) != _cv.Count - 2) return TestResult.Fail;

            _iecv.CancelNew();

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            return TestResult.Pass;
        }

        //--- Remove scenarios

        private TestResult RemovingItemsScenario()
        {
            InitializeView(_dsvpType);

            object itemToRemove = _cv.GetItemAt(0);

            if (_iecv.CanRemove)
            {
                _iecv.RemoveAt(0);

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                if (_cv.Contains(itemToRemove)) return TestResult.Fail;

                itemToRemove = _cv.GetItemAt(0);
                _iecv.Remove(itemToRemove);

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                if (_cv.Contains(itemToRemove)) return TestResult.Fail;

                // Removing an item not in the collection delegates to the collection, in most cases a no-op.
                // Might need to case for certain collections (and possibly move to special test step)
                _iecv.Remove(itemToRemove);

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iecv.RemoveAt(0); }, new InvalidOperationException());
                ExceptionHelper.ExpectException(delegate() { _iecv.Remove(_cv.GetItemAt(0)); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        private TestResult RemoveAtOutOfRangeException()
        {
            InitializeView(_dsvpType);

            // Since this is delegating down to the underlying collection, might need casing...
            if (s_dsvp.Source.GetType() == typeof(DataView))
            {
                ExceptionHelper.ExpectException(delegate() { _iecv.RemoveAt(_cv.Count); }, new IndexOutOfRangeException());
            }
            else if (s_dsvp.Source as IList != null)
            {
                ExceptionHelper.ExpectException(delegate() { _iecv.RemoveAt(_cv.Count); }, new ArgumentOutOfRangeException("index"));
            }

            return TestResult.Pass;
        }

        private TestResult RemoveItemNotInCollection()
        {
            InitializeView(_dsvpType);

            Place p = new Place();
            _iecv.Remove(p);

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            return TestResult.Pass;
        }

        //--- EditItem scenarios

        private TestResult EditingItemsScenario()
        {
            InitializeView(_dsvpType);

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            // Verify initial state is nothing being added currently
            if (_iecv.IsEditingItem) return TestResult.Fail;
            if (_iecv.CurrentEditItem != null) return TestResult.Fail;

            // Edit item
            object editItem = _cv.GetItemAt(0);
            _iecv.EditItem(editItem);

            // Verify CurrentEditItem is the item edit called on
            if (editItem == null) return TestResult.Fail;
            if (editItem != _iecv.CurrentEditItem) return TestResult.Fail;

            // Verify currency moved to the edit item
            if (_cv.CurrentItem != editItem) return TestResult.Fail;

            // Verify IsEditingItem is true
            if (!_iecv.IsEditingItem) return TestResult.Fail;

            // Call CommitEdit
            _iecv.CommitEdit();

            // currency should remain on the item
            if (editItem != _cv.CurrentItem) return TestResult.Fail;

            // Verify IsEditingItem is false, and CurrentEditItem is null
            if (_iecv.IsEditingItem) return TestResult.Fail;
            if (_iecv.CurrentEditItem != null) return TestResult.Fail;

            // Add a new item
            _iecv.EditItem(editItem);

            if (_iecv.CanCancelEdit)
            {
                // Cancel
                _iecv.CancelEdit();

                // currency should remain on item
                if (_cv.CurrentItem != editItem) return TestResult.Fail;

                // IsEditingItem should become false
                if (_iecv.IsEditingItem) return TestResult.Fail;
                if (_iecv.CurrentEditItem != null) return TestResult.Fail;
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iecv.CancelEdit(); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        private TestResult EditItemNotInCollection()
        {
            InitializeView(_dsvpType);

            Place p = new Place();
            _iecv.EditItem(p);
            return TestResult.Pass;
        }

        //--- Transaction scenarios

        private TestResult OverlappingTransactions()
        {
            InitializeView(_dsvpType);

            // AddNew during an AddNew should commit the prior AddNew
            object newItem = _iecv.AddNew();
            _iecv.AddNew();
            if (newItem == _iecv.CurrentAddItem || !_cv.Contains(newItem)) return TestResult.Fail;
            _iecv.CommitNew();

            // AddNew during an Edit should commit the Edit
            _iecv.EditItem(_cv.GetItemAt(5));
            _iecv.AddNew();
            if (_iecv.IsEditingItem || _iecv.CurrentEditItem != null) return TestResult.Fail;
            _iecv.CommitNew();

            // CommitNew or CancelNew when no new or edit transaction is underway is a no-op.
            _iecv.CommitNew();
            _iecv.CancelNew();

            // CommitNew during an Edit should throw an exception
            _iecv.EditItem(_cv.GetItemAt(0));
            ExceptionHelper.ExpectException(delegate() { _iecv.CommitNew(); }, new InvalidOperationException());

            // CancelNew during an Edit should throw an exception
            _iecv.EditItem(_cv.GetItemAt(0));
            ExceptionHelper.ExpectException(delegate() { _iecv.CancelNew(); }, new InvalidOperationException());

            // EditItem during an AddNew should commit the AddNew
            newItem = _iecv.AddNew();
            _iecv.EditItem(_cv.GetItemAt(5));
            if (_iecv.IsAddingNew || _iecv.CurrentAddItem != null) return TestResult.Fail;
            _iecv.CommitEdit();

            // EditItem during an Edit should commit the Edit
            _iecv.EditItem(_cv.GetItemAt(5));
            _iecv.EditItem(_cv.GetItemAt(0));
            if (_iecv.CurrentEditItem != _cv.GetItemAt(0)) return TestResult.Fail;
            _iecv.CommitEdit();

            // CommitEdit or CancelEdit when no new or edit transaction is underway is a no-op.
            _iecv.CommitEdit();
            _iecv.CancelEdit();

            // CommitEdit during an AddNew should throw an exception
            _iecv.AddNew();
            ExceptionHelper.ExpectException(delegate() { _iecv.CommitEdit(); }, new InvalidOperationException());

            // CancelEdit during an AddNew should throw an exception
            _iecv.AddNew();
            ExceptionHelper.ExpectException(delegate() { _iecv.CancelEdit(); }, new InvalidOperationException());

            return TestResult.Pass;
        }

        private TestResult ChangeViewDuringAddEditException()
        {
            for (int i = 0; i < 2; i++)
            {
                InitializeView(_dsvpType);

                // We want to try changing the View during both AddNew and Edit transactions
                if (i == 0)
                {
                    _iecv.AddNew();
                }
                else
                {
                    _iecv.EditItem(_cv.GetItemAt(0));
                }

                // Sorting
                if (s_dsvp.CanSort)
                    // We call Sort on the CollectionView directly instead of using DataSourceViewProvider.Sort, because the specified
                    // behavior is that sorting/filtering at the CV level throws an exception, but no guarantees are made as to behavior
                    // regarding changing this at the data source level, as it is not supported.
                    ExceptionHelper.ExpectException(delegate() { _cv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending)); }, new InvalidOperationException());

                // Filtering
                if (s_dsvp.CanFilter)
                {
                    if (_cv.GetType() == typeof(BindingListCollectionView))
                    {
                        ExceptionHelper.ExpectException(delegate() { ((BindingListCollectionView)_cv).CustomFilter = "State == 'WA'"; }, new InvalidOperationException());
                    }
                    else
                    {
                        ExceptionHelper.ExpectException(delegate()
                        {
                            _cv.Filter = new Predicate<object>(delegate(object objectToFilter)
                            {
                                return Object.Equals(((Place)objectToFilter).State, "WA");
                            });
                        }, new InvalidOperationException());
                    }
                }

                // Grouping
                ExceptionHelper.ExpectException(delegate() { _cv.GroupDescriptions.Add(new PropertyGroupDescription("State")); }, new InvalidOperationException());

                // Refresh
                ExceptionHelper.ExpectException(delegate() { _cv.Refresh(); }, new InvalidOperationException());

                // Remove
                ExceptionHelper.ExpectException(delegate() { _iecv.Remove(_cv.GetItemAt(0)); }, new InvalidOperationException());

                // RemoveAt
                ExceptionHelper.ExpectException(delegate() { _iecv.RemoveAt(0); }, new InvalidOperationException());

                // Changing PlaceHolder Position during AddNew() is prohibited, is allowed during EditItem()
                if (_iecv.IsAddingNew)
                {
                    ExceptionHelper.ExpectException(delegate() { _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning; }, new InvalidOperationException());
                }

                if (_iecv.IsEditingItem)
                {
                    _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;
                }
            }

            return TestResult.Pass;
        }

        //--- PlaceHolder scenarios

        private TestResult PlaceHolderItemScenario()
        {
            InitializeView(_dsvpType);

            if (!s_dsvp.CanSort || !s_dsvp.CanFilter)
                return TestResult.Pass;

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            if (!ValidateCollection(AddPlaceHolder(PlacesList, NewItemPlaceholderPosition.AtBeginning))) return TestResult.Fail;

            // Sort
            s_dsvp.Sort(new SortDescription("Name", ListSortDirection.Ascending));

            // Filter
            s_dsvp.Filter("Name", FilterOperator.GreaterThan, "M");

            // Group
            _cv.GroupDescriptions.Add(new PropertyGroupDescription(null, new NameContainsOGrouper()));

            if (!ValidateCollection(AddPlaceHolder(PlacesListAfterSortingFilteringGrouping, NewItemPlaceholderPosition.AtBeginning))) return TestResult.Fail;

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult PlaceHolderItemCurrency()
        {
            if (_dsvpType == typeof(BindingListViewProvider<Place>)) return TestResult.Pass;

            for (int i = 0; i < 3; i++)
            {
                InitializeView(_dsvpType);

                // The general approach is to try moving to the placeholder via MoveCurrentTo,
                // then try to move to it via MoveCurrentToFirst or MoveCurrentToLast as appropriate,
                // then move back and forth over it via MoveCurrentToNext and MoveCurrentToPrevious,
                // then try to move to it via MoveCurrentToPosition

                _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

                if (i == 1)
                {
                    _iecv.AddNew();
                }

                if (i == 2)
                {
                    // Sort
                    if (s_dsvp.CanSort)
                    {
                        s_dsvp.Sort(new SortDescription("Name", ListSortDirection.Ascending));
                    }

                    // Filter
                    if (s_dsvp.CanFilter)
                    {
                        // We would prefer to use FilterOperator.GreaterThan so that the new item
                        // doesn't pass the filter. 
                        s_dsvp.Filter("Name", FilterOperator.LessThan, "M");
                    }

                    // Group
                    _cv.GroupDescriptions.Add(new PropertyGroupDescription(null, new NameContainsOGrouper()));

                    _iecv.AddNew();


                    if (_dsvpType == typeof(DataViewViewProvider))
                    {
                        _cv.MoveCurrentToPrevious();
                        _cv.MoveCurrentToNext();
                    }
                }

                if (_cv.CurrentItem != _cv.GetItemAt(1) || _cv.CurrentPosition != 1) return TestResult.Fail;

                _cv.MoveCurrentTo(CollectionView.NewItemPlaceholder);

                if (_cv.CurrentItem != _cv.GetItemAt(1) || _cv.CurrentPosition != 1) return TestResult.Fail;

                _cv.MoveCurrentToFirst();

                if (_cv.CurrentItem != _cv.GetItemAt(1) || _cv.CurrentPosition != 1) return TestResult.Fail;

                _cv.MoveCurrentToPrevious();

                if (_cv.CurrentItem != null || _cv.CurrentPosition != -1) return TestResult.Fail;

                _cv.MoveCurrentToNext();

                if (_cv.CurrentItem != _cv.GetItemAt(1) || _cv.CurrentPosition != 1) return TestResult.Fail;

                _cv.MoveCurrentToPosition(0);

                if (_cv.CurrentItem != _cv.GetItemAt(1) || _cv.CurrentPosition != 1) return TestResult.Fail;

                _iecv.CancelNew();
                _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
                _iecv.AddNew();

                if (_cv.CurrentItem != _cv.GetItemAt(_cv.Count - 2) || _cv.CurrentPosition != _cv.Count - 2) return TestResult.Fail;

                _cv.MoveCurrentTo(CollectionView.NewItemPlaceholder);

                if (_cv.CurrentItem != _cv.GetItemAt(_cv.Count - 2) || _cv.CurrentPosition != _cv.Count - 2) return TestResult.Fail;

                _cv.MoveCurrentToLast();

                if (_cv.CurrentItem != _cv.GetItemAt(_cv.Count - 2) || _cv.CurrentPosition != _cv.Count - 2) return TestResult.Fail;

                _cv.MoveCurrentToNext();

                if (_cv.CurrentItem != null || _cv.CurrentPosition != _cv.Count) return TestResult.Fail;

                _cv.MoveCurrentToPrevious();

                if (_cv.CurrentItem != _cv.GetItemAt(_cv.Count - 2) || _cv.CurrentPosition != _cv.Count - 2) return TestResult.Fail;

                _cv.MoveCurrentToPosition(_cv.Count - 1);

                if (_cv.CurrentItem != _cv.GetItemAt(_cv.Count - 2) || _cv.CurrentPosition != _cv.Count - 2) return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult PlaceHolderItemEditRemove()
        {
            InitializeView(_dsvpType);

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            ExceptionHelper.ExpectException(delegate() { _iecv.Remove(CollectionView.NewItemPlaceholder); }, new InvalidOperationException());

            ExceptionHelper.ExpectException(delegate() { _iecv.RemoveAt(0); }, new InvalidOperationException());

            ExceptionHelper.ExpectException(delegate() { _iecv.EditItem(CollectionView.NewItemPlaceholder); }, new ArgumentException(null, "item"));

            return TestResult.Pass;
        }

        private TestResult PlaceHolderItemMisc()
        {
            InitializeView(_dsvpType);

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            // Contains(PlaceHolder) is true
            if (!_cv.Contains(CollectionView.NewItemPlaceholder)) return TestResult.Fail;

            if (s_dsvp.CanFilter)
            {

                s_dsvp.Filter("State", FilterOperator.Equal, "AZ");

                if (_cv.CurrentItem != null || _cv.CurrentPosition != -1) return TestResult.Fail;

                // If nothing passed the filter, then the only item should be the placeholder. (Placeholder isn't subject to filtering)
                if (_cv.IsEmpty || _cv.Count != 1 || _cv.GetItemAt(0) != CollectionView.NewItemPlaceholder) return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        //--- Chicken sink

        private TestResult ISupportInitializeIEditableObject()
        {
            InitializeView(_dsvpType);

            if (_cv.GetItemAt(0).GetType() != typeof(Place))
            {
                return TestResult.Pass;
            }

            // Place has a collection which holds an API Trail. So whenever an ISupportInitialize or IEditableObject API is called,
            // it records that. So we can query this collection at any time to verify the expected API has been called.

            _iecv.AddNew();
            Place newItem = (Place)_iecv.CurrentAddItem;
            if (newItem.CalledInterfaceAPI.Count != 2 || !newItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.BeginEdit) || !newItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.BeginInit)) return TestResult.Fail;
            newItem.CalledInterfaceAPI.Clear();
            _iecv.CommitNew();
            if (newItem.CalledInterfaceAPI.Count != 2 || !newItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.EndEdit) || !newItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.EndInit)) return TestResult.Fail;
            _iecv.AddNew();
            newItem = (Place)_iecv.CurrentAddItem;
            newItem.CalledInterfaceAPI.Clear();
            _iecv.CancelNew();
            if (newItem.CalledInterfaceAPI.Count != 2 || !newItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.CancelEdit) || !newItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.EndInit)) return TestResult.Fail;

            _iecv.EditItem(_cv.GetItemAt(0));
            Place editItem = (Place)_iecv.CurrentEditItem;
            if (editItem.CalledInterfaceAPI.Count != 1 || !editItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.BeginEdit)) return TestResult.Fail;
            editItem.CalledInterfaceAPI.Clear();
            _iecv.CommitEdit();
            if (editItem.CalledInterfaceAPI.Count != 1 || !editItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.EndEdit)) return TestResult.Fail;
            _iecv.EditItem(_cv.GetItemAt(1));
            editItem = (Place)_iecv.CurrentEditItem;
            editItem.CalledInterfaceAPI.Clear();
            _iecv.CancelEdit();
            if (editItem.CalledInterfaceAPI.Count != 1 || !editItem.CalledInterfaceAPI.Contains(Place.InterfaceAPI.CancelEdit)) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult Misc()
        {
            InitializeView(_dsvpType);

            // verify columns
            // All IEditableCollectionViews should implement IItemProperties also
            IItemProperties iip = (IItemProperties)_iecv;

            bool foundName = false;
            bool foundState = false;

            foreach (ItemPropertyInfo ipi in iip.ItemProperties)
            {
                if (ipi.Name == "Name" && ipi.PropertyType == typeof(string)) foundName = true;
                if (ipi.Name == "State" && ipi.PropertyType == typeof(string)) foundState = true;
            }

            if (!foundName || !foundState) return TestResult.Fail;

            // verify changes to underlying collection
            if (_iecv.GetType() == typeof(ListCollectionView) && s_dsvp.Source as INotifyCollectionChanged == null)
            {
                return TestResult.Pass;
            }
            else
            {
                _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

                object addingItem = _iecv.AddNew();

                // Removing a different item doesn't cancel the add
                s_dsvp.Remove(_cv.GetItemAt(5));
                if (!_iecv.IsAddingNew || _iecv.CurrentAddItem != addingItem) return TestResult.Fail;

                s_dsvp.Remove(_iecv.CurrentAddItem);

                // Removing item being added cancels the add
                if (_iecv.IsAddingNew || _iecv.CurrentAddItem != null) return TestResult.Fail;
                _iecv.CommitNew();

                _iecv.EditItem(_cv.GetItemAt(3));

                // Removing a different item doesn't cancel the edit
                s_dsvp.Remove(_cv.GetItemAt(5));
                if (!_iecv.IsEditingItem || _iecv.CurrentEditItem == null) return TestResult.Fail;

                // Removing item being edited cancels the edit
                s_dsvp.Remove(_iecv.CurrentEditItem);
                if (_iecv.IsEditingItem || _iecv.CurrentEditItem != null) return TestResult.Fail;
                _iecv.CommitEdit();
            }

            return TestResult.Pass;
        }

        private TestResult GroupingScenario()
        {
            InitializeView(_dsvpType);

            // A pure IBindingList can't sort/filter, and our validation is tied to
            // sorting+filter+grouping, and would be a mess to test this one variaton.
            // If we built a robust validator down the road we can turn this on.
            if (!s_dsvp.CanSort || !s_dsvp.CanFilter)
                return TestResult.Pass;

            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            // Sort, Filter, Group
            // We need to special case sorting for DataView because the ViewProvider applies the
            // sort to the underlying collection but grouping doesn't play nice with that.
            // Grouping does not preserve Sorting when Sorting is specified on a DataView directly
            if (_dsvpType == typeof(DataViewViewProvider))
            {
                _cv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
            else
            {
                s_dsvp.Sort(new SortDescription("Name", ListSortDirection.Ascending));
            }
            s_dsvp.Filter("Name", FilterOperator.GreaterThan, "M");
            _cv.GroupDescriptions.Add(new PropertyGroupDescription(null, new NameContainsOGrouper()));

            // Add a new item and make it Omaha, Nebraska
            _iecv.AddNew();
            object addItem = _iecv.CurrentAddItem;
            PropertyDescriptorCollection pdc = s_dsvp.ConvertToCanonical(addItem);
            pdc["Name"].SetValue(addItem, "Omaha");
            pdc["State"].SetValue(addItem, "NE");

            // At this point we expect to see 14 items in the CollectionView:
            // 12 from Sorting/Filtering/Grouping, the placeholder item, and the new item
            if (!DoubleCheckCount(14)) return TestResult.Fail;

            // The groups are OR, WA, CA, and ContainsO, and the placeholder and
            // new item each have their own group also, for a total of 6.
            if (_cv.Groups.Count != 6) return TestResult.Fail;

            // Create expected array of places and validate.
            ArrayList constructedArray = PlacesListAfterSortingFilteringGrouping;
            constructedArray.Insert(0, new Place("Omaha", "NE"));
            if (!ValidateCollection(AddPlaceHolder(constructedArray, NewItemPlaceholderPosition.AtBeginning))) return TestResult.Fail;

            // Now lets commit the new item
            _iecv.CommitNew();

            // Swap the placeholder just to cause trouble. :-)
            _iecv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;

            // Count should go up by one since Omaha joins two groups.
            // Groups should also have NE in it, but Omaha is no longer it's own group, so total remains the same
            if (!DoubleCheckCount(15) || _cv.Groups.Count != 6) return TestResult.Fail;

            // Since it was previously just a place, want to make sure Groups property didn't just not get updated
            // Check that the addItem is no longer a direct child of Groups.
            if (_cv.Groups.Contains(addItem)) return TestResult.Fail;

            // Validate order of CollectionView contents
            constructedArray = PlacesListAfterSortingFilteringGrouping;
            constructedArray.Insert(0, new Place("Omaha", "NE"));
            constructedArray.Insert(2, new Place("Omaha", "NE"));
            if (!ValidateCollection(AddPlaceHolder(constructedArray, NewItemPlaceholderPosition.AtEnd))) return TestResult.Fail;

            // Time for Remove
            _iecv.Remove(addItem);

            if (_dsvpType == typeof(ProxyViewProvider<ObservableCollectionViewProvider<Place>>))
            {
                if (_cv.Count == 13) return TestResult.Pass;
                else return TestResult.Fail;
            }

            // Count should go down by two since Omaha was in two groups
            // NE group should no longer exist, so groups count goes down one.
            if (!DoubleCheckCount(13) || _cv.Groups.Count != 5) return TestResult.Fail;

            // Now time for editing
            // Get back to appropriate state for editing
            addItem = _iecv.AddNew();
            pdc["Name"].SetValue(addItem, "Omaha");
            pdc["State"].SetValue(addItem, "NE");
            _iecv.CommitNew();

            // Count should be at 15 - original 12 + placeholder + Omaha which is in two groups
            // Groups should be WA, CA, OR, NE, Contains O, and new itemplaceholder
            if (_cv.Count != 15 || _cv.Groups.Count != 6) return TestResult.Fail;

            _iecv.EditItem(addItem);

            pdc["Name"].SetValue(addItem, "Yakima");
            pdc["State"].SetValue(addItem, "WA");

            // Count should remain the same since haven't committed, and NE group should still exist
            if (!DoubleCheckCount(15) || _cv.Groups.Count != 6) return TestResult.Fail;

            _iecv.CommitEdit();

            // count should go down one since Yakima only in one group, and NE group should no longer exist
            if (!DoubleCheckCount(14) || _cv.Groups.Count != 5) return TestResult.Fail;

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            return TestResult.Pass;
        }

        //--- Debugging scenario

        // Last scenario to provide breakpoint to verify all cases passed (used for when I'm running without breaking for exceptions)
        private TestResult LastStep()
        {
            return TestResult.Pass;
        }

        #endregion

        private class NameContainsOGrouper : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                PropertyDescriptorCollection pdc = s_dsvp.ConvertToCanonical(value);

                string placeName = (string)pdc["Name"].GetValue(value);
                string placeState = (string)pdc["State"].GetValue(value);

                if (placeName.Contains("o") || placeName.Contains("O"))
                {
                    return new string[] { placeState, "ContainsO" };
                }
                else
                {
                    return placeState;
                }
            }

            #region IValueConverter Members


            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
