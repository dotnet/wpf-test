// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests the events raised by changing SelectedValue.
    /// </description>
    /// </summary>
    [Test(1, "Controls", "SelectedValueEvents", Versions="4.7.1+")]
    public class SelectedValueEvents : XamlTest
    {
        private ListBox _myListBox1;
        private ListBox _myListBox2;
        private int _selectedIndexDuringEvent;
        private object _selectedItemDuringEvent;
        private object _selectedValueDuringEvent;

        public SelectedValueEvents()
            : base(@"SelectedValuePath.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ChangeSelectedIndex_NotBound);
            RunSteps += new TestStep(ChangeSelectedItem_NotBound);
            RunSteps += new TestStep(ChangeSelectedValue_NotBound);
            RunSteps += new TestStep(ChangeSelectedIndex_Bound);
            RunSteps += new TestStep(ChangeSelectedItem_Bound);
            RunSteps += new TestStep(ChangeSelectedValue_Bound);
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            Selector selector = (Selector)sender;
            _selectedIndexDuringEvent = selector.SelectedIndex;
            _selectedItemDuringEvent = selector.SelectedItem;
            _selectedValueDuringEvent = selector.SelectedValue;
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _myListBox1 = Util.FindElement(RootElement, "myListBox1") as ListBox;
            if (_myListBox1 == null)
            {
                LogComment("Fail - Unable to reference myListBox1.");
                return TestResult.Fail;
            }

            _myListBox2 = Util.FindElement(RootElement, "myListBox2") as ListBox;
            if (_myListBox2 == null)
            {
                LogComment("Fail - Unable to reference myListBox2.");
                return TestResult.Fail;
            }

            _myListBox1.SelectionChanged += OnSelectionChanged;
            _myListBox2.SelectionChanged += OnSelectionChanged;

            _myListBox1.SelectedIndex = 0;
            _myListBox2.SelectedIndex = 0;

            Status("Setup was successful");
            return TestResult.Pass;
        }


        private TestResult ChangeSelectedIndex_NotBound()
        {
            Status("ChangeSelectedIndex_NotBound");
            return ChangeSelectedIndex(_myListBox1);
        }

        private TestResult ChangeSelectedIndex_Bound()
        {
            Status("ChangeSelectedIndex_Bound");
            return ChangeSelectedIndex(_myListBox2);
        }

        private TestResult ChangeSelectedIndex(Selector selector)
        {
            WaitForPriority(DispatcherPriority.Render);
            _selectedIndexDuringEvent = -2;

            selector.SelectedIndex = 1;

            WaitForPriority(DispatcherPriority.Background);
            VerifyResult result = Verify(selector, 1);

            LogComment(result.Message);
            return result.Result;
        }


        private TestResult ChangeSelectedItem_NotBound()
        {
            Status("ChangeSelectedItem_NotBound");
            return ChangeSelectedItem(_myListBox1);
        }

        private TestResult ChangeSelectedItem_Bound()
        {
            Status("ChangeSelectedItem_NotBound");
            return ChangeSelectedItem(_myListBox2);
        }

        private TestResult ChangeSelectedItem(Selector selector)
        {
            WaitForPriority(DispatcherPriority.Render);
            _selectedIndexDuringEvent = -2;

            selector.SelectedItem = selector.Items[2];

            WaitForPriority(DispatcherPriority.Background);
            VerifyResult result = Verify(selector, 2);

            LogComment(result.Message);
            return result.Result;
        }


        private TestResult ChangeSelectedValue_NotBound()
        {
            Status("ChangeSelectedItem_NotBound");
            return ChangeSelectedValue(_myListBox1);
        }

        private TestResult ChangeSelectedValue_Bound()
        {
            Status("ChangeSelectedItem_NotBound");
            return ChangeSelectedValue(_myListBox2);
        }

        private TestResult ChangeSelectedValue(Selector selector)
        {
            WaitForPriority(DispatcherPriority.Render);
            _selectedIndexDuringEvent = -2;

            Place targetItem = selector.Items[5] as Place;
            switch (selector.SelectedValuePath as String)
            {
                case "Name":    selector.SelectedValue = targetItem.Name; break;
                case "State":   selector.SelectedValue = targetItem.State; break;
            }

            WaitForPriority(DispatcherPriority.Background);
            VerifyResult result = Verify(selector, 5);

            LogComment(result.Message);
            return result.Result;
        }


        private VerifyResult Verify(Selector selector, int index)
        {
            if (_selectedIndexDuringEvent == -2)
            {
                return new VerifyResult(TestResult.Fail, "Fail - SelectionChanged event not received");
            }

            if (index != selector.SelectedIndex)
            {
                return new VerifyResult(TestResult.Fail, "Fail - Wrong item selected.  Expected: " + index + " Actual: " + selector.SelectedIndex);
            }

            if (selector.SelectedIndex != _selectedIndexDuringEvent)
            {
                return new VerifyResult(TestResult.Fail, "Fail - SelectedIndex incorrect during event.  Expected: " + selector.SelectedIndex + " Actual: " + _selectedIndexDuringEvent);
            }

            if (!Object.Equals(selector.SelectedItem,_selectedItemDuringEvent))
            {
                return new VerifyResult(TestResult.Fail, "Fail - SelectedItem incorrect during event.  Expected: " + selector.SelectedItem + " Actual: " + _selectedItemDuringEvent);
            }

            if (!Object.Equals(selector.SelectedValue, _selectedValueDuringEvent))
            {
                return new VerifyResult(TestResult.Fail, "Fail - SelectedValue incorrect during event.  Expected: " + selector.SelectedValue + " Actual: " + _selectedValueDuringEvent);
            }

            return new VerifyResult(TestResult.Pass, "Pass - all properties correct during event.");
        }
    }
}
