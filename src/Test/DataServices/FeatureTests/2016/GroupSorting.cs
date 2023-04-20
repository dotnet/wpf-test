// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using System.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{

    /// <summary>
    /// <description>
    /// Group sorting tests - basic tests
    /// </description>
    /// </summary>
    [Test(1, "Grouping", "GroupSorting")]
    public class GroupSorting : GroupingBaseTest
    {
        CustomerList _list;
        CollectionViewSource _cvs;

        public GroupSorting()
            : base(@"GroupSorting.xaml")
        {
            RunSteps += new TestStep(AddToExistingGroup);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(AddNewAgeGroup);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(AddNewStateGroup);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(AddItemSorting);
            RunSteps += new TestStep(VerifyItemOrder);
            RunSteps += new TestStep(EnableLiveGrouping);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(ChangeAgeGroup);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(ChangeStateGroup);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(VerifyItemOrder);
        }

        protected override TestResult Init()
        {
            // hook up the data
            _list = new CustomerList();
            Panel panel = (Panel)Util.FindElement(RootElement, "panel");
            _cvs = (CollectionViewSource)panel.Resources["GroupedCustomers"];
            _cvs.Source = _list;

            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // add an item whose group(s) already exist
        TestResult AddToExistingGroup()
        {
            _list.Add(new Customer("Jean", 28, "Washington"));
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // add an item that creates a new Age group
        TestResult AddNewAgeGroup()
        {
            _list.Add(new Customer("Manuel", 27, "Oregon"));
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // add an item that creates a new State group
        TestResult AddNewStateGroup()
        {
            _list.Add(new Customer("Sheila", 25, "Utah"));
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // sort items by Customer.Name
        TestResult AddItemSorting()
        {
            SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
            _cvs.SortDescriptions.Add(sd);
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // enable live grouping
        TestResult EnableLiveGrouping()
        {
            _cvs.IsLiveGroupingRequested = true;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // change an item's Age;  it should move to a different group
        TestResult ChangeAgeGroup()
        {
            Customer harry = _list[0];
            harry.Age = 31;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // change an item's State;   it should move to a different group
        TestResult ChangeStateGroup()
        {
            Customer harry = _list[0];
            harry.State = "Idaho";
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }
    }


    /// <summary>
    /// <description>
    /// Group sorting tests - changing group descriptions
    /// </description>
    /// </summary>
    [Test(2, "Grouping", "GroupSortingChanges")]
    public class GroupSortingChanges : GroupingBaseTest
    {
        CustomerList _list;
        CollectionViewSource _cvs;

        public GroupSortingChanges()
            : base(@"GroupSorting.xaml")
        {
            RunSteps += new TestStep(ChangeCustomSort);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(ChangeSortDescription);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(AddSortDescription);
            RunSteps += new TestStep(VerifyGroupOrder);
            RunSteps += new TestStep(AddCustomSort);
            RunSteps += new TestStep(VerifyGroupOrder);
        }

        protected override TestResult Init()
        {
            // hook up the data
            _list = new CustomerList();
            Panel panel = (Panel)Util.FindElement(RootElement, "panel");
            _cvs = (CollectionViewSource)panel.Resources["GroupedCustomers"];
            _cvs.Source = _list;

            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // replace CustomSort - should cause refresh
        TestResult ChangeCustomSort()
        {
            _cvs.GroupDescriptions[0].CustomSort = PropertyGroupDescription.CompareNameDescending;
            StateDirection = ListSortDirection.Descending;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // replace SortDescription - should cause refresh
        TestResult ChangeSortDescription()
        {
            _cvs.GroupDescriptions[1].SortDescriptions[0] = new SortDescription("Name", ListSortDirection.Descending);
            AgeDirection = ListSortDirection.Descending;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

        // add a SortDescription - should supersede CustomSort and cause refresh
        TestResult AddSortDescription()
        {
            _cvs.GroupDescriptions[0].SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            StateDirection = ListSortDirection.Ascending;
            WaitForPriority(DispatcherPriority.Render);
            if (_cvs.GroupDescriptions[0].CustomSort != null)
            {
                LogComment("CustomSort was not cleared after adding SortDescription");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // add a CustomSort - should supersede SortDescriptions and cause refresh
        TestResult AddCustomSort()
        {
            _cvs.GroupDescriptions[1].CustomSort = PropertyGroupDescription.CompareNameAscending;
            AgeDirection = ListSortDirection.Ascending;
            WaitForPriority(DispatcherPriority.Render);
            if (_cvs.GroupDescriptions[1].SortDescriptions.Count != 0)
            {
                LogComment("SortDescriptions was not cleared after adding CustomSort");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

    }


    public abstract class GroupingBaseTest : XamlTest
    {
        public GroupingBaseTest (string filename) : base(filename)
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(VerifyGroupOrder);

            AgeDirection = ListSortDirection.Ascending;
            StateDirection = ListSortDirection.Ascending;
        }

        protected abstract TestResult Init();

        protected ListSortDirection AgeDirection { get; set; }
        protected ListSortDirection StateDirection { get; set; }

        protected TestResult VerifyGroupOrder()
        {
             FrameworkElement[] elements = Util.FindElements(RootElement, "groupName");
             string prevStateName = null;
             int prevAge = -1;

             foreach (FrameworkElement fe in elements)
             {
                TextBlock tb = fe as TextBlock;
                int age;

                if (Int32.TryParse(tb.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out age))
                {
                    bool isAgeOrderWrong = (prevAge == -1) ? false :
                        (AgeDirection == ListSortDirection.Ascending) ? (prevAge > age) : (prevAge < age);
                    if (isAgeOrderWrong)
                    {
                        LogComment(String.Format("Groups in wrong order - State: {0}  Prev Age: {1}   Age: {2}", prevStateName, prevAge, age));
                        return TestResult.Fail;
                    }
                    prevAge = age;
                }
                else
                {
                    string state = tb.Text;
                    int compare = (prevStateName == null) ? 1 : String.Compare(prevStateName, state, StringComparison.InvariantCulture);
                    bool isStateOrderWrong = (prevStateName == null) ? false :
                        (StateDirection == ListSortDirection.Ascending) ? (compare > 0) : (compare < 0);
                    if (isStateOrderWrong)
                    {
                        LogComment(String.Format("Groups in wrong order - Prev state: {0} State: {1}", prevStateName, state));
                        return TestResult.Fail;
                    }
                    if (compare != 0)
                    {
                        prevStateName = state;
                        prevAge = -1;
                    }
                }
             }

            return TestResult.Pass;
        }

        protected TestResult VerifyItemOrder()
        {
            FrameworkElement[] stateElements = Util.FindElements(RootElement, "state");
            FrameworkElement[] ageElements = Util.FindElements(RootElement, "age");
            FrameworkElement[] nameElements = Util.FindElements(RootElement, "name");

            if (!(nameElements.Length == ageElements.Length && nameElements.Length == stateElements.Length))
            {
                LogComment(String.Format("Element counts don't match - State: {0}  Age: {1}  Name: {2}", stateElements.Length, ageElements.Length, nameElements.Length));
                return TestResult.Fail;
            }

            for (int i=1; i<nameElements.Length; ++i)
            {
                // compare prev and current (state,age,name) lexicographically
                TextBlock prevStateTB = (TextBlock)stateElements[i-1], stateTB = (TextBlock)stateElements[i-1];
                TextBlock prevAgeTB = (TextBlock)ageElements[i-1], ageTB = (TextBlock)ageElements[i-1];
                TextBlock prevNameTB = (TextBlock)nameElements[i-1], nameTB = (TextBlock)nameElements[i-1];

                int stateComp = String.Compare(prevStateTB.Text, stateTB.Text, StringComparison.InvariantCulture);
                int ageComp = (stateComp == 0) ? (Int32.Parse(prevAgeTB.Text) - Int32.Parse(ageTB.Text)) : -1;
                int nameComp = (ageComp == 0) ? String.Compare(prevNameTB.Text, nameTB.Text, StringComparison.InvariantCulture) : -1;

                if (StateDirection == ListSortDirection.Descending)                 stateComp = -stateComp;
                if (AgeDirection == ListSortDirection.Descending && stateComp == 0) ageComp = -ageComp;

                if (!(stateComp <= 0 && ageComp <= 0 && nameComp <= 0))
                {
                    LogComment(String.Format("Items in wrong order at index {0}:\n  Previous ({1},{2},{3})\n  Current  ({4},{5},{6}",
                        i,
                        prevStateTB.Text, prevAgeTB.Text, prevNameTB.Text,
                        stateTB.Text, ageTB.Text, nameTB.Text));
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

    }
}




