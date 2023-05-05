// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests ItemsControl Alternation for ListBox, a ListBox with grouping, and TreeView.
    /// </description>
    /// </summary>
    [Test(1, "Controls", "AlternatingRows")]
    public class AlternatingRows : XamlTest
    {
        #region Private Data

        private ItemsControl _itemsControl;
        private string _itemsControlType;

        #endregion

        #region Constructors


        [Variation("ListBox")]
        [Variation("ListBoxWithGrouping")]
        [Variation("TreeView")]
        public AlternatingRows(string icName)
            : base(@"AlternatingRows.xaml")
        {
            this._itemsControlType = icName;

            InitializeSteps += new TestStep(Setup);

            RunSteps += new TestStep(AlternationConverterTest);

            RunSteps += new TestStep(AlternationCount);
            RunSteps += new TestStep(AlternationCountForwarding);

            RunSteps += new TestStep(ChangeCollection);
            RunSteps += new TestStep(ChangeCollectionTreeView);

            RunSteps += new TestStep(Scrolling);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            string nameToFind;

            switch (_itemsControlType)
            {
                case "ListBox":
                    nameToFind = "lb";
                    break;
                case "ListBoxWithGrouping":
                    nameToFind = "lbGrouping";
                    break;
                case "TreeView":
                    nameToFind = "tv";
                    break;
                default:
                    return TestResult.Fail;
            }

            _itemsControl = (ItemsControl)RootElement.FindName(nameToFind);

            if (_itemsControlType == "ListBoxGrouping")
            {
                CollectionViewSource.GetDefaultView(_itemsControl.Items).GroupDescriptions.Add(new PropertyGroupDescription("State"));
            }

            // If it is a ListBox, add a bunch of additional items to make sure we exercise UI Virtualization
            if (_itemsControl is ListBox)
            {
                Places p = (Places)_itemsControl.ItemsSource;

                for (int i = 0; i < 100; i++)
                {
                    p.Add(new Place("Name" + i, "State" + (i % 10)));
                }
            }

            return TestResult.Pass;
        }

        private TestResult AlternationConverterTest()
        {
            AlternationConverter ac = new AlternationConverter();
            
            if (ac.Values.Count != 0) return TestResult.Fail;
            if (ac.Convert(0, null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;
            if (ac.Convert(-1, null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;
            if (ac.Convert("0", null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;
            if (ac.Convert(new Place(), null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;

            ac.Values.Add("Red");

            if (ac.Values.Count != 1) return TestResult.Fail;
            if ((string)ac.Convert(0, null, null, null) != "Red") return TestResult.Fail;
            if ((string)ac.Convert(1, null, null, null) != "Red") return TestResult.Fail;
            if ((string)ac.Convert(-1, null, null, null) != "Red") return TestResult.Fail;
            if (ac.Convert("0", null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;
            if (ac.Convert(new Place(), null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;

            ac.Values.Add("Blue");

            if (ac.Values.Count != 2) return TestResult.Fail;
            if ((string)ac.Convert(0, null, null, null) != "Red") return TestResult.Fail;
            if ((string)ac.Convert(1, null, null, null) != "Blue") return TestResult.Fail;
            if ((string)ac.Convert(2, null, null, null) != "Red") return TestResult.Fail;
            if ((string)ac.Convert(-1, null, null, null) != "Blue") return TestResult.Fail;
            if (ac.Convert("0", null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;
            if (ac.Convert(new Place(), null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;

            ac.Values.Add("Green");

            if (ac.Values.Count != 3) return TestResult.Fail;
            if ((string)ac.Convert(0, null, null, null) != "Red") return TestResult.Fail;
            if ((string)ac.Convert(1, null, null, null) != "Blue") return TestResult.Fail;
            if ((string)ac.Convert(2, null, null, null) != "Green") return TestResult.Fail;
            if ((string)ac.Convert(3, null, null, null) != "Red") return TestResult.Fail;
            if ((string)ac.Convert(-1, null, null, null) != "Green") return TestResult.Fail;
            if (ac.Convert("0", null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;
            if (ac.Convert(new Place(), null, null, null) != DependencyProperty.UnsetValue) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult AlternationCount()
        {
            _itemsControl.AlternationCount = 3;
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            _itemsControl.AlternationCount = 2;
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            _itemsControl.AlternationCount = 1;
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            _itemsControl.AlternationCount = 0;
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            _itemsControl.AlternationCount = -1;
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;

            // Set back to our normal alternation count
            _itemsControl.AlternationCount = 2;

            return TestResult.Pass;
        }

        private TestResult AlternationCountForwarding()
        {
            // AlternationCount in a HierarchicalDataTemplate or GroupStyle is supposed to be forwarded to children.
            // For Grouping the VerifyAlternation method looks at ItemsControl.GroupStyle[0].AlternationCount explicitly,
            // so the verification there accounts for checking forwarding. HDT needs to be explicitly accounted for since
            // there is no conveniently corresponding property.
            if (_itemsControl is TreeView)
            {
                TreeViewItem tvi = (TreeViewItem)_itemsControl.ItemContainerGenerator.ContainerFromIndex(0);
                if (!tvi.IsExpanded)
                {
                    tvi.IsExpanded = true;
                    WaitForPriority(DispatcherPriority.SystemIdle);
                }

                for (int i = 0; i < tvi.Items.Count; i++)
                {
                    TreeViewItem childTvi = (TreeViewItem)tvi.ItemContainerGenerator.ContainerFromIndex(i);
                    if (childTvi.AlternationCount != 0) return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        private TestResult ChangeCollection()
        {
            // For ListBox we are using the Places data source, so this step applies.
            // For TreeView we used the Earth data source because the data needed to
            // be hierarchical, so for this particular TestStep there is a TreeView
            // specific version.
            if (_itemsControl as TreeView != null) return TestResult.Pass;

            Places p = (Places)_itemsControl.ItemsSource;

            p.Add(new Place("Brea", "CA"));
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.Insert(0, new Place("Roanoake", "VA"));
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.Move(0, 3);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.RemoveAt(5);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;

            // Do the same set of operations but for much larger indices so that they are virtualized.
            // Coverage for Regression Test - NullReferenceException when moving an unrealized item, when AlternationCount is set
            // The test was for move specifically, but we'll add coverage for the other ones for completeness.
            p.Insert(40, new Place("Harper's Ferry", "WV"));
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.Move(41, 43);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.RemoveAt(45);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult ChangeCollectionTreeView()
        {
            // This is the TreeView specific version of ChangeCollection
            if (_itemsControl as TreeView == null) return TestResult.Pass;

            ObservableCollection<Hemisphere> p = (ObservableCollection<Hemisphere>)_itemsControl.ItemsSource;

            p.Add(new Hemisphere("Add Hemisphere", new ObservableCollection<Region>()));
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.Insert(0, new Hemisphere("Insert Hemisphere", new ObservableCollection<Region>()));
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.Move(0, 2);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            p.RemoveAt(1);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult Scrolling()
        {
            PropertyInfo info = typeof(ItemsControl).GetProperty("ScrollHost", BindingFlags.NonPublic | BindingFlags.Instance);
            ScrollViewer scroller = (ScrollViewer)info.GetValue(_itemsControl, null);
            double oldVerticalOffset;

            // page down to the end
            oldVerticalOffset = -1.0;
            while (scroller.VerticalOffset != oldVerticalOffset)
            {
                oldVerticalOffset = scroller.VerticalOffset;
                scroller.PageDown();
                WaitForPriority(DispatcherPriority.Background);
                if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            }

            double maxOffset = scroller.VerticalOffset;

            // page up to the beginning
            oldVerticalOffset = -1.0;
            while (scroller.VerticalOffset != oldVerticalOffset)
            {
                oldVerticalOffset = scroller.VerticalOffset;
                scroller.PageUp();
                WaitForPriority(DispatcherPriority.Background);
                if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            }

            // jump to the middle
            scroller.ScrollToVerticalOffset(maxOffset / 2.0);
            WaitForPriority(DispatcherPriority.Background);
            if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;

            // line-down a few times
            for (int i = 0; i < 30; ++i)
            {
                scroller.LineDown();
                WaitForPriority(DispatcherPriority.Background);
                if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            }

            // line-up a few times
            for (int i = 0; i < 60; ++i)
            {
                scroller.LineUp();
                WaitForPriority(DispatcherPriority.Background);
                if (!VerifyAlternation(_itemsControl)) return TestResult.Fail;
            }

            return TestResult.Pass;
        }



        private bool VerifyAlternation(ItemsControl itemsControl)
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            if (itemsControl.ItemsSource == null)
                return true;

            ReadOnlyObservableCollection<object> groups = CollectionViewSource.GetDefaultView(itemsControl.ItemsSource).Groups;

            if (groups == null)
            {
                return VerifyAlternation(itemsControl, itemsControl.ItemsSource);
            }
            else
            {
                return VerifyAlternation(itemsControl, groups);
            }
        }

        private bool VerifyAlternation(ItemsControl itemsControl, IEnumerable items)
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            int alternationCount = itemsControl.AlternationCount;

            if (itemsControl.IsGrouping)
            {
                alternationCount = itemsControl.GroupStyle[0].AlternationCount;
            }

            List<DependencyObject> itemContainers = new List<DependencyObject>();
            foreach (object item in items)
            {
                itemContainers.Add(itemsControl.ItemContainerGenerator.ContainerFromItem(item));
            }

            if (!VerifyAlternation(itemContainers, alternationCount)) return false;

            foreach (DependencyObject itemContainer in itemContainers)
            {
                if (itemContainer as ItemsControl != null)
                {
                    TreeViewItem treeViewItem = itemContainer as TreeViewItem;
                    if (treeViewItem != null && !treeViewItem.IsExpanded)
                    {
                        ((TreeViewItem)itemContainer).IsExpanded = true;
                        WaitForPriority(DispatcherPriority.SystemIdle);
                    }
                    if (!VerifyAlternation((ItemsControl)itemContainer)) return false;
                }

                if (itemContainer as GroupItem != null)
                {
                    ItemsPresenter ip = (ItemsPresenter)Util.FindVisualByType(typeof(ItemsPresenter), itemContainer);
                    Panel panel = (Panel)VisualTreeHelper.GetChild(ip, 0);
                    List<DependencyObject> children = new List<DependencyObject>();
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(panel); i++)
                    {
                        children.Add(VisualTreeHelper.GetChild(panel, i));
                    }

                    // This assumes that the children of the GroupItem are the ListBoxItems, a simplifying assumption for this test suite
                    // So we have a check to ensure this.
                    foreach (DependencyObject dependencyObject in children)
                    {
                        if (!(dependencyObject is ListBoxItem))
                        {
                            TestLog.Current.LogEvidence("This test suite is designed where the children of GroupItems are all ListBoxItems.", dependencyObject.GetType());
                            return false;
                        }
                    }

                    if (!VerifyAlternation(children, itemsControl.AlternationCount)) return false;
                }
            }

            return true;
        }

        private bool VerifyAlternation(IEnumerable<DependencyObject> items, int alternationCount)
        {
            int previousAlternationCount = -1;

            foreach (DependencyObject item in items)
            {
                if (item != null)
                {
                    if (alternationCount > 1)
                    {
                        if (ItemsControl.GetAlternationIndex(item) < 0)
                        {
                            TestLog.Current.LogEvidence("AlternationIndex should never be negative.", new object[] { item, ItemsControl.GetAlternationIndex(item) });
                            return false;
                        }

                        if (ItemsControl.GetAlternationIndex(item) > alternationCount - 1)
                        {
                            TestLog.Current.LogEvidence("AlternationIndex was larger than the expected range based upon the AlternationCount.", new object[] { item, ItemsControl.GetAlternationIndex(item), alternationCount });
                            return false;
                        }

                        if (ItemsControl.GetAlternationIndex(item) == previousAlternationCount)
                        {
                            TestLog.Current.LogEvidence("AlternationIndex of the item was the same as that of the previous item.", new object[] { item, previousAlternationCount });
                            return false;
                        }

                        previousAlternationCount = ItemsControl.GetAlternationIndex(item);
                    }
                    else
                    {
                        if (ItemsControl.GetAlternationIndex(item) != 0)
                        {
                            TestLog.Current.LogEvidence("AlternationIndex should always be zero when AlternationCount is less than or equal to one.", new object[] { item, ItemsControl.GetAlternationIndex(item) });
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
