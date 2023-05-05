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
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Scenario 1: the data source has currency set and IsSynchronizedWithCurrentItem=true.
    /// The current item should be selected in the UI
    /// Scenario 2: list box 1 has SelectionMode=Multiple and several items selected
    /// Binding list box 2 to SelectedItems of list box 1
    /// Scenario 3: When selecting a single item in a CVS scenario, selection and currency are 
    /// synchronized by default. After regression bug was fixed, we don't synchronize selection and
    /// currency for the multiple selection scenario. 
    /// Bind a ListBox to a CVS, select various items, verify that currency and selection are not in
    /// sync. 
    /// Scenario 4: In a single selection scenario, remove the current item. The item that comes
    /// right after this one should now be selected.
    /// </description>
    /// <relatedBugs>






    /// </relatedBugs>
	/// </summary>

    [Test(2, "Views", "SelectorScenarios")]
    public class SelectorScenarios : WindowTest
    {
        public SelectorScenarios() 
        {
            RunSteps += new TestStep(SourceHasCurrentItem);
            RunSteps += new TestStep(SourceIsSelectedItems);
            RunSteps += new TestStep(MultipleNotInSync);
            RunSteps += new TestStep(RemoveCurrentSingle);
        }

        #region SourceHasCurrentItem
        // Scenario 1: the data source has currency set and IsSynchronizedWithCurrentItem=true.
        // The current item should be selected in the UI
        private TestResult SourceHasCurrentItem()
        {
            Status("SourceHasCurrentItem");

            GreekGods gg = new GreekGods();
            ListBox lb = new ListBox();
            SelectedItemsVerifier siv = new SelectedItemsVerifier(lb);

            ICollectionView cv = CollectionViewSource.GetDefaultView(gg);
            cv.MoveCurrentToLast();
            lb.ItemsSource = cv;
            lb.Name = "lb";
            lb.IsSynchronizedWithCurrentItem = true;

            StackPanel sp = new StackPanel();
            sp.Children.Add(lb);

            VerifyResult result = siv.Verify(gg[gg.Count - 1]) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
        }
        #endregion

        #region SourceIsSelectedItems
        // Scenario 2: list box 1 has SelectionMode=Multiple and several items selected
        // Binding list box 2 to SelectedItems of list box 1
        private ListBox _lb1;
        private ListBox _lb2;
        private int _countEvents;
        private TestResult SourceIsSelectedItems()
        {
            Status("SourceIsSelectedItems");

            _countEvents = 0;

            GreekGods gg = new GreekGods();
            _lb1 = new ListBox();
            _lb1.Name = "lb1";
            _lb1.ItemsSource = gg;
            _lb1.SelectionMode = SelectionMode.Multiple;

            // select items in list box 1
            _lb1.SelectedItems.Add(gg[0]);
            _lb1.SelectedItems.Add(gg[1]);
            _lb1.SelectedItems.Add(gg[2]);
            _lb1.SelectedItems.Add(gg[3]);

            _lb2 = new ListBox();
            StackPanel sp = new StackPanel();
            sp.Children.Add(_lb1);
            sp.Children.Add(_lb2);

            // register lb1 and lb2
            NameScope.SetNameScope(this.Window, new NameScope());
            this.Window.RegisterName("lb1", _lb1);
            this.Window.RegisterName("lb2", _lb2);

            this.Window.Content = sp;

            // bind list box 2 to selected items of list box 1
            _lb2.AddHandler(Binding.TargetUpdatedEvent, new EventHandler<DataTransferEventArgs>(MyDataTransferHandler));
            Binding b = new Binding("SelectedItems");
            b.ElementName = "lb1";
            b.NotifyOnTargetUpdated = true;
            _lb2.SetBinding(ItemsControl.ItemsSourceProperty, b);
            if (WaitForSignal("event1") != TestResult.Pass)
            {
                LogComment("Fail - Failure in MyDataTransferHandler");
                return TestResult.Fail;
            }

            // change selected items of list box 1, make sure that was reflected in list box 2
            // this doesn't raise a TargetUpdated event because the collection is still the same
            // changing the collection raises collection change events though

            ((INotifyCollectionChanged)_lb2.Items).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ChangedCollectionHandler);
            _lb1.SelectedItems.Remove(gg[2]);
            if (WaitForSignal("event2") != TestResult.Pass)
            {
                LogComment("Fail - Failure in ChangedCollectionHandler");
                return TestResult.Fail;
            }

            Status("SourceIsSelectedItems was successful");
            return TestResult.Pass;
        }

        private void ChangedCollectionHandler(object sender, NotifyCollectionChangedEventArgs nccea)
        {
            _countEvents++;
            SelectedItemsVerifier siv = new SelectedItemsVerifier(_lb1);
            List<object> list = new List<object>();
            foreach (object item in (ItemCollection)sender)
            {
                list.Add(item);
            }
            VerifyResult result = siv.Verify(list) as VerifyResult;
            LogComment(result.Message);
            Signal("event" + _countEvents, result.Result);
        }

        private void MyDataTransferHandler(object sender, DataTransferEventArgs args)
        {
            _countEvents++;
            SelectedItemsVerifier siv = new SelectedItemsVerifier(_lb1);
            List<object> list = new List<object>();
            foreach (object item in ((ListBox)sender).ItemsSource)
            {
                list.Add(item);
            }
            VerifyResult result = siv.Verify(list) as VerifyResult;
            LogComment(result.Message);
            Signal("event" + _countEvents, result.Result);
        }
        #endregion

        #region MultipleNotInSync
        // Scenario 3: Bind a ListBox to a CVS, select various items, verify that currency and selection 
        // are not in sync. 
        private TestResult MultipleNotInSync()
        {
            Status("RemoveCurrent");

            GreekGods gg = new GreekGods();
            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = gg;
            ListBox lb1 = new ListBox();
            lb1.Name = "lb1";
            Binding b = new Binding();
            b.Source = cvs;
            lb1.SetBinding(ItemsControl.ItemsSourceProperty, b);
            lb1.SelectionMode = SelectionMode.Multiple;

            // select multiple
            lb1.SelectedItems.Add(gg[2]);
            lb1.SelectedItems.Add(gg[3]);
            lb1.SelectedItems.Add(gg[4]);

            // current item is still the first item
            string aphrodite = lb1.Items.CurrentItem.ToString();

            if (!(String.Equals(aphrodite, gg[0])))
            {
                LogComment("Fail - CurrentItem is not as expected. Actual: " + aphrodite.ToString() + ". Expected: Aphrodite.");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        #endregion

        #region RemoveCurrentSingle
        // Scenario 4: In a single selection scenario, remove the current item. The item that comes
        // right after this one should now be selected.
        private TestResult RemoveCurrentSingle()
        {
            Status("RemoveCurrentSingle");

            // Item 0 in ListBox is selected and current
            Places places = new Places();
            ListBox lb = new ListBox();
            lb.IsSynchronizedWithCurrentItem = true;
            lb.ItemsSource = places;

            // Remove item 0 (Seattle)
            places.RemoveAt(0);

            // What was item 1 (Redmond) should now be selected and current
            if (lb.SelectedItem != places[0])
            {
                LogComment("Fail - Actual SelectedItem: " + ((Place)(lb.SelectedItem)).Name + ". Expected: " + places[0].Name);
                return TestResult.Fail;
            }

            if (lb.Items.CurrentItem != places[0])
            {
                LogComment("Fail - Actual CurrentItem: " + ((Place)(lb.Items.CurrentItem)).Name + ". Expected: " + places[0].Name);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion
    }
}

