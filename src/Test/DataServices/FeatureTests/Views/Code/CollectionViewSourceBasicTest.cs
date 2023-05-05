// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This tests basic sorting, grouping and filtering on CollectionViewSource.
	/// </description>
	/// </summary>
    [Test(0, "Views", "CollectionViewSourceBasicTest")]
    public class CollectionViewSourceBasicTest : XamlTest
    {
        CollectionViewSource _cvs;
        ListBox _lb;

        public CollectionViewSourceBasicTest()
            : base(@"CollectionViewSourceBasicTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifySortingAndGrouping);
            RunSteps += new TestStep(AddAndVerifyFilter);
            RunSteps += new TestStep(RemoveAndVerifySort);
            RunSteps += new TestStep(RemoveAndVerifyGrouping);
            RunSteps += new TestStep(RemoveAndVerifyFilter);
        }

        TestResult Setup()
        {
            DockPanel dp = (DockPanel)(this.Window.Content);
            _cvs = (CollectionViewSource)(dp.Resources["cvs"]);

            _lb = (ListBox)(dp.FindName("lb"));

            return TestResult.Pass;
        }

        TestResult VerifySortingAndGrouping()
        {
            List<Place> expectedPlaces = new List<Place>();
            expectedPlaces.Add(new Place("Bellevue", "WA"));
            expectedPlaces.Add(new Place("Bellingham", "WA"));
            expectedPlaces.Add(new Place("Kirkland", "WA"));
            expectedPlaces.Add(new Place("Redmond", "WA"));
            expectedPlaces.Add(new Place("Seattle", "WA"));
            expectedPlaces.Add(new Place("Los Angeles", "CA"));
            expectedPlaces.Add(new Place("San Diego", "CA"));
            expectedPlaces.Add(new Place("San Francisco", "CA"));
            expectedPlaces.Add(new Place("San Jose", "CA"));
            expectedPlaces.Add(new Place("Santa Ana", "CA"));
            expectedPlaces.Add(new Place("Portland", "OR"));
            if (!VerifyPlaces(expectedPlaces)) { return TestResult.Fail; }

            int expectedGroupCount = 3;
            List<string> expectedGroupNames = new List<string>();
            expectedGroupNames.Add("WA");
            expectedGroupNames.Add("CA");
            expectedGroupNames.Add("OR");
            if (!VerifyGroups(expectedGroupCount, expectedGroupNames)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult AddAndVerifyFilter()
        {
            _cvs.Filter += new FilterEventHandler(filterCallbackHandler);

            List<Place> expectedPlaces = new List<Place>();
            expectedPlaces.Add(new Place("Bellevue", "WA"));
            expectedPlaces.Add(new Place("Bellingham", "WA"));
            expectedPlaces.Add(new Place("Kirkland", "WA"));
            expectedPlaces.Add(new Place("Redmond", "WA"));
            expectedPlaces.Add(new Place("Los Angeles", "CA"));
            expectedPlaces.Add(new Place("Portland", "OR"));
            if (!VerifyPlaces(expectedPlaces)) { return TestResult.Fail; }

            int expectedGroupCount = 3;
            List<string> expectedGroupNames = new List<string>();
            expectedGroupNames.Add("WA");
            expectedGroupNames.Add("CA");
            expectedGroupNames.Add("OR");
            if (!VerifyGroups(expectedGroupCount, expectedGroupNames)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult RemoveAndVerifySort()
        {
            _cvs.SortDescriptions.Clear();

            List<Place> expectedPlaces = new List<Place>();
            expectedPlaces.Add(new Place("Redmond", "WA"));
            expectedPlaces.Add(new Place("Bellevue", "WA"));
            expectedPlaces.Add(new Place("Kirkland", "WA"));
            expectedPlaces.Add(new Place("Bellingham", "WA"));
            expectedPlaces.Add(new Place("Portland", "OR"));
            expectedPlaces.Add(new Place("Los Angeles", "CA"));
            if (!VerifyPlaces(expectedPlaces)) { return TestResult.Fail; }

            int expectedGroupCount = 3;
            List<string> expectedGroupNames = new List<string>();
            expectedGroupNames.Add("WA");
            expectedGroupNames.Add("OR");
            expectedGroupNames.Add("CA");
            if (!VerifyGroups(expectedGroupCount, expectedGroupNames)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult RemoveAndVerifyGrouping()
        {
            _cvs.GroupDescriptions.Clear();

            List<Place> expectedPlaces = new List<Place>();
            expectedPlaces.Add(new Place("Redmond", "WA"));
            expectedPlaces.Add(new Place("Bellevue", "WA"));
            expectedPlaces.Add(new Place("Kirkland", "WA"));
            expectedPlaces.Add(new Place("Portland", "OR"));
            expectedPlaces.Add(new Place("Los Angeles", "CA"));
            expectedPlaces.Add(new Place("Bellingham", "WA"));
            if (!VerifyPlaces(expectedPlaces)) { return TestResult.Fail; }

            int expectedGroupCount = 0;
            List<string> expectedGroupNames = new List<string>();
            if (!VerifyGroups(expectedGroupCount, expectedGroupNames)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult RemoveAndVerifyFilter()
        {
            _cvs.Filter -= new FilterEventHandler(filterCallbackHandler);

            List<Place> expectedPlaces = new List<Place>();
            expectedPlaces.Add(new Place("Seattle", "WA"));
            expectedPlaces.Add(new Place("Redmond", "WA"));
            expectedPlaces.Add(new Place("Bellevue", "WA"));
            expectedPlaces.Add(new Place("Kirkland", "WA"));
            expectedPlaces.Add(new Place("Portland", "OR"));
            expectedPlaces.Add(new Place("San Francisco", "CA"));
            expectedPlaces.Add(new Place("Los Angeles", "CA"));
            expectedPlaces.Add(new Place("San Diego", "CA"));
            expectedPlaces.Add(new Place("San Jose", "CA"));
            expectedPlaces.Add(new Place("Santa Ana", "CA"));
            expectedPlaces.Add(new Place("Bellingham", "WA"));
            if (!VerifyPlaces(expectedPlaces)) { return TestResult.Fail; }

            int expectedGroupCount = 0;
            List<string> expectedGroupNames = new List<string>();
            if (!VerifyGroups(expectedGroupCount, expectedGroupNames)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        #region AuxMethods
        bool filterCallback(object item)
        {
            Place place = (Place)item;
            return !(place.Name.StartsWith("S"));
        }

        void filterCallbackHandler(object sender, FilterEventArgs e)
        {
            e.Accepted = filterCallback(e.Item);
        }

        bool VerifyPlaces(List<Place> expectedPlaces)
        {
            ListCollectionView actualPlacesView = (ListCollectionView)(_lb.ItemsSource);
            if (actualPlacesView.Count != expectedPlaces.Count)
            {
                LogComment("Fail - Expected count of Places: " + expectedPlaces.Count + ". Actual: " + actualPlacesView.Count);
                return false;
            }
            int count = actualPlacesView.Count;

            for (int i = 0; i < count; i++)
            {
                if (expectedPlaces[i].Name != ((Place)(actualPlacesView.GetItemAt(i))).Name)
                {
                    LogComment("Fail - Expected Place: " + expectedPlaces[i].Name + ". Actual: " + ((Place)(actualPlacesView.GetItemAt(i))).Name);
                    return false;
                }
            }

            return true;
        }

        bool VerifyGroups(int expectedGroupCount, List<string> expectedGroupNames)
        {
            ListCollectionView actualPlacesView = (ListCollectionView)(_lb.ItemsSource);
            if ((actualPlacesView.Groups == null) && (expectedGroupCount == 0)) { return true; }

            if(actualPlacesView.Groups.Count != expectedGroupCount)
            {
                LogComment("Fail - Expected group count: " + expectedGroupCount + ". Actual: " + actualPlacesView.Groups.Count);
                return false;
            }
            int count = actualPlacesView.Groups.Count;
            for(int i=0; i<count; i++)
            {
                string actualGroupName = ((CollectionViewGroup)(actualPlacesView.Groups[i])).Name.ToString();
                string expectedGroupName = expectedGroupNames[i];
                if (actualGroupName != expectedGroupName)
                {
                    LogComment("Fail - Expected Group: " + expectedGroupName + ". Actual: " + actualGroupName);
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
