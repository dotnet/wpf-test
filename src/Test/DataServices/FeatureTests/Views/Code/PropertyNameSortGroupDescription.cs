// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.Test;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Provides coverage for Regression Bug - "PropertyName on SortDescription and GroupDescription should support Paths".
    /// This tests passing a string to SortDescription and PropertyGroupDescription with PropertyPath syntax.
	/// </description>
    /// <relatedTasks>

    /// </relatedTasks>
	/// </summary>
    [Test(2, "Views", "PropertyNameSortGroupDescription")]
	public class PropertyNameSortGroupDescription : XamlTest
	{
        private ListBox _lb1;
        private CollectionViewSource _cvs2;
        private MediaCollection _coll;
        
        public PropertyNameSortGroupDescription()
            : base(@"PropertyNameSortGroupDescription.xaml")
        {
            InitializeSteps +=new TestStep(Setup);
            RunSteps += new TestStep(TestSorting);
            RunSteps += new TestStep(TestGrouping);
        }

        TestResult Setup()
        {
            Status("Setup");

            _coll = new MediaCollection();
            _coll.Add(new MediaItem("MediaItem 1", "Publisher of MediaItem 1", "1", 240.3, true, new DateTime(2005, 10, 2)));
            _coll.Add(new MediaItem("MediaItem 2", "Publisher of MediaItem 2", "2", 22, false, new DateTime(2005, 3, 22)));
            _coll.Add(new MediaItem("MediaItem 3", "Publisher of MediaItem 3", "3", 30.99, true, new DateTime(2004, 3, 5)));
            _coll.Add(new MediaItem("MediaItem 4", "Publisher of MediaItem 4", "4", 23.34, false, new DateTime(2005, 10, 9)));
            _coll.Add(new MediaItem("MediaItem 5", "Publisher of MediaItem 5", "5", 99.2, false, new DateTime(2004, 4, 24)));
            _coll.Add(new MediaItem("MediaItem 6", "Publisher of MediaItem 6", "6", 53.64, false, new DateTime(2005, 2, 4)));

            WaitForPriority(DispatcherPriority.Render);            
            _cvs2 = (CollectionViewSource)(this.RootElement.Resources["cvs2"]);
            _cvs2.Source = _coll;

            _lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb1"));

            return TestResult.Pass;
        }

        TestResult TestSorting()
        {
            Status("TestSorting");

            string[] expectedDisplayedItems = new string[] { "West", "Central", "East" };

            for (int i = 0; i < 3; i++)
            {
                ListBoxItem lbi = (ListBoxItem)(_lb1.ItemContainerGenerator.ContainerFromIndex(i));
                TextBlock tb = (TextBlock)(Util.FindVisualByType(typeof(TextBlock), lbi));
                if (tb.Text != expectedDisplayedItems[i])
                {
                    LogComment("Fail - Expected item: " + expectedDisplayedItems[i] + ". Actual: " + tb.Text);
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        TestResult TestGrouping()
        {
            Status("TestGrouping");

            GroupingVerifier groupingVerifier = new GroupingVerifier();

            ExpectedGroup group0 = new ExpectedGroup(2005, new object[] { _coll[0], _coll[1], _coll[3], _coll[5] });
            ExpectedGroup group1 = new ExpectedGroup(2004, new object[] { _coll[2], _coll[4] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1 };
            ReadOnlyObservableCollection<object> actualGroups = _cvs2.View.Groups;

            VerifyResult result = (VerifyResult)(groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
