// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test;
using System.Windows.Data;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    public class CollectionViewOrderVerifier : IVerifier
    {
        private ICollectionView _cv;

        public CollectionViewOrderVerifier(ICollectionView cv)
        {
            this._cv = cv;
        }

        // takes ints as a parameter that tell the order of the items of the collection
        // in the collection view
        public IVerifyResult Verify(params object[] expectedState)
        {
            int[] intList = new int[expectedState.Length];
            for (int i = 0; i < expectedState.Length; i++)
            {
                intList[i] = (int)expectedState[i];
            }
            return VerifyIntList(intList);
        }

        private IVerifyResult VerifyIntList(int[] expectedState)
        {
            IList il = _cv.SourceCollection as IList;
            CollectionView uiccv = _cv as CollectionView;
            if (uiccv.Count != expectedState.Length)
            {
                return new VerifyResult(TestResult.Fail, "ICollectionView Count is incorrect Actual: " + uiccv.Count + " Expected: " + expectedState.Length);
            }
            int i = 0;
            foreach (object currentItem in _cv)
            {
                if (il.IndexOf(currentItem) != expectedState[i++])
                {
                    return new VerifyResult(TestResult.Fail, "CurrentItem is incorrect Actual: " + il.IndexOf(currentItem) + " Expected: " + expectedState[i - 1]);
                }
            }
            return new VerifyResult(TestResult.Pass, "CollectionViewVerifier was successful");
        }
    }   
}


