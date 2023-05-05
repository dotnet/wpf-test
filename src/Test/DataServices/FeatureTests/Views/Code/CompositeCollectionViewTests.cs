// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;

using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Diagnostics;

namespace Microsoft.Test.DataServices
{
    #region Data Sources

    public class NumbersCollection : ObservableCollection<int>
    {
        public NumbersCollection()
        {
            Add(1);
            Add(2);
            Add(3);
            Add(4);
            Add(5);
            Add(6);
            Add(7);
        }
    }

    #endregion

#if TESTBUILD_NET_ATLEAST_461
    /// <summary>
    /// Tests the insertion and removal of elements from a ListBox that is backed by a CompositeCollection
    /// and accessed via CompositeCollectionView.
    ///
    /// Added in response to Regression issue where CompositeCollectionView was sending incorrect indices for
    /// underlying collection operations, causing ListBox indices to change incorrectly in response to the new
    /// data.
    /// </summary>
    [Test(2, "Views", "CompositeCollectionViewSelectedIndexTest", Versions="4.6.1+")]
    public class CompositeCollectionViewSelectedIndexTest : XamlTest, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Bound Properties

        public NumbersCollection Numbers { get; set; }

        int _SelectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                _SelectedIndex = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region XAML Helpers

        ListBox lb = null;

        #endregion

        #region Constructor/Setup

        public CompositeCollectionViewSelectedIndexTest()
            : base("CompositeCollectionBackedListBox.xaml")
        {
            Numbers = new NumbersCollection();

            InitializeSteps += new TestStep(Setup);

            RunSteps += new TestStep(SelectTopAddToEnd);
            RunSteps += new TestStep(SelectTopAddToBeginning);
            RunSteps += new TestStep(SelectTopRemoveFromEnd);
            RunSteps += new TestStep(SelectTopRemoveFromBeginning);

            RunSteps += new TestStep(SelectMiddleAddToEnd);
            RunSteps += new TestStep(SelectMiddleAddToBeginning);
            RunSteps += new TestStep(SelectMiddleAddAbove);
            RunSteps += new TestStep(SelectMiddleRemoveAbove);
            RunSteps += new TestStep(SelectMiddleRemoveFromEnd);
            RunSteps += new TestStep(SelectMiddleRemoveFromBeginning);

            RunSteps += new TestStep(SelectEndAddToEnd);
            RunSteps += new TestStep(SelectEndAddToBeginning);
            RunSteps += new TestStep(SelectEndRemoveFromEnd);
            RunSteps += new TestStep(SelectEndRemoveFromBeginning);
        }


        TestResult Setup()
        {
            this.Window.DataContext = this;

            DockPanel dp = (DockPanel)(this.Window.Content);

            if (dp == null)
            {
                LogComment("Could not find DockPanel");
                return TestResult.Fail;
            }

            lb = (ListBox)(dp.FindName("lb"));

            if (lb == null)
            {
                LogComment("Could not find ListBox named \"lb\"");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

        #region Utility

        TestResult SelectManipAndCheck(NotifyCollectionChangedAction action, int initialSelectionIndex, int expectedSelectedIndex, int manipIndex, int manipItem)
        {
            SelectedIndex = initialSelectionIndex;

            LogComment(string.Format("Action: {0}, Init Index: {1}, Expected Index {2}, Manip Index {3}, Manip Item {4}", action.ToString(), initialSelectionIndex, expectedSelectedIndex, manipIndex, manipItem));

            LogComment("\t SelectedIndex Pre-Manip: " + lb.SelectedIndex);
            LogComment("\t SelectedItem Pre-Manip: " + lb.SelectedItem);

            if (action == NotifyCollectionChangedAction.Add)
            {
                Numbers.Insert(manipIndex, manipItem);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                Numbers.RemoveAt(manipIndex);
            }
            else
            {
                LogComment(string.Format("Action {0} not supported!", action.ToString()));
                return TestResult.Fail;
            }

            LogComment("\t SelectedIndex Post-Manip: " + lb.SelectedIndex);
            LogComment("\t SelectedItem Post-Manip: " + lb.SelectedItem);

            if (SelectedIndex != expectedSelectedIndex)
            {
                LogComment("\t SelectedIndex doesn't match expected index: " + expectedSelectedIndex);
                LogComment("\t Data:");

                foreach (var item in Numbers)
                {
                    LogComment("\t\t" + item);
                }

                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

        #region Select Top Tests

        TestResult SelectTopAddToEnd()
        {
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, 0, 0, Numbers.Count, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectTopAddToBeginning()
        {
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, 0, 1, 0, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectTopRemoveFromEnd()
        {
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, 0, 0, Numbers.Count - 1, 0);
        }

        TestResult SelectTopRemoveFromBeginning()
        {
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, 0, -1, 0, 0);
        }

        #endregion

        #region Select Middle Tests

        TestResult SelectMiddleAddToEnd()
        {
            int middle = (Numbers.Count - 1) / 2;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, middle, middle, Numbers.Count, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectMiddleAddToBeginning()
        {
            int middle = (Numbers.Count - 1) / 2;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, middle, middle + 1, 0, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectMiddleAddAbove()
        {
            int middle = (Numbers.Count - 1) / 2;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, middle, middle + 1, middle - 1, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectMiddleRemoveAbove()
        {
            int middle = (Numbers.Count - 1) / 2;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, middle, middle - 1, middle - 1, 0);
        }

        TestResult SelectMiddleRemoveFromEnd()
        {
            int middle = (Numbers.Count - 1) / 2;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, middle, middle, Numbers.Count - 1, 0);
        }

        TestResult SelectMiddleRemoveFromBeginning()
        {
            int middle = (Numbers.Count - 1) / 2;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, middle, middle - 1, 0, 0);
        }

        #endregion

        #region Select End Tests

        TestResult SelectEndAddToEnd()
        {
            int end = Numbers.Count - 1;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, end, end, Numbers.Count, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectEndAddToBeginning()
        {
            int end = Numbers.Count - 1;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Add, end, end + 1, 0, Numbers[Numbers.Count - 1] + 1);
        }

        TestResult SelectEndRemoveFromEnd()
        {
            int end = Numbers.Count - 1;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, end, -1, Numbers.Count - 1, 0);
        }

        TestResult SelectEndRemoveFromBeginning()
        {
            int end = Numbers.Count - 1;
            return SelectManipAndCheck(NotifyCollectionChangedAction.Remove, end, end - 1, 0, 0);
        }

        #endregion
    }
#endif
}
