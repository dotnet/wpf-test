// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DRT
{
    public class DrtTreeViewSuite : DrtTestSuite
    {

        #region Initialization

        public DrtTreeViewSuite() : base("TreeView")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);
            DRT.LoadXamlFile("DrtTreeView.xaml");

            InitializeReferences();

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                {
                    new DrtTest(ChangingSource),
                    new DrtTest(Selection),
                    new DrtTest(ExpandSubtree),
                    new DrtTest(KeyboardTest),
                    new DrtTest(MouseTest),
                    new DrtTest(BringIntoView),
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void InitializeReferences()
        {
            _hierarchicalDB = DRT.FindElementByID("HierarchicalDB") as TreeView;
            DRT.Assert(_hierarchicalDB != null, "HierarchicalDB not found");

            _tv = DRT.FindElementByID("TV") as TreeView;
            DRT.Assert(_tv != null, "TV not found");
            _cp = DRT.FindElementByID("cp") as TreeViewItem;
            DRT.Assert(_cp != null, "cp not found");
            _isSelectedCB = DRT.FindElementByID("IsSelectedCB") as CheckBox;
            DRT.Assert(_isSelectedCB != null, "IsSelectedCB not found");

            _bindingSource = DRT.FindElementByID("BindingSource") as ListBox;
            DRT.Assert(_bindingSource != null, "BindingSource not found");
            _itemBound = DRT.FindElementByID("ItemBound") as TreeView;
            DRT.Assert(_itemBound != null, "ItemBound not found");
        }

        #endregion

        #region ChangingSource

        private void ChangingSource()
        {
            Console.WriteLine("TEST: ChangingSource");

            AddSelectedItemChangedHandler(_itemBound);

            TreeViewItem boundItem = GetContainer(_itemBound, 1);
            DRT.Assert(boundItem != null, "Could not get index 1");

            TreeViewItem boundContainer = GetContainer(boundItem, 0);
            DRT.Assert(boundContainer != null, "Could not get index 1,0");

            // Select an item that is bound
            DRT.Assert(_itemBound.SelectedItem == null, "ItemBound began with a selection");
            boundContainer.Focus();
            VerifyAndResetSelectionChanged(1, GetItem(boundContainer));

            // Change the binding, selection should move to the parent
            _bindingSource.SelectedIndex = 1;
            VerifyAndResetSelectionChanged(1, GetItem(boundItem));

            RemoveSelectedItemChangedHandler(_itemBound);
        }

        #endregion

        #region Selection

        private void Selection()
        {
            Console.WriteLine("TEST: Selection");

            AddSelectedItemChangedHandler(_tv);

            _isSelectedCB.IsChecked = true;
            VerifyAndResetSelectionChanged(1, _cp);
            _isSelectedCB.IsChecked = false;
            VerifyAndResetSelectionChanged(1, null);

            _tv.SelectedValuePath = "Length";
            TreeViewItem applesContainer = GetContainer(_tv, 5);
            object apples = GetItem(_tv, 5);
            DRT.Assert(applesContainer != null, "Could not get container 5 (Apples)");
            DRT.Assert(apples != null, "Could not get item 5 (Apples)");
            applesContainer.IsSelected = true;
            VerifyAndResetSelectionChanged(1, apples);
            DRT.Assert(IsEqual(_tv.SelectedItem, apples), "SelectedItem is not equal to Apples, was: " + MakeString(_tv.SelectedItem));
            DRT.Assert(IsEqual(_tv.SelectedValue, 6), "SelectedValue is not equal to 6, was: " + MakeString(_tv.SelectedValue));
            _tv.ClearValue(TreeView.SelectedValuePathProperty);

            RemoveSelectedItemChangedHandler(_tv);
        }

        private void ExpandSubtree()
        {
            Console.WriteLine("TEST: ExpandSubtree");

            TreeViewItem container = GetContainer(_tv, 7);
            container.IsSelected = true;
            container.IsExpanded = false;
            container.UpdateLayout();
            container.ExpandSubtree();
            AssertExpandedRecursive(container);
            container.IsExpanded = false;
        }

        private void AssertExpandedRecursive(TreeViewItem item)
        {
            DRT.Assert(item.IsExpanded);

            for (int i = 0, count = item.Items.Count; i < count; i++)
            {
                TreeViewItem subitem = (TreeViewItem)item.ItemContainerGenerator.ContainerFromIndex(i);

                if (subitem != null)
                {
                    AssertExpandedRecursive(subitem);
                }
            }
        }

        private static bool IsEqual(object a, object b)
        {
            return a.Equals(b);
        }

        #endregion

        #region KeyboardTest

        private void KeyboardTest()
        {
            Console.WriteLine("TEST: KeyboardTest");
            _go = true;
            DRT.ResumeAt(new DrtTest(KeyboardStep));
        }

        private enum KeyboardSteps
        {
            Start,
            FirstDown,
            FirstRight,
            SecondRight,
            SecondDown,
            FirstUp,
            FirstLeft,
            SecondLeft,
            Plus,
            Minus,
            End,
        }

        private void KeyboardStep()
        {
            Console.WriteLine("    STEP: " + _keyboardStep + (_go ? " (Action)" : " (Verify)"));

            switch (_keyboardStep)
            {
                case KeyboardSteps.Start:
                    if (_go)
                    {
                        AddSelectedItemChangedHandler(_hierarchicalDB);
                        _hierarchicalDB.Focus();
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                    }
                    break;

                case KeyboardSteps.FirstDown:
                    if (_go)
                    {
                        DRT.PressKey(Key.Down);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(1, GetItem(_hierarchicalDB, 0));
                    }
                    break;

                case KeyboardSteps.FirstRight:
                    if (_go)
                    {
                        DRT.PressKey(Key.Right);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.Assert(item != null, "Couldn't get item 0");
                        DRT.Assert(item.IsExpanded, "item 0 should be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                        DRT.Assert(_hierarchicalDB.SelectedItem == GetItem(_hierarchicalDB, 0), "SelectedItem is not item 0");
                    }
                    break;

                case KeyboardSteps.SecondRight:
                    if (_go)
                    {
                        DRT.PressKey(Key.Right);
                    }
                    else
                    {
                        TreeViewItem parent = GetContainer(_hierarchicalDB, 0);
                        VerifyAndResetSelectionChanged(1, GetItem(parent, 0));
                        DRT.Assert(parent.IsExpanded, "item 0 should be expanded");
                        DRT.Assert(!parent.IsSelected, "item 0 should not be selected");

                        TreeViewItem child = GetContainer(parent, 0);
                        DRT.Assert(child != null, "Couldn't get item 0,0");
                        DRT.Assert(!child.IsExpanded, "item 0,0 should not be expanded");
                        DRT.Assert(child.IsSelected, "item 0,0 should be selected");
                        DRT.Assert(_hierarchicalDB.SelectedItem == GetItem(child), "SelectedItem is not item 0,0");
                    }
                    break;

                case KeyboardSteps.SecondDown:
                    if (_go)
                    {
                        DRT.PressKey(Key.Down);
                    }
                    else
                    {
                        TreeViewItem parent = GetContainer(_hierarchicalDB, 0);
                        VerifyAndResetSelectionChanged(1, GetItem(parent, 1));

                        TreeViewItem child = GetContainer(parent, 1);
                        DRT.Assert(child != null, "Couldn't get item 0,1");
                        DRT.Assert(child.IsSelected, "item 0,1 should be selected");
                    }
                    break;

                case KeyboardSteps.FirstUp:
                    if (_go)
                    {
                        DRT.PressKey(Key.Up);
                    }
                    else
                    {
                        TreeViewItem parent = GetContainer(_hierarchicalDB, 0);
                        VerifyAndResetSelectionChanged(1, GetItem(parent, 0));
                    }
                    break;

                case KeyboardSteps.FirstLeft:
                    if (_go)
                    {
                        DRT.PressKey(Key.Left);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(1, GetItem(_hierarchicalDB, 0));
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.Assert(item.IsExpanded, "item 0 should be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                    }
                    break;

                case KeyboardSteps.SecondLeft:
                    if (_go)
                    {
                        DRT.PressKey(Key.Left);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.Assert(!item.IsExpanded, "item 0 should not be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                    }
                    break;

                case KeyboardSteps.Plus:
                    if (_go)
                    {
                        DRT.PressKey(Key.Add);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.Assert(item.IsExpanded, "item 0 should be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                    }
                    break;

                case KeyboardSteps.Minus:
                    if (_go)
                    {
                        DRT.PressKey(Key.Subtract);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.Assert(!item.IsExpanded, "item 0 should not be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                    }
                    break;

                case KeyboardSteps.End:
                    break;
            }

            if (_keyboardStep != KeyboardSteps.End)
            {
                if (_go)
                {
                    _go = false; // Verify this step
                }
                else
                {
                    _go = true; // Run next step
                    _keyboardStep++;
                }
                DRT.ResumeAt(new DrtTest(KeyboardStep));
            }
            else
            {
                RemoveSelectedItemChangedHandler(_hierarchicalDB);
            }
        }

        #endregion

        #region MouseTest

        private void MouseTest()
        {
            Console.WriteLine("TEST: MouseTest");
            _go = true;
            DRT.ResumeAt(new DrtTest(MouseStep));
        }

        private enum MouseSteps
        {
            Start,
            ClickExpand,
            ClickSelectChild,
            ClickCollapse,
            End,
        }

        private void MouseStep()
        {
            Console.WriteLine("    STEP: " + _mouseStep + (_go ? " (Action)" : " (Verify)"));

            switch (_mouseStep)
            {
                case MouseSteps.Start:
                    if (_go)
                    {
                        AddSelectedItemChangedHandler(_hierarchicalDB);
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                    }
                    break;

                case MouseSteps.ClickExpand:
                    if (_go)
                    {
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.MoveMouse(item, 0.1, 0.5);
                        DRT.ClickMouse();
                    }
                    else
                    {
                        VerifyAndResetSelectionChanged(0, null);
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.Assert(item.IsExpanded, "item 0 should be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                        DRT.Assert(_hierarchicalDB.SelectedItem == GetItem(_hierarchicalDB, 0), "SelectedItem is not the item 0");
                    }
                    break;

                case MouseSteps.ClickSelectChild:
                    if (_go)
                    {
                        TreeViewItem item = GetContainer(_hierarchicalDB, new int[] { 0, 1 });
                        DRT.MoveMouse(item, 0.5, 0.5);
                        DRT.ClickMouse();
                    }
                    else
                    {
                        TreeViewItem item = GetContainer(_hierarchicalDB, new int[] { 0, 1 });
                        object selectedItem = GetItem(item);
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                        DRT.Assert(_hierarchicalDB.SelectedItem == selectedItem, "SelectedItem is not the item 0,1");
                        VerifyAndResetSelectionChanged(1, selectedItem);
                    }
                    break;

                case MouseSteps.ClickCollapse:
                    if (_go)
                    {
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        DRT.MoveMouse(item, 0.09, 0.1);
                        DRT.ClickMouse();
                    }
                    else
                    {
                        TreeViewItem item = GetContainer(_hierarchicalDB, 0);
                        object selectedItem = GetItem(item);
                        DRT.Assert(!item.IsExpanded, "item 0 should not be expanded");
                        DRT.Assert(item.IsSelected, "item 0 should be selected");
                        DRT.Assert(_hierarchicalDB.SelectedItem == selectedItem, "SelectedItem is not the item 0");
                        VerifyAndResetSelectionChanged(1, selectedItem);
                    }
                    break;

                case MouseSteps.End:
                    break;
            }

            if (_mouseStep != MouseSteps.End)
            {
                if (_go)
                {
                    _go = false; // Verify this step
                }
                else
                {
                    _go = true; // Run next step
                    _mouseStep++;
                }
                DRT.ResumeAt(new DrtTest(MouseStep));
            }
            else
            {
                RemoveSelectedItemChangedHandler(_hierarchicalDB);
            }
        }

        #endregion

        #region BringIntoView

        private void BringIntoView()
        {
            Console.WriteLine("TEST: BringIntoView");

            TreeViewItem top = (TreeViewItem)_tv.Items[0];
            TreeViewItem middle = (TreeViewItem)top.Items[1];
            TreeViewItem bottom = (TreeViewItem)middle.Items[2];

            DRT.Assert(!top.IsExpanded, "My Computer shouldn't be expanded");
            DRT.Assert(!middle.IsExpanded, "Hard Drive shouldn't be expanded");
            DRT.Assert(!bottom.IsExpanded, "autoexec.bat shouldn't be expanded");

            bottom.BringIntoView();

            DRT.Assert(top.IsExpanded, "My Computer should be expanded");
            DRT.Assert(middle.IsExpanded, "Hard Drive should be expanded");
            DRT.Assert(!bottom.IsExpanded, "autoexec.bat should not be expanded");
        }

        #endregion

        #region SelectedItemChanged

        private void VerifyAndResetSelectionChanged(int number, object item)
        {
            DRT.Assert(_selectionFired == number, "SelectedItemChanged should have fired " + number + " times. Was " + _selectionFired);
            DRT.Assert(_lastSelectedItem == item, "SelectedItemChanged should have fired with " + MakeString(item) + ". Was " + MakeString(_lastSelectedItem));
            _selectionFired = 0;
            _lastSelectedItem = null;
        }

        private static string MakeString(object o)
        {
            if (o == null)
            {
                return "null";
            }
            else
            {
                return o.ToString();
            }
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _selectionFired++;
            _lastSelectedItem = e.NewValue;
        }

        private void AddSelectedItemChangedHandler(TreeView tree)
        {
            tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(OnSelectedItemChanged);

        }

        private void RemoveSelectedItemChangedHandler(TreeView tree)
        {
            tree.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(OnSelectedItemChanged);
        }

        #endregion

        #region TreeViewItem

        private TreeViewItem GetContainer(ItemsControl parent, int[] indexes)
        {
            TreeViewItem item = null;
            foreach (int index in indexes)
            {
                item = GetContainer(parent, index);
                if (item == null)
                {
                    return null;
                }

                parent = item;
            }

            return item;
        }

        private TreeViewItem GetContainer(ItemsControl parent, int index)
        {
            return parent.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
        }

        private object GetItem(TreeViewItem container)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(container);
            return parent.ItemContainerGenerator.ItemFromContainer(container);
        }

        private object GetItem(ItemsControl parent, int index)
        {
            TreeViewItem container = GetContainer(parent, index);
            if (container != null)
            {
                return GetItem(container);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Data

        private TreeView _hierarchicalDB;
        private TreeView _tv;
        private TreeViewItem _cp;
        private CheckBox _isSelectedCB;
        private ListBox _bindingSource;
        private TreeView _itemBound;

        private int _selectionFired = 0;
        private object _lastSelectedItem = null;

        private bool _go = true;

        private KeyboardSteps _keyboardStep = KeyboardSteps.Start;
        private MouseSteps _mouseStep = MouseSteps.Start;

        #endregion
    }
}

