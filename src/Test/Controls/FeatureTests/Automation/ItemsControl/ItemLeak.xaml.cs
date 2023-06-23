using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls.ItemLeak
{
    public partial class ItemLeak : Page
    {
        Model _model;

        public ItemLeak()
        {
            InitializeModel();
            InitializeComponent();
            InitializeFullTest();
        }

        void InitializeModel()
        {
            _model = new Model(25, 35, 20, 10);
            DataContext = _model;
        }

        private void DG_Add(object sender, RoutedEventArgs e)
        {
            _model.AddWideItem();
        }

        private void DG_Remove(object sender, RoutedEventArgs e)
        {
            _model.RemoveWideItem(dataGrid.SelectedIndex);
            WorkAroundDataGridLeak();
        }

        private void DG_Clear(object sender, RoutedEventArgs e)
        {
            _model.ClearWideItems();
            WorkAroundDataGridLeak();
        }

        private void DG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            object anchorItem = GetAnchorItem();
            tbAnchor.Text = String.Format("{0}", anchorItem);
        }

        // workaround - DataGrid._selectionAnchor holds a reference to removed item
        void WorkAroundDataGridLeak()
        {
            if (!_model.WorkaroundDataGridLeak)
                return;

            object item = GetAnchorItem();

            // if the item is being tracked, set dataGrid._selectionAnchor to null
            if (item != null && _model.TrackingList.IsTracked(item))
            {
                System.Reflection.FieldInfo fiSelectionAnchor = typeof(DataGrid).GetField("_selectionAnchor",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fiSelectionAnchor.SetValue(dataGrid, null);

                DG_SelectedCellsChanged(null, null);
            }
        }

        object GetAnchorItem()
        {
            // get dataGrid._selectionAnchor  (type Nullable<DataGridCellInfo>)
            System.Reflection.FieldInfo fiSelectionAnchor = typeof(DataGrid).GetField("_selectionAnchor",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fiSelectionAnchor == null)
                return null;

            object selectionAnchor = fiSelectionAnchor.GetValue(dataGrid);
            if (selectionAnchor == null)
                return null;

            // get DataGridCellInfo.Item
            System.Reflection.PropertyInfo piItem = selectionAnchor.GetType().GetProperty("Item",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (piItem == null)
                return null;

            return piItem.GetValue(selectionAnchor, null);
        }

        private void LB_Add(object sender, RoutedEventArgs e)
        {
            _model.AddSimpleItem();
        }

        private void LB_Remove(object sender, RoutedEventArgs e)
        {
            _model.RemoveSimpleItem(listBox.SelectedIndex);
        }

        private void LB_Clear(object sender, RoutedEventArgs e)
        {
            _model.ClearSimpleItems();
        }

        private void LV_Add(object sender, RoutedEventArgs e)
        {
            _model.AddNarrowItem();
        }

        private void LV_Remove(object sender, RoutedEventArgs e)
        {
            _model.RemoveNarrowItem(listView.SelectedIndex);
        }

        private void LV_Clear(object sender, RoutedEventArgs e)
        {
            _model.ClearNarrowItems();
        }

        private void TV_Add(object sender, RoutedEventArgs e)
        {
            _model.AddHierarchicalItem(treeView.SelectedItem as HierarchicalItem);
        }

        private void TV_Remove(object sender, RoutedEventArgs e)
        {
            _model.RemoveHierarchicalItem(treeView.SelectedItem as HierarchicalItem);
        }

        private void TV_Clear(object sender, RoutedEventArgs e)
        {
            _model.ClearHierarchicalItems(treeView.SelectedItem as HierarchicalItem);
        }

        private void TV_Unselect(object sender, RoutedEventArgs e)
        {
            _model.UnselectHierarchicalItem(treeView.SelectedItem as HierarchicalItem);
        }

        private void DoGC(object sender, RoutedEventArgs e)
        {
            _model.DoGC();
        }

        #region Full Test

        DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        List<Action> _steps = new List<Action>();
        int _stepIndex;
        int _testCount;
        ObservableCollection<string> _testLog = new ObservableCollection<string>();
        int _startingLogLength;
        bool _isUiaActive;
        System.Reflection.FieldInfo _fiEventsTable;


        void InitializeFullTest()
        {
            Type eventMapType = typeof(UIElement).Assembly.GetType("MS.Internal.Automation.EventMap");
            _fiEventsTable = eventMapType.GetField("_eventsTable", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            _timer.Tick += RunNextStep;
            lbTestLog.ItemsSource = _testLog;
            InitializeSteps();
        }

        bool IsUIAutomationActive
        {
            get
            {
                // once we've detected UIA, no need to check again
                if (!_isUiaActive)
                {
                    // there's no explicit test, but it appears that UIA always
                    // causes lazy creation of the table in the static class
                    // MS.Internal.Automation.EventMap (internal to PresentationCore).
                    _isUiaActive = (null != _fiEventsTable.GetValue(null));
                }
                return _isUiaActive;
            }
        }

        private void RunTest(object sender, RoutedEventArgs e)
        {
            InitializeModel();
            Log("Starting test run {0}.", ++_testCount);
            _startingLogLength = _testLog.Count;
            _timer.Interval = TimeSpan.FromMilliseconds(0);
            _stepIndex = 0;
            _timer.Start();
        }

        void RunNextStep(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_stepIndex < _steps.Count)
            {
                _steps[_stepIndex]();
                ++_stepIndex;
                _timer.Interval = TimeSpan.FromMilliseconds(IsUIAutomationActive ? 1000 : 0);  // give UIA time to respond
                _timer.Start();
            }
            else
            {
                InitializeModel();
            }
        }

        void Log(string format, params object[] args)
        {
            _testLog.Add(String.Format(format, args));
        }

        void InitializeSteps()
        {
            _steps.Add(StartTestRun);

            _steps.Add(ShowDataGrid);
            _steps.Add(SelectDataGridItem);
            _steps.Add(RemoveDataGridItem);
            _steps.Add(CheckForLeaks);
            _steps.Add(ClearDataGrid);
            _steps.Add(CheckForLeaks);

            _steps.Add(ShowListBox);
            _steps.Add(SelectListBoxItem);
            _steps.Add(RemoveListBoxItem);
            _steps.Add(CheckForLeaks);
            _steps.Add(ClearListBox);
            _steps.Add(CheckForLeaks);

            _steps.Add(ShowListView);
            _steps.Add(SelectListViewItem);
            _steps.Add(RemoveListViewItem);
            _steps.Add(CheckForLeaks);
            _steps.Add(ClearListView);
            _steps.Add(CheckForLeaks);

            _steps.Add(ShowTreeView);
            _steps.Add(ExpandTreeViewItem);
            _steps.Add(SelectSecondLevelTreeViewItem);
            _steps.Add(RemoveSelectedTreeViewItem);
            _steps.Add(CheckForLeaks);
            _steps.Add(SelectFirstLevelTreeViewItem);
            _steps.Add(ClearSelectedTreeViewItem);
            _steps.Add(CheckForLeaks);
            _steps.Add(SelectFirstLevelTreeViewItem);
            _steps.Add(RemoveSelectedTreeViewItem);
            _steps.Add(CheckForLeaks);
            _steps.Add(UnselectTreeView);
            _steps.Add(ClearSelectedTreeViewItem);
            _steps.Add(CheckForLeaks);

            _steps.Add(EndTestRun);
            _steps.Add(SignalEnd);   // gives automation time to catch up
        }

        void StartTestRun()
        {
            cbFailures.IsChecked = false;
            cbRunning.IsChecked = true;
        }

        void EndTestRun()
        {
            bool failed = (_testLog.Count > _startingLogLength);
            Log("Test run {0} {1}.", _testCount, (failed ? "failed" : "passed"));
            if (failed)
            {
                cbFailures.IsChecked = true;
            }
            tabControl.SelectedIndex = 4;
        }

        void SignalEnd()
        {
            cbRunning.IsChecked = false;
        }

        void CheckForLeaks()
        {
            _model.DoGC();
            int leakCount = _model.TrackingList.Count;
            if (leakCount > 0)
            {
                Log("  {0}.  {1} leaked {2} items, including '{3}'",
                    _stepIndex - 1,
                    _steps[_stepIndex - 1].Method.Name,
                    leakCount,
                    _model.TrackingList[0]);
                _model.TrackingList.Clear();
            }
        }

        void ShowDataGrid()
        {
            tabControl.SelectedIndex = 0;
        }

        void SelectDataGridItem()
        {
            dataGrid.SelectedIndex = 3;
        }

        void RemoveDataGridItem()
        {
            DG_Remove(null, null);
        }

        void ClearDataGrid()
        {
            DG_Clear(null, null);
        }

        void ShowListBox()
        {
            tabControl.SelectedIndex = 1;
        }

        void SelectListBoxItem()
        {
            listBox.SelectedIndex = 3;
        }

        void RemoveListBoxItem()
        {
            LB_Remove(null, null);
        }

        void ClearListBox()
        {
            LB_Clear(null, null);
        }

        void ShowListView()
        {
            tabControl.SelectedIndex = 2;
        }

        void SelectListViewItem()
        {
            listView.SelectedIndex = 3;
        }

        void RemoveListViewItem()
        {
            LV_Remove(null, null);
        }

        void ClearListView()
        {
            LV_Clear(null, null);
        }

        void ShowTreeView()
        {
            tabControl.SelectedIndex = 3;
        }

        void ExpandTreeViewItem()
        {
            TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromIndex(3) as TreeViewItem;
            tvi.IsExpanded = true;
        }

        void SelectSecondLevelTreeViewItem()
        {
            TreeViewItem tviParent = treeView.ItemContainerGenerator.ContainerFromIndex(3) as TreeViewItem;
            TreeViewItem tvi = tviParent.ItemContainerGenerator.ContainerFromIndex(1) as TreeViewItem;
            tvi.IsSelected = true;
        }

        void RemoveSelectedTreeViewItem()
        {
            TV_Remove(null, null);
        }

        void SelectFirstLevelTreeViewItem()
        {
            TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromIndex(3) as TreeViewItem;
            tvi.IsSelected = true;
        }

        void ClearSelectedTreeViewItem()
        {
            TV_Clear(null, null);
        }

        void UnselectTreeView()
        {
            TV_Unselect(null, null);
        }

        #endregion Full Test
    }
}

