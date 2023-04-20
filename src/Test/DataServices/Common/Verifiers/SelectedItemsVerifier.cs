// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Data;
using Microsoft.Test.DataServices;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    public class SelectedItemsVerifier : IVerifier
    {
        private ListBox _sel;

        public SelectedItemsVerifier(ListBox sel)
        {
            if (sel == null)
            {
                throw new Exception("ItemsControl is null");
            }
            this._sel = sel;
        }

        // this tests both the SelectedItem and SelectedItems dps
        // parameter: 
        // the selected item object in case of single selection 
        // List<object> of selected items in case of multiple selection
        // null if no selection is expected, either Verify(null) or Verify(expected) where object expected=null;
        public IVerifyResult Verify(params object[] ExpectedState)
        {
            IList actualSelectedItems = _sel.SelectedItems;
            if (actualSelectedItems == null)
            {
                return new VerifyResult(TestResult.Fail, "SelectedItems is null");
            }

            if (ExpectedState == null || ExpectedState[0] == null)
            {
                if (_sel.SelectedItem != null || actualSelectedItems.Count != 0)
                {
                    return new VerifyResult(TestResult.Fail, "Expect nothing to be selected but it is");
                }
            }
            else
            {
                List<object> expectedSelectedItems = ExpectedState[0] as List<object>;
                if (expectedSelectedItems == null)
                {
                    expectedSelectedItems = new List<object>();
                    expectedSelectedItems.Add(ExpectedState[0]);
                }

                // test SelectedItems

                int expectedCount = expectedSelectedItems.Count;
                int actualCount = actualSelectedItems.Count;

                if (expectedCount != actualCount)
                {
                    return new VerifyResult(TestResult.Fail, "Expected count of SelectedItems: " + expectedCount + " Actual: " + actualCount);
                }

                for (int i = 0; i < expectedCount; i++)
                {
                    if (!actualSelectedItems.Contains(expectedSelectedItems[i]))
                    {
                        return new VerifyResult(TestResult.Fail, "The following item is not part of SelectedItems as expected: " + expectedSelectedItems[i]);
                    }
                }

                // test SelectedItem and SelectedItems are in sync
                if ((actualSelectedItems.Count > 0) && (_sel.SelectedItem != actualSelectedItems[0]))
                {
                    return new VerifyResult(TestResult.Fail, "SelectedItem should be the first item of SelectedItems collection");
                }
            }
            return new VerifyResult(TestResult.Pass, "SelectedItems are as expected");
        }

    }
}

