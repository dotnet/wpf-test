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
using System.Xml;
using System.Collections.Generic;
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
    /// expected. Uses a composite collection with 2 collections and a ListBoxItem.
	/// </description>
    /// <relatedBugs>





    /// </relatedBugs>
	/// </summary>

    [Test(2, "Views", "SelectionAndCurrencyComposite")]
	public class SelectionAndCurrencyComposite : XamlTest
	{
        private ListBox _lb;
        private ReadOnlyObservableCollection<XmlNode> _firstCollection;
        private ObservableCollection<string> _secondCollection;
        private ListBoxItem _lbi;

        private SelectionMode _selectionMode;
        private bool _isSynchronizedWithCurrentItem;

        private SelectedItemsVerifier _siv;
        private CurrentItemVerifier _civFlatView;
        private CurrentItemVerifier _civFirstCollection;
        private CurrentItemVerifier _civSecondCollection;

        private int _firstIndex;
        private int _validIndexFirstCollectionCurrency;
        private int _validIndexSecondCollectionCurrency;
        private int _validIndexFirstCollectionSingleSelection;
        private int _validIndexSecondCollectionSingleSelection;
        private int _validIndexFirstCollectionMultipleSelection;
        private int _validIndexSecondCollectionMultipleSelection;

        // expected results arrays
        // currency
        private object[] _moveLastResults;
        private object[] _moveNextResults;
        private object[] _moveFirstResults;
        private object[] _movePrevResults;
        private object[] _moveToPosResults;
        private object[] _moveToResults;
        private object[] _moveToListBoxItemResults;
        // single selection
        private object[] _selectedIndexResults;
        private object[] _selectedItemResults;
        private object[] _selectListBoxItemResults;
        // multiple selection
        private object[] _selectedItemsResults;

        [Variation("Single", "False")]
        [Variation("Single", "True")]
        [Variation("Multiple", "False")]
        [Variation("Multiple", "True")]
        public SelectionAndCurrencyComposite(string sm, string iswci)
            : base(@"SelectionAndCurrencyComposite.xaml")
        {
            _selectionMode = GetSelectionMode(sm);
            _isSynchronizedWithCurrentItem = GetBooleanFromString(iswci);

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(InitializeExpectedResults);
            RunSteps += new TestStep(MoveCurrency);
            RunSteps += new TestStep(ChangeSelection);
        }

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.SystemIdle);

            _lb = Util.FindElement(RootElement, "lb") as ListBox;
            XmlDataProvider xds = RootElement.Resources["xds"] as XmlDataProvider;
            ObjectDataProvider ods = RootElement.Resources["ods"] as ObjectDataProvider;
            _lbi = Util.FindElement(RootElement, "lbi") as ListBoxItem;

            if (_lb == null)
            {
                LogComment("Fail - Unable to reference the ListBox lb");
                return TestResult.Fail;
            }
            if (xds == null)
            {
                LogComment("Fail - Unable to reference XmlDataSource");
                return TestResult.Fail;
            }
            if (ods == null)
            {
                LogComment("Fail - Unable to reference the ObjectDataSource");
                return TestResult.Fail;
            }
            if (_lbi == null)
            {
                LogComment("Fail - Unable to reference the ListBoxItem lbi");
                return TestResult.Fail;
            }

            _lb.SelectionMode = _selectionMode;
            _lb.IsSynchronizedWithCurrentItem = _isSynchronizedWithCurrentItem;

            WaitForPriority(DispatcherPriority.SystemIdle);

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && ((xds.Data == null) || (ods.Data == null)))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }
            _firstCollection = xds.Data as ReadOnlyObservableCollection<XmlNode>;
            _secondCollection = ods.Data as ObservableCollection<string>;
            _firstIndex = 0;
            _validIndexFirstCollectionCurrency = 3;
            _validIndexSecondCollectionCurrency = 2;
            _validIndexFirstCollectionSingleSelection = 1;
            _validIndexSecondCollectionSingleSelection = 7;
            _validIndexFirstCollectionMultipleSelection = 4;
            _validIndexSecondCollectionMultipleSelection = 5;

            _siv = new SelectedItemsVerifier(_lb);
            // Needed for slower machines (like VMs with low memory).  Just give everything 5 seconds or so to settle out.
            retryCount = 0;
            while (retryCount < 5 && ((_lb.Items.Count == 0) || (_lb.Items.IsEmpty) || 
                (CollectionViewSource.GetDefaultView(_firstCollection) == null) || (CollectionViewSource.GetDefaultView(_secondCollection) == null)))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }
            _civFlatView = new CurrentItemVerifier(_lb.Items);
            _civFirstCollection = new CurrentItemVerifier(CollectionViewSource.GetDefaultView(_firstCollection));
            _civSecondCollection = new CurrentItemVerifier(CollectionViewSource.GetDefaultView(_secondCollection));

            Status("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult InitializeExpectedResults()
        {
            if (!_isSynchronizedWithCurrentItem) // Table E
            {
                // currency
                _moveLastResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveNextResults = new object[] { null, _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveFirstResults = new object[] { _firstCollection[_firstIndex], _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _movePrevResults = new object[] {null, _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveToPosResults = new object[] { _firstCollection[_validIndexFirstCollectionCurrency], _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveToResults = new object[] { _secondCollection[_validIndexSecondCollectionCurrency], _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveToListBoxItemResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                // single selection
                _selectedIndexResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], _firstCollection[_validIndexFirstCollectionSingleSelection] };
                _selectedItemResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], _secondCollection[_validIndexSecondCollectionSingleSelection] };
                _selectListBoxItemResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], _lbi };
                // multiple selection
                List<object> selectedItems = new List<object>();
                selectedItems.Add(_firstCollection[_validIndexFirstCollectionMultipleSelection]);
                selectedItems.Add(_secondCollection[_validIndexSecondCollectionMultipleSelection]);
                _selectedItemsResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], selectedItems };
            }
            else // Table F
            {
                // currency
                _moveLastResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], _lbi };
                _moveNextResults = new object[] { null, _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveFirstResults = new object[] { _firstCollection[_firstIndex], _firstCollection[_firstIndex], _secondCollection[_firstIndex], _firstCollection[_firstIndex] };
                _movePrevResults = new object[] { null, _firstCollection[_firstIndex], _secondCollection[_firstIndex], null };
                _moveToPosResults = new object[] { _firstCollection[_validIndexFirstCollectionCurrency], _firstCollection[_firstIndex], _secondCollection[_firstIndex], _firstCollection[_validIndexFirstCollectionCurrency] };
                _moveToResults = new object[] { _secondCollection[_validIndexSecondCollectionCurrency], _firstCollection[_firstIndex], _secondCollection[_firstIndex], _secondCollection[_validIndexSecondCollectionCurrency] };
                _moveToListBoxItemResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], _lbi };
                // single selection
                _selectedIndexResults = new object[] { _firstCollection[_validIndexFirstCollectionSingleSelection], _firstCollection[_firstIndex], _secondCollection[_firstIndex], _firstCollection[_validIndexFirstCollectionSingleSelection] };
                _selectedItemResults = new object[] { _secondCollection[_validIndexSecondCollectionSingleSelection], _firstCollection[_firstIndex], _secondCollection[_firstIndex], _secondCollection[_validIndexSecondCollectionSingleSelection] };
                _selectListBoxItemResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], _lbi };
                // multiple selection
                List<object> selectedItems = new List<object>();
                selectedItems.Add(_firstCollection[_validIndexFirstCollectionMultipleSelection]);
                selectedItems.Add(_secondCollection[_validIndexSecondCollectionMultipleSelection]);
                selectedItems.Add(_lbi);
                _selectedItemsResults = new object[] { _lbi, _firstCollection[_firstIndex], _secondCollection[_firstIndex], selectedItems };
            }
            return TestResult.Pass;
        }

        private TestResult MoveCurrency()
        {
            Status("MoveCurrency");

            _lb.Items.MoveCurrentToLast();
            if (CheckCurrencyAndSelection(_moveLastResults) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToNext();
            if (CheckCurrencyAndSelection(_moveNextResults) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToFirst();
            if (CheckCurrencyAndSelection(_moveFirstResults) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToPrevious();
            if (CheckCurrencyAndSelection(_movePrevResults) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentToPosition(_validIndexFirstCollectionCurrency);
            if (CheckCurrencyAndSelection(_moveToPosResults) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentTo(_secondCollection[_validIndexSecondCollectionCurrency]);
            if (CheckCurrencyAndSelection(_moveToResults) != TestResult.Pass) { return TestResult.Fail; }
            _lb.Items.MoveCurrentTo(_lbi);
            if (CheckCurrencyAndSelection(_moveToListBoxItemResults) != TestResult.Pass) { return TestResult.Fail; }

            Status("MoveCurrency was successful");
            return TestResult.Pass;
        }

        private TestResult ChangeSelection()
        {
            Status("ChangeSelection");

            if(_selectionMode == SelectionMode.Single)
            {
                _lb.SelectedIndex = _validIndexFirstCollectionSingleSelection;
                if (CheckCurrencyAndSelection(_selectedIndexResults) != TestResult.Pass) { return TestResult.Fail; }
                _lb.SelectedItem = _secondCollection[_validIndexSecondCollectionSingleSelection];
                if (CheckCurrencyAndSelection(_selectedItemResults) != TestResult.Pass) { return TestResult.Fail; }
                _lb.SelectedItem = _lbi;
                if (CheckCurrencyAndSelection(_selectListBoxItemResults) != TestResult.Pass) { return TestResult.Fail; }
            }
            else // SelectionMode.Multiple
            {
                _lb.SelectedItems.Add(_lb.Items[_validIndexFirstCollectionMultipleSelection]);
                _lb.SelectedItems.Add(_secondCollection[_validIndexSecondCollectionMultipleSelection]);
                if (CheckCurrencyAndSelection(_selectedItemsResults) != TestResult.Pass) { return TestResult.Fail; }
            }

            Status("ChangeSelection was successful");
            return TestResult.Pass;
        }

        private TestResult CheckCurrencyAndSelection(object[] results)
        {
            if (CheckCurrency(results[0], results[1], results[2]) != TestResult.Pass) { return TestResult.Fail; }
            if (CheckSelection(results[3]) != TestResult.Pass) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        private TestResult CheckCurrency(object currencyFlatView, object currencyFirstCollection, object currencySecondCollection)
        {
            VerifyResult resultFlatView = _civFlatView.Verify(currencyFlatView) as VerifyResult;
            if(resultFlatView.Result != TestResult.Pass)
            {
                LogComment(resultFlatView.Message + " (flat view)");
                return resultFlatView.Result;
            }

            VerifyResult resultFirstCollection = _civFirstCollection.Verify(currencyFirstCollection) as VerifyResult;
            if (resultFirstCollection.Result != TestResult.Pass)
            {
                LogComment(resultFirstCollection.Message + " (first collection)");
                return resultFirstCollection.Result;
            }

            VerifyResult resultSecondCollection = _civSecondCollection.Verify(currencySecondCollection) as VerifyResult;
            if (resultSecondCollection.Result != TestResult.Pass)
            {
                LogComment(resultSecondCollection.Message + " (second collection)");
                return resultSecondCollection.Result;
            }

            return TestResult.Pass;
        }

        private TestResult CheckSelection(object expectedSelectedItem)
        {
            VerifyResult resultSelection = _siv.Verify(expectedSelectedItem) as VerifyResult;
            if (resultSelection.Result != TestResult.Pass)
            {
                LogComment(resultSelection.Message);
                return resultSelection.Result;
            }
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

