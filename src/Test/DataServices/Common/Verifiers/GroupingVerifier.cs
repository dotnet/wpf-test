// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    public class GroupingVerifier : IVerifier
    {
        // param 0: ExpectedGroup[] expectedGroups 
        // param 1: ReadOnlyObservableCollection<object> actualgroups
        public IVerifyResult Verify(params object[] expectedState)
        {
            ExpectedGroup[] expectedGroups = (ExpectedGroup[])(expectedState[0]);
            ReadOnlyObservableCollection<object> actualGroups = (ReadOnlyObservableCollection<object>)(expectedState[1]);

            return VerifyGrouping(expectedGroups, actualGroups);
        }

        private IVerifyResult VerifyGrouping(object[] expectedGroups, ReadOnlyObservableCollection<object> actualGroups)
        {
            if ((expectedGroups == null) && (actualGroups == null))
            {
                return new VerifyResult(TestResult.Pass, "Groups are correct");
            }

            int expectedCount = expectedGroups.Length;
            int actualCount = actualGroups.Count;

            if (expectedCount != actualCount)
            {
                return new VerifyResult(TestResult.Fail, "Fail - There should be " + expectedCount + " groups or data items, instead there are " + actualCount);
            }

            for (int i = 0; i < expectedCount; i++)
            {
                // not leaf node
                if ((expectedGroups[i] is ExpectedGroup) && (actualGroups[i] is CollectionViewGroup))
                {
                    ExpectedGroup expectedGroup = (ExpectedGroup)(expectedGroups[i]);
                    CollectionViewGroup actualGroup = (CollectionViewGroup)(actualGroups[i]);

                    if (!(Object.Equals(expectedGroup.Name, actualGroup.Name)))
                    {
                        return new VerifyResult(TestResult.Fail, "Fail - Expected group:" + expectedGroup.Name + ". Actual:" + actualGroup.Name + ".");
                    }
                    VerifyResult result = (VerifyResult)(VerifyGrouping(expectedGroup.Items, actualGroup.Items));
                    if (result.Result != TestResult.Pass)
                    {
                        return result;
                    }
                }
                // leaf node
                else if (!(expectedGroups[i] is ExpectedGroup) && !(actualGroups[i] is CollectionViewGroup))
                {
                    object expectedData = expectedGroups[i];
                    object actualData = actualGroups[i];

                    if (expectedData != actualData)
                    {
                        return new VerifyResult(TestResult.Fail, "Fail - Expected leaf node data: " + expectedData + ". Actual: " + actualData);
                    }
                }
                else
                {
                    return new VerifyResult(TestResult.Fail, "Fail - The depth of the grouping hierarchy is not as expected");
                }
            }

            return new VerifyResult(TestResult.Pass, "Groups are correct");
        }
    }

    public class ExpectedGroup
    {
        // Items will container either other ExpectedGroup instances of leaf nodes (which are data items)
        private object _name;
        private object[] _items;

        #region Properties
        public object Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public object[] Items
        {
            get { return _items; }
            set { _items = value; }
        }
        #endregion

        public ExpectedGroup(object name, object[] items)
        {
            this._name = name;
            this._items = items;
        }
    }

}
