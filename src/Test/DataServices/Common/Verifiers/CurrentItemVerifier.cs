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
    public class CurrentItemVerifier : IVerifier
    {
        private ICollectionView _coll;

        public CurrentItemVerifier(ICollectionView coll)
        {
            if (coll == null)
            {
                throw new Exception("ICollectionView is null");
            }
            this._coll = coll;
        }

        // pass the current item object
        // null if no currency is expected, either Verify(null) or Verify(expected) where object expected=null;
        public IVerifyResult Verify(params object[] ExpectedState)
        {
            if (ExpectedState == null || ExpectedState[0] == null)
            {
                if (_coll.CurrentItem != null)
                {
                    return new VerifyResult(TestResult.Fail, "Expect current item to be null but it isn't");
                }
            }
            else
            {
                if (_coll.CurrentItem != ExpectedState[0])
                {
                    return new VerifyResult(TestResult.Fail, "Expected current item: " + ExpectedState[0].ToString() + ". Actual: " + _coll.CurrentItem);
                }
            }
            return new VerifyResult(TestResult.Pass, "SelectedItems are as expected");
        }

    }
}

