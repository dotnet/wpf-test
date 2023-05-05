// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Coverage for Regression Test. This test has a list box bound to a collection
    /// with a DataTrigger inside the DataTemplate for each item. Before the bug fix, filtering/sorting/grouping
    /// would cause a NullReferenceException. This tests that applying filtering to this scenario works as
    /// expected.
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(2, "Styles", "ChangeCollectionDataTrigger")]
	public class ChangeCollectionDataTrigger : XamlTest
	{
        private ListBox _lb;
        private Places _places;

        public ChangeCollectionDataTrigger()
            : base(@"ChangeCollectionDataTrigger.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyItems);
            RunSteps += new TestStep(SortAndVerifyItems);
        }

        private TestResult Setup()
        {
            Status("Setup");
            _lb = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb"));
            _places = (Places)(this.RootElement.Resources["places"]);

            return TestResult.Pass;
        }

        private TestResult VerifyItems()
        {
            Status("VerifyItems");

            Place[] expectedItems = new Place[_places.Count];
            for (int i = 0; i < _places.Count; i++)
            {
                expectedItems[i] = _places[i];
            }

            if (!CompareExpectedItems(expectedItems))
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult SortAndVerifyItems()
        {
            Status("SortAndVerifyItems");

            _lb.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            Place[] expectedItems = new Place[_places.Count];
            expectedItems[0] = _places[2]; // Bellevue
            expectedItems[1] = _places[10]; // Bellingham
            expectedItems[2] = _places[3]; // Kirkland
            expectedItems[3] = _places[6]; // Los Angeles
            expectedItems[4] = _places[4]; // Portland
            expectedItems[5] = _places[1]; // Redmond
            expectedItems[6] = _places[7]; // San Diego
            expectedItems[7] = _places[5]; // San Francisco
            expectedItems[8] = _places[8]; // San Jose
            expectedItems[9] = _places[9]; // Santa Ana
            expectedItems[10] = _places[0]; // Seattle

            if (!CompareExpectedItems(expectedItems))
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private bool CompareExpectedItems(Place[] expectedPlaces)
        {
            int expectedCount = expectedPlaces.Length;
            int actualCount = _lb.Items.Count;

            if (expectedCount != actualCount)
            {
                LogComment("Fail - Expected item count: " + expectedCount + ". Actual: " + actualCount);
                return false;
            }

            for (int i = 0; i < actualCount; i++)
            {
                if (expectedPlaces[i] != _lb.Items[i])
                {
                    LogComment("Fail - Expected item: " + ((Place)(expectedPlaces[i])).Name + ". Actual: " + ((Place)(_lb.Items[i])).Name);
                    return false;
                }
            }
            return true;
        }
    }
}

