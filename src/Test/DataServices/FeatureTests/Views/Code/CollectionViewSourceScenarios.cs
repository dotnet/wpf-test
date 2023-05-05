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
using System.Collections;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Provides coverage for the following :
	/// Enable set up of filtering on CollectionViewSource using an EventHandler,
    ///  
    /// CollectionViewSource supports InheritanceContext and mentoring 
	/// - In other words, it is now possible to set DataContext to a CVS,
    /// Optimize CollViewProxy and IndexedEnumerable to use Count and IndexOf - In this DCR, code was
    /// added to IndexedEnumerable to improve performance in the scenario where the source is an IEnumerable 
    /// with IndexOf, indexer this[] and Count. In this case, we use reflection to call those methods instead
    /// of enumerating through the items to find that information.
	/// </description>
	/// </summary>
    [Test(1, "Views", "CollectionViewSourceScenarios")]
    public class CollectionViewSourceScenarios : XamlTest
    {
        Places _places;
        PeopleAndPlaces _peopleAndPlaces;
        CollectionViewSource _cvs1;
        CollectionViewSource _cvs2;
        CollectionViewSource _cvs3;
        ListBox _lb1;
        ListBox _lb2;
        ListBox _lb3;
        ListBox _lb4;
        ListBox _lb5;
        ListBox _lb6;

        public CollectionViewSourceScenarios()
            : base(@"CollectionViewSourceScenarios.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyItems);
            RunSteps += new TestStep(VerifyCurrency);
            RunSteps += new TestStep(VerifyInheritanceContext);
            RunSteps += new TestStep(VerifyEnumerable);
        }

        TestResult Setup()
        {
            Status("Setup");

            _places = (Places)(RootElement.Resources["places"]);
            _peopleAndPlaces = (PeopleAndPlaces)(RootElement.Resources["peopleAndPlaces"]);
            _cvs1 = (CollectionViewSource)(RootElement.Resources["cvs1"]);
            _cvs2 = (CollectionViewSource)(RootElement.Resources["cvs2"]);
            _lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb1"));
            _lb2 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb2"));
            _lb3 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb3"));
            _lb4 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb4"));
            _lb5 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb5"));
            _lb6 = (ListBox)(LogicalTreeHelper.FindLogicalNode(RootElement, "lb6"));
            StackPanel sp = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp"));
            _cvs3 = (CollectionViewSource)(sp.DataContext);

            _cvs1.Filter += new FilterEventHandler(cvsFilter);
            _cvs2.Filter += new FilterEventHandler(cvsFilter);
            _cvs2.Filter += new FilterEventHandler(cvsFilter1);

            return TestResult.Pass;
        }

        TestResult VerifyItems()
        {
            Status("VerifyItems");
            Place[] expectedItemsLb1 = new Place[] { _places[0], _places[5], _places[7], _places[8], _places[9] };
            if (!VerifyExpectedItems(expectedItemsLb1, _lb1)) { return TestResult.Fail; }
            if (!VerifyExpectedItems(expectedItemsLb1, _lb2)) { return TestResult.Fail; }
            Place[] expectedItemsLb3 = new Place[] { _places[0], _places[1], _places[2], _places[3], _places[4], _places[5], _places[6], _places[7], _places[8], _places[9], _places[10]};
            if (!VerifyExpectedItems(expectedItemsLb3, _lb3)) { return TestResult.Fail; }
            Place[] expectedItemsLb4 = new Place[] { _places[1], _places[2], _places[3], _places[4], _places[6], _places[10] };
            // notice that when assigning 2 filters to the same CollectionViewSource, the last one wins
            if (!VerifyExpectedItems(expectedItemsLb4, _lb4)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        // Verify that currency of lb1 and lb2 is in sync and that they do not affect currency in lb3 and lb4
        TestResult VerifyCurrency()
        {
            Status("VerifyCurrency");
            _lb1.Items.MoveCurrentToLast();

            if (!VerifyCurrentAndSelectedItem(_places[9], _places[9], _lb1)) { return TestResult.Fail; }
            if (!VerifyCurrentAndSelectedItem(_places[9], _places[9], _lb2)) { return TestResult.Fail; }
            if (!VerifyCurrentAndSelectedItem(_places[0], null, _lb3)) { return TestResult.Fail; }
            if (!VerifyCurrentAndSelectedItem(_places[1], _places[1], _lb4)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult VerifyInheritanceContext()
        {
            Status("VerifyInheritanceContext");
            // Verify items
            if (!VerifyExpectedItems(_peopleAndPlaces.ToArray(), _lb5)) { return TestResult.Fail; }
            if (!VerifyExpectedItems(_peopleAndPlaces.ToArray(), _lb6)) { return TestResult.Fail; }

            // Verify currency
            _lb5.Items.MoveCurrentToLast();
            object lastObject = _peopleAndPlaces[_peopleAndPlaces.Count - 1];
            if (!VerifyCurrentAndSelectedItem(lastObject, lastObject, _lb5)) { return TestResult.Fail; }
            if (!VerifyCurrentAndSelectedItem(lastObject, lastObject, _lb6)) { return TestResult.Fail; }

            // Verify grouping
            string[] expectedGroupNames = new string[] { "Seattle", "Kirkland", "Marisa", "Lilly", "Redmond", "San Jose", "Radu", "Bellingham", "Beatriz", "Vincent", "Michael" };
            if (!VerifyGrouping(expectedGroupNames, _cvs3)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        bool VerifyExpectedItems(object[] expectedItems, ListBox lb)
        {
            int expectedCount = expectedItems.Length;
            int actualCount = lb.Items.Count;

            if (expectedCount != actualCount)
            {
                LogComment("Fail - Expected count: " + expectedCount + ". Actual count: " + actualCount);
                return false;
            }

            for (int i = 0; i < actualCount; i++)
            {
                if (expectedItems[i] != lb.Items[i])
                {
                    LogComment("Fail - Expected item: " + expectedItems[i] + ". Actual item:" + lb.Items[i]);
                    return false;
                }
            }
            return true;
        }

        bool VerifyCurrentAndSelectedItem(object currentItem, object selectedItem, ListBox lb)
        {
            if (lb.Items.CurrentItem != currentItem)
            {
                LogComment("Fail - " + lb.Name + " - Current item not as expected");
                return false;
            }
            if (lb.SelectedItem != selectedItem)
            {
                LogComment("Fail - " + lb.Name + " - Selected item not as expected");
                return false;
            }
            return true;
        }

        bool VerifyGrouping(string[] expectedGroupNames, CollectionViewSource cvs)
        {
            int expectedNumberGroups = expectedGroupNames.Length;
            int actualNumberGroups = cvs.View.Groups.Count;
            if (actualNumberGroups != expectedNumberGroups)
            {
                LogComment("Fail - There should be " + expectedNumberGroups + " groups, instead there are " + actualNumberGroups);
                return false;
            }

            ReadOnlyObservableCollection<object> groups = cvs.View.Groups;
            for (int i = 0; i < expectedGroupNames.Length; i++)
            {
                if ((string)(((CollectionViewGroup)(groups[i])).Name) != expectedGroupNames[i])
                {
                    LogComment("Fail - Group name should be " + expectedGroupNames[i] + ", instead it is " + ((CollectionViewGroup)(groups[i])).Name);
                    return false;
                }
            }
            return true;
        }

        void cvsFilter(object sender, FilterEventArgs e)
        {
            Place placeSender = (Place)(e.Item);
            if (placeSender.Name.StartsWith("S"))
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        void cvsFilter1(object sender, FilterEventArgs e)
        {
            Place placeSender = (Place)(e.Item);
            if (!placeSender.Name.StartsWith("S"))
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        TestResult VerifyEnumerable()
        {
            Status("VerifyEnumerable");

            ListBox lb7 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb7"));
            MyFancyIEnumerable fancyEnumerable = (MyFancyIEnumerable)(this.RootElement.Resources["fancyEnumerable"]);
            
            // Count
            if (lb7.Items.Count != fancyEnumerable.Count)
            {
                LogComment("Fail - Expected count: " + fancyEnumerable.Count + ". Actual: " + lb7.Items.Count);
                return TestResult.Fail;
            }

            // this[int index] getter and IndexOf
            for(int i=0; i<lb7.Items.Count; i++)
            {
                if (lb7.Items[i] != fancyEnumerable[i])
                {
                    LogComment("Fail - Expected item in index " + i + ": " + fancyEnumerable[i] + ". Actual: " + lb7.Items[i]);
                    return TestResult.Fail;
                }
                if (lb7.Items.IndexOf(fancyEnumerable[i]) != i)
                {
                    LogComment("Fail - Expected index of " + fancyEnumerable[i] + ": " + i + ". Actual: " + lb7.Items.IndexOf(fancyEnumerable[i]));
                    return TestResult.Fail;
                }
            }

            // Contains
            if (!(lb7.Items.Contains(fancyEnumerable[0])))
            {
                LogComment("Fail - ListBox should contain item " + fancyEnumerable[0]);
                return TestResult.Fail;
            }
            if (lb7.Items.Contains(null))
            {
                LogComment("Fail - ListBox should not contain null");
                return TestResult.Fail;
            }

            // IsEmpty
            if (lb7.Items.IsEmpty)
            {
                LogComment("Fail - ListBox's Items are empty, they should not be");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
