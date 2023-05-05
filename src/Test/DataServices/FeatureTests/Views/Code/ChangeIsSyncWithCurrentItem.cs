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
    /// Tests changing IsSynchronizedWithCurrentItem from false to true under several 
    /// circunstances.
    /// </description>
    /// </summary>

    [Test(3, "Views", "ChangeIsSyncWithCurrentItem")]
    public class ChangeIsSyncWithCurrentItem : WindowTest
    {
        private string _selection;
        private string _currency;
        private ListBox _lb;
        private GreekGods _gg;
        private int _validIndexSelection1;
        private int _validIndexSelection2;
        private int _validIndexCurrency;
        private string _resultSelection;
        private string _resultCurrency;

        [Variation("Null", "Null", "Null", "Null")]
        [Variation("Null", "ValidItem", "ValidItemCurrency", "ValidItemCurrency")]
        [Variation("Null", "AfterLast", "Null", "Null")]

        // These tests have never worked on 3.5 but may be modifiable to do so
        // TODO: Rewrite for 3.X ?
#if TESTBUILD_CLR40        
        [Variation("ValidItem", "Null", "ValidItem", "ValidItem")]
        [Variation("ValidItem", "ValidItem", "ValidItem", "ValidItem")]
        [Variation("ValidItem", "AfterLast", "ValidItem", "ValidItem")]
        [Variation("Multiple", "Null", "ValidSelectionItem", "ValidItem")]
        [Variation("Multiple", "ValidItem", "ValidSelectionItem", "ValidItem")]
        [Variation("Multiple", "AfterLast", "ValidSelectionItem", "ValidItem")]
#endif
        public ChangeIsSyncWithCurrentItem(string selection, string currency, string resultSelection, string resultCurrency)
        {
            this._selection = selection;
            this._currency = currency;
            this._resultSelection = resultSelection;
            this._resultCurrency = resultCurrency;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(CheckSelection);
            RunSteps += new TestStep(CheckCurrency);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _lb = new ListBox();
            _gg = new GreekGods();
            _lb.ItemsSource = _gg;
            _lb.IsSynchronizedWithCurrentItem = false;
            this.Window.Content = _lb;
            _validIndexSelection1 = 3;
            _validIndexSelection2 = 4;
            _validIndexCurrency = 5;

            setSelection();
            setCurrency();

            _lb.IsSynchronizedWithCurrentItem = true;

            Status("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult CheckSelection()
        {
            Status("CheckSelection");
            SelectedItemsVerifier siv = new SelectedItemsVerifier(_lb);
            VerifyResult result = null;

            switch (_resultSelection)
            {
                case "Null":
                    result = siv.Verify(null) as VerifyResult;
                    break;
                case "ValidItem":
                    result = siv.Verify(_gg[_validIndexSelection1]) as VerifyResult;
                    break;
                case "ValidItemCurrency":
                    result = siv.Verify(_gg[_validIndexCurrency]) as VerifyResult;
                    break;
                case "ValidSelectionItem":
                    IList sic = new List<object>();
                    sic.Add(_gg[_validIndexSelection1]);
                    sic.Add(_gg[_validIndexSelection2]);					
                    result = siv.Verify(sic) as VerifyResult;
                    break;
                default:
                    throw new Exception("ResultSelection invalid in variation file!");

            }
            LogComment(result.Message);
            return result.Result;
        }

        private TestResult CheckCurrency()
        {
            Status("CheckCurrency");
            CurrentItemVerifier civ = new CurrentItemVerifier(_lb.Items);
            VerifyResult result = null;

            switch (_resultCurrency)
            {
                case "Null":
                    result = civ.Verify(null) as VerifyResult;
                    break;
                case "ValidItem":
                    result = civ.Verify(_gg[_validIndexSelection1]) as VerifyResult;
                    break;
                case "ValidItemCurrency":
                    result = civ.Verify(_gg[_validIndexCurrency]) as VerifyResult;
                    break;	
                case "ValidSelectionItem":
                    IList sic = new List<object>();
                    sic.Add(_gg[_validIndexSelection1]);
                    sic.Add(_gg[_validIndexSelection2]);					
                    result = civ.Verify(sic) as VerifyResult;
                    break;
                default:
                    throw new Exception("ResultCurrency invalid in variation file!");
            }
            LogComment(result.Message);
            return result.Result;
        }

        private void setSelection()
        {
            switch (_selection)
            {
                case "Null":
                    _lb.UnselectAll();
                    break;
                case "ValidItem":
                    _lb.SelectedItem = _lb.Items[_validIndexSelection1];
                    break;
                case "Multiple":
                    _lb.SelectionMode = SelectionMode.Multiple;
                    _lb.SelectedItems.Add(_lb.Items[_validIndexSelection1]);
                    _lb.SelectedItems.Add(_lb.Items[_validIndexSelection2]);
                    break;
                default:
                    throw new Exception("Invalid selection in variation file!");
            }
        }

        private void setCurrency()
        {
            switch (_currency)
            {
                case "Null":
                    _lb.Items.MoveCurrentTo(null);
                    break;
                case "AfterLast":
                    _lb.Items.MoveCurrentToLast();
                    _lb.Items.MoveCurrentToNext();
                    break;
                case "ValidItem":
                    _lb.Items.MoveCurrentToPosition(_validIndexCurrency);
                    break;
                default:
                    throw new Exception("Invalid currency in variation file!");
            }
        }
    }
}

