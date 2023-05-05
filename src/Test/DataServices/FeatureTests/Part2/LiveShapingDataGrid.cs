// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Tests Live Shaping on a DataGrid, especially when NewItemPlaceholder
    /// is at the beginning.
    /// </summary>
    [Test(1, "Views", "LiveShapingDataGrid", Versions="4.8+")]
    public class LiveShapingDataGrid : XamlTest
    {
        NewItemPlaceholderPosition _nipPosition;
        DataGrid _dataGrid;
        CollectionViewSource _cvs;
        ObservableCollection<Order> _orders;
        static DateTime s_date2010 = new DateTime(2010, 1, 1);

        [Variation(NewItemPlaceholderPosition.AtBeginning)]
        [Variation(NewItemPlaceholderPosition.AtEnd)]
        [Variation(NewItemPlaceholderPosition.None)]
        public LiveShapingDataGrid(NewItemPlaceholderPosition nipPosition)
            : base(@"LiveShapingDataGrid.xaml")
        {
            _nipPosition = nipPosition;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        TestResult Setup()
        {
            _dataGrid = (DataGrid)(LogicalTreeHelper.FindLogicalNode(RootElement, "datagrid"));

            Panel panel = (Panel)(LogicalTreeHelper.FindLogicalNode(RootElement, "panel"));
            _cvs = (CollectionViewSource)panel.Resources["cvs"];
            _cvs.Filter += new FilterEventHandler(FilterOrder);

            _orders = GenerateOrders();

            _cvs.Source = _orders;
            IEditableCollectionView view = _cvs.View as IEditableCollectionView;
            view.NewItemPlaceholderPosition = _nipPosition;

            return TestResult.Pass;
        }

        TestResult RunTest()
        {
            // change the sort order of the first item
            _orders[0].Number = 41;
            WaitForPriority(DispatcherPriority.SystemIdle);

            // filter away the 3rd item
            _orders[2].Time = _orders[2].Time + TimeSpan.FromDays(365);
            WaitForPriority(DispatcherPriority.SystemIdle);

            // unfilter the 2nd item
            _orders[1].Time = _orders[1].Time - TimeSpan.FromDays(365);
            WaitForPriority(DispatcherPriority.SystemIdle);

            // if there's no crash, regression is fixed and test passes
            return TestResult.Pass;
        }

        void FilterOrder(object sender, FilterEventArgs e)
        {
            Order o = (Order)e.Item;
            e.Accepted = (o.Time < s_date2010);
        }

        // Generate our test collection here
        private static ObservableCollection<Order> GenerateOrders()
        {
            ObservableCollection<Order> generatedOrders = new ObservableCollection<Order>();
            for (int i = 0; i < 50; i++)
            {
                generatedOrders.Add(GenerateOrder(i));
            }
            return generatedOrders;
        }

        private static Order GenerateOrder(int i)
        {
            Order o = new Order();
            o.Number = i*10;
            int year = (i%2 == 0) ? 2009 : 2010;
            o.Time = new DateTime(year,1,1) + TimeSpan.FromDays(i);
            return o;
        }
    }
}
