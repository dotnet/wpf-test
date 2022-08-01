// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;
using System.Windows.Threading;
using System.IO;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Markup;
using System.Collections.Generic;

namespace DRT
{

    public enum ScrollingTest
    {
        None,
        SingleScrolling,
        ListBoxNoSV,
        HorizontalScrolling,
        SimpleListViewExtended,
        LargeItems,
        ChangeItemsPanel,
    }

    public class ListBoxSuite : DrtTestSuite, INotifyPropertyChanged
    {
        public ListBoxSuite(SelectionMode mode, bool recycling) : base("ListBox." + mode.ToString() + (recycling == true ? ".RecyclingMode" : ""))
        {
            Contact = "Microsoft";
            _mode = mode;
            _containerRecycling = recycling;
        }

        public ListBoxSuite(ScrollingTest scrollingTest, bool recycling) : base("ListBox." + scrollingTest.ToString() + (recycling == true ? ".RecyclingMode" : ""))
        {
            Contact = "Microsoft";
            _scrollingTest = scrollingTest;
            _containerRecycling = recycling;
        }

        bool _containerRecycling = false;
        SelectionMode _mode;
        ScrollingTest _scrollingTest;

        static bool s_prevRecycling;
        static bool s_isPageLoaded;
        static ListBox s_listBox,s_listBox2,s_listBoxNoSV,s_listBoxLargeItems;
        static ListBox s_selectionModeRBL;
        static ListBox s_horizontalListBox;
        static ListBox s_listView;
        static ComboBox s_comboView;

        static ListBoxSuite s_currentDrtSuite;

        public override DrtTest[] PrepareTests()
        {
            // Set this so that Action can get at it
            s_currentDrtSuite = this;

            if (s_prevRecycling != _containerRecycling)
            {
                s_isPageLoaded = false;
                s_prevRecycling = _containerRecycling;
            }


            if (!s_isPageLoaded)
            {
                string fileName = DRT.BaseDirectory + (_containerRecycling == true ? "DrtListBoxRecycling.xaml" : "DrtListBox.xaml");
                LoadXamlPage(fileName);

                s_isPageLoaded = true;
            }
            else
            {
                Keyboard.Focus(null);
            }

            s_listBox.UnselectAll();
            s_listBox2.UnselectAll();
            s_selectionModeRBL.SelectedItem = _mode;

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();
                tests.Add(new DrtTest(Setup));
                tests.Add(new DrtTest(Select));
                tests.Add(new DrtTest(Verify));
                if (_scrollingTest == ScrollingTest.None)
                {
                    tests.Add(new DrtTest(AddItemAt0));
                    tests.Add(new DrtTest(VirtualizingStackPanel));
                }
                tests.Add(new DrtTest(Cleanup));

                return tests.ToArray();
            }
            else
            {
                return new DrtTest[] { Setup };
            }
        }

        private void LoadXamlPage(string fileName)
        {
            System.IO.Stream stream = File.OpenRead(fileName);
            Visual root = (Visual)XamlReader.Load(stream);
            InitTree(root);
            DRT.Show(root);
        }

        private void InitTree(DependencyObject root)
        {
            s_listBox = DRT.FindElementByID("ListBox1", root) as ListBox;
            s_listBox2 = DRT.FindElementByID("ListBox2", root) as ListBox;
            s_listBoxNoSV = DRT.FindElementByID("ListBoxNoSV", root) as ListBox;
            s_listBoxLargeItems = DRT.FindElementByID("ListBoxOversizedItems", root) as ListBox;
            s_selectionModeRBL = DRT.FindElementByID("SelectionModeRBL", root) as ListBox;
            s_horizontalListBox = DRT.FindElementByID("HorizontalListBox", root) as ListBox;
            s_listView = DRT.FindElementByID("ListView", root) as ListBox;
            s_comboView = DRT.FindElementByID("ComboView", root) as ComboBox;

            DRT.Assert(s_listBox != null);
            DRT.Assert(s_listBox2 != null);
            DRT.Assert(s_listBoxNoSV != null);
            DRT.Assert(s_listBoxLargeItems != null);
            DRT.Assert(s_selectionModeRBL != null);
            DRT.Assert(s_horizontalListBox != null);
            DRT.Assert(s_listView != null);
            DRT.Assert(s_comboView != null);

            FrameworkElementFactory horizontalStackPanel = new FrameworkElementFactory(typeof(StackPanel));
            horizontalStackPanel.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
            ItemsPanelTemplate horizontalStackPanelTemplate = new ItemsPanelTemplate(horizontalStackPanel);
            s_horizontalListBox.ItemsPanel = horizontalStackPanelTemplate;

            FrameworkElementFactory listViewPanel = new FrameworkElementFactory(typeof(StackPanel));
            listViewPanel.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Vertical);
            ItemsPanelTemplate listViewPanelTemplate = new ItemsPanelTemplate(listViewPanel);
            s_listView.ItemsPanel = listViewPanelTemplate;

            FrameworkElementFactory comboViewPanel = new FrameworkElementFactory(typeof(StackPanel));
            comboViewPanel.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Vertical);
            ItemsPanelTemplate comboViewPanelTemplate = new ItemsPanelTemplate(comboViewPanel);
            s_comboView.ItemsPanel = comboViewPanelTemplate;

            ((FrameworkElement)root).DataContext = this;
        }

        ListBox _currentListBox = null;

        private void Setup()
        {
            _currentListBox = s_listBox;
            if (_scrollingTest == ScrollingTest.None)
            {
                switch (_mode)
                {
                    case SelectionMode.Single:
                        _actions = _singleSelections;
                        break;

                    case SelectionMode.Multiple:
                        _actions = _multipleSelections;
                        break;

                    case SelectionMode.Extended:
                        _actions = _extendedSelections;
                        break;
                }
            }
            else
            {
                switch (_scrollingTest)
                {
                    case ScrollingTest.SingleScrolling:
                        _actions = _singleSelectionComplex;
                        _currentListBox = s_listBox2;
                        break;

                    case ScrollingTest.ListBoxNoSV:
                        _actions = _singleSelectionNoSV;
                        _currentListBox = s_listBoxNoSV;
                        break;

                    case ScrollingTest.HorizontalScrolling:
                        _actions = _singleSelectionHorizontal;
                        _currentListBox = s_horizontalListBox;
                        break;

                    case ScrollingTest.SimpleListViewExtended:
                        _actions = _simpleListViewExtended;
                        _currentListBox = s_listView;
                        s_selectionModeRBL.SelectedItem = SelectionMode.Extended;
                        break;

                    case ScrollingTest.LargeItems:
                        _actions = _largeItemsSingle;
                        _currentListBox = s_listBoxLargeItems;
                        break;

                    case ScrollingTest.ChangeItemsPanel:
                        DRT.ResumeAt(new DrtTest(ChangeItemsPanelToDockPanel));
                        break;
                }
            }
            _currentListBox.Focus();

            _curr = 0;

        }

        Action[] _actions;
        int _curr = 0;

        Action[] _singleSelections = new Action[] {
            new MouseSelection(0), new MouseSelection(1), new MouseSelection(2), new MouseSelection(3), new MouseSelection(1), new MouseSelection(19),
        };

        Action[] _multipleSelections = new Action[]
        {
            new MouseSelection(0),
            new MouseSelection(1, new int[] { 0, 1}),
            new MouseSelection(2, new int[] { 0, 1, 2 }),
            new MouseSelection(3, new int[] { 0, 1, 2, 3}),
            new MouseSelection(1, new int[] { 0, 2, 3 }),
            new MouseSelection(2, new int[] { 0, 3 }),
            new MouseSelection(5, new int[] { 0, 3, 5 }),
        };

        Action[] _extendedSelections = new Action[]
        {
            new MouseSelection(0),
            new MouseSelection(10),
            new MouseSelection(5, false, true, new int[] { 5, 10 }),
            new MouseSelection(7, true, true, new int[] { 5, 6, 7, 10 }),
            new MouseSelection(2, true, true, new int[] { 2, 3, 4, 5, 6, 7, 10 }),
            new MouseSelection(9, false, true, new int[] { 2, 3, 4, 5, 6, 7, 9, 10}),
            new MouseSelection(11, true, true, new int[] { 2, 3, 4, 5, 6, 7, 9, 10, 11 }),
            new MouseSelection(8, true, true, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }),
            new MouseSelection(5),
            new KeyboardSelection(Key.Down, false, true, new int[] { 5 }, 6), // ctrl-down
            new KeyboardSelection(Key.Space, false, false, new int[] { 6 }, 6), // space should single select
            new KeyboardSelection(Key.Down, false, true, new int[] { 6 }, 7), // ctrl-down (move focus)
            new KeyboardSelection(Key.Down, false, true, new int[] { 6 }, 8), // ctrl-down
            new KeyboardSelection(Key.Space, true, false, new int[] { 6, 7, 8 }, 8), // shift-space should anchor select
        };

        Action[] _multipleSelectionsSelectionRequired = new Action[]
        {
            new MouseSelection(1, new int[] { 0, 1}),
            new MouseSelection(2, new int[] { 0, 1, 2 }),
            new MouseSelection(3, new int[] { 0, 1, 2, 3}),
            new MouseSelection(1, new int[] { 0, 2, 3 }),
            new MouseSelection(2, new int[] { 0, 3 }),
            new MouseSelection(5, new int[] { 0, 3, 5 }),
        };

        Action[] _singleSelectionComplex = new Action[]
        {
            new KeyboardSelection(Key.Down, false, false, new int[] { 0 }, 0),
            new KeyboardSelection(Key.Down, false, false, new int[] { 1 }, 1),
            new KeyboardSelection(Key.Down, false, true, new int[] { 1 }, 2),
            new KeyboardSelection(Key.Down, false, false, new int[] { 3 }, 3),
            new KeyboardSelection(Key.Down, false, false, new int[] { 4 }, 4),
            new KeyboardSelection(Key.Down, false, false, new int[] { 5 }, 5),
            new KeyboardSelection(Key.Down, false, false, new int[] { 6 }, 6),
            new KeyboardSelection(Key.Down, false, false, new int[] { 7 }, 7),
            new KeyboardSelection(Key.End, false, false, new int[] { 19 }, 19),
            new KeyboardSelection(Key.Home, false, true, new int[] { 19 }, 0),
            new MouseDownSelection(1, new int[] { 1 }),
            new MouseUpSelection(6, new int[] { 6 }),
            new ScrollAction(1, 6), // Top should be 1
            new KeyboardSelection(Key.Up, false, false, new int[] { 5 }, 5),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 1 }, 1),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 7 }, 7),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 14 }, 14),
            new KeyboardSelection(Key.PageUp, false, true, new int[] { 14 }, 8),
            new KeyboardSelection(Key.Up, false, false, new int[] { 7 }, 7),
            new ScrollAction(-1, 7), // Top should be 6
            new ScrollAction(1, 7), // Top should be 7
            new ScrollAction(1, 7), // Top should be 8, focus should stay of 7
        };

        Action[] _singleSelectionNoSV = new Action[]
        {
            new KeyboardSelection(Key.Down, false, false, new int[] { 0 }, 0),
            new KeyboardSelection(Key.Down, false, false, new int[] { 1 }, 1),
            new KeyboardSelection(Key.Down, false, true, new int[] { 1 }, 2),
            new KeyboardSelection(Key.Down, false, false, new int[] { 3 }, 3),
            new KeyboardSelection(Key.Down, false, false, new int[] { 4 }, 4),
            new KeyboardSelection(Key.Down, false, false, new int[] { 5 }, 5),
            new KeyboardSelection(Key.Down, false, false, new int[] { 6 }, 6),
            new KeyboardSelection(Key.Down, false, false, new int[] { 7 }, 7),
            new KeyboardSelection(Key.End, false, false, new int[] { 15 }, 15),
            new KeyboardSelection(Key.Home, false, true, new int[] { 15 }, 0),
            new MouseDownSelection(1, new int[] { 1 }),
            new MouseUpSelection(6, new int[] { 6 }),
            new KeyboardSelection(Key.Up, false, false, new int[] { 5 }, 5),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 0 }, 0),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 15 }, 15),
        };

        Action[] _singleSelectionHorizontal = new Action[]
        {
            new KeyboardSelection(Key.Left, false, false, new int[] { 0 }, 0),
            new KeyboardSelection(Key.Right, false, false, new int[] { 1 }, 1),
            new KeyboardSelection(Key.PageDown, false, true, new int[] { 1 }, 17),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 33 }, 33),
            new KeyboardSelection(Key.End, false, false, new int[] { 140 }, 140),
            new KeyboardSelection(Key.Home, false, true, new int[] { 140 }, 0),
            new MouseDownSelection(2, new int[] { 2 }),
            new MouseUpSelection(5, new int[] { 5 }),
        };

        Action[] _simpleListViewExtended = new Action[]
        {
            new MouseDownSelection(5, new int[] { 5 }),
            new MouseUpSelection(10, true, false, new int[] { 5, 6, 7, 8, 9, 10 }),
            new KeyboardSelection(Key.Down, false, false, new int[] { 14 }, 14),
            new KeyboardSelection(Key.PageDown, false, true, new int[] { 14 }, 18),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 2 }, 2),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 18 }, 18),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 38 }, 38),
            new KeyboardSelection(Key.Left, false, false, new int[] { 37 }, 37),
            new KeyboardSelection(Key.Left, false, false, new int[] { 36 }, 36),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 24 }, 24),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 36 }, 36),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 56 }, 56),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 80 }, 80),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 100 }, 100),
            new KeyboardSelection(Key.Right, false, false, new int[] { 101 }, 101),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 121 }, 121),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 140 }, 140),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 124 }, 124),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 104 }, 104),
        };

        Action[] _largeItemsSingle = new Action[]
        {
            new KeyboardSelection(Key.Down, false, false, new int[] { 0 }, 0),
            new ScrollAction(1, 0),
            new KeyboardSelection(Key.Down, false, false, new int[] { 1 }, 1),
            new KeyboardSelection(Key.End, false, false, new int[] { 140 }, 140),
            new KeyboardSelection(Key.PageUp, false, false, new int[] { 139 }, 139),
            new KeyboardSelection(Key.PageDown, false, false, new int[] { 140 }, 140),
        };

        static TimeSpan AutoScrollTimeout
        {
            get
            {
                PropertyInfo info = typeof(ItemsControl).GetProperty("AutoScrollTimeout", BindingFlags.NonPublic | BindingFlags.Static);
                return (TimeSpan)info.GetValue(null, null);
            }
        }

        private void Select()
        {
            if (_actions == null || _actions.Length == 0) return;

            Console.WriteLine("{0}: {1}", _curr, _actions[_curr]);

            _actions[_curr].DoAction(_currentListBox);
        }

        private void Verify()
        {
            if (_actions == null || _actions.Length == 0) return;

            VerifySelection(_actions[_curr]);

            _curr++;
            if (_curr < _actions.Length)
            {
                // They get pushed on the resume stack in reverse order -- this API should be different...
                DRT.ResumeAt(new DrtTest(Verify));
                DRT.ResumeAt(new DrtTest(Select));
            }
        }

        private void VerifySelection(Action action)
        {
            if (action.ExpectedSelected != null)
            {
                foreach (object item in _currentListBox.SelectedItems)
                {
                    int index = _currentListBox.Items.IndexOf(item);
                    bool found = false;
                    for (int i = 0; i < action.ExpectedSelected.Length; i++)
                    {
                        if (action.ExpectedSelected[i] == index) found = true;
                    }
                    DRT.Assert(found, Name + "[" + _curr + "]: Item " + index + " selected but not expected");
                }
                for (int i = 0; i < action.ExpectedSelected.Length; i++)
                {
                    bool found = false;
                    foreach (object item in _currentListBox.SelectedItems)
                    {
                        int index = _currentListBox.Items.IndexOf(item);
                        if (action.ExpectedSelected[i] == index) found = true;
                    }
                    DRT.Assert(found, Name + "[" + _curr + "]: Item " + action.ExpectedSelected[i] + " expected but not selected");
                }
            }

            if (action.ExpectedFocused.HasValue)
            {
                if (action.ExpectedFocused == -1)
                {
                    DRT.AssertEqual(_currentListBox, Keyboard.FocusedElement, "Focus should be on the listbox since focus was scrolled out of view");
                    DRT.Assert(Selector.GetIsSelectionActive(_currentListBox), "Selector.IsSelectionActive should be true for " + _currentListBox);
                }
                else
                {
                    DRT.AssertEqual(_currentListBox.ItemContainerGenerator.ContainerFromIndex(action.ExpectedFocused.Value), Keyboard.FocusedElement, "Focus should be on item " + action.ExpectedFocused.Value);

                    // 

                    if (_currentListBox != s_listBox2 && _currentListBox != s_horizontalListBox && _currentListBox != s_listBoxLargeItems)
                        DRT.Assert(Selector.GetIsSelectionActive(_currentListBox.ItemContainerGenerator.ContainerFromIndex(action.ExpectedFocused.Value)), "Selector.IsSelectionActive should be true for " + _currentListBox.ItemContainerGenerator.ContainerFromIndex(action.ExpectedFocused.Value));
                }
            }

        }

        private void AddItemAt0()
        {
            // 


            ListBoxItem item = new ListBoxItem();

            item.Content = "Added Item " + _addedItemCount++;
            _currentListBox.Items.Insert(0, item);
        }

        int _addedItemCount = 0;

        private void ChangeItemsPanelToDockPanel()
        {
            FrameworkElementFactory root = new FrameworkElementFactory(typeof(DockPanel));
            s_listBox.ItemsPanel = new ItemsPanelTemplate(root);

            DRT.ResumeAt(new DrtTest(ChangeItemsPanelToDefaultPanel));
        }

        private void ChangeItemsPanelToDefaultPanel()
        {
            // Verify that we can still get to one of the children
            DependencyObject listItem = DRT.FindVisualByID("ListBox1_ListItem1", s_listBox);
            DRT.Assert(listItem != null, "Could not find list item in the visual tree.  Regressed bug 1004742.");

            Panel panel = GetItemsHostForItemsControl(s_listBox);
            DRT.Assert(panel != null, "ItemsHost for _listBox should not be null");
            DRT.Assert(panel is DockPanel, "ItemsHost for _listBox should be DockPanel, was " + panel);

            s_listBox.ItemsPanel = ListBox.ItemsPanelProperty.GetMetadata(s_listBox).DefaultValue as ItemsPanelTemplate;

            DRT.ResumeAt(new DrtTest(VerifyItemsPanelChanged));
        }

        private void VerifyItemsPanelChanged()
        {
            // Verify that we can still get to one of the children
            DependencyObject listItem = DRT.FindVisualByID("ListBox1_ListItem1", s_listBox);
            DRT.Assert(listItem != null, "Could not find list item in the visual tree (after second panel change).  Regressed bug 1004742.");

            Panel panel = GetItemsHostForItemsControl(s_listBox);
            DRT.Assert(panel != null, "ItemsHost for _listBox should not be null");
            DRT.Assert(panel is VirtualizingStackPanel, "ItemsHost for _listBox should be VirtualizedStackPanel, was " + panel);
        }

        private Panel GetItemsHostForItemsControl(ItemsControl itemsControl)
        {
            PropertyInfo info = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);
            return (Panel)info.GetValue(itemsControl, null);
        }

        private void Cleanup()
        {
            // Remove the item we added
            ListBoxItem listItem = _currentListBox.Items[0] as ListBoxItem;
            if (listItem != null)
            {
                string content = listItem.Content as string;
                if (content != null && content.StartsWith("Added Item"))
                {
                    _currentListBox.Items.RemoveAt(0);
                }
            }
        }

        private class Action
        {
            public int[] ExpectedSelected;
            public Nullable<int> ExpectedFocused;

            public Action(int[] expectedSelected, Nullable<int> expectedFocused)
            {
                ExpectedSelected = expectedSelected;
                ExpectedFocused = expectedFocused;
            }

            public Action(Nullable<int> expectedFocused) : this(null, expectedFocused)
            {
            }

            public Action() : this(null, null)
            {
            }

            protected DrtBase DRT
            {
                get
                {
                    return s_currentDrtSuite.DRT;
                }
            }

            public virtual void DoAction(Selector selector)
            {
            }
        }

        private class MouseSelection : Action
        {
            private int _index;
            private bool _shift;
            private bool _control;
            protected bool _doMouseDown;
            protected bool _doMouseUp;

            public MouseSelection(int index, bool shift, bool control, int[] expectedSelected)
                : this(index, shift, control, expectedSelected, index)
            {
            }

            public MouseSelection(int index, bool shift, bool control, int[] expectedSelected, int expectedFocused)
                : base(expectedSelected, expectedFocused)
            {
                _index = index;
                _shift = shift;
                _control = control;
                _doMouseDown = _doMouseUp = true;
            }

            public MouseSelection(int index, int[] expected) : this (index, false, false, expected) {}
            public MouseSelection(int index) : this(index, new int[] { index }) { }

            public override void DoAction(Selector selector)
            {
                if (DRT.Verbose) Console.WriteLine("item is " + selector.Items[_index].GetType());

                ListBoxItem item = selector.ItemContainerGenerator.ContainerFromIndex(_index) as ListBoxItem;

                DRT.Assert(item != null, "Item should not be null: item " + _index);
                if (_shift) DRT.SendKeyboardInput(Key.LeftShift, true);
                if (_control) DRT.SendKeyboardInput(Key.LeftCtrl, true);
                DRT.MoveMouse(item, 0.5, 0.5);
                if (_doMouseDown)
                {
                    DRT.MouseButtonDown();
                }
                if (_doMouseUp)
                {
                    DRT.MouseButtonUp();
                }
                if (_control) DRT.SendKeyboardInput(Key.LeftCtrl, false);
                if (_shift) DRT.SendKeyboardInput(Key.LeftShift, false);

                base.DoAction(selector);
            }

            public override string ToString()
            {
                string type = "";
                if (_doMouseUp && _doMouseDown) type = "Click";
                else if (_doMouseUp) type = "MouseUp";
                else if (_doMouseDown) type = "MouseDown";
                return "MouseSelection: " + type + " on " + _index + ", " + (_shift ? "Shift" : "") + ", " + (_control ? "Control" : "");
            }


        }

        private class MouseUpSelection : MouseSelection
        {
            public MouseUpSelection(int index, int[] expected) : base (index, expected)
            {
                _doMouseDown = false;
            }

            public MouseUpSelection(int index, bool shift, bool control, int[] expected) : base (index, shift, control, expected)
            {
                _doMouseDown = false;
            }
        }

        private class MouseDownSelection : MouseSelection
        {
            public MouseDownSelection(int index, int[] expected) : base (index, expected)
            {
                _doMouseUp = false;
            }
        }

        private class KeyboardSelection : Action
        {
            private Key _key;
            private bool _shift;
            private bool _control;

            public KeyboardSelection(Key key, bool shift, bool control, int[] expectedSelected, int expectedFocused)
                : base(expectedSelected, expectedFocused)
            {
                _key = key;
                _shift = shift;
                _control = control;
            }

            public override void DoAction(Selector selector)
            {
                if (_shift) DRT.SendKeyboardInput(Key.LeftShift, true);
                if (_control) DRT.SendKeyboardInput(Key.LeftCtrl, true);
                DRT.PressKey(_key);
                if (_control) DRT.SendKeyboardInput(Key.LeftCtrl, false);
                if (_shift) DRT.SendKeyboardInput(Key.LeftShift, false);

                base.DoAction(selector);
            }

            public override string ToString()
            {
                return "Keyboard: " + _key + ", " + (_shift ? "Shift" : "") + ", " + (_control ? "Control" : "");
            }

        }

        private class ScrollAction : Action
        {
            public int Amount;

            public ScrollAction(int amount, int expectedFocused) : base(expectedFocused)
            {
                Amount = amount;
            }

            public override void DoAction(Selector selector)
            {
                PropertyInfo info = typeof(ItemsControl).GetProperty("ScrollHost", BindingFlags.NonPublic | BindingFlags.Instance);
                ScrollViewer scrollHost = (ScrollViewer)info.GetValue(selector, null);

                DRT.Assert(scrollHost != null, "ListBox should have a ScrollViewer in its style");

                if (Amount == 1) scrollHost.LineDown();
                else if (Amount == -1) scrollHost.LineUp();
                else if (Amount > 1) scrollHost.PageDown();
                else if (Amount < -1) scrollHost.PageUp();
                else DRT.Assert(false, "Invalid scroll amount");

                base.DoAction(selector);
            }

            public override string ToString()
            {
                return "Scroll: " + Amount;
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public List<string> Colors
        {
            get
            {
                if (_colors == null)
                {
                    _colors = new List<string>();
                    foreach (PropertyInfo brushInfo in typeof(Brushes).GetProperties())
                    {
                        Brush brush = brushInfo.GetValue(null, null) as Brush;
                        if (brush != null)
                        {
                            _colors.Add(brushInfo.Name);
                        }
                    }
                }

                return _colors;
            }
        }

        public List<string> ItemsList
        {
            get
            {
                if (_itemsList == null)
                {
                    _itemsList = new List<string>();
                    for (int i = 0; i < NumItems; i++)
                    {
                        _itemsList.Add(String.Format("Item {0}", i));
                    }
                }

                return _itemsList;
            }
        }

        public int NumItems
        {
            get
            {
                return _numItems;
            }
            set
            {
                _numItems = value;
                _itemsList = null;
                OnPropertyChanged("NumItems");
                OnPropertyChanged("ItemsList");
            }
        }

        List<string> _colors;
        List<string> _itemsList;
        int _numItems = 20;

        private void VirtualizingStackPanel()
        {
            // This section is to test bugs with the panel
            Console.WriteLine("TEST: VirtualizingStackPanel");
            _go = true;
            DRT.ResumeAt(new DrtTest(VSPStep));
        }

        private enum VSPSteps
        {
            Start,
            Switch,
            End,
        }

        private VSPSteps _vspStep;
        private bool _go;

        private void VSPStep()
        {
            Console.WriteLine("    STEP: " + _vspStep + (_go ? " (Action)" : " (Verify)"));
            switch (_vspStep)
            {
                case VSPSteps.Start:
                    break;

                case VSPSteps.Switch:
                    if (_go)
                    {
                        object item2 = _currentListBox.Items[2];
                        object item5 = _currentListBox.Items[5];
                        _currentListBox.Items.RemoveAt(5);
                        _currentListBox.Items.Insert(2, item5);
                        _currentListBox.Items.RemoveAt(3);
                        _currentListBox.Items.Insert(5, item2);
                    }
                    else
                    {
                    }
                    break;

                case VSPSteps.End:
                    break;
            }

            if (_vspStep != VSPSteps.End)
            {
                if (_go)
                {
                    _go = false; // Verify this step
                }
                else
                {
                    _go = true; // Run next step
                    _vspStep++;
                }
                DRT.ResumeAt(new DrtTest(VSPStep));
            }
        }
    }
}
