// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Regression test for a race condition observed in regression bugs.
    /// </description>
    /// </summary>
    [Test(1, "Views", "LiveShapingRace", Versions="4.7.1+")]
    public class LiveShapingRace : XamlTest
    {
        #region Private Data

        private CollectionView _cv;
        private CollectionViewSource _cvs;
        private bool _running = true;

        #endregion

        ObservableCollection<Order> _orders;
        static int s_orderCounter;
        Order _targetOrder;

        #region Constructors

        public LiveShapingRace() : base(@"LiveShaping.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestRace);
        }

        #endregion

        #region Setup

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _orders = GenerateOrders();

            // attach a ListBox to the collection
            Panel panel = (Panel)(LogicalTreeHelper.FindLogicalNode(RootElement, "SortingPanel"));
            _cvs = (CollectionViewSource)panel.Resources["cvs"];
            _cvs.Source = _orders;
            _cv = (CollectionView)_cvs.View;

            return TestResult.Pass;
        }

        #endregion

        #region Test race condition

        // Regression Test : NullReference exception that occurs if a
        // second thread raises a PropertyChanged event on an item that's being
        // added to a live-sorted collection - after the LiveShaping item has
        // bound to the sort property, but before it has been added to the
        // LiveShaping tree.
        //   We can't test this directly without instrumenting the product code
        // with hooks to control the timing.  Instead, we add a new item to the
        // collection, while a second thread raises PropertyChanged events as
        // fast as it can.   Before the fix, this hit the NRE pretty reliably.
        // If we do this many times without hitting the NRE, we can declare
        // the bug to be fixed with high confidence.

        private TestResult TestRace()
        {
            Status("Test race condition");

            // start a timer to periodically add a new item to the collection
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += AddNewItem;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

            // start a worker thread to raise PropertyChanged events on the target order
            Thread t = new Thread(new ThreadStart(RaisePropertyChangedEvents));
            t.Start();

            // repeat the test many times - hopefully enough to
            // catch the race condition if it's broken.
            while (s_orderCounter < 100)
            {
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            // if we get here, the race condition NRE didn't happen.
            // Clean up and declare victory.
            timer.Stop();
            _running = false;
            t.Join();
            return TestResult.Pass;
        }

        // timer callback
        private void AddNewItem(object sender, EventArgs e)
        {
            // create a new order, make it the target, add it to the collection
            Order order = GenerateOrder();
            _targetOrder = order;
            _orders.Add(order);
        }

        // thread procedure - raise PropertyChanged events on the target order
        private void RaisePropertyChangedEvents()
        {
            while (_running)
            {
                Order target = _targetOrder;     // local reference, for thread safety
                if (target != null)
                {
                    target.Number += 1;
                }
            }
        }

        #endregion

        // Generate our test collection here
        private static ObservableCollection<Order> GenerateOrders()
        {
            ObservableCollection<Order> generatedOrders = new ObservableCollection<Order>();
            for (int i = 0; i < 5; i++)
            {
                generatedOrders.Add(GenerateOrder());
            }
            return generatedOrders;
        }

        private static Order GenerateOrder()
        {
            Order o = new Order();
            o.Number = s_orderCounter * 1000000;
            s_orderCounter++;
            o.Time = DateTime.Now;
            return o;
        }
    }
}
