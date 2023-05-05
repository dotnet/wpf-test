// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
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
    /// Tests cross-thread access to a collection
    /// Test Adding, Removing, Sorting, Filtering and Grouping actions.
    /// Access from IECV, Background and UI Threads.
    /// </description>
    /// </summary>
    [Test(0, "Collections", "CrossThreadCollections",ExecutionGroupingLevel=ExecutionGroupingLevel.FullIsolation)]
    public class CrossThreadCollections : XamlTest
    {
        #region Private Data

        private Type _dataSourceType = null;
        private bool _testContext,_testCallback;
        private int _filterValue;
        private ManualResetEventSlim _addMres,_insertMres,_moveMres,_removeMres,_noneMres;
        private ManualResetEventSlimWrapper _slimWrapper;
        private CollectionView _cv;
        private string _cvType;
        private IEditableCollectionView _iecv;
        private IEditableCollectionViewAddNewItem _iecvana;
        private ListView _lv;

        #endregion

        public static int mresCounter;
        public static ManualResetEventSlim mresHelper;
        public ObservableCollection<Order> orders;
        public DataSourceViewProvider dsvp;
        static Random s_r;
        static int s_orderCounter;

        #region Constructors

        //The variations destinguish between data view providers
        //and how collection can be registered for the cross thread access
        [Variation(typeof(DataViewViewProvider), true, true)]
        [Variation(typeof(DataViewViewProvider), false, true)]
        [Variation(typeof(DataViewViewProvider), true, false)]
        [Variation(typeof(ObservableCollectionViewProvider<Order>), true, true)]
        [Variation(typeof(ObservableCollectionViewProvider<Order>), false, true)]
        [Variation(typeof(ObservableCollectionViewProvider<Order>), true, false)]
        public CrossThreadCollections(Type dataType, bool context, bool callback)
            : base(@"XThreadCollections.xaml")
        {
            _dataSourceType = dataType;
            _testContext = context;
            _testCallback = callback;

            InitializeSteps += new TestStep(Setup);

            if (_testContext && _testCallback)
            {
                RunSteps += new TestStep(ContextAddCallbackWithGroup);
                RunSteps += new TestStep(ContextAddCallbackWithSort);
                RunSteps += new TestStep(ContextAddCallbackWithFilter);
                RunSteps += new TestStep(ContextRemoveCallbackWithSort);
                RunSteps += new TestStep(ContextRemoveCallbackWithGroup);
                RunSteps += new TestStep(ContextRemoveCallbackWithFilter);
            }
            else
            {
                RunSteps += new TestStep(CollectionAddOnBT);
                RunSteps += new TestStep(CollectionRemoveOnBT);
            }
            if ((_testCallback) && (_testContext) && (_dataSourceType == typeof(DataViewViewProvider)))
            {
            }
            else
            {
                RunSteps += new TestStep(ContextAddWithIECVAddNew);
                RunSteps += new TestStep(ContextRemoveWithIECVAddNew);
            }
            RunSteps += new TestStep(ContextAddWithIECVRemove);
            RunSteps += new TestStep(ContextRemoveWithIECVRemove);
        }

        #endregion

        #region Setup

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _filterValue = 30;
            s_orderCounter = 0;
            mresCounter = 0;
            mresHelper = new ManualResetEventSlim();
            s_r = new Random();
            orders = GenerateOrders();
            _slimWrapper = new ManualResetEventSlimWrapper();

            dsvp = DataSourceViewProvider.CreateDataSourceViewProvider(_dataSourceType, orders);

            _cv = (CollectionView)CollectionViewSource.GetDefaultView(dsvp.Source);
            _iecv = dsvp.View as IEditableCollectionView;
            _iecvana = _cv as IEditableCollectionViewAddNewItem;
            _cvType = _cv.GetType().FullName;

            #region Initialize ManualReset events

            //here we setup actions for each of the ManualReset events we'll be using
            _addMres = XThreadHelpers.CreateSignaledWorkerThread((Action)delegate
            {
                lock (dsvp.Source)
                {
                    if (dsvp.Source.Cast<object>().Count() < 125) dsvp.Add(GenerateOrder());
                }
            });

            _insertMres = XThreadHelpers.CreateSignaledWorkerThread((Action)delegate
            {
                lock (dsvp.Source)
                {
                    //

                }
            });

            _moveMres = XThreadHelpers.CreateSignaledWorkerThread((Action)delegate
            {
                lock (dsvp.Source)
                {
                    //

                }
            });

            _removeMres = XThreadHelpers.CreateSignaledWorkerThread((Action)delegate
            {
                lock (dsvp.Source)
                {
                    if (dsvp.Source.Cast<object>().Count() > 10) dsvp.Remove(dsvp.Source.Cast<object>().ElementAt((dsvp.Source.Cast<object>().Count()) - 5));
                }
            });

            _noneMres = XThreadHelpers.CreateSignaledWorkerThread((Action)delegate
            {
                //This one is used to reset context
            });

            #endregion

            XThreadHelpers.CreateSignaledWorkerThread((Action)delegate
            {
                while (true)
                {
                    Thread.Sleep(100);
                }

            }).Set();

            _slimWrapper.SlimObject = _noneMres;

            //Decide on which type of registration is needed for the current Test Variation
            if (_testContext)
            {
                if (_testCallback)
                {
                    BindingOperations.EnableCollectionSynchronization(dsvp.Source, _slimWrapper, XThreadHelpers.SignalingSynchronizationCallback);
                }
                else
                {
                    BindingOperations.EnableCollectionSynchronization(dsvp.Source, dsvp.Source);
                }
            }
            else
            {
                BindingOperations.EnableCollectionSynchronization(dsvp.Source, null, XThreadHelpers.SignalingSynchronizationCallback);
            }

            _lv = (ListView)(LogicalTreeHelper.FindLogicalNode(RootElement, "lv"));
            _lv.ItemsSource = dsvp.Source;

            return TestResult.Pass;
        }
        #endregion Setup

        private TestResult CollectionAddOnBT()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _addMres.Set();
            Thread.Sleep(10);
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult CollectionRemoveOnBT()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _removeMres.Set();
            Thread.Sleep(10);
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextAddCallbackWithSort()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _addMres;
            SortCV("Number", ListSortDirection.Ascending);
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextAddCallbackWithGroup()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _addMres;

            GroupCV("Number");
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextAddCallbackWithFilter()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _addMres;

            FilterCV();
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextRemoveCallbackWithSort()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _removeMres;

            SortCV("Number", ListSortDirection.Descending);
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextRemoveCallbackWithGroup()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _removeMres;

            GroupCV("Number");
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextRemoveCallbackWithFilter()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _removeMres;

            FilterCV();
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;
            mresHelper.Wait();

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextAddWithIECVAddNew()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _addMres;

            IECVAddNew();
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextRemoveWithIECVAddNew()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _removeMres;

            IECVAddNew();
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextAddWithIECVRemove()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _addMres;

            _iecv.Remove(_lv.Items[_lv.Items.Count - 1]);
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
        }

        private TestResult ContextRemoveWithIECVRemove()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Collection items count before is: " + dsvp.Source.Cast<object>().Count());

            _slimWrapper.SlimObject = _removeMres;

            _iecv.Remove(_lv.Items[_lv.Items.Count - 1]);
            Thread.Sleep(10);

            _slimWrapper.SlimObject = _noneMres;

            return Helper.CompareViewToSource(dsvp.View, dsvp.Source, "Number");
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

        private void SortCV(string propertyName, ListSortDirection direction)
        {
            if (dsvp.View.CanSort)
            {
                mresHelper.Reset();
                dsvp.View.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }
        private void GroupCV(string propertyName)
        {
            if (dsvp.View.CanGroup)
            {
                mresHelper.Reset();
                dsvp.View.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
            }
        }
        private void FilterCV()
        {
            if (dsvp.View.CanFilter)
            {
                mresHelper.Reset();
                dsvp.View.Filter = CustomerFilter;
            }
        }
        private bool CustomerFilter(object item)
        {
            Order customer = item as Order;
            return (customer.Number > _filterValue);
        }
        private void IECVAddNew()
        {
            object item = _iecv.AddNew();
            if (_cvType == "System.Windows.Data.BindingListCollectionView")
            {
                ((DataRowView)item)["Number"] = s_r.Next(100);
            }
            else if (_cvType == "System.Windows.Data.ListCollectionView")
            {
                ((Order)(item)).Number = s_r.Next(100);
            }

            _iecv.CommitNew();
            mresHelper.Wait();
        }
    }
    public class XThreadHelpers
    {
        public static ManualResetEventSlim CreateSignaledWorkerThread(Action action)
        {
            ManualResetEventSlim mres = new ManualResetEventSlim();
            Thread t = new Thread(new ThreadStart((Action)delegate
            {
                while (true)
                {
                    mres.Wait();
                    mres.Reset();
                    do
                    {
                        action.Invoke();
                        CrossThreadCollections.mresHelper.Set();
                    }
                    while (Interlocked.Decrement(ref CrossThreadCollections.mresCounter) > 0);

                }
            }));
            t.IsBackground = true;
            t.Start();
            return mres;
        }
        public static void SignalingSynchronizationCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            if (context is ManualResetEventSlimWrapper)
            {
                //Incrementing counter is needed here to make sure rapid .Set() calls don't get ignored
                if ((((ManualResetEventSlimWrapper)context).SlimObject != null) && (Interlocked.Increment(ref CrossThreadCollections.mresCounter) == 1))
                {
                    ((ManualResetEventSlimWrapper)context).SlimObject.Set();
                }
            }

            lock (collection)
            {
                accessMethod();
            }
        }
    }
}

