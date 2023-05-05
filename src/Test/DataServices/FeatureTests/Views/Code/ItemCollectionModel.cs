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
using Microsoft.Test.Modeling;
using System.Xml;
using System.Data;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// State based test for ItemCollection. Tests setting/removing filter and sort in both Items and
    /// ItemsSource mode. Tests most of ItemCollection methods/properties on each state.
    /// </description>
    /// <relatedBugs>



    /// </relatedBugs>
    /// <relatedTasks>

    /// </relatedTasks>
    /// </summary>








    // add filtering for BindingListCollectionView
    [Test(3, "Views", Disabled = true)]
    public class ItemCollectionModel : WindowModel
    {
        private ListBox _lb;
        private Places _places;
        private DataTemplate _dataTemplateCLR;

        [Variation("ItemCollectionTestCases.xtc", 1)]
        [Variation("ItemCollectionTestCases.xtc", 2)]
        [Variation("ItemCollectionTestCases.xtc", 3)]
        public ItemCollectionModel(string xtcFileName, int testCaseNumber)
            : base(xtcFileName, testCaseNumber)
        {
            // happens once before all xtc test case runs
            OnInitialize += new EventHandler(ItemCollectionModel_OnInitialize);
            // happens at the beginning of each xtc test case run
            OnBeginCase += new StateEventHandler(ItemCollectionModel_OnBeginCase);
            OnGetCurrentState += new StateEventHandler(OnGetCurrentState_Handler);

            AddAction("AddItems", new ActionHandler(AddItemsAction));
            AddAction("SetItemsSource", new ActionHandler(SetItemsSourceAction));
            AddAction("SetSort", new ActionHandler(SetSortAction));
            AddAction("RemoveSort", new ActionHandler(RemoveSortAction));
            AddAction("SetFilter", new ActionHandler(SetFilterAction));
            AddAction("RemoveFilter", new ActionHandler(RemoveFilterAction));
        }

        public ItemCollectionModel(string xtcFileName)
            : this(xtcFileName, -1)
        {
        }

        // happens once before all xtc test case runs
        private void ItemCollectionModel_OnInitialize(object sender, EventArgs e)
        {
            _lb = new ListBox();
            _lb.IsSynchronizedWithCurrentItem = true;
            Window.Content = _lb;

            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            Binding placeBinding = new Binding();
            fef.SetBinding(TextBlock.TextProperty, placeBinding);
            placeBinding.Path = new PropertyPath("Name");
            _dataTemplateCLR = new DataTemplate();
            _dataTemplateCLR.VisualTree = fef;

            _places = new Places();
        }

        // happens at the beginning of each xtc test case run
        private void ItemCollectionModel_OnBeginCase(object sender, StateEventArgs e)
        {
            _lb.Items.Filter = null;
            BindingListCollectionView blcv = CollectionViewSource.GetDefaultView(_lb.ItemsSource) as BindingListCollectionView;
            if (blcv != null)
            {
                blcv.CustomFilter = null;
            }
            _lb.Items.SortDescriptions.Clear();
            AddItems();
        }

        private void OnGetCurrentState_Handler(object sender, StateEventArgs e)
        {
            // Mode state variable
            if (_lb.ItemsSource == null && _lb.GetBindingExpression(ListBox.ItemsSourceProperty) == null)
            {
                e.State["Mode"] = "Items";
            }
            else
            {
                e.State["Mode"] = "ItemsSource";
            }

            // Sort state variable
            if (_lb.Items.SortDescriptions.Count == 0)
            {
                e.State["Sort"] = "Null";
            }
            else
            {
                e.State["Sort"] = "NotNull";
            }

            // Filter state variable
            BindingListCollectionView blcv = CollectionViewSource.GetDefaultView(_lb.ItemsSource) as BindingListCollectionView;
            if (blcv == null)
            {
                if (_lb.Items.Filter == null)
                {
                    e.State["Filter"] = "Null";
                }
                else
                {
                    e.State["Filter"] = "NotNull";
                }
            }
            else
            {
                if (blcv.CustomFilter == null)
                {
                    e.State["Filter"] = "Null";
                }
                else
                {
                    e.State["Filter"] = "NotNull";
                }
            }
            // Error state variable
            if (!VerifySortAllStates(e)) { e.State["Error"] = "True"; }
            if (!VerifyFilterAllStates(e)) { e.State["Error"] = "True"; }
            if (!VerifyItemsGetter(e)) { e.State["Error"] = "True"; }
            if (!VerifyPassesFilter(e)) { e.State["Error"] = "True"; }
            if (!VerifyCount(e)) { e.State["Error"] = "True"; }
            if (!VerifyContains(e)) { e.State["Error"] = "True"; }
            if (!VerifyIndexOf(e)) { e.State["Error"] = "True"; }
            if (!VerifyAddRemove(e)) { e.State["Error"] = "True"; }
            if (!VerifyItemsSetter(e)) { e.State["Error"] = "True"; }
            if (!VerifyMoveCurrent(e)) { e.State["Error"] = "True"; }
        }

        private bool AddItemsAction(State endState, State inParams, State outParams)
        {
            AddItems();
            return true;
        }

        private bool SetItemsSourceAction(State endState, State inParams, State outParams)
        {
            _lb.Items.Clear();
            if (inParams["Source"] == "CLR")
            {
                GlobalLog.LogStatus("CLR");
                _lb.ItemTemplate = _dataTemplateCLR;
                Binding placesBinding = new Binding();
                placesBinding.Source = _places;
                _lb.SetBinding(ListBox.ItemsSourceProperty, placesBinding);
                _lb.Items.MoveCurrentToPosition(1);
                return true;
            }
            else if (inParams["Source"] == "XmlDataSource")
            {
                GlobalLog.LogStatus("XmlDataSource");
                FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
                Binding placeBinding = new Binding();
                placeBinding.XPath = "Name";
                fef.SetBinding(TextBlock.TextProperty, placeBinding);
                fef.SetValue(TextBlock.BackgroundProperty, Brushes.AliceBlue);
                DataTemplate dataTemplateXML = new DataTemplate();
                dataTemplateXML.VisualTree = fef;
                _lb.ItemTemplate = dataTemplateXML;

                XmlDataProvider xds = new XmlDataProvider();
                xds.Source = new Uri("Places.xml", UriKind.RelativeOrAbsolute);
                Binding placesBinding = new Binding();
                placesBinding.Source = xds;
                placesBinding.XPath = "/Places/Place";
                _lb.SetBinding(ListBox.ItemsSourceProperty, placesBinding);
                xds.DataChanged += new EventHandler(xds_DataChanged);
                TestResult resDataChanged = WaitForSignal("dataChanged");
                if (resDataChanged != TestResult.Pass)
                {
                    LogComment("Fail - Could not load xml file Places.xml");
                    return false;
                }
                _lb.Items.MoveCurrentToPosition(1);
                return true;
            }

            else if (inParams["Source"] == "DataTable")
            {
                GlobalLog.LogStatus("DataTable");
                _lb.ItemTemplate = _dataTemplateCLR;
                Binding placesBinding = new Binding();
                placesBinding.Source = new PlacesDataTable();
                _lb.SetBinding(ListBox.ItemsSourceProperty, placesBinding);
                _lb.Items.MoveCurrentToPosition(1);
                return true;
            }
            LogComment("Fail - Invalid source action parameter: " + inParams["Source"]);
            return false;
        }

        void xds_DataChanged(object sender, EventArgs e)
        {
            Signal("dataChanged", TestResult.Pass);
        }

        // In VerifyItemsGetter and VerifyIndexOf, in the sorted mode, change index 6 to index 8

        private bool SetSortAction(State endState, State inParams, State outParams)
        {
            _lb.Items.SortDescriptions.Add(new SortDescription("State", ListSortDirection.Ascending));
            _lb.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            return true;
        }

        private bool RemoveSortAction(State endState, State inParams, State outParams)
        {
            _lb.Items.SortDescriptions.Clear();
            return true;
        }

        private bool SetFilterAction(State endState, State inParams, State outParams)
        {
            BindingListCollectionView blcv = CollectionViewSource.GetDefaultView(_lb.ItemsSource) as BindingListCollectionView;
            if (blcv == null)
            {
                _lb.Items.Filter = new Predicate<object>(PlaceFilter);
            }
            else
            {
                blcv.CustomFilter = "State = 'WA'";
            }
            return true;
        }

        private bool RemoveFilterAction(State endState, State inParams, State outParams)
        {
            BindingListCollectionView blcv = CollectionViewSource.GetDefaultView(_lb.ItemsSource) as BindingListCollectionView;
            if (blcv == null)
            {
                _lb.Items.Filter = null;
            }
            else
            {
                blcv.CustomFilter = null;
            }
            return true;
        }

        private void AddItems()
        {
            _lb.ItemsSource = null;
            _lb.Items.Clear();
            foreach (Place place in _places)
            {
                _lb.Items.Add(place);
            }
            _lb.ItemTemplate = _dataTemplateCLR;
            _lb.Items.MoveCurrentToPosition(1);
        }

        private bool PlaceFilter(object item)
        {
            if (GetPlaceState(item) == "WA")
            {
                return true;
            }
            return false;
        }

        // returns the Place's Name for source=CLR, source=XML and source=DataSet
        private string GetPlaceName(object item)
        {
            if (item is Place)
            {
                return ((Place)item).Name;
            }
            else if (item is XmlElement)
            {
                return ((XmlElement)item).ChildNodes[0].InnerText;
            }
            else if (item is DataRowView)
            {
                return ((DataRowView)item).Row[0].ToString();
            }
            throw new Exception("Item is not a Place or XmlElement");
        }

        // returns the Place's State for source=CLR, source=XML and source=DataSet
        private string GetPlaceState(object item)
        {
            if (item is Place)
            {
                return ((Place)item).State;
            }
            else if (item is XmlElement)
            {
                return ((XmlElement)item).ChildNodes[1].InnerText;
            }
            else if (item is DataRowView)
            {
                return ((DataRowView)item).Row[1].ToString();
            }
            throw new Exception("Item is not a Place or XmlElement");
        }

        // Tests Sort
        private bool VerifySortAllStates(StateEventArgs e)
        {
            if (e.State["Sort"] == "NotNull")
            {
                return VerifyIsSorted();
            }
            else if (e.State["Sort"] == "Null")
            {
                return !VerifyIsSorted();
            }
            LogComment("Fail - Invalid sort state variable: " + e.State["Sort"]);
            return false;
        }

        private bool VerifyIsSorted()
        {
            object previous = null;
            foreach (object place in _lb.Items)
            {
                if (previous != null)
                {
                    string previousState = GetPlaceState(previous);
                    string actualState = GetPlaceState(place);
                    if (previousState.CompareTo(actualState) <= 0)
                    {
                        if (previousState == actualState)
                        {
                            if (GetPlaceName(previous).CompareTo(GetPlaceName(place)) <= 0)
                            {
                                return true;
                            }
                            else
                            {
                                Status("Not sorted");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Status("Not sorted");
                        return false;
                    }
                }
                previous = place;
            }
            return true;
        }

        // Tests Filter
        private bool VerifyFilterAllStates(StateEventArgs e)
        {
            if (e.State["Filter"] == "NotNull")
            {
                return VerifyIsFiltered();
            }
            else if (e.State["Filter"] == "Null")
            {
                return !VerifyIsFiltered();
            }
            LogComment("Fail - Invalid filter state variable: " + e.State["Filter"]);
            return false;
        }

        private bool VerifyIsFiltered()
        {
            if (_lb.Items.Count != 5)
            {
                Status("Not filtered");
                return false;
            }
            foreach (object place in _lb.Items)
            {
                if (GetPlaceState(place) != "WA")
                {
                    Status("Not filtered");
                    return false;
                }
            }
            return true;
        }

        // Tests Items[i] (get)
        private bool VerifyItemsGetter(StateEventArgs e)
        {
            string expectedPlaceNameInIndexThree = "";

            if ((e.State["Filter"] == "NotNull") && (e.State["Sort"] == "NotNull")) // filtered and sorted mode
            {
                expectedPlaceNameInIndexThree = _places[1].Name;
            }
            else if (e.State["Filter"] == "NotNull") // filtered mode
            {
                expectedPlaceNameInIndexThree = _places[3].Name;
            }
            else if (e.State["Sort"] == "NotNull") // sorted mode
            {
                expectedPlaceNameInIndexThree = _places[8].Name;
            }
            else // direct mode
            {
                expectedPlaceNameInIndexThree = _places[3].Name;
            }

            if (GetPlaceName(_lb.Items[3]) != expectedPlaceNameInIndexThree)
            {
                LogComment("Fail - Place Name in index 3 is " + GetPlaceName(_lb.Items[3]) + " Expected: " + expectedPlaceNameInIndexThree);
                return false;
            }
            return true;
        }

        // Tests the method PassesFilter
        private bool VerifyPassesFilter(StateEventArgs e)
        {
            object expectedItemAllContain = _places[0];

            // PassesFilter works differently in the DataTable scenario. We should pass as a parameter a DataRowView,
            // which we can get by doing blcv[i]. But of course any item that is in the view passes the filter.
            // Passing any item in the actual DataTable will always return false because it is different, it is even of
            // a different type (DataRow). This is already tested in BLCVFilterAndSort.cs so I'm not going to add
            // special cases here, I will simply skip this verification.
            BindingListCollectionView blcv = CollectionViewSource.GetDefaultView(_lb.ItemsSource) as BindingListCollectionView;
            if (blcv != null) { return true; }

            if (e.State["Filter"] == "NotNull") // filter
            {
                if (!_lb.Items.PassesFilter(_places[0]))
                {
                    LogComment("Fail - " + GetPlaceName(_places[0]) + " should have passed the filter");
                    return false;
                }
                if (_lb.Items.PassesFilter(_places[4]))
                {
                    LogComment("Fail - " + GetPlaceName(_places[4]) + " should not have passed the filter");
                    return false;
                }
                return true;
            }
            else if (e.State["Filter"] == "Null") // no filter
            {
                if (!_lb.Items.PassesFilter("hello"))
                {
                    LogComment("Fail - There is no filter so PassesFilter should always return true (hello)");
                    return false;
                }
                if (!_lb.Items.PassesFilter(null))
                {
                    LogComment("Fail - There is no filter so PassesFilter should always return true (null)");
                    return false;
                }
                if (!_lb.Items.PassesFilter(_places[0]))
                {
                    LogComment("Fail - There is no filter so PassesFilter should always return true (places[0])");
                    return false;
                }
                return true;
            }
            LogComment("Fail - Invalid filter state variable: " + e.State["Filter"]);
            return false;
        }

        // Tests Count
        private bool VerifyCount(StateEventArgs e)
        {
            int expectedCount = 0;
            if (e.State["Filter"] == "NotNull")
            {
                expectedCount = 5;
            }
            else if (e.State["Filter"] == "Null")
            {
                expectedCount = 11;
            }
            else
            {
                LogComment("Fail - Invalid filter state variable: " + e.State["Filter"]);
                return false;
            }

            if (_lb.Items.Count != expectedCount)
            {
                LogComment("Fail - Expected count: " + expectedCount + " Actual: " + _lb.Items.Count);
                return false;
            }
            return true;
        }

        // Tests Contains
        private bool VerifyContains(StateEventArgs e)
        {
            object expectedPlaceDoesNotContain = null;
            object expectedPlaceContains = null;
            BindingListCollectionView blcv = CollectionViewSource.GetDefaultView(_lb.ItemsSource) as BindingListCollectionView;

            if (e.State["Filter"] == "NotNull")
            {
                expectedPlaceDoesNotContain = ((IList)(_lb.Items.SourceCollection))[4];
                expectedPlaceContains = ((IList)(_lb.Items.SourceCollection))[0];
            }
            else if (e.State["Filter"] == "Null")
            {
                expectedPlaceDoesNotContain = null;
                expectedPlaceContains = ((IList)(_lb.Items.SourceCollection))[4];
            }
            else
            {
                LogComment("Fail - Invalid filter state variable: " + e.State["Filter"]);
                return false;
            }

            if (blcv == null) // see comment on DataTable in method VerifyPassesFilter()
            {
                if (_lb.Items.Contains(expectedPlaceDoesNotContain))
                {
                    LogComment("Fail - Item should not be contained in ItemCollection");
                    return false;
                }
                if (!(_lb.Items.Contains(expectedPlaceContains)))
                {
                    LogComment("Fail - Item should be contained in ItemCollection");
                    return false;
                }
            }
            return true;
        }

        // Tests IndexOf
        private bool VerifyIndexOf(StateEventArgs e)
        {
            object expectedPlaceInIndexThree = null;

            if ((e.State["Filter"] == "NotNull") && (e.State["Sort"] == "NotNull")) // filtered and sorted mode
            {
                expectedPlaceInIndexThree = _places[1];
            }
            else if (e.State["Filter"] == "NotNull") // filtered mode
            {
                expectedPlaceInIndexThree = _places[3];
            }
            else if (e.State["Sort"] == "NotNull") // sorted mode
            {
                expectedPlaceInIndexThree = _places[8];
            }
            else // direct mode
            {
                expectedPlaceInIndexThree = _places[3];
            }

            if (GetPlaceName(expectedPlaceInIndexThree) != GetPlaceName(_lb.Items[3]))
            {
                LogComment("Fail - Expected Item in index 3: " + GetPlaceName(expectedPlaceInIndexThree) + " Actual: " + GetPlaceName(_lb.Items[3]));
                return false;
            }
            return true;
        }

        private void addItemsCallback()
        {
            _lb.Items.Add(new Place("New place", "New state"));
        }

        private void removeItemsCallback()
        {
            IList list = (IList)(_lb.Items.SourceCollection);
            _lb.Items.Remove(list[list.Count - 1]);
        }

        private void removeItemsAtCallback()
        {
            IList list = (IList)(_lb.Items.SourceCollection);
            _lb.Items.RemoveAt(list.Count - 1);
        }

        // Tests Add, Remove, RemoveAt
        private bool VerifyAddRemove(StateEventArgs e)
        {
            if ((e.State["Filter"] == "NotNull") || (e.State["Sort"] == "NotNull") || (e.State["Mode"] == "ItemsSource"))
            {
                // Add
                if (!ExpectException("System.InvalidOperationException", addItemsCallback)) { return false; }
                // Remove
                if (!ExpectException("System.InvalidOperationException", removeItemsCallback)) { return false; }
                // RemoveAt
                if (!ExpectException("System.InvalidOperationException", removeItemsAtCallback)) { return false; }
            }
            else if((e.State["Filter"] == "Null") && (e.State["Sort"] == "Null") && (e.State["Mode"] == "Items"))
            {
                Place place1 = new Place("New Place 1", "New State 1");
                Place place2 = new Place("New Place 2", "New State 2");
                // Add
                _lb.Items.Add(place1);
                _lb.Items.Add(place2);
                if (_lb.Items[_lb.Items.Count - 1] != place2)
                {
                    LogComment("Fail - " + GetPlaceName(place2) + " not added correctly");
                    return false;
                }
                // Remove
                _lb.Items.Remove(place2);
                if (_lb.Items[_lb.Items.Count - 1] != place1)
                {
                    LogComment("Fail - " + GetPlaceName(place2) + " not removed correctly. Last item: " +
                        GetPlaceName(_lb.Items[_lb.Items.Count - 1]) + " (Remove)");
                    return false;
                }
                // RemoveAt
                _lb.Items.RemoveAt(_lb.Items.Count - 1);
                if (_lb.Items[_lb.Items.Count - 1] != _places[10])
                {
                    LogComment("Fail - " + GetPlaceName(place1) + " not removed correctly. Last item: " +
                        GetPlaceName(_lb.Items[_lb.Items.Count - 1]) + " (RemoveAt)");
                    return false;
                }
            }
            else
            {
                LogComment("Fail - Invalid filter state variable: " + e.State["Filter"] +
                    " or sort state variable: " + e.State["Sort"] + " or mode state variable: " +
                    e.State["Mode"]);
                return false;
            }
            return true;
        }

        private void SetPlace()
        {
            _lb.Items[0] = new Place("New place", "New state");
        }

        // Tests Items[i] (set)
        private bool VerifyItemsSetter(StateEventArgs e)
        {
            if ((e.State["Filter"] == "NotNull") || (e.State["Sort"] == "NotNull") || (e.State["Mode"] == "ItemsSource"))
            {
                // Add
                if(!ExpectException("System.InvalidOperationException", SetPlace)) { return false; }
            }
            else if ((e.State["Filter"] == "Null") && (e.State["Sort"] == "Null") && (e.State["Mode"] == "Items"))
            {
                object oldItem0 = _lb.Items[0];
                object newItem0 = "hello";
                _lb.Items[0] = newItem0;
                if (_lb.Items[0] != newItem0)
                {
                    LogComment("Fail - Items setter failed");
                    return false;
                }
                _lb.Items[0] = oldItem0;
            }
            else
            {
                LogComment("Fail - Invalid filter state variable: " + e.State["Filter"] +
                    " or sort state variable: " + e.State["Sort"] + " or mode state variable: " +
                    e.State["Mode"]);
                return false;
            }
            return true;
        }

        // Tests currency
        // Notice that the second item (Redmond) was set to current both in Items and ItemsSource modes
        private bool VerifyMoveCurrent(StateEventArgs e)
        {
            if (_lb.Items.CurrentItem != ((IList)(_lb.Items.SourceCollection))[1])
            {
                LogComment("Fail - Current item not as expected.");
                return false;
            }
            return true;
        }
    }
}
