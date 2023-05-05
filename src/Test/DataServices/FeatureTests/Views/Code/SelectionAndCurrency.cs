// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
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
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Sets IsSynchronizedWithCurrentItem=true/false,
    /// SelectionMode=Single/Multiple and verifies that CurrentItem and selection are as
    /// expected.
	/// </description>
	/// </summary>

    [Test(1, "Views", "SelectionAndCurrency")]
	public class SelectionAndCurrency : WindowTest
	{
        private ListBox _lb;
        private GreekGods _gg;
        private SelectionMode _selectionMode;
        private bool _isSynchronizedWithCurrentItem;
        private object _previousCurrentItem;
        private object _previousSelectedItem;
        private SelectedItemsVerifier _siv;
        private CurrentItemVerifier _civFlatView;
        private CurrentItemVerifier _civCollection;

        [Variation("Single", "False")]
        [Variation("Single", "True")]
        [Variation("Multiple", "False")]
        [Variation("Multiple", "True")]
        public SelectionAndCurrency(string sm, string iswci)
        {
            _selectionMode = GetSelectionMode(sm);
            _isSynchronizedWithCurrentItem = GetBooleanFromString(iswci);

            InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(MoveCurrency);
            RunSteps += new TestStep(ChangeSelection);
        }

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

            _gg = new GreekGods();
            _lb = new ListBox();
            _lb.ItemsSource = _gg;
            _lb.Name = "lb";
            _lb.SelectionMode = _selectionMode;
            _lb.IsSynchronizedWithCurrentItem = _isSynchronizedWithCurrentItem;

            _siv = new SelectedItemsVerifier(_lb);
            _civFlatView = new CurrentItemVerifier(_lb.Items);
            _civCollection = new CurrentItemVerifier(CollectionViewSource.GetDefaultView(_lb.ItemsSource));

            StackPanel sp = new StackPanel();
            sp.Children.Add(_lb);
            this.Window.Content = sp;

            LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult MoveCurrency()
        {
            Status("MoveCurrency");

            if (checkWhenMovingCurrency(_gg[0]) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToLast();
            if (checkWhenMovingCurrency(_gg[_gg.Count-1]) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToNext();
            if (checkWhenMovingCurrency(null) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToFirst();
            if (checkWhenMovingCurrency(_gg[0]) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToPrevious();
            if (checkWhenMovingCurrency(null) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentTo(_gg[3]);
            if (checkWhenMovingCurrency(_gg[3]) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToPosition(5);
            if (checkWhenMovingCurrency(_gg[5]) != TestResult.Pass) { return TestResult.Fail; }

            LogComment("MoveCurrency was successful");
            return TestResult.Pass;
        }

        private TestResult ChangeSelection()
        {
            Status("ChangeSelection");

            if (_selectionMode == SelectionMode.Single)
            {
                _lb.SelectedIndex = 2;
                if (checkWhenChangingSelection(_gg[2]) != TestResult.Pass) { return TestResult.Fail; }
                _lb.SelectedItem = _gg[4];
                if (checkWhenChangingSelection(_gg[4]) != TestResult.Pass) { return TestResult.Fail; }
            }
            else if(_selectionMode == SelectionMode.Multiple)
            {
                _lb.UnselectAll();

                IList sic = new List<object>();
                sic.Add(_gg[0]);
                sic.Add(_gg[3]);
                sic.Add(_gg[5]);
                _lb.SelectedItems.Add(_lb.Items[0]);
                _lb.SelectedItems.Add(_lb.Items[3]);
                _lb.SelectedItems.Add(_lb.Items[5]);

                if(checkWhenChangingSelection(sic) != TestResult.Pass) { return TestResult.Fail; }
            }

            LogComment("ChangeSelection was successful");
            return TestResult.Pass;
        }

        // Checks currency of flat view, currency of collection and selected item when
        // moving currency
        private TestResult checkWhenMovingCurrency(object expectedCurrentItem)
        {
            VerifyResult resultSelection = null;
            VerifyResult resultCurrency = null;

            // currency of flat view
            resultCurrency = _civFlatView.Verify(expectedCurrentItem) as VerifyResult;
            LogComment(resultCurrency.Message);
            if (resultCurrency.Result != TestResult.Pass) { return resultCurrency.Result; }

            // currency of collection
            resultCurrency = _civCollection.Verify(expectedCurrentItem) as VerifyResult;
            LogComment(resultCurrency.Message);
            if (resultCurrency.Result != TestResult.Pass) { return resultCurrency.Result; }

            // selected item
            //if ((isSynchronizedWithCurrentItem == false) && (isSelectionRequired == false))
            if (_isSynchronizedWithCurrentItem == false)
            {
                resultSelection = _siv.Verify(null) as VerifyResult;
            }
            //else if ((isSynchronizedWithCurrentItem == false) && (isSelectionRequired == true))
            //{
            //    resultSelection = siv.Verify(gg[0]) as VerifyResult;
            //}
            //else if ((isSynchronizedWithCurrentItem == true) && (isSelectionRequired == false))
            else if (_isSynchronizedWithCurrentItem == true)
            {
                resultSelection = _siv.Verify(expectedCurrentItem) as VerifyResult;
            }
            //else // true, true
            //{
            //    if (lb.Items.IsCurrentAfterLast || lb.Items.IsCurrentBeforeFirst)
            //    {
            //        resultSelection = siv.Verify(previousSelectedItem) as VerifyResult;
            //    }
            //    else
            //    {
            //        resultSelection = siv.Verify(expectedCurrentItem) as VerifyResult;
            //    }
            //}

            LogComment(resultSelection.Message);
            _previousSelectedItem = _lb.SelectedItem;
            // this will be useful when invoking checkWhenChangingSelection for the first time
            _previousCurrentItem = _lb.Items.CurrentItem;
            return resultSelection.Result;
        }

        // Checks currency of flat view, currency of collection and selected item
        // when changing selection
        private TestResult checkWhenChangingSelection(object expectedSelectedItemOrItems)
        {
            VerifyResult resultCurrency = null;
            VerifyResult resultSelection = null;
            object expectedCurrentItem;

            if (expectedSelectedItemOrItems is List<object>)
            {
                expectedCurrentItem = _lb.SelectedItems[0];
            }
            else
            {
                expectedCurrentItem = expectedSelectedItemOrItems;
            }

            // selected item or items
            resultSelection = _siv.Verify(expectedSelectedItemOrItems) as VerifyResult;
            LogComment(resultSelection.Message);
            if (resultSelection.Result != TestResult.Pass) { return resultSelection.Result; }

            // currency of flat view and collection
            if (_isSynchronizedWithCurrentItem == true)
            {
                resultCurrency = _civFlatView.Verify(expectedCurrentItem) as VerifyResult;
                LogComment(resultCurrency.Message);
                if (resultCurrency.Result != TestResult.Pass) { return resultCurrency.Result; }

                resultCurrency = _civCollection.Verify(expectedCurrentItem) as VerifyResult;
                LogComment(resultCurrency.Message);
                if (resultCurrency.Result != TestResult.Pass) { return resultCurrency.Result; }
            }
            else
            {
                resultCurrency = _civFlatView.Verify(_previousCurrentItem) as VerifyResult;
                LogComment(resultCurrency.Message);
                if (resultCurrency.Result != TestResult.Pass) { return resultCurrency.Result; }

                resultCurrency = _civCollection.Verify(_previousCurrentItem) as VerifyResult;
                LogComment(resultCurrency.Message);
                if (resultCurrency.Result != TestResult.Pass) { return resultCurrency.Result; }
            }
            // it would be the same to have previousCurrentItem = CollectionViewSource.GetDefaultView(lb.ItemsSource).CurrentItem;
            _previousCurrentItem = _lb.Items.CurrentItem;
            return TestResult.Pass;
        }

        private SelectionMode GetSelectionMode(string sm)
        {
            if(String.Compare(sm, "Single", true) == 0)
            {
                return SelectionMode.Single;
            }
            else if (String.Compare(sm, "Multiple", true) == 0)
            {
                return SelectionMode.Multiple;
            }
            else
            {
                throw new Exception("Incorrect SelectionMode");
            }
        }

        private bool GetBooleanFromString(string iswci)
        {
            if (String.Compare(iswci, "True", true) == 0)
            {
                return true;
            }
            else if (String.Compare(iswci, "False", true) == 0)
            {
                return false;
            }
            else
            {
                throw new Exception("Incorrect boolean value in string");
            }
        }
    }
}

