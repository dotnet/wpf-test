// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
    /// Tests Live Shaping
    /// Test Sorting, Filtering and Grouping actions
    /// with property changes while properties are changing.
    /// </description>
    /// </summary>
    [Test(0, "Views", "LiveShaping")]
    public class LiveShaping : XamlTest
    {
        #region Private Data

        private Type _dataSourceType = null;
        private bool _useLiveProperties;
        private int _filterValue;
        private CollectionView _cv;
        private CollectionViewSource _cvs;
        private string _cvType;
        private IEditableCollectionView _iecv;
        private ListView _lv;
        private ICollectionViewLiveShaping _icvls;
        private string _testProperty = "Number";

        #endregion

        ObservableCollection<Order> _orders;
        public DataSourceViewProvider dsvp;
        static int s_orderCounter;

        #region Constructors
        //The variations destinguish between data view providers
        //and how collection can be registered for the cross thread access

        [Variation(typeof(DataViewViewProvider), false)]
        [Variation(typeof(DataViewViewProvider), true)]
        [Variation(typeof(ObservableCollectionViewProvider<Order>), false)]
        [Variation(typeof(ObservableCollectionViewProvider<Order>), true)]
        [Variation(typeof(ItemsCollectionViewProvider<ObservableCollectionViewProvider<Order>>), false)]
        [Variation(typeof(ItemsCollectionViewProvider<ObservableCollectionViewProvider<Order>>), true)]
        public LiveShaping(Type dataType, bool registerLiveProperties)
            : base(@"LiveShaping.xaml")
        {
            _dataSourceType = dataType;
            _useLiveProperties = registerLiveProperties;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(LiveGroupingAndVerify);
            RunSteps += new TestStep(LiveSortingAndVerify);
            if (_useLiveProperties) RunSteps += new TestStep(LiveFilteringAndVerify);
            RunSteps += new TestStep(CVSLiveSortingAndVerify);
            RunSteps += new TestStep(CVSLiveGroupingAndVerify);
            if (_useLiveProperties) RunSteps += new TestStep(CVSLiveFilteringAndVerify);
        }
        #endregion

        #region Setup

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _filterValue = 3;
            s_orderCounter = 0;
            _orders = GenerateOrders();
            dsvp = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, _orders);
            _icvls = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(dsvp.Source);
            _cv = (CollectionView)CollectionViewSource.GetDefaultView(dsvp.Source);
            _iecv = dsvp as IEditableCollectionView;

            if (dsvp.View.ToString().Equals("System.Windows.Controls.ItemCollection")) _cvType = "ic";
            else if (_cv.GetType().FullName.Equals("System.Windows.Data.BindingListCollectionView")) _cvType = "blcv";
            else if (_cv.GetType().FullName.Equals("System.Windows.Data.ListCollectionView")) _cvType = "lcv";
            
            if (_cvType == null)
            {
                LogComment("Unexpected CollectionView type");
                return TestResult.Fail;
            }

            _lv = (ListView)(LogicalTreeHelper.FindLogicalNode(RootElement, "lv"));
            _lv.ItemsSource = dsvp.Source;

            return TestResult.Pass;
        }
        #endregion Setup

        private TestResult LiveGroupingAndVerify()
        {
            if (!_useLiveProperties) GroupCV(_testProperty);
            if (!_icvls.CanChangeLiveGrouping)
            {
                LogComment("CanChangeLiveGrouping should always be true");
                return TestResult.Fail;
            }
            if (((bool)_icvls.IsLiveGrouping))
            {
                LogComment("We shouldn't have IsLiveGrouping true by default");
                return TestResult.Fail;
            }
            if (_useLiveProperties)
            {
                //Register path collections to properties that triger live shaping
                //This should always work
                if (!RegisterPropertyPath("group", _testProperty)) return TestResult.Fail;
                GroupCV(_testProperty);
            }
            if (_icvls.CanChangeLiveGrouping)
            {
                _icvls.IsLiveGrouping = true;
            }
            if (!(bool)_icvls.IsLiveGrouping)
            {
                LogComment("LiveGrouping should be True but it is not");
                return TestResult.Fail;
            }
            if (ChangeProperty(5, 13, _testProperty))
            {
                if (Helper.CompareViewToSource(dsvp.View, dsvp.Source, _testProperty) == TestResult.Fail) return TestResult.Fail;
                _cv.GroupDescriptions.Clear();
                _icvls.LiveGroupingProperties.Clear();
                if (_icvls.CanChangeLiveGrouping)
                {
                    _icvls.IsLiveGrouping = false;
                }
                return TestResult.Pass;
            }
            else
            {
                LogComment("Could not change property");
                return TestResult.Fail;
            }
        }
        private TestResult LiveSortingAndVerify()
        {
            if (!_useLiveProperties) SortCV(_testProperty, ListSortDirection.Descending);
            if (_icvls.CanChangeLiveSorting)
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("BindingListCollectionView shouldn't allow changes to Live Sorting");
                    return TestResult.Fail;
                }
            }
            else
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("CanChangeLiveSorting is always false with BindingList because there is no API to switch it on and off");
                }
                else
                {
                    LogComment("CanChangeLiveSorting is expected to be True");
                    return TestResult.Fail;
                }
            }
            if (((bool)_icvls.IsLiveSorting) && (!_cvType.Equals("blcv")))
            {
                LogComment("Only BindingListCollectionView should always be Live Sorting otherwise it's a bug");
                return TestResult.Fail;
            }
            if (_useLiveProperties)
            {
                //Register path collections to properties that triger live shaping
                //This should always work
                if (!RegisterPropertyPath("sort", _testProperty)) return TestResult.Fail;
                SortCV(_testProperty, ListSortDirection.Descending);
            }
            if (_icvls.CanChangeLiveSorting)
            {
                _icvls.IsLiveSorting = true;
            }
            if (!(bool)_icvls.IsLiveSorting)
            {
                LogComment("LiveSorting should be True but it is not");
                return TestResult.Fail;
            }
            if (ChangeProperty(13, 5, _testProperty))
            {
                if (Helper.CompareViewToSource(dsvp.View, dsvp.Source, _testProperty) == TestResult.Fail) return TestResult.Fail;
                _cv.SortDescriptions.Clear();
                _icvls.LiveSortingProperties.Clear();
                if (_icvls.CanChangeLiveSorting)
                {
                    _icvls.IsLiveSorting = true;
                }
                return TestResult.Pass;
            }
            else
            {
                LogComment("Could not change property");
                return TestResult.Fail;
            }
        }
        private TestResult LiveFilteringAndVerify()
        {
            FilterCV();
            if (_icvls.CanChangeLiveFiltering)
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("BindingListCollectionView shouldn't allow changes to Live Filtering");
                    return TestResult.Fail;
                }
            }
            else
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("CanChangeLiveFiltering is always false with BindingList because there is no API to switch it on and off");
                }
                else
                {
                    LogComment("CanChangeLiveFiltering is expected to be True");
                    return TestResult.Fail;
                }
            }
            if (((bool)_icvls.IsLiveFiltering) && (!_cvType.Equals("blcv")))
            {
                LogComment("Only BindingListCollectionView should always be Live Filtering otherwise it's a bug");
                return TestResult.Fail;
            }

            //Register path collections to properties that triger live shaping
            //This should always work
            if (!RegisterPropertyPath("filter", _testProperty)) return TestResult.Fail;

            if (_icvls.CanChangeLiveFiltering)
            {
                _icvls.IsLiveFiltering = true;
            }

            if (!(bool)_icvls.IsLiveFiltering)
            {
                LogComment("LiveFiltering should be True but it is not");
                return TestResult.Fail;
            }

            if (ChangeProperty(2, 13, _testProperty))
            {
                if (Helper.CompareViewToSource(dsvp.View, dsvp.Source, _testProperty) == TestResult.Fail) return TestResult.Fail;
                if (_cv.CanFilter)
                {
                    _cv.Filter = null;
                }
                _icvls.LiveFilteringProperties.Clear();
                if (_icvls.CanChangeLiveFiltering)
                {
                    _icvls.IsLiveFiltering = true;
                }
                return TestResult.Pass;
            }
            else
            {
                LogComment("Could not change property");
                return TestResult.Fail;
            }
        }
        private TestResult CVSLiveSortingAndVerify()
        {
            string targetElement;
            if (!_useLiveProperties)
            {
                targetElement = "SortingPanel";
            }
            else
            {
                targetElement = "SortingPanelWithPath";
            }

            Panel liveSortingPanel = (Panel)(LogicalTreeHelper.FindLogicalNode(RootElement, targetElement));
            _cvs = (CollectionViewSource)liveSortingPanel.Resources["cvs"];
            _cvs.Source = dsvp.Source;
            _cv = (CollectionView)_cvs.View;

            if (_cvs.CanChangeLiveSorting)
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("BindingListCollectionView shouldn't allow changes to Live Sorting");
                    return TestResult.Fail;
                }
            }
            else
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("CanChangeLiveSorting is always false with BindingList because there is no API to switch it on and off");
                }
                else
                {
                    LogComment("CanChangeLiveSorting is expected to be True");
                    return TestResult.Fail;
                }
            }

            if (!(bool)_cvs.IsLiveSorting)
            {
                LogComment("LiveSorting should be True but it is not");
                return TestResult.Fail;
            }
            if (ChangeProperty(13, 5, _testProperty))
            {
                if (Helper.CompareViewToSource(_cvs.View, dsvp.Source, _testProperty) == TestResult.Fail) return TestResult.Fail;
                _cvs.View.SortDescriptions.Clear();
                _cvs.LiveSortingProperties.Clear();
                return TestResult.Pass;
            }
            else
            {
                LogComment("Could not change property");
                return TestResult.Fail;
            }

        }
        private TestResult CVSLiveGroupingAndVerify()
        {
            string targetElement;
            if (!_useLiveProperties)
            {
                targetElement = "GroupingPanel";
            }
            else
            {
                targetElement = "GroupingPanelWithPath";
            }
            Panel liveSortingPanel = (Panel)(LogicalTreeHelper.FindLogicalNode(RootElement, targetElement));
            _cvs = (CollectionViewSource)liveSortingPanel.Resources["cvs"];
            _cvs.Source = dsvp.Source;
            _cv = (CollectionView)_cvs.View;

            if (!_cvs.CanChangeLiveGrouping)
            {
                LogComment("CanChangeLiveGrouping should always be true");
                return TestResult.Fail;
            }

            if (!(bool)_cvs.IsLiveGrouping)
            {
                LogComment("LiveGrouping should be True but it is not");
                return TestResult.Fail;
            }
            if (ChangeProperty(6, 21, _testProperty))
            {
                if (Helper.CompareViewToSource(_cvs.View, dsvp.Source, _testProperty) == TestResult.Fail) return TestResult.Fail;
                _cvs.GroupDescriptions.Clear();
                _cvs.LiveGroupingProperties.Clear();
                return TestResult.Pass;
            }
            else
            {
                LogComment("Could not change property");
                return TestResult.Fail;
            }
        }
        private TestResult CVSLiveFilteringAndVerify()
        {
            Panel liveSortingPanel = (Panel)(LogicalTreeHelper.FindLogicalNode(RootElement, "FilteringPanel"));
            _cvs = (CollectionViewSource)liveSortingPanel.Resources["cvs"];
            _cvs.Source = dsvp.Source;
            _cv = (CollectionView)_cvs.View;

            if (_cvs.CanChangeLiveFiltering)
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("BindingListCollectionView shouldn't allow changes to Live Filtering");
                    return TestResult.Fail;
                }
            }
            else
            {
                if (_cvType.Equals("blcv"))
                {
                    LogComment("CanChangeLiveFiltering is always false with BindingList because there is no API to switch it on and off");
                }
                else
                {
                    LogComment("CanChangeLiveFiltering is expected to be True");
                    return TestResult.Fail;
                }
            }


            if (!(bool)_cvs.IsLiveFiltering)
            {
                LogComment("LiveFiltering should be True but it is not");
                return TestResult.Fail;
            }

            if (ChangeProperty(2, 13, _testProperty))
            {
                if (Helper.CompareViewToSource(_cvs.View, dsvp.Source, _testProperty) == TestResult.Fail) return TestResult.Fail;
                if (_cvs.View.CanFilter)
                {
                    _cvs.View.Filter = null;
                }
                _cvs.LiveFilteringProperties.Clear();
                return TestResult.Pass;
            }
            else
            {
                LogComment("Could not change property");
                return TestResult.Fail;
            }
        }
        // Generate our test collection here
        private static ObservableCollection<Order> GenerateOrders()
        {
            ObservableCollection<Order> generatedOrders = new ObservableCollection<Order>();
            for (int i = 0; i < 50; i++)
            {
                generatedOrders.Add(GenerateOrder());
            }
            return generatedOrders;
        }
        private static Order GenerateOrder()
        {
            Order o = new Order();
            o.Number = s_orderCounter;
            s_orderCounter++;
            o.Time = DateTime.Now;
            return o;
        }
        private bool RegisterPropertyPath(string shapingAction, string propertyName)
        {
            if (shapingAction == "sort")
            {
                _icvls.LiveSortingProperties.Add(propertyName);
                return true;
            }
            else if (shapingAction == "filter")
            {
                _icvls.LiveFilteringProperties.Add(propertyName);
                return true;
            }
            else if (shapingAction == "group")
            {
                _icvls.LiveGroupingProperties.Add(propertyName);
                return true;
            }
            else
            {
                LogComment("Unknown shaping action " + shapingAction + " .  Only sort, filter and group are supported");
                return false;
            }
        }
        private bool ChangeProperty(int value, int index, string property)
        {
            //Here we will change value of a property to trigger Live Shaping
            object actualValue;
            IEnumerable<object> iev = _cv.Cast<object>();
            if (index >= _cv.Count)
            {
                LogComment("Index is not valid for current collection view");
                return false;
            }
            actualValue = Helper.GetPropertyAsObject(_cv.GetType(), iev.ElementAt(index), _testProperty);
            if (actualValue == null)
            {
                return false;
            }
            LogComment("Instead of value: " + actualValue.ToString() + " for property " + property);
            LogComment("Writing new value: " + value.ToString() + " at index: " + index.ToString());

            actualValue = Helper.SetPropertyAsObject(_cv.GetType(), iev.ElementAt(index), _testProperty, value);
            if (actualValue == null)
            {
                return false;
            }
            LogComment("Value: " + actualValue.ToString() + " is written");
            return true;
        }
        private void SortCV(string propertyName, ListSortDirection direction)
        {
            if (dsvp.View.CanSort)
            {
                dsvp.View.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }
        private void GroupCV(string propertyName)
        {
            if (dsvp.View.CanGroup)
            {
                dsvp.View.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
            }
        }
        private void FilterCV()
        {
            if (dsvp.View.CanFilter)
            {
                dsvp.View.Filter = CustomerFilter;
            }
        }
        private bool CustomerFilter(object item)
        {
            Order customer = item as Order;
            return (customer.Number > _filterValue);
        }
    }
}

