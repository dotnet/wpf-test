using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

#if TESTBUILD_NET_ATLEAST_45
using System.Windows.Controls.Ribbon;
#endif

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Valid test scenarios
    /// It covers key below only because only these keys will cause directional navigation
    /// Down, Up, Right, Left
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(1, "KeyboardNavigation", "NavigationWithHyperlink", Versions="4.7.1+")]
    public class NavigationWithHyperlink : DirectionalNavigationTestBase
    {

        [Variation("NavigationWithHyperlink.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue)]
        public NavigationWithHyperlink(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode)
            : base(fileName, rootElementFlowDirection, keyboardNavigationMode)
        {
            InitializeSteps += new TestStep(LocalSetup);
        }

        private TestResult LocalSetup()
        {
            SetupDataGrid();

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            return TestResult.Pass;
        }

        public override TestResult RunBatchCore(KeyboardNavigationValidator validator)
        {
            _result = TestResult.Pass;

            foreach (DirectionalNavigationKey dnk in Enum.GetValues(typeof(DirectionalNavigationKey)))
            {
                switch (dnk)
                {
                    case DirectionalNavigationKey.Up:
                    case DirectionalNavigationKey.Down:
                    case DirectionalNavigationKey.Left:
                    case DirectionalNavigationKey.Right:
                        Key key = (Key)Enum.Parse(typeof(Key), dnk.ToString(), true);

                        TestTreeView(key, validator);
#if TESTBUILD_NET_ATLEAST_48
                        TestListBox(key, validator);
#endif
                        TestDataGrid(key, validator);
#if TESTBUILD_NET_ATLEAST_45
                        TestRibbonApplicationMenu(key, validator);
                        TestRibbonTab(key, validator);
                        TestRibbonGallery(key, validator);
#endif
                        break;
                    default:
                        break;
                }
            }

            return _result;
        }

        #region TreeView

        void TestTreeView(Key key, KeyboardNavigationValidator validator)
        {
            TreeView tv = FindElement<TreeView>("tv");
            foreach (object o in tv.Items)
            {
                TestTreeViewItem((TreeViewItem)o, key, validator);
            }
        }

        void TestTreeViewItem(TreeViewItem tvi, Key key, KeyboardNavigationValidator validator)
        {
            // verify navigation from all focusable elements
            List<DependencyObject> focusableElements = GetFocusableElements(tvi);
            for (int i=0, n=focusableElements.Count; i<n; ++i)
            {
                DependencyObject fromElement = focusableElements[i];
                DependencyObject toElement = GetExpectedFocus(tvi, focusableElements, i, key);
                if (toElement != null)
                {
                    tvi.IsSelected = true;  // ensure that tvi tries to handle the key
                    Validate(validator, fromElement, toElement, key);
                }
            }

            // verify navigation recursively in children
            foreach (object o in tvi.Items)
            {
                TestTreeViewItem((TreeViewItem)o, key, validator);
            }
        }

        List<DependencyObject> GetFocusableElements(TreeViewItem tvi)
        {
            List<DependencyObject> list = new List<DependencyObject>();
            list.Add(tvi);
            AddFocusableElements(list, tvi.Header as DependencyObject);
            return list;
        }

        DependencyObject GetExpectedFocus(TreeViewItem tvi, List<DependencyObject> list, int i, Key key)
        {
            DependencyObject result = null;
            TreeViewItem neighbor;

            switch (key)
            {
                case Key.Up:
                    if (i == 0)             // from=tvi, to=last child
                    {
                        result = list[list.Count - 1];
                    }
                    else if (i == 1)        // from=first child, to=previous tvi (or self)
                    {
                        neighbor = GetPreviousTreeViewItem(tvi);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    else                    // from=other child, to=previous child
                    {
                        result = list[i-1];
                    }
                    break;
                case Key.Down:
                    if (i == list.Count-1)  // from=last child, to=next tvi (or self)
                    {
                        neighbor = GetNextTreeViewItem(tvi);
                        result = (neighbor != null) ? neighbor : list[list.Count-1];
                    }
                    else                    // from=other child, to=next child
                    {
                        result = list[i+1];
                    }
                    break;
                case Key.Left:
                    if (tvi.IsExpanded)     // ignore - Left will collapse
                    {
                        result = null;
                    }
                    else                    // from=any (in leaf TVI), to=parent tvi
                    {
                        result = GetParentTreeViewItem(tvi);
                    }
                    break;
                case Key.Right:
                    if (i==0)               // from=tvi, to=first child
                    {
                        result = list[1];
                    }
                    else if (i == list.Count-1 && tvi.IsExpanded)
                    {                       // from=last child of expanded node, to=next tvi (=first child tvi)
                        result = GetNextTreeViewItem(tvi);
                    }
                    else                    // from=other, to=depends on width of current focus - can't test reliably
                    {
                        result = null;
                    }
                    break;
            }
            return result;
        }

        TreeViewItem GetPreviousTreeViewItem(TreeViewItem tvi)
        {
            ItemsControl current = tvi;
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(current);
            int index = parent.Items.IndexOf(current);

            // if tvi is a first child, return parent
            if (index == 0)
            {
                return parent as TreeViewItem;
            }

            // otherwise find last leaf in left-sibling's subtree
            tvi = parent.Items[index - 1] as TreeViewItem;
            while (tvi.Items.Count > 0)
            {
                tvi = tvi.Items[parent.Items.Count-1] as TreeViewItem;
            }
            return tvi;
        }

        TreeViewItem GetNextTreeViewItem(TreeViewItem tvi)
        {
            // if tvi has children, return the first child
            if (tvi.Items.Count > 0)
            {
                return tvi.Items[0] as TreeViewItem;
            }

            // otherwise find the nearest ancestor that has more children
            ItemsControl current = tvi;
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(current);
            int index = parent.Items.IndexOf(current);
            while (index == parent.Items.Count - 1)
            {
                current = parent;
                parent = ItemsControl.ItemsControlFromItemContainer(current);
                if (parent == null)     // last tvi - return null
                    return null;
                index = parent.Items.IndexOf(current);
            }

            // return its next child
            return parent.Items[index+1] as TreeViewItem;
        }

        TreeViewItem GetParentTreeViewItem(TreeViewItem tvi)
        {
            return ItemsControl.ItemsControlFromItemContainer(tvi) as TreeViewItem;
        }

        #endregion TreeView

        #region ListBox

        void TestListBox(Key key, KeyboardNavigationValidator validator)
        {
            ListBox lb = FindElement<ListBox>("lb");
            foreach (object o in lb.Items)
            {
                TestListBoxItem((ListBoxItem)o, key, validator);
            }
        }

        void TestListBoxItem(ListBoxItem lbi, Key key, KeyboardNavigationValidator validator)
        {
            // verify navigation from all focusable elements
            List<DependencyObject> focusableElements = GetFocusableElements(lbi);
            for (int i=0, n=focusableElements.Count; i<n; ++i)
            {
                DependencyObject fromElement = focusableElements[i];
                DependencyObject toElement = GetExpectedFocus(lbi, focusableElements, i, key);
                if (toElement != null)
                {
                    Validate(validator, fromElement, toElement, key);
                }
            }
        }

        List<DependencyObject> GetFocusableElements(ListBoxItem lbi)
        {
            List<DependencyObject> list = new List<DependencyObject>();
            AddFocusableElements(list, lbi as DependencyObject);
            return list;
        }

        DependencyObject GetExpectedFocus(ListBoxItem lbi, List<DependencyObject> list, int i, Key key)
        {
            DependencyObject result = null;
            ListBoxItem neighbor;

            switch (key)
            {
                case Key.Up:                // to=previous lbi (or self)
                    neighbor = GetNeighborContainer<ListBoxItem>(lbi, -1);
                    result = (neighbor != null) ? neighbor : list[i];
                    break;
                case Key.Down:              // to=next lbi (or self)
                    neighbor = GetNeighborContainer<ListBoxItem>(lbi, +1);
                    result = (neighbor != null) ? neighbor : list[i];
                    break;
                case Key.Left:              // to=self
                case Key.Right:
                    result = list[i];
                    break;
            }
            return result;
        }

        #endregion ListBox

        #region DataGrid

        DataGrid _dg;

        public class MyItem
        {
            public MyItem(Uri u) { URL = u; }
            public Uri URL { get; private set; }
        }

        void SetupDataGrid()
        {
            ObservableCollection<MyItem> list = new ObservableCollection<MyItem>();
            list.Add(new MyItem(new Uri("http://microsoft.com")));
            list.Add(new MyItem(new Uri("http://wikipedia.com")));

            _dg = FindElement<DataGrid>("dg");
            _dg.ItemsSource = list;
        }

        void TestDataGrid(Key key, KeyboardNavigationValidator validator)
        {
            ItemContainerGenerator generator = _dg.ItemContainerGenerator;

            foreach (object o in _dg.Items)
            {
                TestDataGridRow((DataGridRow)generator.ContainerFromItem(o), key, validator);
            }

            // test the headers too
            TestDataGridRow(null, key, validator);
        }

        void TestDataGridRow(DataGridRow row, Key key, KeyboardNavigationValidator validator)
        {
            for (int i=0, n=_dg.Columns.Count; i<n; ++i)
            {
                TestDataGridCell(row, i, key, validator);
            }
        }

        void TestDataGridCell(DataGridRow row, int colIndex, Key key, KeyboardNavigationValidator validator)
        {
            FrameworkElement cell = GetCell(row, colIndex);

            // verify navigation from all focusable elements
            List<DependencyObject> focusableElements = GetFocusableElementsDG(cell);
            for (int i=0, n=focusableElements.Count; i<n; ++i)
            {
                DependencyObject fromElement = focusableElements[i];
                DependencyObject toElement = GetExpectedFocus(row, colIndex, focusableElements, i, key);
                if (toElement != null)
                {
                    Validate(validator, fromElement, toElement, key);
                }
            }
        }

        FrameworkElement GetCell(DataGridRow row, int colIndex)
        {
            if (row != null)
            {
                DataGridCellsPresenter cp = VisualTreeHelper.GetVisualChild<DataGridCellsPresenter>(row);
                return (FrameworkElement)cp.ItemContainerGenerator.ContainerFromIndex(colIndex);
            }
            else
            {
                return (FrameworkElement)_dg.Columns[colIndex].Header;
            }
        }

        List<DependencyObject> GetFocusableElementsDG(FrameworkElement cell)
        {
            List<DependencyObject> list = new List<DependencyObject>();
            AddFocusableElements(list, cell);
            return list;
        }

        DependencyObject GetExpectedFocus(DataGridRow row, int colIndex, List<DependencyObject> list, int i, Key key)
        {
            DependencyObject result = null;
            FrameworkElement neighbor;

            switch (key)
            {
                case Key.Up:
                    if (i < 2)              // from=cell or first child, to=previous cell
                    {
                        result = GetNeighborCell(row, colIndex, 0, -1);
                    }
                    else                    // from=other child, to=previous child
                    {
                        result = list[i-1];
                    }
                    break;
                case Key.Down:
                    if (i==0 || i==list.Count-1)    // from=cell or last child, to=next cell
                    {
                        result = GetNeighborCell(row, colIndex, 0, +1);
                    }
                    else                    // from=other child, to=next child
                    {
                        result = list[i+1];
                    }
                    break;
                case Key.Left:
                    if (row != null)        // from in normal cell, to=left-cell (or self)
                    {
                        neighbor = GetNeighborCell(row, colIndex, -1, 0);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    else                    // from in header, to=left-cell of first row (or self)
                    {
                        neighbor = GetNeighborCell(row, colIndex, -1, +1);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    break;
                case Key.Right:
                    if (row != null)        // from in normal cell, to=right-cell (or self)
                    {
                        neighbor = GetNeighborCell(row, colIndex, +1, 0);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    else                    // from in header, to=right-cell of first row (or self)
                    {
                        neighbor = GetNeighborCell(row, colIndex, +1, +1);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    break;
            }
            return result;
        }

        FrameworkElement GetNeighborCell(DataGridRow row, int colIndex, int deltaCol, int deltaRow)
        {
            ItemContainerGenerator generator = _dg.ItemContainerGenerator;
            int rowIndex = (row == null) ? -1 : generator.IndexFromContainer(row);
            rowIndex += deltaRow;
            colIndex += deltaCol;

            if (rowIndex < -1 || rowIndex >= _dg.Items.Count || colIndex < 0 || colIndex >= _dg.Columns.Count)
            {
                return null;
            }
            else if (rowIndex == -1)
            {
                return (FrameworkElement)_dg.Columns[colIndex].Header;
            }
            else
            {
                row = (DataGridRow)generator.ContainerFromIndex(rowIndex);
                DataGridCellsPresenter cp = VisualTreeHelper.GetVisualChild<DataGridCellsPresenter>(row);
                return (FrameworkElement)cp.ItemContainerGenerator.ContainerFromIndex(colIndex);
            }
        }

        #endregion DataGrid

#if TESTBUILD_NET_ATLEAST_45
        #region RibbonApplicationMenu

        void TestRibbonApplicationMenu(Key key, KeyboardNavigationValidator validator)
        {
            Ribbon ribbon = FindElement<Ribbon>("rb");
            RibbonApplicationMenu ram = ribbon.ApplicationMenu;
            ram.IsDropDownOpen = true;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            foreach (object o in ram.Items)
            {
                TestRibbonApplicationMenuItem((RibbonApplicationMenuItem)o, key, validator);
            }

            ram.IsDropDownOpen = true;
        }

        void TestRibbonApplicationMenuItem(RibbonApplicationMenuItem rami, Key key, KeyboardNavigationValidator validator)
        {
            List<DependencyObject> focusableElements = new List<DependencyObject>();
            AddFocusableElements(focusableElements, rami);
            for (int i=0, n=focusableElements.Count; i<n; ++i)
            {
                DependencyObject fromElement = focusableElements[i];
                DependencyObject toElement = GetExpectedFocus(rami, focusableElements, i, key);
                if (toElement != null)
                {
                    rami.Focus();       // ensure test starts from a known state
                    Validate(validator, fromElement, toElement, key);
                }
            }
        }

        DependencyObject GetExpectedFocus(RibbonApplicationMenuItem rami, List<DependencyObject> list, int i, Key key)
        {
            DependencyObject result = null;
            FrameworkElement neighbor;

            switch (key)
            {
                case Key.Up:
                    if (i==0)               // from=rami, to=previous rami
                    {
                        result = GetNeighbor(rami, -1, wrap:true);
                    }
                    else                    // from=child, to=previous rami (or previous child)
                    {
                        neighbor = GetNeighbor(rami, -1, wrap:false);
                        result = (neighbor != null) ? neighbor :
                                 (i == 1) ? list[i] : list[i-1];
                    }
                    break;
                case Key.Down:
                    if (i==0)               // from=rami, to=next rami
                    {
                        result = GetNeighbor(rami, +1, wrap:true);
                    }
                    else                    // from=child, to=next rami (or next child)
                    {
                        neighbor = GetNeighbor(rami, +1, wrap:false);
                        result = (neighbor != null) ? neighbor :
                                 (i < list.Count-1) ? list[i+1] : list[i];
                    }
                    break;
                case Key.Left:
                case Key.Right:             // from=all, to=self
                    result = list[i];
                    break;
            }

            return result;
        }

        FrameworkElement GetNeighbor(RibbonApplicationMenuItem rami, int delta, bool wrap)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(rami);
            ItemContainerGenerator generator = parent.ItemContainerGenerator;
            int index = generator.IndexFromContainer(rami) + delta;
            int n = parent.Items.Count;
            if (wrap)
            {
                while (index < 0)   index += n;
                while (index >= n)  index -= n;
            }
            if (index < 0 || index >= n)
                return null;
            else
                return generator.ContainerFromIndex(index) as FrameworkElement;
        }

        #endregion RibbonApplicationMenu

        #region RibbonTab

        void TestRibbonTab(Key key, KeyboardNavigationValidator validator)
        {
            Ribbon ribbon = FindElement<Ribbon>("rb");
            RibbonTabHeaderItemsControl tabHeaders = VisualTreeHelper.GetVisualChild<RibbonTabHeaderItemsControl>(ribbon);
            ItemContainerGenerator generator = tabHeaders.ItemContainerGenerator;
            for (int i=0, n=tabHeaders.Items.Count; i<n-1; ++i)  // skip last tab = RibbonGallery
            {
                RibbonTabHeader rth = (RibbonTabHeader)generator.ContainerFromIndex(i);
                TestRibbonTabHeader(rth, key, validator);
            }
        }

        void TestRibbonTabHeader(RibbonTabHeader rth, Key key, KeyboardNavigationValidator validator)
        {
            List<DependencyObject> focusableElements = new List<DependencyObject>();
            AddFocusableElements(focusableElements, rth);
            for (int i=0, n=focusableElements.Count; i<n; ++i)
            {
                DependencyObject fromElement = focusableElements[i];
                DependencyObject toElement = GetExpectedFocus(rth, focusableElements, i, key);
                if (toElement != null)
                {
                    Validate(validator, fromElement, toElement, key);
                }
            }
        }

        DependencyObject GetExpectedFocus(RibbonTabHeader rth, List<DependencyObject> list, int i, Key key)
        {
            DependencyObject result = null;

            switch (key)
            {
                case Key.Up:
                    if (i==0)               // from=rth, to=last child
                    {
                        result = list[list.Count-1];
                    }
                    else                    // from=child, to=rth
                    {
                        result = list[0];
                    }
                    break;
                case Key.Down:
                    if (i<list.Count-1)     // from=any, to=next child
                    {
                        result = list[i+1];
                    }
                    else                    // from=last child, to=self
                    {
                        result = list[i];
                    }
                    break;
                case Key.Left:
                    if (i==0)               // from=rth, to=widest child
                    {
                        // can't really test this, as we cannot control the
                        // width of a hyperlink.  So skip this case.
                        result = null;
                    }
                    else                    // from=child, to=previous rth
                    {
                        result = GetNeighbor(rth, -1);
                    }
                    break;
                case Key.Right:
                    if (i==0)               // from=rth, to=first child
                    {
                        result = list[1];
                    }
                    else                    // from=child, to=next rth
                    {
                        result = GetNeighbor(rth, +1);
                    }
                    break;
            }

            return result;
        }

        FrameworkElement GetNeighbor(RibbonTabHeader rth, int delta)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(rth);
            ItemContainerGenerator generator = parent.ItemContainerGenerator;
            int index = generator.IndexFromContainer(rth) + delta;
            int n = parent.Items.Count;

            if (index < 0 || index >= n-1)  // ignoring last tab header (= RibbonGallery)
                return null;
            else
                return generator.ContainerFromIndex(index) as FrameworkElement;
        }

        #endregion RibbonTab

        #region RibbonGallery

        void TestRibbonGallery(Key key, KeyboardNavigationValidator validator)
        {
            Ribbon ribbon = FindElement<Ribbon>("rb");
            RibbonGallery ribbonGallery = VisualTreeHelper.GetVisualChild<RibbonGallery>(ribbon);
            ItemContainerGenerator generator = ribbonGallery.ItemContainerGenerator;
            for (int i=0, n=ribbonGallery.Items.Count; i<n; ++i)
            {
                RibbonGalleryCategory rgc = (RibbonGalleryCategory)generator.ContainerFromIndex(i);
                TestRibbonGalleryCategory(rgc, key, validator);
            }
        }

        void TestRibbonGalleryCategory(RibbonGalleryCategory rgc, Key key, KeyboardNavigationValidator validator)
        {
            ItemContainerGenerator generator = rgc.ItemContainerGenerator;
            for (int i=0, n=rgc.Items.Count; i<n; ++i)
            {
                RibbonGalleryItem rgi = (RibbonGalleryItem)generator.ContainerFromIndex(i);
                TestRibbonGalleryItem(rgi, key, validator);
            }
        }

        void TestRibbonGalleryItem(RibbonGalleryItem rgi, Key key, KeyboardNavigationValidator validator)
        {
            List<DependencyObject> focusableElements = new List<DependencyObject>();
            AddFocusableElements(focusableElements, rgi);
            for (int i=0, n=focusableElements.Count; i<n; ++i)
            {
                DependencyObject fromElement = focusableElements[i];
                DependencyObject toElement = GetExpectedFocus(rgi, focusableElements, i, key);
                if (toElement != null)
                {
                    Validate(validator, fromElement, toElement, key);
                }
            }
        }

        DependencyObject GetExpectedFocus(RibbonGalleryItem rgi, List<DependencyObject> list, int i, Key key)
        {
            DependencyObject result = null;
            FrameworkElement neighbor;

            switch (key)
            {
                case Key.Up:
                    if (i==0)               // from=rgi, to=last child
                    {
                        result = list[list.Count-1];
                    }
                    else if (i==1)          // from=first child, to=rgi from previous category (or self)
                    {
                        neighbor = GetCategoryNeighbor(rgi, -1);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    else                    // from=other child, to=previous child
                    {
                        result = list[i-1];
                    }
                    break;
                case Key.Down:
                    if (i<list.Count-1)     // from=any, to=next child
                    {
                        result = list[i+1];
                    }
                    else                    // from=last child, to=rgi from next category (or self)
                    {
                        neighbor = GetCategoryNeighbor(rgi, +1);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    break;
                case Key.Left:
                    if (i==0)               // from=rgi, to=widest child
                    {
                        // can't really test this, as we cannot control the
                        // width of a hyperlink.  So skip this case.
                        result = null;
                    }
                    else                    // from=child, to=previous rgi (or self)
                    {
                        neighbor = GetNeighborContainer<RibbonGalleryItem>(rgi, -1);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    break;
                case Key.Right:
                    if (i==0)               // from=rgi, to=first child
                    {
                        result = list[1];
                    }
                    else                    // from=child, to=next rgi (or self)
                    {
                        neighbor = GetNeighborContainer<RibbonGalleryItem>(rgi, +1);
                        result = (neighbor != null) ? neighbor : list[i];
                    }
                    break;
            }

            return result;
        }

        FrameworkElement GetCategoryNeighbor(RibbonGalleryItem rgi, int delta)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(rgi);
            ItemContainerGenerator generator = parent.ItemContainerGenerator;
            int itemIndex = generator.IndexFromContainer(rgi);

            ItemsControl parent2 = ItemsControl.ItemsControlFromItemContainer(parent);
            ItemContainerGenerator generator2 = parent2.ItemContainerGenerator;
            int categoryIndex = generator2.IndexFromContainer(parent) + delta;
            int categoryCount = parent2.Items.Count;

            if (categoryIndex < 0 || categoryIndex >= categoryCount)
                return null;

            RibbonGalleryCategory rgc = (RibbonGalleryCategory)generator2.ContainerFromIndex(categoryIndex);
            int itemCount = rgc.Items.Count;
            if (itemIndex < 0 || itemIndex >= itemCount)
                return null;
            else
                return rgc.ItemContainerGenerator.ContainerFromIndex(itemIndex) as FrameworkElement;
        }

        #endregion RibbonGallery
#endif

        T FindElement<T>(string name)
        {
            T element = (T)RootElement.FindName(name);
            if (element == null)
            {
                throw new NullReferenceException(String.Format("Fail: cannot find element named '{0}'.", name));
            }
            return element;
        }

        T GetNeighborContainer<T>(T container, int delta) where T : DependencyObject
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(container);
            ItemContainerGenerator generator = parent.ItemContainerGenerator;
            int index = generator.IndexFromContainer(container) + delta;

            if (index < 0) return null; // CFI returns null if index is too large, but throws if index is too small
            return (T)generator.ContainerFromIndex(index);
        }

        // add focusable elements under d to the list
        void AddFocusableElements(List<DependencyObject> list, DependencyObject d)
        {
            if (d == null) return;

            // add d itself
            if ((bool)d.GetValue(UIElement.FocusableProperty))
            {
                if (!(d is TextBoxBase))    // TextBox handles keynav itself, don't include it
                {
                    list.Add(d);
                }
            }

            // add d's descendants
            IContentHost ich;
            Visual v;
            if ((ich = d as IContentHost) != null)
            {
                // case for elements that can host ContentElements (e.g. TextBlock hosts Hyperlink)
                IEnumerator<IInputElement> enumerator = ich.HostedElements;
                while (enumerator.MoveNext())
                {
                    IInputElement current = enumerator.Current;
                    AddFocusableElements(list, current as DependencyObject);
                }
            }
            else if ((v = d as Visual) != null)
            {
                // case for normal visual tree
                for (int i=0, n=System.Windows.Media.VisualTreeHelper.GetChildrenCount(v); i<n; ++i)
                {
                    AddFocusableElements(list, System.Windows.Media.VisualTreeHelper.GetChild(v, i));
                }
            }
        }

        void Validate(KeyboardNavigationValidator validator, DependencyObject fromElement, DependencyObject toElement, Key key)
        {
            string s = String.Format("key: {0}  from: {1} ({2})  to: {3} ({4})",
                key,
                fromElement.GetValue(FrameworkElement.NameProperty),
                fromElement.GetType().Name,
                (toElement == null) ? String.Empty : toElement.GetValue(FrameworkElement.NameProperty),
                (toElement == null) ? "null" : toElement.GetType().Name);
            LogComment(s);

            if (FakeValidate)
            {
                try
                {
                    validator.DirectionalNavigate(fromElement, toElement, key);
                }
                catch (Exception ex)
                {
                    toElement = System.Windows.Input.Keyboard.FocusedElement as DependencyObject;
                    s = String.Format("  Fail.  Actual focus: {0} ({1})  Msg: '{2}'",
                        (toElement == null) ? String.Empty : toElement.GetValue(FrameworkElement.NameProperty),
                        (toElement == null) ? "null" : toElement.GetType().Name,
                        ex.Message);
                    LogComment(s);
                    _result = TestResult.Fail;
                }
            }
            else
            {
                validator.DirectionalNavigate(fromElement, toElement, key);
            }
        }

        static bool FakeValidate
        {
            get { return _fakeValidate; }
            set { _fakeValidate = value; }
        }

        TestResult _result;

        // set this to true when debugging the test, to show more details in the log,
        // and to continue past the first error to find them all
        static bool _fakeValidate = false;

    }
}


