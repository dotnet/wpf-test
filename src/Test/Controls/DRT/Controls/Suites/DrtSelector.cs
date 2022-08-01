// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using System.Reflection;

using System.Windows.Automation;
using System.Collections.Generic;

namespace DRT
{

    public class DrtSelectorSuite : DrtTestSuite
    {
        public DrtSelectorSuite() : base("Selector")
        {
            Contact = "Microsoft";
        }

        Border _rootBorder;
        public override DrtTest[] PrepareTests()
        {
            DrtControls.NotUsingDrtWindow(DRT);
            _rootBorder = DRT.RootElement as Border;

            return new DrtTest[]
                {
                    new DrtTest(Start),
                    new DrtTest(TestSelector),
                    new DrtTest(TestListBox),
#if OLD_AUTOMATION
                    new DrtTest(TestAccessibility),
#endif
                    new DrtTest(Cleanup),
                };
        }

        private void Start()
        {
        }

        public void Cleanup()
        {
            Console.WriteLine("{0} tests", _numtests);
        }

        private void TestSelector()
        {
            Console.WriteLine("Testing basic Selector...");
            SelectorTest s = new SelectorTest();
            DRT.Assert(s.SelectedItems.Count == 0,
                "New Selector has selected items");

            s.Items.Add("Test 1");
            s.Items.Add("Test 2");
            s.Items.Add("Test 3");
            s.Items.Add("Test 4");
            s.Items.Add("Test 5");

            s.SelectedIndex = 0;
            Expected(s, "0", 1, 0, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetSelected(3, true);
            Expected(s, "1", 1, 0, 2); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SelectedIndex = 3;
            Expected(s, "3", 1, 3, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SelectedIndex = -1;
            Expected(s, "4", 1, -1, 0); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetSelected(1, true);
            Expected(s, "5a", 1, 1, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */
            s.SetSelected(2, true);
            Expected(s, "5b", 1, 1, 2); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */
            s.SetSelected(3, true);
            Expected(s, "5c", 1, 1, 3); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */
            s.SetSelected(1, false);
            Expected(s, "6", 1, 2, 2); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetSelected(2, false);
            Expected(s, "7a", 1, 3, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetSelected(3, false);
            Expected(s, "7b", 1, -1, 0); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetSelected(2, false);
            Expected(s, "7b", 0, -1, 0); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SelectAll();
            Expected(s, "8", 1, 0, 5); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.UnselectAll();
            Expected(s, "9", 1, -1, 0); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetCanSelectMultiple(false);
            s.SetSelected(1, true);
            Expected(s, "14", 1, 1, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SelectedIndex = 2;
            Expected(s, "16", 1, 2, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SelectedIndex = 3;
            Expected(s, "17", 1, 3, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.SetCanSelectMultiple(true);
            Expected(s, "17b", 0, 3, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            s.UnselectAll();
            Expected(s, "17c", 1, -1, 0); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            Console.WriteLine("Testing Selectable Item pattern...");

            ListBoxItem listItem1, listItem2;

            listItem1 = new ListBoxItem();
            listItem1.Content = "ListBoxItem 1";
            s.Items.Add(listItem1);

            listItem2 = new ListBoxItem();
            listItem2.Content = "ListBoxItem 2";
            Expected(s, "18", 0, -1, 0); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            listItem1.IsSelected = true;
            Expected(s, "listItem.IsSelected = true", 1, 5, 1); /* testnum, delta selection Changes, SelectedIndex, SelectedItems.Count */

            // here we have {1,2,3,4,5, [ListItem1], ListItem2} -> {1,2,3,4,5, ListItem2}
            s.Items.Remove(listItem1);
            Expected(s, "remove selected listitem", 1, -1, 0);

            // Test that the defer load scenario works
            s = new SelectorTest();
            ((System.ComponentModel.ISupportInitialize)s).BeginInit();
            s.SelectedIndex = 1;
            s.Items.Add("one");
            Expected(s, "set SelectedIndex = 1, one item (deferload)", 0, -1, 0);
            s.Items.Add("two");
            ((System.ComponentModel.ISupportInitialize)s).EndInit();
            Expected(s, "two items -- SelectedIndex should be fulfilled (end defer load)", 1, 1, 1);

            // Try to add a bunch of things with CanSelectMultiple = false
            s.Items.Clear();
            Expected(s, "Clear selection - should remove items", 1, -1, 0);
            s.SetCanSelectMultiple(false);

            listItem1 = new ListBoxItem();
            listItem1.Content = "one";
            listItem1.IsSelected = true;
            listItem2 = new ListBoxItem();
            listItem2.Content = "two";
            listItem2.IsSelected = true;
            s.Items.Add(listItem1);
            Expected(s, "add one selected ListBoxItem", 1, 0, 1);

            s.Items.Add(listItem2);
            Expected(s, "add another selected ListBoxItem", 1, 1, 1);
            DRT.Assert(listItem1.IsSelected == false, "Expected listItem1.IsSelected to be false");

            s.Items.Add("test");
            s.SetSelected("test", true);
            Expected(s, "set selected by object", 1, 2, 1);

            s.Items.RemoveAt(2);
            Expected(s, "remove only selected item", 1, -1, 0);

            s.Items.Add("test2");
            s.Items.Insert(0, "test0");
            Expected(s, "SR = false, items = {{test0, !S, !S, test, test2}}", 0, -1, 0);

            s.SetSelected("test0", true);
            Expected(s, "SR = false, items = {{[test0], !S, !S, test, test2}}", 1, 0, 1);

            s.Items.Insert(0, "test3");
            Expected(s, "SR = false, items = {{test3, [test0], !S, !S, test, test2}}", 0, 1, 1);

            s.Items.Insert(0, "test4");
            Expected(s, "SR = false, items = {{test4, test3, [test0], !S, !S, test, test2}}", 0, 2, 1);

            s.Items.RemoveAt(2);
            Expected(s, "SR = true, items = {{[test4], test3, !S, !S, test, test2}}", 1, -1, 0);

            s.IsSynchronizedWithCurrentItem = true;
            s.SelectedIndex = 4;
            Expected(s, "SR = true, items = {{test4, test3, !S, !S, [test], test2}}", 1, 4, 1);

            s.Items.RemoveAt(0);
            Expected(s, "SR = true, items = {{test3, !S, !S, [test], test2}}", 0, 3, 1);

            s.Items.Remove("test");

            s.SetCanSelectMultiple(true);
            Expected(s, "SR = true, items = {{test3, !S, !S, [test2]}}", 0, 3, 1);

            s.SetSelected(0, true);
            Expected(s, "SR = true, items = {{[test3], !S, !S, [test2]}}", 1, 3, 2);

            s.SelectedIndex = 0;
            Expected(s, "SR = false, items = {{[test3], !S, !S, test2}}", 1, 0, 1);

            s.SelectedIndex = 20;
            // the selection shouldn't have changed.
            Expected(s, "SR = false, items = {{[test3], !S, !S, test2}}", 0, 0, 1);
            
            // 
            try
            {
                s.SelectedIndex = -2;
                DRT.Assert(false, "Should have thrown exception with s.SelectedIndex = -2");
            }
            catch (ArgumentException)
            {
            }
            // the selection shouldn't have changed.
            Expected(s, "SR = false, items = {{[test3], !S, !S, test2}}", 0, 0, 1);
            

            // ListBox:  No exception is thrown when trying to select an item that is not in the list using the SetSelected method.
            // Note: This behavior no longer throws since SetSelected no longer exists as a public API
            // Try selecting an item outside of the collection:
            s.SetSelected(new Button(), true);
            Expected(s, "No change: SR = false, items = {{[test3], !S, !S, test2}}", 0, 0, 1);

            // ListBox:  Correct item not selected when passing over a non Selectable item at the end of the list

            s.Items.Clear();
            SelectorTest.counter = 0;

            for (int i = 1; i <= 5; i++)
            {
                ListBoxItem li = new ListBoxItem();
                li.Content = "ListBoxItem " + i;
                s.Items.Add(li);
            }

            int removeAtIndex = s.Items.Count - 1;

            //s.SetSelectable(removeAtIndex - 1, false);
            s.SetSelected(removeAtIndex, true);
            s.Items.RemoveAt(removeAtIndex);

            Expected(s, "SR = true, items = {{1, 2, [3], !4}}", 2, -1, 0);


            // 
            s.UnselectAll();

            Binding binding = new Binding("SelectedItem");
            binding.Mode = BindingMode.OneWay;
            binding.Source = s;
            s.SetBinding(ContentControl.ContentProperty, binding);
            s.SetSelected(2, true);
            DRT.Assert(s.GetValue(ContentControl.ContentProperty) == s.Items[2], "Binding did not propogate the correct value from SelectedItem to ContentControl.Content (attached property), value was " + s.GetValue(ContentControl.ContentProperty) + " should have been " + s.Items[2]);

            SelectorTest.counter = 0;
            _numtests++;


            s.Items.Clear();
            SelectorTest.counter = 0;

            // _desiredSelectedIndex should be stored as 1.
            s.SelectedIndex = 1;

            s.Items.Add("item 0");
            Expected(s, "Test deferred select by index -- _desiredSelectedIndex should be 1 even though selection was forced to 0", 0, -1, 0);

            s.Items.Add("item 1");
            Expected(s, "Test deferred select by index -- now there are enough items, so deferred selection should be fulfilled.", 1, 1, 1);

            s.Items.Clear();
            SelectorTest.counter = 0;

            // Check that I can set a deferred selection and, before it's fulfilled,
            // I can change it again to something valid.

            // _desiredSelectedIndex should be stored as 2.
            s.SelectedIndex = 2;

            s.Items.Add("item 0");
            Expected(s, "Test deferred select by index (2) -- ISR should force item 0 to be selected", 0, -1, 0);
            s.Items.Add("item 1");
            // no selection change should have happened yet.
            Expected(s, "Test deferred select by index (2) -- adding another item shouldn't change selection", 0, -1, 0);

            s.SelectedIndex = 1;
            Expected(s, "Test deferred select by index (2) -- setting SelectedIndex to 1 (something valid) when _desiredSelectedIndex != -1 should change selection", 1, 1, 1);

            s.Items.Add("item 2");
            Expected(s, "Test deferred select by index (2) -- adding item 2 shouldn't change selection b/c _desiredSelectedIndex should be -1", 0, 1, 1);


            s.Items.Clear();
            SelectorTest.counter = 0;

            // Try an update where the selection changes from the collection
            ObservableCollection<string> collection = new ObservableCollection<string>();
            collection.Add("item 0");

            s.SelectedIndex = 1;
            s.ItemsSource = collection;

            // should have gotten a Refresh and synced currency of the collection (item 0)
            // with the selection.  SelectedIndex should be 0.
            Expected(s, "Test deferred select by index (3) -- a collection refresh should sync selection with currency", 1, 0, 1);

            s = new SelectorTest();
            s.Items.Add("item 0");
            s.Items.Add("item 1");
            s.Items.Add("item 2");
            s.Items.Add("item 3");

            s.SelectedItems.Add("item 2");

            Expected(s, "Add 'item 2' to SelectedItems", 1, 2, 1);
            
            // Test SetSelectedItems method
            List<object> selectedItems = new List<object>();
            selectedItems.Add("item 1");
            selectedItems.Add("item 3");

            MethodInfo setSelectedItemsMethod = typeof(Selector).GetMethod("SetSelectedItemsImpl", BindingFlags.NonPublic | BindingFlags.Instance);
            if (setSelectedItemsMethod == null) throw new Exception("couldn't find Selector.SetSelectedItems method");
            bool selectionResult = (bool)setSelectedItemsMethod.Invoke(s, new object[]{selectedItems});
            
            DRT.AssertEqual(true, selectionResult, "Calling to SetSelectedItems() should succeeded.");

            Expected(s, "Set SelectedItems to ('test 1', 'test 3')", 1, 1, 2);
            DRT.AssertEqual("item 1", s.SelectedItems[0], "Checking first selected item");
            DRT.AssertEqual("item 3", s.SelectedItems[1], "Checking second selected item");
            DRT.AssertEqual(2, s.SelectedItems.Count, "Should have removed previously selected item.");
            
            selectedItems.Add("item 5");
            selectionResult = (bool)setSelectedItemsMethod.Invoke(s, new object[]{selectedItems});
            
            DRT.AssertEqual(false, selectionResult, "Calling to SetSelectedItems() with some invalid selection should failed.");
            // Previous selection should still intact.
            DRT.AssertEqual("item 1", s.SelectedItems[0], "Checking first selected item");
            DRT.AssertEqual("item 3", s.SelectedItems[1], "Checking second selected item");
            DRT.AssertEqual(2, s.SelectedItems.Count, "Previous selection should not change.");
            
            // Test that DeferSelectionByIndex is cancel, when user select something
            s.Items.Clear();
            s.SelectedIndex = 6;
            s.Items.Add("item 0");
            s.Items.Add("item 1");
            s.Items.Add("item 2");
            s.Items.Add("item 3");
            s.Items.Add("item 4");
            s.Items.Add("item 5");
            DRT.AssertEqual(-1, s.SelectedIndex, "SelectedIndex should be -1");
            s.SelectedItem = "item 3";
            s.Items.Add("item 6");
            s.Items.Add("item 7");
            s.Items.Add("item 8");
            s.Items.Add("item 9");
            DRT.AssertEqual(3, s.SelectedIndex, "SelectedIndex should be 3, because we select 'item 3'.");
        
            SelectorTest.counter = 0;

            // Test defered selection
            // First we add items to SelectedItems and then add items to Items collection
            s.Items.Clear();
            s.SelectedItems.Add("item 1");
            s.SelectedItems.Add("item 3");
            s.Items.Add("item 0");
            s.Items.Add("item 1");
            s.Items.Add("item 2");
            s.Items.Add("item 3");
            DRT.AssertEqual(2, s.SelectedItems.Count, "Defer selection: SelectedItems should be 2.");
            DRT.AssertEqual("item 1", s.SelectedItems[0], "Defer selection: Checking first selected item");
            DRT.AssertEqual("item 3", s.SelectedItems[1], "Defer selection: Checking second selected item");

            SelectorTest.counter = 0;

        }


        private void TestListBox()
        {
            Console.WriteLine("Testing ListBox...");

            ListBox lb = new ListBox();

            ListBoxItem listItem1 = new ListBoxItem();
            ListBoxItem listItem2 = new ListBoxItem();

            lb.Items.Add(listItem1);
            lb.Items.Add(listItem2);

            // There should be nothing selected right now
            Expected(lb, "add two listitems", 0, -1, 0);

            listItem1.IsSelected = true;
            // Now it should be selected
            Expected(lb, "select 1st listitem", 0, 0, 1);
        }

#if OLD_AUTOMATION
        private void TestAccessibility()
        {
            Console.WriteLine("Testing Accessibility...");

            lb = new MyListBox(this);
            lb.Name = "ListBox";
            _rootBorder.Child = lb;
            _rootBorder.IsEnabled = true;

            for (int i = 0; i < 30; i++) lb.Items.Add(i);

            WaitForPriority(DispatcherPriority.Background);

            // Get the parent of the Element in questing to set up event listening on it.
            // It is more efficient than listening for events from Root.
            AutomationElement lelb = System.Windows.Automation.Provider.AutomationProvider.AutomationElementFromUIElement(lb);
            AutomationElement le = TreeWalker.ControlViewWalker.GetParent(lelb);
            DRT.Assert(le != null, "Logical element was null");

            AutomationEventHandler handler = new AutomationEventHandler(LBAutomationEventHandler);
            Automation.AddAutomationEventHandler( SelectionPattern.InvalidatedEvent, le, TreeScope.Subtree, handler );
            Automation.AddAutomationEventHandler( SelectionItemPattern.ElementSelectedEvent, le, TreeScope.Subtree, handler );
            Automation.AddAutomationEventHandler( SelectionItemPattern.ElementAddedToSelectionEvent, le, TreeScope.Subtree, handler );
            Automation.AddAutomationEventHandler( SelectionItemPattern.ElementRemovedFromSelectionEvent, le, TreeScope.Subtree, handler );

            // Everything's set up! now we can start testing.

            lb.ExpectSelectedEventFrom(1);
            lb.SetSelected(1, true);

            lb.VerifyAutomationEventOccurred();

            lb.ExpectSelectedEventFrom(2);
            lb.SetSelected(2, true);

            lb.VerifyAutomationEventOccurred();

            lb.ExpectRemovedEventFrom(2);
            lb.SetSelected(2, false);

            lb.VerifyAutomationEventOccurred();

            lb.ExpectSelectedEventFrom(3);
            lb.SetSelected(3, true);

            lb.VerifyAutomationEventOccurred();

            lb.SelectionMode = SelectionMode.Extended;

            lb.ExpectAddedEventFrom(4);
            lb.SetSelected(4, true);

            lb.VerifyAutomationEventOccurred();

            lb.SelectionChangeBegin();
            for (int i = 5; i < 24; i++)
            {
                lb.ExpectAddedEventFrom(i);
                lb.SelectionChangeSelect(i);
            }
            lb.SelectionChangeEnd();

            lb.VerifyAutomationEventOccurred();

            lb.SelectionChangeBegin();
            for (int i = 3; i < 24; i++)
            {
                lb.SelectionChangeUnselect(i);
            }
            lb.ExpectInvalidatedEvent();
            lb.SelectionChangeEnd();

            lb.VerifyAutomationEventOccurred();

            Automation.RemoveAutomationEventHandler(SelectionPattern.InvalidatedEvent, le, handler);
            Automation.RemoveAutomationEventHandler(SelectionItemPattern.ElementSelectedEvent, le, handler);
            Automation.RemoveAutomationEventHandler(SelectionItemPattern.ElementAddedToSelectionEvent, le, handler);
            Automation.RemoveAutomationEventHandler(SelectionItemPattern.ElementRemovedFromSelectionEvent, le, handler);

        }

        MyListBox lb;

        private void LBAutomationEventHandler(object sender, AutomationEventArgs e)
        {
            lb.OnAutomationEvent(sender, e);
        }
#endif

        private void Expected(Selector s, string testname, int selectionChanges, int selectedIndex, int selectedItemsCount)
        {
            DRT.Assert(SelectorTest.counter == selectionChanges, "Test (" + testname + "): Selector should have registered " + selectionChanges + " selection changes, was: " + SelectorTest.counter);
            DRT.Assert(s.SelectedIndex == selectedIndex, "Test (" + testname + "): SelectedIndex should be " + selectedIndex + ", was: " + s.SelectedIndex);

            Type selector = typeof(Selector);
            PropertyInfo property = selector.GetProperty("SelectedItemsImpl", BindingFlags.NonPublic | BindingFlags.Instance);
            IList selectedItems = (IList)property.GetValue(s, null);
            int count = selectedItems.Count;

            DRT.Assert(count == selectedItemsCount, "Test (" + testname + "): " + selectedItemsCount + " items should be selected, not " + count);
            SelectorTest.counter = 0;
            _numtests++;
        }

        public static void WaitForTime(int idleTime, ref DispatcherTimer timer)
        {
            DispatcherFrame frame = new DispatcherFrame();

            timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = TimeSpan.FromMilliseconds(idleTime);
            
            timer.Tick += delegate(object sender, EventArgs e)
                {
                    ((DispatcherTimer)sender).Stop();
                    frame.Continue = false;
                };
            timer.Start();

            Dispatcher.PushFrame(frame);
        }

        public static void WaitForPriority(DispatcherPriority priority)
        {
            Dispatcher.CurrentDispatcher.Invoke(
                priority, 
                (DispatcherOperationCallback)delegate(object unused) { return null; },
                null
                );
        }

        internal class MyListBox : ListBox
        {
            public MyListBox(DrtSelectorSuite suite)
            {
                _owner = suite;
            }

            public void SetSelected(int index, bool selected)
            {
                SetSelected(Items[index], selected);
            }

            public void SetSelected(object item, bool selected)
            {
                if (SelectionMode != SelectionMode.Single)
                {
                    IList selectedItems = SelectedItems;
                    if (selected)
                    {
                        selectedItems.Add(item);
                    }
                    else
                    {
                        selectedItems.Remove(item);
                    }
                }
                else
                {
                    if (selected)
                    {
                        SelectedItem = item;
                    }
                    else
                    {
                        SelectedItem = null;
                    }
                }
            }

            DrtSelectorSuite _owner;

            public void SelectionChangeBegin()
            {
                SelectorTest.SelectionChange(this, "Begin");
            }

            public void SelectionChangeEnd()
            {
                SelectorTest.SelectionChange(this, "End");
            }

            public void SelectionChangeSelect(object o)
            {
                SelectorTest.SelectionChange(this, "Select", new object[] { o, false });
            }

            public void SelectionChangeUnselect(object o)
            {
                SelectorTest.SelectionChange(this, "Unselect", new object[] { o });
            }

            private List<object> _expectSelected = new List<object>();
            private List<object> _expectAdded = new List<object>();
            private List<object> _expectRemoved = new List<object>();
            private bool _automationEventOccurred;
            private bool _expectInvalidatedEvent;

            public void ExpectSelectedEventFrom(params object[] args)
            {
                _automationEventOccurred = false;
                foreach (object arg in args) _expectSelected.Add(arg);
            }

            public void ExpectAddedEventFrom(params object[] args)
            {
                _automationEventOccurred = false;
                foreach (object arg in args) _expectAdded.Add(arg);
            }

            public void ExpectRemovedEventFrom(params object[] args)
            {
                _automationEventOccurred = false;
                foreach (object arg in args) _expectRemoved.Add(arg);
            }

            public void ExpectInvalidatedEvent()
            {
                _expectInvalidatedEvent = true;
            }

            public DispatcherTimer _waitOperation;

            public void VerifyAutomationEventOccurred()
            {
                // Give them 60 seconds to deliver all the events.
                WaitForTime(60000, ref _waitOperation);

                if (!_automationEventOccurred)
                {
                    throw new Exception("Automation event was not raised.");
                }

                _owner.DRT.Assert(!_expectInvalidatedEvent, "Expected InvalidatedEvent, but it did not happen");
                for (int i = 0; i < _expectSelected.Count; i++)
                {
                    _owner.DRT.Assert(false, "Expected ElementSelectedEvent from " + _expectSelected[i]);
                }
                for (int i = 0; i < _expectAdded.Count; i++)
                {
                    _owner.DRT.Assert(false, "Expected ElementAddedToSelection from " + _expectAdded[i]);
                }
                for (int i = 0; i < _expectRemoved.Count; i++)
                {
                    _owner.DRT.Assert(false, "Expected ElementRemovedFromSelection from " + _expectRemoved[i]);
                }
            }

            public void OnAutomationEvent(object sender, AutomationEventArgs e)
            {
                _automationEventOccurred = true;

                if (e.EventId == SelectionPattern.InvalidatedEvent)
                {
                    _owner.DRT.Assert(_expectInvalidatedEvent, "Received Invalidated event, but did not expect it");
                    _expectInvalidatedEvent = false;
                }
                else
                {
                    AutomationElement listItem = sender as AutomationElement;
                    _owner.DRT.Assert(listItem != null, "received event " + e.EventId + " but sender was not a AutomationElement, sender was " + sender);
                    object item = int.Parse((string)listItem.GetCurrentPropertyValue(AutomationElement.NameProperty));

                    if (e.EventId == SelectionItemPattern.ElementSelectedEvent)
                    {
                        _owner.DRT.Assert(_expectSelected.Contains(item), "Received unexpected ElementSelectedEvent from " + item);
                        _expectSelected.Remove(item);
                    }
                    if (e.EventId == SelectionItemPattern.ElementAddedToSelectionEvent)
                    {
                        _owner.DRT.Assert(_expectAdded.Contains(item), "Received unexpected ElementAddedToSelection from " + item);
                        _expectAdded.Remove(item);
                    }
                    if (e.EventId == SelectionItemPattern.ElementRemovedFromSelectionEvent)
                    {
                        _owner.DRT.Assert(_expectRemoved.Contains(item), "Received unexpected ElementRemovedFromSelection from " + item);
                        _expectRemoved.Remove(item);
                    }
                }

                // If we finally got everything we expected, schedule the event to happen right away.
                if (!_expectInvalidatedEvent && _expectSelected.Count == 0
                    && _expectAdded.Count == 0 && _expectRemoved.Count == 0)
                {
                    _waitOperation.Interval = TimeSpan.FromMilliseconds(10);
                }
            }
        }

        private int _numtests = 0;
    }

    internal class SelectorTest : Selector
    {
        public SelectorTest()
        {
            CanSelectMultiple = true;
        }

        public void SetCanSelectMultiple(bool m)
        {
            CanSelectMultiple = m;
            SelectionChange(this, "Begin");
            SelectionChange(this, "End");
        }

        private bool CanSelectMultiple
        {
            get
            {
                Type selector = typeof(Selector);
                PropertyInfo property = selector.GetProperty("CanSelectMultiple", BindingFlags.NonPublic | BindingFlags.Instance);
                return (bool)property.GetValue(this, null);
            }

            set
            {
                Type selector = typeof(Selector);
                PropertyInfo property = selector.GetProperty("CanSelectMultiple", BindingFlags.NonPublic | BindingFlags.Instance);
                property.SetValue(this, value, null);
            }
        }

        public void SelectAll()
        {
            Type selector = typeof(Selector);
            MethodInfo method = selector.GetMethod("SelectAllImpl", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(this, null);
        }

        public void UnselectAll()
        {
            Type selector = typeof(Selector);
            MethodInfo method = selector.GetMethod("UnselectAllImpl", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(this, null);
        }

        public static int counter = 0;

        public IList SelectedItems
        {
            get
            {
                Type selector = typeof(Selector);
                PropertyInfo property = selector.GetProperty("SelectedItemsImpl", BindingFlags.NonPublic | BindingFlags.Instance);
                return (IList)property.GetValue(this, null);
            }
        }

        public void SetSelected(int index, bool selected)
        {
            SetSelected(Items[index], selected);
        }

        public void SetSelected(object item, bool selected)
        {
            if (CanSelectMultiple)
            {
                IList selectedItems = SelectedItems;
                if (selected)
                {
                    selectedItems.Add(item);
                }
                else
                {
                    selectedItems.Remove(item);
                }
            }
            else
            {
                if (selected)
                {
                    SelectedItem = item;
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            counter++;
        }

        public static void SelectionChange(Selector instance, string method)
        {
            SelectorTest.SelectionChange(instance, method, new object[] {});
        }

        public static void SelectionChange(Selector instance, string method, object []parameters)
        {
            PropertyInfo info = typeof(Selector).GetProperty("SelectionChange", BindingFlags.NonPublic | BindingFlags.Instance);
            if (info == null) throw new Exception("couldn't find Selector.SelectionChange property");
            object SelectionChange = info.GetValue(instance, new object[] {});

            Type SelectionChangerType = typeof(Selector).GetNestedType("SelectionChanger", BindingFlags.NonPublic);
            if (SelectionChangerType == null) throw new Exception("couldn't find Selector.SelectionChanger type");

            MethodInfo methodInfo = SelectionChangerType.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null) throw new Exception("couldn't find SelectionChanger." + method + " method");
            methodInfo.Invoke(SelectionChange, parameters);
        }
    }
}
