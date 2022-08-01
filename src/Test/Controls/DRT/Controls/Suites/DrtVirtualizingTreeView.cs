// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text; // StringBuilder
using System.Threading;
using System.Xml;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace DRT
{
    public class DrtVirtualizingTreeViewSuite : DrtTestSuite
    {

        #region Initialization

        public DrtVirtualizingTreeViewSuite() : base("VirtualizingTreeView")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);
            DRT.LoadXamlFile("DrtVirtualizingTreeView.xaml");

            InitializeReferences();

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                {
                    new DrtTest(ScrollingTest)
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void InitializeReferences()
        {
            _treeView = DRT.FindElementByID("_treeView") as TreeView;
            DRT.Assert(_treeView != null, "TreeView not found");


            PropertyInfo info = typeof(ItemsControl).GetProperty("ScrollHost", BindingFlags.NonPublic | BindingFlags.Instance);
            _scrollViewer = (ScrollViewer)info.GetValue(_treeView, null);
        }

        #endregion

        #region Tests

        #region ScrollingTest

        private void ScrollingTest()
        {
            Console.WriteLine("TEST: ScrollingTest");
            _go = true;
            DRT.ResumeAt(new DrtTest(ScrollingStep));
        }

        private enum ScrollingSteps
        {
            Start,
            FirstPageDown,
            SecondPageDown,
            ThirdPageDown,
            PageUp,
            LineUp,
            End,
        }

        private void ScrollingStep()
        {
            Console.WriteLine("    STEP: " + _scrollingStep + (_go ? " (Action)" : " (Verify)"));

            if (_go)
            {
                switch (_scrollingStep)
                {
                    case ScrollingSteps.Start:
                        break;

                    case ScrollingSteps.FirstPageDown:
                    case ScrollingSteps.SecondPageDown:
                    case ScrollingSteps.ThirdPageDown:
                        _scrollViewer.PageDown();
                        break;

                    case ScrollingSteps.PageUp:
                        _scrollViewer.PageUp();
                        break;

                    case ScrollingSteps.LineUp:
                        _scrollViewer.LineUp();
                        break;

                    case ScrollingSteps.End:
                        break;
                }
            }
            else
            {
                // Output the state of the tree to help with debugging DRT failures
                Debug.WriteLine(_scrollingStep);
                DRT.LogOutput(_scrollingStep.ToString());
                WriteTree(_treeView, 0);

                VerifyStep();
            }


            if (_scrollingStep != ScrollingSteps.End)
            {
                if (_go)
                {
                    _go = false;
                }
                else
                {
                    _go = true; // Run next step
                    _scrollingStep++;
                }
                DRT.ResumeAt(new DrtTest(ScrollingStep));
            }
        }

        // Returns a list describing which TreeViewItems are expected to be alive after each step of the scrolling test
        private VerificationState GetVerificationState(ScrollingSteps step)
        {
            List<ContainerState> list = new List<ContainerState>();
           
            switch (step)
            {
                case ScrollingSteps.Start:

                    // items 1 through 30 alive
                    for (int i = 1; i <= 30; i++)
                    {
                        list.Add(new ContainerState(i, true));
                    }
                    list.Add(new ContainerState(31, false));
                    break;

                case ScrollingSteps.FirstPageDown:

                    // items 1 through 46
                    for (int i = 1; i <= 46; i++)
                    {
                        list.Add(new ContainerState(i, true));
                    }
                    list.Add(new ContainerState(47, false));
                    break;

                case ScrollingSteps.SecondPageDown:
                case ScrollingSteps.PageUp:             // page up after third page down should be equivalent to the 2nd page down

                    // 1, 10, 11, 15-60
                    list.Add(new ContainerState(1, true));
                    list.Add(new ContainerState(2, false));
                    list.Add(new ContainerState(9, false));
                    list.Add(new ContainerState(10, true));
                    list.Add(new ContainerState(11, true));
                    list.Add(new ContainerState(12, false));
                    list.Add(new ContainerState(14, false));

                    for (int i = 15; i <= 60; i++)
                    {
                        list.Add(new ContainerState(i, true));
                    }

                    list.Add(new ContainerState(61, false));
                    break;

                case ScrollingSteps.ThirdPageDown:

                    // 1, 30-74
                    list.Add(new ContainerState(1, true));
                    list.Add(new ContainerState(2, false));
                    list.Add(new ContainerState(29, false));

                    for (int i = 30; i <= 74; i++)
                    {
                        list.Add(new ContainerState(i, true));
                    }

                    list.Add(new ContainerState(75, false));
                    break;

                case ScrollingSteps.LineUp:

                    // 1, 10, 11, 15-59
                    list.Add(new ContainerState(1, true));
                    list.Add(new ContainerState(2, false));
                    list.Add(new ContainerState(9, false));
                    list.Add(new ContainerState(10, true));
                    list.Add(new ContainerState(11, true));
                    list.Add(new ContainerState(12, false));
                    list.Add(new ContainerState(14, false));

                    for (int i = 15; i <= 59; i++)
                    {
                        list.Add(new ContainerState(i, true));
                    }

                    list.Add(new ContainerState(60, false));
                    break;
            }


            return new VerificationState(list);
        }

        private void VerifyStep()
        {
            VerificationState state = GetVerificationState(_scrollingStep);

            Panel itemsHost = GetItemsHost(_treeView);
            UIElementCollection children = itemsHost.Children;

            for (int i = 0; i < children.Count; i++)
            {
                VerifyRecursive((TreeViewItem)children[i], ref state);
            }
        }

        #endregion

        #endregion
      

        #region Verification

        private void VerifyRecursive(TreeViewItem item, ref VerificationState state)
        {
            Panel itemsHost;
            int id = GetTreeViewItemId(item);

            //
            // Test this item against the verification list
            //

            DRT.Assert(VirtualizingStackPanel.GetIsVirtualizing(item) == true);

            while (state.MoveNext())
            {
                if (id > state.Current.Id)
                {
                    DRT.Assert(state.Current.IsAlive == false, 
                                string.Format("TreeViewItem with id {0} is virtualized.  Expected it to be alive", state.Current.Id));
                }
                else if (id == state.Current.Id)
                {
                    DRT.Assert(state.Current.IsAlive == true, 
                                string.Format("TreeViewItem with id {0} is alive.  Expected it to be virtualized", id));
                    break;
                }
                else
                {
                    // We went past this item.  It probably wasn't specified in the list.  No big deal, the next one will get it
                    state.MoveBack();
                    break;
                }
            }
            
            //
            // Now verify children
            //
            if (item.IsExpanded)
            {
                itemsHost = GetItemsHost(item);
                DRT.Assert(itemsHost != null);
                DRT.Assert(itemsHost is VirtualizingStackPanel, "All TreeViewItems under a virtualizing TreeView should use VSP as the itemshost");

                UIElementCollection children = itemsHost.Children;

                for (int i = 0; i < children.Count; i++)
                {
                    VerifyRecursive(children[i] as TreeViewItem, ref state);
                }
            }
        }


        #endregion

        #region Helpers

        private void WriteTree(ItemsControl item, int level)
        {
            Panel itemsHost = GetItemsHost(item);
            WriteTreeViewItem(item as TreeViewItem, level);

            if (itemsHost != null)
            {
                UIElementCollection children = itemsHost.Children;

                for (int i = 0; i < children.Count; i++)
                {
                    WriteTree((ItemsControl)children[i], level + 1);
                }
            }
        }

        private void WriteTreeViewItem(TreeViewItem item, int level)
        {
            if (item != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < level; i++)
                {
                    stringBuilder.Append("  ");
                }

                stringBuilder.Append(item);

                string output = stringBuilder.ToString();
                Debug.WriteLine(output);
                DRT.LogOutput(output);
            }
        }

        private int GetTreeViewItemId(TreeViewItem container)
        {
            int result = -1;

            XmlElement item = GetItem(container) as XmlElement;

            if (item != null)
            {
                Int32.TryParse(item.Attributes[0].Value, out result);
            }

            return result;
        }

        private static Panel GetItemsHost(ItemsControl itemsControl)
        {
            //return itemsControl.ItemsHost;
            return (Panel)typeof(ItemsControl).InvokeMember("ItemsHost",
                                                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                                                null, itemsControl, null);
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

        private TreeView        _treeView;
        private ScrollViewer    _scrollViewer;
        private ScrollingSteps  _scrollingStep = ScrollingSteps.Start;
        private bool            _go = true;

        #endregion

        private class VerificationState : IEnumerator<ContainerState>
        {
            public VerificationState(List<ContainerState> list)
            {
                _list = list.ToArray();
            }

            public ContainerState Current
            {
                get
                {
                    if (IsValid)
                    {
                        return _list[_currentIndex];
                    }

                    throw new InvalidOperationException("Current");
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }


            public void Dispose()
            {
            }

            public bool MoveBack()
            {
                if (IsValid)
                {
                    _currentIndex--;
                }

                return _currentIndex > -1;
            }

            public bool MoveNext()
            {
                if (_list!= null && _currentIndex < _list.Length - 1)
                {
                    _currentIndex++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            private bool IsValid
            {
                get
                {
                    return _list != null && _currentIndex >= 0 && _currentIndex < _list.Length;
                }
            }

            private int _currentIndex = -1;
            private ContainerState[] _list;
        }

        private struct ContainerState
        {
            public ContainerState(int id, bool isAlive)
            {
                _id = id;
                _isAlive = isAlive;
            }

            public int Id { get { return _id; } }
            public bool IsAlive { get { return _isAlive; } }

            private int _id;
            private bool _isAlive;
        }
    }
}

namespace DRT.DrtVirtualizingTreeView
{
    public class TestTreeView : System.Windows.Controls.TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TestTreeViewItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            TestTreeViewItem.PrepareContainer((TreeViewItem)element, item);
        }
    }

    public class TestTreeViewItem : TreeViewItem
    {

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TestTreeViewItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            PrepareContainer((TreeViewItem)element, item);
        }


        public override string ToString()
        {
            if (HasHeader)
            {
                return string.Format("TreeViewItem {0} {1}",
                    Header is System.Xml.XmlElement ? ((System.Xml.XmlElement)Header).Attributes[0].Value : Header.ToString(),
                    (Items != null && Items.Count > 0) ? "Child Count: " + Items.Count.ToString() : "");
            }
            else
            {
                return base.ToString();
            }
        }


        internal static void PrepareContainer(TreeViewItem element, object item)
        {
            Binding binding = new Binding();
            binding.XPath = "@IsExpanded";
            binding.Source = item;
            element.SetBinding(TreeViewItem.IsExpandedProperty, binding);

            binding = new Binding();
            binding.XPath = "@Height";
            binding.Source = item;
            element.SetBinding(TextBlock.FontSizeProperty, binding);
        }
    }
}

