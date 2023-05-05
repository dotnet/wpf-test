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

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Test coverage for IEditableCollectionViewAddNewItem. 
    /// </description>
    /// </summary>
    [Test(1, "MyViews", "AddNewItem", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class AddNewItem : WindowTest
    {
        #region Private Data

        private Type _dsvpType = null;
        private static DataSourceViewProvider s_dsvp = null;
        private CollectionView _cv = null;        
        private IEditableCollectionViewAddNewItem _iana = null;
        private ListBox _lb = null;

        #endregion

        #region Constructors

        [Variation(typeof(IListViewProvider))]
        [Variation(typeof(ObservableCollectionViewProvider<Place>))]
        [Variation(typeof(IListINotifyCollectionChangedViewProvider))]
        [Variation(typeof(ItemsCollectionViewProvider<ObservableCollectionViewProvider<Place>>))]
        [Variation(typeof(ProxyViewProvider<ObservableCollectionViewProvider<Place>>))]
        public AddNewItem(Type dsvpType)
        {
            this._dsvpType = dsvpType;

            // AddNewItem scenarios            
            RunSteps += new TestStep(BasicAddNewItemScenario);
            RunSteps += new TestStep(ImplicitCommitScenario);
            RunSteps += new TestStep(NullItemsScenario);
            RunSteps += new TestStep(IllegalItemsScenario);
            RunSteps += new TestStep(hetergenousCollectionScenario);
            RunSteps += new TestStep(AddMultipleScenario);   
            RunSteps += new TestStep(NewItemPosition);

            // Transaction scenarios
            RunSteps += new TestStep(OverlappingTransactions);                       

            // Grouping Scenario            
            RunSteps += new TestStep(GroupingScenario);

            // Convenience step for testing in VS
            RunSteps += new TestStep(LastStep);
        }

        #endregion

        #region Private Members

        //--- AddNew scenarios

        // Scenario 1: Basic call to AddNewItem.
        private TestResult BasicAddNewItemScenario()
        {
            InitializeView(_dsvpType);

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            LogComment("In BasicAddNewItemScenario...");

            if (_iana.CanAddNewItem)
            {
                LogComment("In CanAddNewItem");

                // Verify initial state is nothing being added currently
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Add new item
                Place toBeAdded = new Place("Toronto", "Canada");
                object newItem = _iana.AddNewItem(toBeAdded);

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Verify a new item was created (technically that it is at least not null) and that AddNew() return value and CurrentAddItem match
                if (newItem == null) return TestResult.Fail;
                if (!_iana.IsAddingNew) return TestResult.Fail;
                if (newItem != _iana.CurrentAddItem) return TestResult.Fail;

                // Verify new item is at the end of the collection
                if (_cv.GetItemAt(_cv.Count - 1) != newItem) return TestResult.Fail;

                // Verify currency moved to the new item
                if (_cv.CurrentItem != newItem) return TestResult.Fail;

                // Verify IsAddingNew is true
                if (!_iana.IsAddingNew) return TestResult.Fail;

                // When Adding an item, CanAddNew should remain true.
                if (!_iana.CanAddNewItem)
                {
                    LogComment("CanAddNewItem is true when already adding an item.");
                    return TestResult.Fail;
                }

                // Call CommitNew
                _iana.CommitNew();

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Item should remain at end, currency should remain on the item
                if (_cv.GetItemAt(_cv.Count - 1) != newItem || newItem != _cv.CurrentItem)
                    return TestResult.Fail;

                // Verify IsAddingNew is false, and CurrentAddItem is null
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Verify item is part of collection
                if (!_cv.Contains(newItem)) return TestResult.Fail;

                // Add a new item
                object secondNewItem = _iana.AddNew();

                // Cancel
                _iana.CancelNew();

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Cancelled item should be removed from collection
                if (_cv.Contains(secondNewItem)) return TestResult.Fail;

                // currency should go back to the previous item
                if (_cv.CurrentItem != newItem || _cv.CurrentPosition != _cv.IndexOf(newItem))
                    return TestResult.Fail;

                // IsAddingNew should become false
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(null); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        // Scenario 2: Call AddNewItem after AddNew and Edit to test implicit commit.
        private TestResult ImplicitCommitScenario()
        {
            InitializeView(_dsvpType);

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            LogComment("In ImplicitCommitScenario...");

            if (_iana.CanAddNewItem)
            {
                LogComment("In CanAddNewItem...");

                // Verify initial state is nothing being added currently
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Call AddNew (from iecv), verify CanAddNewItem is false when called.
                object newObject = _iana.AddNew();

                // Verify IsAddingNew is true
                if (!_iana.IsAddingNew) return TestResult.Fail;

                // When Adding an item, CanAddNew should remain true.
                if (!_iana.CanAddNewItem)
                {
                    LogComment("CanAddNewItem is true when already adding an item.");
                    return TestResult.Fail;
                }

                // Calling AddNewItem should implicitly call iana.CommitNew();
                _iana.AddNewItem(null);                

                // Verify item added to collection.
                if (!_cv.Contains(newObject))
                {
                    LogComment("Calling AddNewItem did not implicitly commit last item - via AddNew.");
                    return TestResult.Fail;
                }
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(null); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        // Scenario 3: Add Null Items
        private TestResult NullItemsScenario()
        {
            InitializeView(_dsvpType);

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            LogComment("In NullItemsScenario");

            if (_iana.CanAddNewItem)
            {
                LogComment("In CanAddNewItem");

                // Add new item                
                object newItem = new object();

                try
                {
                    newItem = _iana.AddNewItem(null);
                }
                catch (Exception e)
                {
                    LogComment("Unexpected exception occured when trying to add null item. \n " + e.Message);
                    return TestResult.Fail;
                }

                // Verify a new item was created (technically that it is at least not null) and that AddNew() return value and CurrentAddItem match
                if (newItem != null) return TestResult.Fail;
                if (!_iana.IsAddingNew) return TestResult.Fail;
                if (newItem != _iana.CurrentAddItem) return TestResult.Fail;

                // Verify new item is at the end of the collection
                if (_cv.GetItemAt(_cv.Count - 1) != newItem) return TestResult.Fail;

                // Verify currency moved to the new item
                if (_cv.CurrentItem != newItem) return TestResult.Fail;

                // Verify IsAddingNew is true
                if (!_iana.IsAddingNew) return TestResult.Fail;

                // Call CommitNew
                _iana.CommitNew();                

                // Item should remain at end, currency should remain on the item
                if (_cv.GetItemAt(_cv.Count - 1) != newItem || newItem != _cv.CurrentItem) return TestResult.Fail;

                // Verify IsAddingNew is false, and CurrentAddItem is null
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Verify item is part of collection
                if (!_cv.Contains(newItem)) return TestResult.Fail;

            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(null); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }


        // Scenario 4: Add ILLEGAL Items (Invalid types)
        private TestResult IllegalItemsScenario()
        {
            InitializeView(_dsvpType);

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            LogComment("In IllegalItemsScenario...");            

            if (_iana.CanAddNewItem)
            {
                LogComment("In CanAddNewItem");

                if (IsHeterogenousCollection(_dsvpType))
                {
                    ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(new Button()); }, new ArgumentException(null, "value"));
                }
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(null); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        // Scenario 5: Add to heterogenous collection.
        private TestResult hetergenousCollectionScenario()
        {
            InitializeView(_dsvpType);

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            LogComment("In hetergenousCollectionScenario...");

            if (_iana.CanAddNewItem)
            {
                LogComment("In CanAddNewItem");

                // Verify initial state is nothing being added currently
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                SubPlace toBeAdded = new SubPlace("Toronto", "Ontario", "Canada");
                object newItem = _iana.AddNewItem(toBeAdded);

                if (newItem == null) return TestResult.Fail;
                if (!_iana.IsAddingNew) return TestResult.Fail;
                if (newItem != _iana.CurrentAddItem) return TestResult.Fail;

                // Verify new item is at the end of the collection
                if (_cv.GetItemAt(_cv.Count - 1) != newItem) return TestResult.Fail;

                // Verify currency moved to the new item
                if (_cv.CurrentItem != newItem) return TestResult.Fail;

                // Call CommitNew
                _iana.CommitNew();

                // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
                if (!ValidateSync()) return TestResult.Fail;

                // Item should remain at end, currency should remain on the item
                if (_cv.GetItemAt(_cv.Count - 1) != newItem || newItem != _cv.CurrentItem)
                    return TestResult.Fail;

                // Verify IsAddingNew is false, and CurrentAddItem is null
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Verify item is part of collection
                if (!_cv.Contains(newItem)) return TestResult.Fail;
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(null); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }

        // Scenario 6: Add same item MultipleTimes.
        private TestResult AddMultipleScenario()
        {
            InitializeView(_dsvpType);

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;

            LogComment("In AddMultipleScenario...");

            if (_iana.CanAddNewItem)
            {
                LogComment("In CanAddNewItem");

                // Verify initial state is nothing being added currently
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Add new item

                int initialCount = _cv.Count;

                Place toBeAdded = new Place("Toronto", "Canada");
                object firstItem = _iana.AddNewItem(toBeAdded);                
                object secondItem = _iana.AddNewItem(toBeAdded);                

                // Verify a new item was created (technically that it is at least not null) and that AddNew() return value and CurrentAddItem match
                if (firstItem == null || secondItem == null) return TestResult.Fail;
                if (!_iana.IsAddingNew) return TestResult.Fail;
                if (secondItem != _iana.CurrentAddItem) return TestResult.Fail;

                // Verify new item is at the end of the collection
                if (_cv.GetItemAt(_cv.Count - 1) != secondItem) return TestResult.Fail;

                // Verify currency moved to the new item
                if (_cv.CurrentItem != secondItem) return TestResult.Fail;

                // When Adding an item, CanAddNew should remain true.
                if (!_iana.CanAddNewItem)
                {
                    LogComment("CanAddNewItem is true when already adding an item.");
                    return TestResult.Fail;
                }

                // Call CommitNew
                _iana.CommitNew();
                if (_cv.Count != initialCount + 2)
                {
                    LogComment("CollectionView count was not incremented when duplicate items were added.");
                    return TestResult.Fail;
                }

                // Item should remain at end, currency should remain on the item
                if (_cv.GetItemAt(_cv.Count - 1) != secondItem || secondItem != _cv.CurrentItem)
                    return TestResult.Fail;

                // Verify IsAddingNew is false, and CurrentAddItem is null
                if (_iana.IsAddingNew) return TestResult.Fail;
                if (_iana.CurrentAddItem != null) return TestResult.Fail;

                // Verify item is part of collection
                if (!_cv.Contains(firstItem)) return TestResult.Fail;
                if (!_cv.Contains(secondItem)) return TestResult.Fail;

                // Check if both Items are equal.
                if (firstItem != secondItem)
                {
                    LogComment("Added Items are not equal");
                    return TestResult.Fail;
                }
            }
            else
            {
                ExceptionHelper.ExpectException(delegate() { _iana.AddNewItem(null); }, new InvalidOperationException());
            }

            return TestResult.Pass;
        }        

        private TestResult NewItemPosition()
        {
            InitializeView(_dsvpType);

            if (!_iana.CanAddNewItem) return TestResult.Pass;

            // Scenario tested when PlaceHolder position was None, so this needs to cover AtBeginning and AtEnd
            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;
            _iana.AddNewItem(new Place("Melbourn", "Australia"));

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            // The placeholder is at position 0, so new item should be at position 1
            if (_cv.IndexOf(_iana.CurrentAddItem) != 1) return TestResult.Fail;

            _iana.CancelNew();

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            _iana.AddNewItem(new Place("Sydney", "Australia"));

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            // The placeholder is at position Count - 1, so new item should be at position count - 2
            if (_cv.IndexOf(_iana.CurrentAddItem) != _cv.Count - 2) return TestResult.Fail;

            _iana.CancelNew();

            // Validate CollectionView and Listbox Items are in sync. (Makes sure CollectionView events are being raised correctly)
            if (!ValidateSync()) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult OverlappingTransactions()
        {
            InitializeView(_dsvpType);

            if (!_iana.CanAddNewItem) return TestResult.Pass;

            // AddNew during an AddNew should commit the prior AddNew
            object newItem = _iana.AddNewItem(new Place("Sun City", "South Africa"));
            _iana.AddNew();
            if (newItem == _iana.CurrentAddItem || !_cv.Contains(newItem)) return TestResult.Fail;
            _iana.CommitNew();

            // AddNew during an Edit should commit the Edit
            _iana.EditItem(_cv.GetItemAt(5));
            _iana.AddNewItem(new Place("Montreal", "Quebec"));
            if (_iana.IsEditingItem || _iana.CurrentEditItem != null) return TestResult.Fail;
            _iana.CommitNew();

            // EditItem during an AddNew should commit the AddNew
            newItem = _iana.AddNewItem(new Place("Winnipeg", "Ontario"));
            _iana.EditItem(_cv.GetItemAt(5));
            if (_iana.IsAddingNew || _iana.CurrentAddItem != null) return TestResult.Fail;
            _iana.CommitEdit();

            // EditItem during an Edit should commit the Edit
            _iana.EditItem(_cv.GetItemAt(5));
            _iana.EditItem(_cv.GetItemAt(0));
            if (_iana.CurrentEditItem != _cv.GetItemAt(0)) return TestResult.Fail;
            _iana.CommitEdit();

            // CommitEdit during an AddNew should throw an exception
            _iana.AddNewItem(new Place("Vancouver", "BC"));
            ExceptionHelper.ExpectException(delegate() { _iana.CommitEdit(); }, new InvalidOperationException());

            // CancelEdit during an AddNew should throw an exception
            _iana.AddNewItem(new Place("Surrey", "BC"));
            ExceptionHelper.ExpectException(delegate() { _iana.CancelEdit(); }, new InvalidOperationException());

            return TestResult.Pass;
        }

        private TestResult GroupingScenario()
        {
            InitializeView(_dsvpType);

            // A pure IBindingList can't sort/filter, and our validation is tied to
            // sorting+filter+grouping, and would be a mess to test this one variaton.
            // If we built a robust validator down the road we can turn this on.
            if (!s_dsvp.CanSort || !s_dsvp.CanFilter || !_iana.CanAddNewItem)
                return TestResult.Pass;

            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            // Sort, Filter, Group
            // We need to special case sorting for DataView because the ViewProvider applies the
            // sort to the underlying collection but grouping doesn't play nice with that.
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
            _iana.AddNewItem(new Place("Omaha", "NE"));
            object addItem = _iana.CurrentAddItem;            
            PropertyDescriptorCollection pdc = s_dsvp.ConvertToCanonical(addItem);            

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
            _iana.CommitNew();

            // Swap the placeholder just to cause trouble. :-)
            _iana.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;

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
            _iana.Remove(addItem);


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
            addItem = _iana.AddNewItem(new Place("Omaha", "NE"));                        
            _iana.CommitNew();            

            // Count should be at 15 - original 12 + placeholder + Omaha which is in two groups
            // Groups should be WA, CA, OR, NE, Contains O, and new itemplaceholder
            if (_cv.Count != 15 || _cv.Groups.Count != 6) return TestResult.Fail;

            _iana.EditItem(addItem);
            pdc["Name"].SetValue(addItem, "Yakima");
            pdc["State"].SetValue(addItem, "WA");

            // Count should remain the same since haven't committed, and NE group should still exist
            if (!DoubleCheckCount(15) || _cv.Groups.Count != 6) return TestResult.Fail;            

            _iana.CommitEdit();

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

        // This method checks whether the collection is a generic one or a typed one. 
        // Its somewhat hacky as it checks for the type using strings. 
        // If we were to have differently typed collections (i.e. not place) then this will need to be altered.
        private bool IsHeterogenousCollection (Type dsvpType)
        {
            string dsvpName = dsvpType.ToString();
            return dsvpName.Contains("Place");
        }

        private void InitializeView(Type dsType)
        {
            s_dsvp = DataSourceViewProvider.CreateDataSourceViewProvider(dsType, PlacesList);            
            _iana = (IEditableCollectionViewAddNewItem)s_dsvp.View;
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
            IEnumerable nonCanonicalForm = (IEnumerable)_iana;

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
	
	#region Helper Classes

    public class SubPlace : Place
    {
        private string _country;

        public SubPlace(string name, string state, string country)
        {
            this.Name = name;
            this.State = state;
            this._country = country;
        }
    }
	
	#endregion
}
