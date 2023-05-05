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
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    public class SelectedValueVerifier : IVerifier
    {
        private Selector _sel;

        public SelectedValueVerifier(Selector sel)
        {
            if (sel == null)
            {
                throw new Exception("Selector is null");
            }
            this._sel = sel;
        }

        // args: int expectedIndex, string expectedSelectedValue, string expectedSelectedValuePath
        public IVerifyResult Verify(params object[] expectedValues)
        {
            int expectedIndex = (int)expectedValues[0];
            string expectedSelectedValue = ((expectedValues[1] != null) ? ((expectedValues[1]).ToString()) : null);
            string expectedSelectedValuePath = ((expectedValues[2] != null) ? ((expectedValues[2]).ToString()) : null);

            // verify SelectedValue is as expected
            string selVal = _sel.SelectedValue as String;
            if (selVal != expectedSelectedValue)
            {
                return new VerifyResult(TestResult.Fail, "Fail - SelectedValue is not correct. Actual: " + _sel.SelectedValue + ", Expected: " + expectedSelectedValue);
            }

            // verify SelectedValuePath is as expected
            string selValPath = _sel.SelectedValuePath as String;
            if (selValPath != expectedSelectedValuePath)
            {
                return new VerifyResult(TestResult.Fail, "Fail - SelectedValuePath is not correct. Actual: " + _sel.SelectedValuePath + ", Expected: " + expectedSelectedValuePath);
            }

            // verify selected index is as expected
            int selIndex = _sel.SelectedIndex;
            if (selIndex != expectedIndex)
            {
                return new VerifyResult(TestResult.Fail, "Fail - Selected index is not correct. Actual: " + _sel.SelectedIndex + ", Expected: " + expectedIndex);
            }

            return new VerifyResult(TestResult.Pass, "SelectedValue, SelectedValuePath and SelectedIndex are as expected");
        }

    }
}

