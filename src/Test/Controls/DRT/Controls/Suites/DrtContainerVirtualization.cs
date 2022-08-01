// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace DRT
{
    public class DrtContainerVirtualizationSuite : DrtTestSuite, INotifyPropertyChanged
    {
        #region Setup

        public DrtContainerVirtualizationSuite()
            : base("ContainerVirtualization")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DrtControls controlsDRT = (DrtControls)DRT;
            TraceUpdateOffsets = controlsDRT.TraceUpdateOffsets;

            if (!_isPageLoaded)
            {
                Stopwatch stopWatch = Stopwatch.StartNew();

                string fileName;
                if (!controlsDRT.InPerformanceMode)
                {
                    fileName =  DRT.BaseDirectory + "DrtContainerVirtualization.xaml";
                }
                else
                {
                    fileName =  DRT.BaseDirectory + "DrtContainerVirtualizationPerf.xaml";
                    IsVirtualizing = true;

                    bool isRecycling = controlsDRT.VirtualizationMode.Equals("Recycling");

                    switch (controlsDRT.ControlNameForPerformanceTesting)
                    {
                        case "ListBox":
                            {
                                SelectedTabIndex = isRecycling ? 0 : 1;
                            }
                            break;
                        case "ListView":
                            {
                                SelectedTabIndex = isRecycling ? 2 : 3;
                            }
                            break;
                        case "DataGrid":
                            {
                                SelectedTabIndex = isRecycling ? 4 : 5;
                            }
                            break;
                        case "TreeView":
                            {
                                SelectedTabIndex = isRecycling ? 6 : 7;
                            }
                            break;
                    }

                    if (!DRT.KeepAlive)
                    {
                        _perfLogWriter = new StreamWriter("DrtContainerVirtualizationPerf.log", true /*append*/);
                    }
                }

                LoadXamlPage(fileName);

                if (controlsDRT.InPerformanceMode && !DRT.KeepAlive)
                {
                    stopWatch.Stop();
                    _perfLogWriter.WriteLine("Startup Time for {0} in {1} : {2}", controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
                }

                _isPageLoaded = true;
            }
            else
            {
                Keyboard.Focus(null);
            }

            // private reflection to determine if the current theme is active
            Type uxThemeWrapper = typeof(FrameworkElement).Assembly.GetType("MS.Win32.UxThemeWrapper");
            PropertyInfo piIsActive = uxThemeWrapper.GetProperty("IsActive", BindingFlags.Static | BindingFlags.NonPublic);
            IsThemeActive = (bool)piIsActive.GetValue(null, null);

            if (!DRT.KeepAlive)
            {
                if (!controlsDRT.InPerformanceMode)
                {
                    List<DrtTest> tests = new List<DrtTest>();

                    tests.Add(PrepareListBox);
                    tests.Add(GenerateMasterListBox);
                    tests.Add(TestPixelListBox);
                    tests.Add(TestItemListBox);
                    tests.Add(TestPixelListBoxWhenVirtualizing);
                    tests.Add(TestItemListBoxWhenVirtualizing);
                    tests.Add(TestPixelListBoxWhenGrouping);
                    tests.Add(TestPixelListBoxWhenGroupingWithVSP);
                    tests.Add(TestItemListBoxWhenGroupingWithVSP);
                    tests.Add(TestPixelListBoxWhenGroupingAndVirtualizing);
                    tests.Add(TestItemListBoxWhenGroupingAndVirtualizing);
                    tests.Add(TestPixelListBoxWhenGroupingWithExpanders);
                    tests.Add(TestPixelListBoxWhenGroupingWithVSPAndExpanders);
                    tests.Add(TestItemListBoxWhenGroupingWithVSPAndExpanders);
                    tests.Add(TestPixelListBoxWhenGroupingAndVirtualizingWithExpanders);
                    tests.Add(TestItemListBoxWhenGroupingAndVirtualizingWithExpanders);

                    tests.Add(PrepareListView);
                    tests.Add(GenerateMasterListView);
                    tests.Add(TestPixelListView);
                    tests.Add(TestItemListView);
                    tests.Add(TestPixelListViewWhenVirtualizing);
                    tests.Add(TestItemListViewWhenVirtualizing);
                    tests.Add(TestPixelListViewWhenGrouping);
                    tests.Add(TestPixelListViewWhenGroupingWithVSP);
                    tests.Add(TestItemListViewWhenGroupingWithVSP);
                    tests.Add(TestPixelListViewWhenGroupingAndVirtualizing);
                    tests.Add(TestItemListViewWhenGroupingAndVirtualizing);
                    tests.Add(TestPixelListViewWhenGroupingWithExpanders);
                    tests.Add(TestPixelListViewWhenGroupingWithVSPAndExpanders);
                    tests.Add(TestItemListViewWhenGroupingWithVSPAndExpanders);
                    tests.Add(TestPixelListViewWhenGroupingAndVirtualizingWithExpanders);
                    tests.Add(TestItemListViewWhenGroupingAndVirtualizingWithExpanders);

                    tests.Add(PrepareDataGrid);
                    tests.Add(GenerateMasterDataGrid);
                    tests.Add(TestPixelDataGrid);
                    // This test is commented because the custom key nav logic in DataGrid does not gel with the general key nav scheme used by all ItemsControls and verified by this test.
                    //tests.Add(TestItemDataGrid);
                    tests.Add(TestPixelDataGridWhenVirtualizing);
                    // This test is commented because the custom key nav logic in DataGrid does not gel with the general key nav scheme used by all ItemsControls and verified by this test.
                    //tests.Add(TestItemDataGridWhenVirtualizing);
                    tests.Add(TestPixelDataGridWhenGrouping);
                    tests.Add(TestPixelDataGridWhenGroupingWithVSP);
                    tests.Add(TestItemDataGridWhenGroupingWithVSP);
                    tests.Add(TestPixelDataGridWhenGroupingAndVirtualizing);
                    tests.Add(TestItemDataGridWhenGroupingAndVirtualizing);
                    tests.Add(TestPixelDataGridWhenGroupingWithExpanders);
                    tests.Add(TestPixelDataGridWhenGroupingWithVSPAndExpanders);
                    tests.Add(TestItemDataGridWhenGroupingWithVSPAndExpanders);
                    tests.Add(TestPixelDataGridWhenGroupingAndVirtualizingWithExpanders);
                    tests.Add(TestItemDataGridWhenGroupingAndVirtualizingWithExpanders);

                    tests.Add(PrepareTreeView);
                    tests.Add(GenerateMasterTreeView);
                    tests.Add(TestPixelTreeView);
                    tests.Add(TestItemTreeView);
                    tests.Add(TestPixelTreeViewWhenVirtualizing);
                    tests.Add(TestItemTreeViewWhenVirtualizing);
                    //tests.Add(TestPixelTreeViewWhenGrouping);
                    //tests.Add(TestPixelTreeViewWhenGroupingWithVSP);
                    //tests.Add(TestItemTreeViewWhenGroupingWithVSP);
                    //tests.Add(TestPixelTreeViewWhenGroupingAndVirtualizing);
                    //tests.Add(TestItemTreeViewWhenGroupingAndVirtualizing);

                    return tests.ToArray();
                }
                else
                {
                    return new DrtTest [] { TestPerformance };
                }
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void LoadXamlPage(string fileName)
        {
            System.IO.Stream stream = File.OpenRead(fileName);
            Visual root = (Visual)XamlReader.Load(stream);
            InitTree(root);

            if (DRT.KeepAlive)
            {
                //DRT.MainWindow.SizeToContent = SizeToContent.Manual;
                //Matrix deviceTransform = DRT.MainWindow.CompositionTarget.TransformToDevice;
                //Point pt = deviceTransform.Transform(new Point(_rootBorder.Width, _rootBorder.Height));
                //DrtBase.SetWindowPos(DRT.MainWindow.Handle, IntPtr.Zero, 0, 0, (int)pt.X, (int)pt.Y, DrtBase.SWP_NOMOVE | DrtBase.SWP_NOZORDER);
                //_rootBorder.ClearValue(FrameworkElement.WidthProperty);
                //_rootBorder.ClearValue(FrameworkElement.HeightProperty);
            }

            DRT.Show(root);
        }

        private void InitTree(Visual root)
        {
            _rootBorder = (FrameworkElement)DRT.FindVisualByID("Root_Border", root);
            _rootBorder.DataContext = this;
            _tabControl = DRT.FindVisualByID("TabControl1", root) as TabControl;

            DrtControls controlsDRT = (DrtControls)DRT;
            if (!controlsDRT.InPerformanceMode)
            {
                _tabControl.SelectionChanged += new SelectionChangedEventHandler(TabControlSelectionChanged);
                _cacheLengthTextBox = DRT.FindVisualByID("CacheLength", root) as TextBox;
                _cacheLengthUnitTextBox = DRT.FindVisualByID("CacheLengthUnit", root) as TextBox;
                _refreshCacheSizesButton = DRT.FindVisualByID("RefreshCacheSizes", root) as Button;
                _refreshCacheSizesButton.Click += new RoutedEventHandler(OnClickRefreshCacheSizes);
            }
        }

        private void TabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != _tabControl)
            {
                return;
            }

            if (_tabControl.SelectedIndex == 3)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    (Action)delegate()
                    {
                        ItemsControl itemsControl = _tabControl.FindName("MyTreeView") as TreeView;
                        ScrollViewer scrollViewer = FindChild<ScrollViewer>(itemsControl);

                        Binding canContentScrollBinding = new Binding();
                        canContentScrollBinding.Path = new PropertyPath(ScrollViewer.CanContentScrollProperty);
                        canContentScrollBinding.Source = itemsControl;
                        BindingOperations.SetBinding(scrollViewer, ScrollViewer.CanContentScrollProperty, canContentScrollBinding);
                    },
                    DispatcherPriority.ContextIdle);
            }
        }

        private void OnClickRefreshCacheSizes(object sender, RoutedEventArgs e)
        {
            BindingExpression b = _cacheLengthTextBox.GetBindingExpression(TextBox.TextProperty);
            b.UpdateSource();

            b = _cacheLengthUnitTextBox.GetBindingExpression(TextBox.TextProperty);
            b.UpdateSource();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
              PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public bool TraceUpdateOffsets { get; set; }

        public WorkingPeople PeopleDataLB
        {
            get
            {
                if (_peopleDataLB == null)
                {
                    _peopleDataLB = new WorkingPeople(100);
                }

                return _peopleDataLB;
            }
        }

        public WorkingPeople PeoplePerfDataLB
        {
            get
            {
                if (_peoplePerfDataLB == null)
                {
                    _peoplePerfDataLB = new WorkingPeople(1000);
                }

                return _peoplePerfDataLB;
            }
        }

        public WorkingPeople PeopleDataLV
        {
            get
            {
                if (_peopleDataLV == null)
                {
                    _peopleDataLV = new WorkingPeople(100);
                }

                return _peopleDataLV;
            }
        }

        public WorkingPeople PeoplePerfDataLV
        {
            get
            {
                if (_peoplePerfDataLV == null)
                {
                    _peoplePerfDataLV = new WorkingPeople(100);
                }

                return _peoplePerfDataLV;
            }
        }

        public WorkingPeople PeopleDataDG
        {
            get
            {
                if (_peopleDataDG == null)
                {
                    _peopleDataDG = new WorkingPeople(100);
                }

                return _peopleDataDG;
            }
        }

        public WorkingPeople PeoplePerfDataDG
        {
            get
            {
                if (_peoplePerfDataDG == null)
                {
                    _peoplePerfDataDG = new WorkingPeople(100);
                }

                return _peoplePerfDataDG;
            }
        }

        public ObservableCollection<Folder> FolderDataTV
        {
            get
            {
                if (_folderDataTV == null)
                {
                    _folderDataTV = new Folder("Folder", null, 3, 5, false);
                }

                return _folderDataTV.SubFolders;
            }
        }

        public ObservableCollection<Folder> FolderPerfDataTV
        {
            get
            {
                if (_folderPerfDataTV == null)
                {
                    _folderPerfDataTV = new Folder("Folder", null, 3, 10, false);
                }

                return _folderPerfDataTV.SubFolders;
            }
        }

        public int SelectedTabIndex {

            get { return _selectedTabIndex;}
            set {
                if (_selectedTabIndex != value)
                {
                  _selectedTabIndex = value;
                  NotifyPropertyChanged("SelectedTabIndex");
                }
            }
        }
        private int _selectedTabIndex = 0;


        public VirtualizationCacheLength CacheLength
        {
            get { return _cacheLength; }
            set {
                if (_cacheLength != value)
                {
                    _cacheLength = value;
                    NotifyPropertyChanged("CacheLength");
                }
            }
        }
        private VirtualizationCacheLength _cacheLength = new VirtualizationCacheLength(0);

        public VirtualizationCacheLengthUnit CacheLengthUnit
        {
            get { return _cacheLengthUnit; }
            set {
                if (_cacheLengthUnit != value)
                {
                    _cacheLengthUnit = value;
                    NotifyPropertyChanged("CacheLengthUnit");
                }
            }
        }
        private VirtualizationCacheLengthUnit _cacheLengthUnit = VirtualizationCacheLengthUnit.Pixel;
//#endif

        public bool IsVirtualizing {

            get { return _isVirtualizing;}
            set {
                if (_isVirtualizing != value)
                {
                  _isVirtualizing = value;
                  NotifyPropertyChanged("IsVirtualizing");
                }
            }
        }
        private bool _isVirtualizing = false;

        public bool IsItemScrolling {

            get { return _isItemScrolling;}
            set {
                if (_isItemScrolling != value)
                {
                  _isItemScrolling = value;
                  NotifyPropertyChanged("IsItemScrolling");
                }
            }
        }
        private bool _isItemScrolling = false;

        public bool IsGrouping {

            get { return _isGrouping;}
            set {
                if (_isGrouping != value)
                {
                  _isGrouping = value;
                  NotifyPropertyChanged("IsGrouping");

                  ItemsControl itemsControl = null;
                  for (int i=0; i<4; i++)
                  {
                    switch(i)
                    {
                        case 0:
                            itemsControl = _tabControl.FindName("MyListBox") as ListBox;
                            break;
                        case 1:
                            itemsControl = _tabControl.FindName("MyListView") as ListView;
                            break;
                        case 2:
                            itemsControl = _tabControl.FindName("MyDataGrid") as DataGrid;
                            break;
                        case 3:
                            itemsControl = _tabControl.FindName("MyTreeView") as TreeView;
                            break;
                    }

                    if (i < 3)
                    {
                        if (value)
                        {
                            ICollectionView collectionView = itemsControl.Items;
                            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Occupation"));
                            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Class"));

                            GroupStyle groupStyle = new GroupStyle();
                            if (UseVirtualizingStackPanelAsGroupPanel)
                            {
                                groupStyle.Panel = (ItemsPanelTemplate)_tabControl.FindResource("VerticalItemsVirtualizingStackPanel");
                            }
                            if (UseExpanderInGroupItemStyle)
                            {
                                groupStyle.ContainerStyle = (Style)_tabControl.FindResource("FirstLevelGroupItemStyle");
                            }
                            else
                            {
                                groupStyle.HeaderTemplate = (DataTemplate)_tabControl.FindResource("FirstLevelGroupHeader");
                            }
                            itemsControl.GroupStyle.Add(groupStyle);
                            groupStyle = new GroupStyle();
                            if (UseVirtualizingStackPanelAsGroupPanel)
                            {
                                groupStyle.Panel = (ItemsPanelTemplate)_tabControl.FindResource("VerticalItemsVirtualizingStackPanel");
                            }
                            if (UseExpanderInGroupItemStyle)
                            {
                                groupStyle.ContainerStyle = (Style)_tabControl.FindResource("SecondLevelGroupItemStyle");
                            }
                            else
                            {
                                groupStyle.HeaderTemplate = (DataTemplate)_tabControl.FindResource("SecondLevelGroupHeader");
                            }
                            itemsControl.GroupStyle.Add(groupStyle);
                            if (UseVirtualizingStackPanelAsGroupPanel)
                            {
                                itemsControl.SetValue(ScrollViewer.CanContentScrollProperty, true);
                            }

                            if (IsFirstGroupCollapsed)
                            {
                                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
                                Expander expander = FindChild<Expander>(itemsControl);
                                if (expander != null)
                                {
                                    expander.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                                    DRT.PressKey(Key.Enter);
                                }
                                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
                            }

                            ICollectionViewLiveShaping liveShapingView = collectionView as ICollectionViewLiveShaping;
                            if (liveShapingView != null)
                            {
                                liveShapingView.IsLiveGrouping = true;
                            }
                        }
                        else
                        {
                            ICollectionView collectionView = itemsControl.Items;
                            collectionView.GroupDescriptions.Clear();
                            itemsControl.GroupStyle.Clear();
                            itemsControl.ClearValue(ScrollViewer.CanContentScrollProperty);

                            ICollectionViewLiveShaping liveShapingView = collectionView as ICollectionViewLiveShaping;
                            if (liveShapingView != null)
                            {
                                liveShapingView.IsLiveGrouping = false;
                            }
                        }
                    }
                  }
                }
            }
        }
        private bool _isGrouping = false;

        public bool UseVirtualizingStackPanelAsGroupPanel {

            get { return _useVirtualizingStackPanelAsGroupPanel;}
            set {
                if (_useVirtualizingStackPanelAsGroupPanel != value)
                {
                  _useVirtualizingStackPanelAsGroupPanel = value;
                  NotifyPropertyChanged("UseVirtualizingStackPanelAsGroupPanel");
                }
            }
        }
        private bool _useVirtualizingStackPanelAsGroupPanel = false;

        public bool UseExpanderInGroupItemStyle {

            get { return _useExpanderInGroupItemStyle;}
            set {
                if (_useExpanderInGroupItemStyle != value)
                {
                  _useExpanderInGroupItemStyle = value;
                  NotifyPropertyChanged("UseExpanderInGroupItemStyle");
                }
            }
        }
        private bool _useExpanderInGroupItemStyle = false;

        public bool IsFirstGroupCollapsed {

            get { return _isFirstGroupCollapsed;}
            set {
                if (_isFirstGroupCollapsed != value)
                {
                  _isFirstGroupCollapsed = value;
                  NotifyPropertyChanged("IsFirstGroupCollapsed");
                }
            }
        }
        private bool _isFirstGroupCollapsed = false;

        public bool IsSorting {

            get { return _isSorting;}
            set {
                if (_isSorting != value)
                {
                  _isSorting = value;

                  ItemsControl itemsControl = null;
                  for (int i=0; i<4; i++)
                  {
                    switch(i)
                    {
                        case 0:
                            itemsControl = _tabControl.FindName("MyListBox") as ListBox;
                            break;
                        case 1:
                            itemsControl = _tabControl.FindName("MyListView") as ListView;
                            break;
                        case 2:
                            itemsControl = _tabControl.FindName("MyDataGrid") as DataGrid;
                            break;
                        case 3:
                            itemsControl = _tabControl.FindName("MyTreeView") as TreeView;
                            break;
                    }

                    if (i < 3)
                    {
                      ICollectionView collectionView = itemsControl.Items;
                      collectionView.SortDescriptions.Add(new SortDescription("Occupation", ListSortDirection.Ascending));
                      collectionView.SortDescriptions.Add(new SortDescription("Class", ListSortDirection.Ascending));
                      collectionView.SortDescriptions.Add(new SortDescription("LastName", ListSortDirection.Ascending));
                    }
                  }

                  NotifyPropertyChanged("IsSorting");
                }
            }
        }
        private bool _isSorting = false;

        public bool IsThemeActive
        {
            get { return _isThemeActive; }
            private set { _isThemeActive = value; }
        }
        private bool _isThemeActive;

        public CollectionChangeOperation CollectionChangeOperation
        {
            get { return _collectionChangeOperation; }

            set
            {
                if (value != _collectionChangeOperation)
                {
                    IList itemsSource = null;
                    int changeIndex = 0;
                    object newItem = new WorkingPerson("NewItemFirstName", "NewItemLastName") { Occupation="Engineer", Class="Class A" };

                    for (int i=0; i<4; i++)
                    {
                      switch(i)
                      {
                          case 0:
                              itemsSource = PeopleDataLB;
                              break;
                          case 1:
                              itemsSource = PeopleDataLV;
                              break;
                          case 2:
                              itemsSource = PeopleDataDG;
                              break;
                          case 3:
                              itemsSource = FolderDataTV;
                              newItem = new Folder("NewItemFolder", _folderDataTV, 1, 2, true);
                              break;
                      }

                      if (itemsSource != null)
                      {
                          //
                          // Property Change
                          //
                          bool isReset = (value == CollectionChangeOperation.None);
                          CollectionChangeOperation state = isReset ? _collectionChangeOperation : value;

                          switch (state)
                          {
                              case CollectionChangeOperation.Insert:
                                  {
                                      if (!isReset)
                                      {
                                          itemsSource.Insert(changeIndex, newItem);
                                      }
                                      else
                                      {
                                          itemsSource.RemoveAt(changeIndex);
                                      }
                                  }
                                  break;

                              case CollectionChangeOperation.Remove:
                                  {
                                      if (!isReset)
                                      {
                                          _removeOrReplaceChangedItem[i] = itemsSource[changeIndex];
                                          itemsSource.RemoveAt(changeIndex);
                                      }
                                      else
                                      {
                                          itemsSource.Insert(changeIndex, _removeOrReplaceChangedItem[i]);
                                      }
                                  }
                                  break;

                              case CollectionChangeOperation.Replace:
                                  {
                                      if (!isReset)
                                      {
                                          _removeOrReplaceChangedItem[i] = itemsSource[changeIndex];
                                          itemsSource[changeIndex] = newItem;
                                      }
                                      else
                                      {
                                          itemsSource[changeIndex] = _removeOrReplaceChangedItem[i];
                                      }
                                  }
                                  break;

                              case CollectionChangeOperation.Move:
                                  {
                                      if (!isReset)
                                      {
                                          if (itemsSource is WorkingPeople)
                                          {
                                              ((WorkingPeople)itemsSource).Move(changeIndex, changeIndex+1);
                                          }
                                          else if (itemsSource is ObservableCollection<Folder>)
                                          {
                                              ((ObservableCollection<Folder>)itemsSource).Move(changeIndex, changeIndex+1);
                                          }
                                      }
                                      else
                                      {
                                          if (itemsSource is WorkingPeople)
                                          {
                                              ((WorkingPeople)itemsSource).Move(changeIndex+1, changeIndex);
                                          }
                                          else if (itemsSource is ObservableCollection<Folder>)
                                          {
                                              ((ObservableCollection<Folder>)itemsSource).Move(changeIndex+1, changeIndex);
                                          }
                                      }
                                  }
                                  break;

                              case CollectionChangeOperation.PropertyChange:
                                  {
                                      if (!isReset)
                                      {
                                          if (itemsSource is WorkingPeople)
                                          {
                                              WorkingPerson wp = ((WorkingPeople)itemsSource)[changeIndex];
                                              _propertyChangePrevValue[i] = wp.Class;
                                              wp.Class = "NewClass";
                                          }
                                          else if (itemsSource is ObservableCollection<Folder>)
                                          {
                                              Folder folder = ((ObservableCollection<Folder>)itemsSource)[changeIndex];
                                              _propertyChangePrevValue[i] = folder.Name;
                                              folder.Name = "NewName";
                                          }
                                      }
                                      else
                                      {
                                          if (itemsSource is WorkingPeople)
                                          {
                                              WorkingPerson wp = ((WorkingPeople)itemsSource)[changeIndex];
                                              wp.Class = _propertyChangePrevValue[i];
                                          }
                                          else if (itemsSource is ObservableCollection<Folder>)
                                          {
                                              Folder folder = ((ObservableCollection<Folder>)itemsSource)[changeIndex];
                                              folder.Name = _propertyChangePrevValue[i];
                                          }
                                      }
                                  }
                                  break;
                          }
                      }
                    }

                    _collectionChangeOperation = value;
                    NotifyPropertyChanged("CollectionChangeOperation");
                }
            }
        }
        private CollectionChangeOperation _collectionChangeOperation = CollectionChangeOperation.None;
        private String[] _propertyChangePrevValue = new String[4];
        private object[] _removeOrReplaceChangedItem = new object[4];

        #endregion

        #region Tests

        private void PrepareListBox()
        {
            _tabControl.SelectedIndex = 0;
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            _itemsControl = DRT.FindVisualByID("MyListBox", _tabControl) as ListBox;
            _scrollViewer = FindChild<ScrollViewer>(_itemsControl);
            _scrollContentPresenter = FindChild<ScrollContentPresenter>(_scrollViewer);
        }

        private void GenerateMasterListBox()
        {
            GenerateMasterItemsControl<ListBoxItem>(ref _masterArrangeDataListBox);
        }

        private void TestPixelListBox()
        {
            TestItemsControl<ListBoxItem>(_masterArrangeDataListBox);
        }

        private void TestItemListBox()
        {
            TestItemsControlWhenItemScrolling(TestPixelListBox);
        }

        private void TestPixelListBoxWhenVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelListBox);
        }

        private void TestItemListBoxWhenVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelListBoxWhenVirtualizing);
        }

        private void TestPixelListBoxWhenGrouping()
        {
            TestItemsControlWhenGrouping(TestPixelListBox);
        }

        private void TestPixelListBoxWhenGroupingWithVSP()
        {
            TestItemsControlWhenGroupingWithVSP(TestPixelListBoxWhenGrouping);
        }

        private void TestItemListBoxWhenGroupingWithVSP()
        {
            TestItemsControlWhenItemScrolling(TestPixelListBoxWhenGroupingWithVSP);
        }

        private void TestPixelListBoxWhenGroupingAndVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelListBoxWhenGrouping);
        }

        private void TestItemListBoxWhenGroupingAndVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelListBoxWhenGroupingAndVirtualizing);
        }

        private void TestPixelListBoxWhenGroupingWithExpanders()
        {
            TestItemsControlWhenGroupingWithExpanders(TestPixelListBoxWhenGrouping);
        }

        private void TestPixelListBoxWhenGroupingWithVSPAndExpanders()
        {
            TestItemsControlWhenGroupingWithVSP(TestPixelListBoxWhenGroupingWithExpanders);
        }

        private void TestItemListBoxWhenGroupingWithVSPAndExpanders()
        {
            TestItemsControlWhenItemScrolling(TestPixelListBoxWhenGroupingWithVSPAndExpanders);
        }

        private void TestPixelListBoxWhenGroupingAndVirtualizingWithExpanders()
        {
            TestItemsControlWhenVirtualizing(TestPixelListBoxWhenGroupingWithVSPAndExpanders);
        }

        private void TestItemListBoxWhenGroupingAndVirtualizingWithExpanders()
        {
            TestItemsControlWhenItemScrolling(TestPixelListBoxWhenGroupingAndVirtualizingWithExpanders);
        }

        private void PrepareListView()
        {
            _tabControl.SelectedIndex = 1;
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            _itemsControl = DRT.FindVisualByID("MyListView", _tabControl) as ListView;
            _scrollViewer = FindChild<ScrollViewer>(_itemsControl);
            _scrollContentPresenter = (ScrollContentPresenter)_scrollViewer.Template.FindName("PART_ScrollContentPresenter", _scrollViewer);
        }

        private void GenerateMasterListView()
        {
            GenerateMasterItemsControl<ListViewItem>(ref _masterArrangeDataListView);
        }

        private void TestPixelListView()
        {
            TestItemsControl<ListViewItem>(_masterArrangeDataListView);
        }

        private void TestItemListView()
        {
            TestItemsControlWhenItemScrolling(TestPixelListView);
        }

        private void TestPixelListViewWhenVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelListView);
        }

        private void TestItemListViewWhenVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelListViewWhenVirtualizing);
        }

        private void TestPixelListViewWhenGrouping()
        {
            TestItemsControlWhenGrouping(TestPixelListView);
        }

        private void TestPixelListViewWhenGroupingWithVSP()
        {
            TestItemsControlWhenGroupingWithVSP(TestPixelListViewWhenGrouping);
        }

        private void TestItemListViewWhenGroupingWithVSP()
        {
            TestItemsControlWhenItemScrolling(TestPixelListViewWhenGroupingWithVSP);
        }

        private void TestPixelListViewWhenGroupingAndVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelListViewWhenGrouping);
        }

        private void TestItemListViewWhenGroupingAndVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelListViewWhenGroupingAndVirtualizing);
        }

        private void TestPixelListViewWhenGroupingWithExpanders()
        {
            TestItemsControlWhenGroupingWithExpanders(TestPixelListViewWhenGrouping);
        }

        private void TestPixelListViewWhenGroupingWithVSPAndExpanders()
        {
            TestItemsControlWhenGroupingWithVSP(TestPixelListViewWhenGroupingWithExpanders);
        }

        private void TestItemListViewWhenGroupingWithVSPAndExpanders()
        {
            TestItemsControlWhenItemScrolling(TestPixelListViewWhenGroupingWithVSPAndExpanders);
        }

        private void TestPixelListViewWhenGroupingAndVirtualizingWithExpanders()
        {
            TestItemsControlWhenVirtualizing(TestPixelListViewWhenGroupingWithVSPAndExpanders);
        }

        private void TestItemListViewWhenGroupingAndVirtualizingWithExpanders()
        {
            TestItemsControlWhenItemScrolling(TestPixelListViewWhenGroupingAndVirtualizingWithExpanders);
        }

        private void PrepareDataGrid()
        {
            _tabControl.SelectedIndex = 2;
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            _itemsControl = DRT.FindVisualByID("MyDataGrid", _tabControl) as DataGrid;
            _scrollViewer = FindChild<ScrollViewer>(_itemsControl);
            _scrollContentPresenter = (ScrollContentPresenter)_scrollViewer.Template.FindName("PART_ScrollContentPresenter", _scrollViewer);
        }

        private void GenerateMasterDataGrid()
        {
            GenerateMasterItemsControl<DataGridRow>(ref _masterArrangeDataDataGrid);
        }

        private void TestPixelDataGrid()
        {
            TestItemsControl<DataGridRow>(_masterArrangeDataDataGrid);
        }

        private void TestItemDataGrid()
        {
            TestItemsControlWhenItemScrolling(TestPixelDataGrid);
        }

        private void TestPixelDataGridWhenVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelDataGrid);
        }

        private void TestItemDataGridWhenVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelDataGridWhenVirtualizing);
        }

        private void TestPixelDataGridWhenGrouping()
        {
            TestItemsControlWhenGrouping(TestPixelDataGrid);
        }

        private void TestPixelDataGridWhenGroupingWithVSP()
        {
            TestItemsControlWhenGroupingWithVSP(TestPixelDataGridWhenGrouping);
        }

        private void TestItemDataGridWhenGroupingWithVSP()
        {
            TestItemsControlWhenItemScrolling(TestPixelDataGridWhenGroupingWithVSP);
        }

        private void TestPixelDataGridWhenGroupingAndVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelDataGridWhenGrouping);
        }

        private void TestItemDataGridWhenGroupingAndVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelDataGridWhenGroupingAndVirtualizing);
        }

        private void TestPixelDataGridWhenGroupingWithExpanders()
        {
            TestItemsControlWhenGroupingWithExpanders(TestPixelDataGridWhenGrouping);
        }

        private void TestPixelDataGridWhenGroupingWithVSPAndExpanders()
        {
            TestItemsControlWhenGroupingWithVSP(TestPixelDataGridWhenGroupingWithExpanders);
        }

        private void TestItemDataGridWhenGroupingWithVSPAndExpanders()
        {
            TestItemsControlWhenItemScrolling(TestPixelDataGridWhenGroupingWithVSPAndExpanders);
        }

        private void TestPixelDataGridWhenGroupingAndVirtualizingWithExpanders()
        {
            TestItemsControlWhenVirtualizing(TestPixelDataGridWhenGroupingWithVSPAndExpanders);
        }

        private void TestItemDataGridWhenGroupingAndVirtualizingWithExpanders()
        {
            TestItemsControlWhenItemScrolling(TestPixelDataGridWhenGroupingAndVirtualizingWithExpanders);
        }

        private void PrepareTreeView()
        {
            _tabControl.SelectedIndex = 3;
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            _itemsControl = DRT.FindVisualByID("MyTreeView", _tabControl) as TreeView;
            _scrollViewer = FindChild<ScrollViewer>(_itemsControl);
            _scrollContentPresenter = FindChild<ScrollContentPresenter>(_scrollViewer);
        }

        private void GenerateMasterTreeView()
        {
            _itemsControl.Resources[typeof(TreeView)] = _itemsControl.FindResource("TreeViewStyleWithStackPanel");
            _itemsControl.Resources[typeof(TreeViewItem)] = _itemsControl.FindResource("TreeViewItemStyleWithStackPanel");

            _masterArrangeDataTreeView = new Collection<MasterArrangeData>();
            MasterArrangeData masterArrangeData = null;

            for (int i=0; i<(int)CollectionChangeOperation.Last; i++)
            {
                try
                {
                    CollectionChangeOperation = (CollectionChangeOperation)i;

                    masterArrangeData = null;
                    GenerateMasterForItemsControl<TreeViewItem>(ref masterArrangeData);
                    _masterArrangeDataTreeView.Add(masterArrangeData);

                    if (CollectionChangeOperation == CollectionChangeOperation.None)
                    {
                        try
                        {
                            // Collapse Folder00 and Expand Folder1
                            FolderDataTV[0].SubFolders[0].IsExpanded = false;
                            FolderDataTV[1].IsExpanded = true;

                            masterArrangeData = null;
                            GenerateMasterForItemsControl<TreeViewItem>(ref masterArrangeData);
                            _masterArrangeDataTreeView.Add(masterArrangeData);
                        }
                        finally
                        {
                            // Revert changes to Folder00 and Folder1
                            FolderDataTV[0].SubFolders[0].IsExpanded = true;
                            FolderDataTV[1].IsExpanded = false;
                        }
                    }
                    else
                    {
                        _masterArrangeDataTreeView.Add(null);
                    }
                }
                finally
                {
                    CollectionChangeOperation = CollectionChangeOperation.None;
                }
            }

            _itemsControl.Resources.Clear();
        }

        private void TestPixelTreeView()
        {
            int testIndex = (int)CollectionChangeOperation * 2;
            bool isHorizontal = false;
            IList<int> iterationsDataList = IterationsDataList;

            TestItemsControl<TreeViewItem>(
                isHorizontal,
                iterationsDataList,
                _masterArrangeDataTreeView[testIndex++]);

            if (CollectionChangeOperation == CollectionChangeOperation.None)
            {
                try
                {
                    // Collapse Folder00 and Expand Folder1
                    FolderDataTV[0].SubFolders[0].IsExpanded = false;
                    FolderDataTV[1].IsExpanded = true;

                    TestItemsControl<TreeViewItem>(
                        isHorizontal,
                        iterationsDataList,
                        _masterArrangeDataTreeView[testIndex++]);
                }
                finally
                {
                    // Revert changes to Folder00 and Folder1
                    FolderDataTV[0].SubFolders[0].IsExpanded = true;
                    FolderDataTV[1].IsExpanded = false;
                }
            }
        }

        private void TestItemTreeView()
        {
            TestItemsControlWhenItemScrolling(TestPixelTreeView);
        }

        private void TestPixelTreeViewWhenVirtualizing()
        {
            TestItemsControlWhenVirtualizing(TestPixelTreeView);
        }

        private void TestItemTreeViewWhenVirtualizing()
        {
            TestItemsControlWhenItemScrolling(TestPixelTreeViewWhenVirtualizing);
        }

        private void TestPixelTreeViewWhenGrouping()
        {
            TestItemsControlWhenGrouping(TestPixelTreeView);
        }

        private void TestItemTreeViewWhenGrouping()
        {
            TestItemsControlWhenItemScrolling(TestPixelTreeViewWhenGrouping);
        }

        #endregion

        #region Helpers

        // Walks the subtree of the given visual and finds a child of the given type.
        private static T FindChild<T>(Visual parent) where T : Visual
        {
            T child = null;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;

                if (child == null)
                {
                    child = FindChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        // Walks up the visual tree to find a parent of the given type
        private static T FindParent<T>(Visual child) where T : Visual
        {
            DependencyObject current = VisualTreeHelper.GetParent(child);
            while (current != null)
            {
                T parent = current as T;
                if (parent != null)
                    return parent;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private static int FindFirstIndexInViewport(
            bool isHorizontal,
            double svHorizontalOffset,
            double svVerticalOffset,
            MasterArrangeData masterArrangeData)
        {
            IList<MasterContainerArrangeData> masterContainerArrangeDataList = masterArrangeData.MasterContainerArrangeDataList;

            for (int i=0; i<masterContainerArrangeDataList.Count; i++)
            {
                if (isHorizontal)
                {
                    if (DoubleUtil.GreaterThan(masterContainerArrangeDataList[i].ArrangeRect.Right, svHorizontalOffset))
                    {
                        return i;
                    }
                }
                else
                {
                    if (DoubleUtil.GreaterThan(masterContainerArrangeDataList[i].ArrangeRect.Bottom, svVerticalOffset))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static int FindLastIndexInViewport(
            bool isHorizontal,
            double svHorizontalOffset,
            double svVerticalOffset,
            MasterArrangeData masterArrangeData)
        {
            IList<MasterContainerArrangeData> masterContainerArrangeDataList = masterArrangeData.MasterContainerArrangeDataList;

            for (int i=1; i<masterContainerArrangeDataList.Count; i++)
            {
                if (isHorizontal)
                {
                    if (DoubleUtil.GreaterThanOrClose(masterContainerArrangeDataList[i].ArrangeRect.Left, svHorizontalOffset + masterArrangeData.MasterViewportSize.Width))
                    {
                        return i-1;
                    }
                }
                else
                {
                    if (DoubleUtil.GreaterThanOrClose(masterContainerArrangeDataList[i].ArrangeRect.Top, svVerticalOffset + masterArrangeData.MasterViewportSize.Height))
                    {
                        return i-1;
                    }
                }
            }

            return masterContainerArrangeDataList.Count-1;
        }

        private void GenerateMasterItemsControl<FocusableType>(
            ref IList<MasterArrangeData> masterArrangeDataList) where FocusableType : FrameworkElement
        {
            masterArrangeDataList = new Collection<MasterArrangeData>();
            MasterArrangeData masterArrangeData = null;

            for (int i=0; i<(int)CollectionChangeOperation.Last; i++)
            {
                try
                {
                    CollectionChangeOperation = (CollectionChangeOperation)i;

                    for (int j=0; j<4; j++)
                    {
                        try
                        {
                            _itemsControl.SetValue(ItemsControl.ItemsPanelProperty, (ItemsPanelTemplate)_rootBorder.FindResource("VerticalItemsStackPanel"));
                            _itemsControl.SetValue(ScrollViewer.CanContentScrollProperty, false);

                            if (j>=3)
                            {
                                IsFirstGroupCollapsed = true;
                            }
                            if (j>=2)
                            {
                                UseExpanderInGroupItemStyle = true;
                            }
                            if (j>=1)
                            {
                                IsGrouping = true;
                            }

                            masterArrangeData = null;
                            GenerateMasterForItemsControl<FocusableType>(ref masterArrangeData);
                            masterArrangeDataList.Add(masterArrangeData);
                        }
                        finally
                        {
                            IsGrouping = false;
                            UseExpanderInGroupItemStyle = false;
                            IsFirstGroupCollapsed = false;

                            _itemsControl.ClearValue(ScrollViewer.CanContentScrollProperty);
                            _itemsControl.ClearValue(ItemsControl.ItemsPanelProperty);
                        }
                    }
                }
                finally
                {
                    CollectionChangeOperation = CollectionChangeOperation.None;
                }
            }
        }

        private void GenerateMasterForItemsControl<FocusableType>(
            ref MasterArrangeData masterArrangeData) where FocusableType : FrameworkElement
        {
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            ItemsPresenter itemsPresenter = FindChild<ItemsPresenter>(_scrollContentPresenter);
            Panel itemsHost = VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;

            masterArrangeData = new MasterArrangeData();
            masterArrangeData.MasterViewportSize = new Size(_scrollViewer.ViewportWidth, _scrollViewer.ViewportHeight);
            masterArrangeData.MasterExtentSize = new Size(_scrollViewer.ExtentWidth, _scrollViewer.ExtentHeight);

            GenerateMasterFromItemsHostRecursive<FocusableType>(_scrollContentPresenter, itemsHost, masterArrangeData.MasterContainerArrangeDataList);
        }

        private static void GenerateMasterFromItemsHostRecursive<FocusableType>(
            ScrollContentPresenter scrollContentPresenter,
            Panel itemsHost,
            IList<MasterContainerArrangeData> masterContainerArrangeDataList) where FocusableType : FrameworkElement
        {
            IList children = itemsHost.Children;
            int count = children.Count;

            for (int i=0; i<count; i++)
            {
                FrameworkElement child = (FrameworkElement)children[i];
                if (child != null)
                {
                    IHierarchicalVirtualizationAndScrollInfo virtualizingChild = child as IHierarchicalVirtualizationAndScrollInfo;
                    if (virtualizingChild != null)
                    {
                        Control controlChild = (Control)virtualizingChild;
                        Expander expander  = null;
                        FrameworkElement headerPresenter = controlChild.Template.FindName("PART_Header", controlChild) as FrameworkElement;
                        if (headerPresenter == null)
                        {
                            expander = FindChild<Expander>(child);
                            if (expander != null)
                            {
                                headerPresenter = expander.Template.FindName("HeaderSite", expander) as FrameworkElement;
                            }
                        }

                        if (headerPresenter != null)
                        {
                            Rect arrangeRect = headerPresenter.TransformToAncestor(scrollContentPresenter).TransformBounds(new Rect(0,0, headerPresenter.ActualWidth, headerPresenter.ActualHeight));

                            String str = String.Format("{0} {1}", headerPresenter, headerPresenter.DataContext);
                            bool isFocusable = (child is FocusableType || headerPresenter.Focusable);
                            masterContainerArrangeDataList.Add(new MasterContainerArrangeData(arrangeRect, str, isFocusable, true));
                        }

                        if (virtualizingChild.ItemsHost != null && virtualizingChild.ItemsHost.IsVisible)
                        {
                            GenerateMasterFromItemsHostRecursive<FocusableType>(scrollContentPresenter, virtualizingChild.ItemsHost, masterContainerArrangeDataList);
                        }
                    }
                    else
                    {
                        Rect arrangeRect = child.TransformToAncestor(scrollContentPresenter).TransformBounds(new Rect(0,0, child.ActualWidth, child.ActualHeight));
                        String str = String.Format("{0} {1}", child, child.DataContext);
                        bool isFocusable = IsFocusable<FocusableType>(child);
                        masterContainerArrangeDataList.Add(new MasterContainerArrangeData(arrangeRect, str, isFocusable, false));
                    }
                }
            }
        }

        private static bool IsFocusable<FocusableType>(FrameworkElement child) where FocusableType : FrameworkElement
        {
            FocusableType focusableChild = child as FocusableType;
            if (focusableChild == null)
            {
                focusableChild = FindChild<FocusableType>(child);
            }
            return focusableChild != null;
        }

        private void TestItemsControlWhenItemScrolling(Action testAction)
        {
            try
            {
                IsItemScrolling = true;
                testAction();
            }
            finally
            {
                IsItemScrolling = false;
            }
        }

        private void TestItemsControlWhenVirtualizing(Action testAction)
        {
            try
            {
                IsVirtualizing = true;

                // When ItemScrolling we get tripped up by a number of double precision issues leading to
                // false positives. Hence we test collection changes only when pixel scrolling.
                if (!IsItemScrolling)
                {
                    for (int i=0; i<(int)CollectionChangeOperation.Last; i++)
                    {
                        try
                        {
                            CollectionChangeOperation = (CollectionChangeOperation)i;
                            testAction();
                        }
                        finally
                        {
                            CollectionChangeOperation = CollectionChangeOperation.None;
                        }
                    }
                }
                else
                {
                    testAction();
                }

                CacheLength = new VirtualizationCacheLength(50);
                CacheLengthUnit = VirtualizationCacheLengthUnit.Pixel;
                testAction();

                CacheLength = new VirtualizationCacheLength(5);
                CacheLengthUnit = VirtualizationCacheLengthUnit.Item;
                testAction();
            }
            finally
            {
                CacheLength = new VirtualizationCacheLength(0);
                CacheLengthUnit = VirtualizationCacheLengthUnit.Pixel;
                IsVirtualizing = false;
            }
        }

        private void TestItemsControlWhenGrouping(Action testAction)
        {
            try
            {
                IsGrouping = true;
                testAction();
            }
            finally
            {
                IsGrouping = false;
            }
        }

        private void TestItemsControlWhenGroupingWithVSP(Action testAction)
        {
            try
            {
                UseVirtualizingStackPanelAsGroupPanel = true;
                testAction();
            }
            finally
            {
                UseVirtualizingStackPanelAsGroupPanel = false;
            }
        }

        private void TestItemsControlWhenGroupingWithExpanders(Action testAction)
        {
            try
            {
                UseExpanderInGroupItemStyle = true;
                testAction();
                IsFirstGroupCollapsed = true;

                // 
                if (!(IsVirtualizing && IsItemScrolling && UseExpanderInGroupItemStyle))
                {
                    testAction();
                }
            }
            finally
            {
                IsFirstGroupCollapsed = false;
                UseExpanderInGroupItemStyle = false;
            }
        }

        private void TestItemsControl<FocusableType>(
            IList<MasterArrangeData> masterArrangeDataList) where FocusableType : FrameworkElement
        {
            int testIndex = (int)CollectionChangeOperation * 4;

            if (IsGrouping)
            {
                if (UseExpanderInGroupItemStyle)
                {
                    if (IsFirstGroupCollapsed)
                    {
                        testIndex += 3;
                    }
                    else
                    {
                        testIndex += 2;
                    }
                }
                else
                {
                    testIndex += 1;
                }
            }

            bool isHorizontal = false;
            IList<int> iterationsDataList = IterationsDataList;

            TestItemsControl<FocusableType>(
                isHorizontal,
                iterationsDataList,
                masterArrangeDataList[testIndex]);
        }

        private void TestItemsControl<FocusableType>(
            bool isHorizontal,
            IList<int> iterationsDataList,
            MasterArrangeData masterArrangeData) where FocusableType : FrameworkElement
        {

            if (iterationsDataList == null || iterationsDataList.Count != (int)TestIndex.LastValue)
            {
                return;
            }

            if (IsItemScrolling && (IsGrouping || _itemsControl is TreeView) && !IsThemeActive)
            {
                return;
            }

            bool isDataGrid = _itemsControl is DataGrid;

            double cacheSizeBeforeViewport = 0;
            double cacheSizeAfterViewport = 0;
            VirtualizationCacheLengthUnit cacheUnit = VirtualizationCacheLengthUnit.Pixel;

            if (IsVirtualizing)
            {
                VirtualizationCacheLength cacheLength = VirtualizingPanel.GetCacheLength(_itemsControl);

                cacheSizeBeforeViewport = cacheLength.CacheBeforeViewport;
                cacheSizeAfterViewport = cacheLength.CacheAfterViewport;
                cacheUnit = VirtualizingPanel.GetCacheLengthUnit(_itemsControl);
            }
            else
            {
                cacheSizeBeforeViewport = Double.PositiveInfinity;
                cacheSizeAfterViewport = Double.PositiveInfinity;
            }

            double svHorizontalOffset = 0;
            double svVerticalOffset = 0;
            ValidateArrangeResult validateArrangeResult = null;

            // InitialLoad
            ValidateItemsControl<FocusableType>(
                isHorizontal,
                IsItemScrolling,
                svHorizontalOffset,
                svVerticalOffset,
                cacheSizeBeforeViewport,
                cacheSizeAfterViewport,
                cacheUnit,
                masterArrangeData,
                ref validateArrangeResult);

            // 


            if (!(isDataGrid && IsItemScrolling && IsGrouping && !IsVirtualizing && UseExpanderInGroupItemStyle))
            {

            // ScrollToEndAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToEndAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.ScrollToBottom();
                }
                else
                {
                    _scrollViewer.ScrollToRightEnd();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.End,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }
            }

            // ScrollToHomeAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToHomeAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.ScrollToTop();
                }
                else
                {
                    _scrollViewer.ScrollToLeftEnd();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Home,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // PageDownAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageDownAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.PageDown();
                }
                else
                {
                    _scrollViewer.PageRight();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.PageDown,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // PageUpAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageUpAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.PageUp();
                }
                else
                {
                    _scrollViewer.PageLeft();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.PageUp,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // LineDownAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineDownAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.LineDown();
                }
                else
                {
                    _scrollViewer.LineRight();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Down,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            int iterationCount = iterationsDataList[(int)TestIndex.LineUpAlongMajorAxis];
            // LineUpAlongMajorAxis
            for (int i = 0; i < iterationCount; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.LineUp();
                }
                else
                {
                    _scrollViewer.LineLeft();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Up,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // ScrollToEndAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToEndAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.ScrollToBottom();
                }
                else
                {
                    _scrollViewer.ScrollToRightEnd();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    true /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.End,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // ScrollToHomeAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToHomeAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.ScrollToTop();
                }
                else
                {
                    _scrollViewer.ScrollToLeftEnd();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    true /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Home,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // PageDownAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageDownAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.PageDown();
                }
                else
                {
                    _scrollViewer.PageRight();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    true /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.PageDown,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // PageUpAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageUpAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.PageUp();
                }
                else
                {
                    _scrollViewer.PageLeft();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    true /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.PageUp,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // LineDownAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineDownAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.LineDown();
                }
                else
                {
                    _scrollViewer.LineRight();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    true /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Down,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // LineUpAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineUpAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.LineUp();
                }
                else
                {
                    _scrollViewer.LineLeft();
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    true /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Up,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // SetFocus to the ItemsControl in preparation for keyboard operations
            _itemsControl.Focus();
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            // 
            if (!(IsGrouping && isDataGrid && IsVirtualizing))
            {
                // KeyEnd
                for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyEnd]; i++)
                {
                    if (isDataGrid)
                    {
                        DRT.SendKeyboardInput(Key.LeftCtrl, true);
                    }
                    DRT.PressKey(Key.End);
                    if (isDataGrid)
                    {
                        DRT.SendKeyboardInput(Key.LeftCtrl, false);
                    }

                    UpdateOffsets(
                        isHorizontal,
                        IsItemScrolling,
                        true /*isKeyboardOperation*/,
                        false /*isMouseWheelOperation*/,
                        false /*isMinorAxis*/,
                        false /*hackToIgnoreFocusedContainer*/,
                        Key.End,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }
            }

            // KeyHome
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyHome]; i++)
            {
                if (isDataGrid)
                {
                    DRT.SendKeyboardInput(Key.LeftCtrl, true);
                }
                DRT.PressKey(Key.Home);
                if (isDataGrid)
                {
                    DRT.SendKeyboardInput(Key.LeftCtrl, false);
                }

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    true /*isKeyboardOperation*/,
                    false /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Home,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // 
            if (!(IsGrouping && isDataGrid && IsVirtualizing))
            {
                // PageDownAlongMajorAxis
                for (int i=0; i<iterationsDataList[(int)TestIndex.PageDownAlongMajorAxis]; i++)
                {
                    if (!isHorizontal)
                    {
                        _scrollViewer.PageDown();
                    }
                    else
                    {
                        _scrollViewer.PageRight();
                    }

                    UpdateOffsets(
                        isHorizontal,
                        IsItemScrolling,
                        false /*isKeyboardOperation*/,
                        false /*isMouseWheelOperation*/,
                        false /*isMinorAxis*/,
                        false /*hackToIgnoreFocusedContainer*/,
                        Key.PageDown,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }

                // KeyPageDown
                for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyPageDown]; i++)
                {
                    DRT.PressKey(Key.PageDown);

                    UpdateOffsets(
                        isHorizontal,
                        IsItemScrolling,
                        true /*isKeyboardOperation*/,
                        false /*isMouseWheelOperation*/,
                        false /*isMinorAxis*/,
                        i>0 /*hackToIgnoreFocusedContainer*/,
                        Key.PageDown,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }

                // PageUpAlongMajorAxis
                for (int i=0; i<iterationsDataList[(int)TestIndex.PageUpAlongMajorAxis]; i++)
                {
                    if (!isHorizontal)
                    {
                        _scrollViewer.PageUp();
                    }
                    else
                    {
                        _scrollViewer.PageLeft();
                    }

                    UpdateOffsets(
                        isHorizontal,
                        IsItemScrolling,
                        false /*isKeyboardOperation*/,
                        false /*isMouseWheelOperation*/,
                        false /*isMinorAxis*/,
                        false /*hackToIgnoreFocusedContainer*/,
                        Key.PageUp,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }

                // KeyPageUp
                iterationCount = iterationsDataList[(int)TestIndex.KeyPageUp];
                if (IsGrouping && IsItemScrolling && UseExpanderInGroupItemStyle)
                {
                    // For items based scrolling with expanders in group headers, it
                    // is difficult to estimate the viewport size in tests. Hence
                    // skip the tests.
                    iterationCount = 0;
                }
                for (int i = 0; i < iterationCount; i++)
                {
                    DRT.PressKey(Key.PageUp);

                    UpdateOffsets(
                            isHorizontal,
                            IsItemScrolling,
                            true /*isKeyboardOperation*/,
                            false /*isMouseWheelOperation*/,
                            false /*isMinorAxis*/,
                            i > 0 /*hackToIgnoreFocusedContainer*/,
                            Key.PageUp,
                            masterArrangeData,
                            validateArrangeResult,
                            ref svHorizontalOffset,
                            ref svVerticalOffset);

                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }

                // KeyLineDown
                iterationCount = iterationsDataList[(int)TestIndex.KeyLineDown];
                if (IsGrouping && IsItemScrolling && UseExpanderInGroupItemStyle)
                {
                    // For items based scrolling with expanders in group headers, it
                    // is difficult to estimate the viewport size in tests. Hence
                    // skip the tests.
                    iterationCount = 0;
                }
                for (int i = 0; i<iterationCount; i++)
                {
                    DRT.PressKey(Key.Down);

                    UpdateOffsets(
                        isHorizontal,
                        IsItemScrolling,
                        true /*isKeyboardOperation*/,
                        false /*isMouseWheelOperation*/,
                        false /*isMinorAxis*/,
                        false /*hackToIgnoreFocusedContainer*/,
                        Key.Down,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }

                // KeyLineUp
                iterationCount = iterationsDataList[(int)TestIndex.KeyLineUp];
                if (IsGrouping && IsItemScrolling && UseExpanderInGroupItemStyle)
                {
                    // For items based scrolling with expanders in group headers, it
                    // is difficult to estimate the viewport size in tests. Hence
                    // skip the tests.
                    iterationCount = 0;
                }
                for (int i = 0; i<iterationCount; i++)
                {
                    DRT.PressKey(Key.Up);

                    UpdateOffsets(
                        isHorizontal,
                        IsItemScrolling,
                        true /*isKeyboardOperation*/,
                        false /*isMouseWheelOperation*/,
                        false /*isMinorAxis*/,
                        false /*hackToIgnoreFocusedContainer*/,
                        Key.Up,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                    ValidateItemsControl<FocusableType>(
                        isHorizontal,
                        IsItemScrolling,
                        svHorizontalOffset,
                        svVerticalOffset,
                        cacheSizeBeforeViewport,
                        cacheSizeAfterViewport,
                        cacheUnit,
                        masterArrangeData,
                        ref validateArrangeResult);
                }
            }

            // MoveMouse over the first container in viewport in preparation for mouse wheel operations
            DRT.MoveMouse(validateArrangeResult.FirstContainerInViewport, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            // ScrollWheelDown
            for (int i = 0; i<iterationsDataList[(int)TestIndex.ScrollWheelDown]; i++)
            {
                DRT.MouseWheelDown();
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    true /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Down,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            // ScrollWheelUp
            for (int i = 0; i<iterationsDataList[(int)TestIndex.ScrollWheelUp]; i++)
            {
                DRT.MouseWheelUp();
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

                UpdateOffsets(
                    isHorizontal,
                    IsItemScrolling,
                    false /*isKeyboardOperation*/,
                    true /*isMouseWheelOperation*/,
                    false /*isMinorAxis*/,
                    false /*hackToIgnoreFocusedContainer*/,
                    Key.Up,
                    masterArrangeData,
                    validateArrangeResult,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
                ValidateItemsControl<FocusableType>(
                    isHorizontal,
                    IsItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    cacheSizeBeforeViewport,
                    cacheSizeAfterViewport,
                    cacheUnit,
                    masterArrangeData,
                    ref validateArrangeResult);
            }

            _scrollViewer.ScrollToHome();

        }



        private void CompensateVerticalOffsetForExpanderBorderThickness(MasterContainerArrangeData containerArrangeData,
            ref double svVerticalOffset)
        {
            if (UseExpanderInGroupItemStyle && IsGrouping && containerArrangeData.IsHeader)
            {
                // compensate for border thickness
                svVerticalOffset -= 2;
            }
        }

        private void UpdateOffsets(
            bool isHorizontal,
            bool isItemScrolling,
            bool isKeyboardOperation,
            bool isMouseWheelOperation,
            bool isMinorAxis,
            bool hackToIgnoreFocusedContainer,
            Key key,
            MasterArrangeData masterArrangeData,
            ValidateArrangeResult validateArrangeResult,
            ref double svHorizontalOffset,
            ref double svVerticalOffset)
        {
            bool isDataGrid = _itemsControl is DataGrid;

            bool isFirst = false;
            bool isLast = false;

            // Floating point math results in a rounding
            // error such that the first or the last container in the viewport are reported as being clipped
            // by some minutely larger than epsilon value. This trips up keyboard navigation and hence
            // confuses the computations we are using in this DRT to verify results. This is a temporary
            // hack to enable these tests despite this 

            if (isKeyboardOperation && !hackToIgnoreFocusedContainer && key != Key.Home && key != Key.End &&
                !IsFocusedContainerWithinViewport(
                    isHorizontal,
                    isItemScrolling,
                    svHorizontalOffset,
                    svVerticalOffset,
                    masterArrangeData,
                    validateArrangeResult,
                    out isFirst,
                    out isLast))
            {
                validateArrangeResult.FirstContainerInViewport = null;
                validateArrangeResult.LastContainerInViewport = null;

                // MakeVisible operation
                MasterContainerArrangeData focusedContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[validateArrangeResult.FocusedContainerIndexInMaster];
                if (validateArrangeResult.FocusedContainerIndexInMaster < validateArrangeResult.FirstContainerIndexInMaster)
                {
                    isFirst = true;

                    if (isHorizontal)
                    {
                        svHorizontalOffset = focusedContainerArrangeData.ArrangeRect.Left;
                    }
                    else
                    {
                        svVerticalOffset = focusedContainerArrangeData.ArrangeRect.Top;
                        if (isItemScrolling)
                        {
                            // Items scrolling always scrolls to the container boundaries, hence offset border thickness
                            CompensateVerticalOffsetForExpanderBorderThickness(focusedContainerArrangeData, ref svVerticalOffset);
                        }
                    }

                    validateArrangeResult.FirstContainerIndexInMaster = validateArrangeResult.FocusedContainerIndexInMaster;
                    validateArrangeResult.FirstContainerInViewport = validateArrangeResult.FocusedContainer;
                    validateArrangeResult.LastContainerIndexInMaster = FindLastIndexInViewport(isHorizontal, svHorizontalOffset, svVerticalOffset, masterArrangeData);
                }
                else // if (validateArrangeResult.FocusedContainerIndexInMaster > validateArrangeResult.LastContainerIndexInMaster)
                {
                    isLast = true;

                    if (isHorizontal)
                    {
                        svHorizontalOffset = focusedContainerArrangeData.ArrangeRect.Right - masterArrangeData.MasterViewportSize.Width;
                    }
                    else
                    {
                        svVerticalOffset = focusedContainerArrangeData.ArrangeRect.Bottom - masterArrangeData.MasterViewportSize.Height;
                    }

                    validateArrangeResult.LastContainerIndexInMaster = validateArrangeResult.FocusedContainerIndexInMaster;
                    validateArrangeResult.LastContainerInViewport = validateArrangeResult.FocusedContainer;
                    validateArrangeResult.FirstContainerIndexInMaster = FindFirstIndexInViewport(isHorizontal, svHorizontalOffset, svVerticalOffset, masterArrangeData);
                }

                if (isItemScrolling)
                {
                    AdjustOffsetsForItemScrolling(
                        isHorizontal,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset);
                }
            }

            int logicalPageSize = 0;
            int logicalDelta = 0;

            if (IsItemScrolling)
            {
                logicalPageSize = validateArrangeResult.LastContainerIndexInMaster - validateArrangeResult.FirstContainerIndexInMaster;
                logicalDelta = isMouseWheelOperation ? SystemParameters.WheelScrollLines : 1;

                if (IsLastItemFullyVisibleInViewport(
                        isHorizontal,
                        masterArrangeData,
                        validateArrangeResult,
                        ref svHorizontalOffset,
                        ref svVerticalOffset))
                {
                    logicalPageSize++;
                }
            }

            switch (key)
            {
                case Key.End:
                    {
                        if (isMinorAxis == isHorizontal)
                        {
                            svVerticalOffset = masterArrangeData.MasterExtentSize.Height - masterArrangeData.MasterViewportSize.Height;
                        }
                        else
                        {
                            svHorizontalOffset = masterArrangeData.MasterExtentSize.Width - masterArrangeData.MasterViewportSize.Width;
                        }

                        if (isItemScrolling && !isMinorAxis)
                        {
                            CoerceOffsets(
                                isHorizontal,
                                isMinorAxis,
                                masterArrangeData,
                                ref svHorizontalOffset,
                                ref svVerticalOffset);

                            AdjustOffsetsForItemScrolling(
                                isHorizontal,
                                masterArrangeData,
                                validateArrangeResult,
                                ref svHorizontalOffset,
                                ref svVerticalOffset);
                        }
                    }
                    break;

                case Key.Home:
                    {
                        if (isMinorAxis == isHorizontal)
                        {
                            svVerticalOffset = 0;
                        }
                        else
                        {
                            svHorizontalOffset = 0;
                        }
                    }
                    break;

                case Key.PageDown:
                    {
                        // Please note that the assumption in this computation is that we will find a focusable
                        // element on the page that we've navigated to and hence wont need to scroll any further.

                        if (!isKeyboardOperation || isLast || hackToIgnoreFocusedContainer)
                        {
                            if (isMinorAxis == isHorizontal)
                            {
                                if (isItemScrolling && !isMinorAxis)
                                {
                                    int index = Math.Min(validateArrangeResult.FirstContainerIndexInMaster + logicalPageSize, masterArrangeData.MasterContainerArrangeDataList.Count-logicalPageSize);
                                    svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Top;

                                    // Items scrolling always scrolls to the container boundaries, hence offset border thickness
                                    CompensateVerticalOffsetForExpanderBorderThickness(masterArrangeData.MasterContainerArrangeDataList[index],
                                        ref svVerticalOffset);
                                }
                                else
                                {
                                    svVerticalOffset += masterArrangeData.MasterViewportSize.Height;
                                }
                            }
                            else
                            {
                                if (isItemScrolling && !isMinorAxis)
                                {
                                    int index = Math.Min(validateArrangeResult.FirstContainerIndexInMaster + logicalPageSize, masterArrangeData.MasterContainerArrangeDataList.Count-logicalPageSize);
                                    svHorizontalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Left;
                                }
                                else
                                {
                                    svHorizontalOffset += masterArrangeData.MasterViewportSize.Height;
                                }
                            }
                        }
                    }
                    break;

                case Key.PageUp:
                    {
                        // Please note that the assumption in this computation is that we will find a focusable
                        // element on the page that we've navigated to and hence wont need to scroll any further.

                        if (!isKeyboardOperation || isFirst || hackToIgnoreFocusedContainer)
                        {
                            if (isMinorAxis == isHorizontal)
                            {
                                if (isItemScrolling && !isMinorAxis)
                                {
                                    int index = Math.Max(validateArrangeResult.FirstContainerIndexInMaster - logicalPageSize, 0);
                                    svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Top;

                                    // Items scrolling always scrolls to the container boundaries, hence offset border thickness
                                    CompensateVerticalOffsetForExpanderBorderThickness(masterArrangeData.MasterContainerArrangeDataList[index],
                                        ref svVerticalOffset);
                                }
                                else
                                {
                                    svVerticalOffset -= masterArrangeData.MasterViewportSize.Height;
                                }
                            }
                            else
                            {
                                if (isItemScrolling && isMinorAxis)
                                {
                                    int index = Math.Max(validateArrangeResult.FirstContainerIndexInMaster - logicalPageSize, 0);
                                    svHorizontalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Left;
                                }
                                else
                                {
                                    svHorizontalOffset -= masterArrangeData.MasterViewportSize.Width;
                                }
                            }
                        }
                    }
                    break;

                case Key.Down:
                    {
                        int index = -1;
                        bool foundFocuable = false;

                        if (isItemScrolling && !isMinorAxis)
                        {
                            index = Math.Min(validateArrangeResult.FirstContainerIndexInMaster + logicalDelta, masterArrangeData.MasterContainerArrangeDataList.Count-logicalPageSize);

                            if (isKeyboardOperation)
                            {
                                index = FindFocusableContainerIndexInDirection(masterArrangeData, validateArrangeResult.FocusedContainerIndexInMaster+1, false /*upwards*/, out foundFocuable);
                                double y = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Bottom - masterArrangeData.MasterViewportSize.Height;
                                for (int i=0; index<masterArrangeData.MasterContainerArrangeDataList.Count; ++i)
                                {
                                    if (!DoubleUtil.GreaterThan(y, masterArrangeData.MasterContainerArrangeDataList[i].ArrangeRect.Top))
                                    {
                                        index = i;
                                        break;
                                    }
                                }

                                if (isDataGrid && !foundFocuable)
                                {
                                    index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);
                                }
                            }

                            svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Top;

                            // Items scrolling always scrolls to the container boundaries, hence offset border thickness
                            CompensateVerticalOffsetForExpanderBorderThickness(masterArrangeData.MasterContainerArrangeDataList[index],
                                ref svVerticalOffset);
                        }
                        else if (isMinorAxis == isHorizontal)
                        {
                            if (!isKeyboardOperation)
                            {
                                svVerticalOffset += isMouseWheelOperation ? _scrollLineDelta * SystemParameters.WheelScrollLines : _scrollLineDelta;
                            }
                            else if (isLast)
                            {
                                if (DoubleUtil.GreaterThan(
                                        masterArrangeData.MasterContainerArrangeDataList[validateArrangeResult.LastContainerIndexInMaster].ArrangeRect.Bottom,
                                        svVerticalOffset + masterArrangeData.MasterViewportSize.Height))
                                {
                                    index = validateArrangeResult.LastContainerIndexInMaster;
                                }
                                else
                                {
                                    index = Math.Min(validateArrangeResult.LastContainerIndexInMaster + 1, masterArrangeData.MasterContainerArrangeDataList.Count-logicalPageSize);
                                }
                                index = FindFocusableContainerIndexInDirection(masterArrangeData, index, false /*upwards*/, out foundFocuable);

                                if (isDataGrid && !foundFocuable)
                                {
                                    index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);
                                }

                                svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Bottom - masterArrangeData.MasterViewportSize.Height;
                            }
                        }
                        else if (!isKeyboardOperation)
                        {
                            svHorizontalOffset += isMouseWheelOperation ? _scrollLineDelta * SystemParameters.WheelScrollLines : _scrollLineDelta;
                        }
                        else if (isLast)
                        {
                            if (DoubleUtil.GreaterThan(
                                    masterArrangeData.MasterContainerArrangeDataList[validateArrangeResult.LastContainerIndexInMaster].ArrangeRect.Right,
                                    svHorizontalOffset + masterArrangeData.MasterViewportSize.Width))
                            {
                                index = validateArrangeResult.LastContainerIndexInMaster;
                            }
                            else
                            {
                                index = Math.Min(validateArrangeResult.LastContainerIndexInMaster + 1, masterArrangeData.MasterContainerArrangeDataList.Count-logicalPageSize);
                            }
                            index = FindFocusableContainerIndexInDirection(masterArrangeData, index, false /*upwards*/, out foundFocuable);

                            if (isDataGrid && !foundFocuable)
                            {
                                index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);
                            }

                            svHorizontalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Right - masterArrangeData.MasterViewportSize.Width;
                        }
                    }
                    break;

                case Key.Up:
                    {
                        if (!isKeyboardOperation || isFirst)
                        {
                            int index = -1;
                            bool foundFocuable = false;

                            if (isMinorAxis == isHorizontal)
                            {
                                if (isItemScrolling && !isMinorAxis)
                                {
                                    index = Math.Max(validateArrangeResult.FirstContainerIndexInMaster - logicalDelta, 0);

                                    if (isKeyboardOperation)
                                    {
                                        index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);
                                    }

                                    svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Top;

                                    // Items scrolling always scrolls to the container boundaries, hence offset border thickness
                                    CompensateVerticalOffsetForExpanderBorderThickness(masterArrangeData.MasterContainerArrangeDataList[index],
                                        ref svVerticalOffset);
                                }
                                else if (isKeyboardOperation)
                                {
                                    if (DoubleUtil.LessThan(
                                           masterArrangeData.MasterContainerArrangeDataList[validateArrangeResult.FirstContainerIndexInMaster].ArrangeRect.Top,
                                           svVerticalOffset))
                                    {
                                        index = validateArrangeResult.FirstContainerIndexInMaster;
                                    }
                                    else
                                    {
                                        index = Math.Max(validateArrangeResult.FirstContainerIndexInMaster - 1, 0);
                                    }
                                    index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);

                                    svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Top;

                                    // Subsequent UP keys when already at top most focusable element should
                                    // always result in 0 vertical offset.
                                    if (index == 0 &&
                                        validateArrangeResult.FocusedContainerIndexInMaster == 0)
                                    {
                                        svVerticalOffset = 0;
                                    }
                                }
                                else
                                {
                                    svVerticalOffset -= isMouseWheelOperation ? _scrollLineDelta * SystemParameters.WheelScrollLines : _scrollLineDelta;
                                }
                            }
                            else
                            {
                                if (isItemScrolling && !isMinorAxis)
                                {
                                    index = Math.Max(validateArrangeResult.FirstContainerIndexInMaster - logicalDelta, 0);

                                    if (isKeyboardOperation)
                                    {
                                        index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);

                                        if (isDataGrid && !foundFocuable)
                                        {
                                            index = FindFocusableContainerIndexInDirection(masterArrangeData, index, false /*upwards*/, out foundFocuable);
                                        }
                                    }

                                    svHorizontalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Left;
                                }
                                else if (isKeyboardOperation)
                                {
                                    if (DoubleUtil.LessThan(
                                           masterArrangeData.MasterContainerArrangeDataList[validateArrangeResult.FirstContainerIndexInMaster].ArrangeRect.Left,
                                           svHorizontalOffset))
                                    {
                                        index = validateArrangeResult.FirstContainerIndexInMaster;
                                    }
                                    else
                                    {
                                        index = Math.Max(validateArrangeResult.FirstContainerIndexInMaster - 1, 0);
                                    }
                                    index = FindFocusableContainerIndexInDirection(masterArrangeData, index, true /*upwards*/, out foundFocuable);

                                    if (isDataGrid && !foundFocuable)
                                    {
                                        index = FindFocusableContainerIndexInDirection(masterArrangeData, index, false /*upwards*/, out foundFocuable);
                                    }

                                    svHorizontalOffset = masterArrangeData.MasterContainerArrangeDataList[index].ArrangeRect.Left;
                                }
                                else
                                {
                                    svHorizontalOffset -= isMouseWheelOperation ? _scrollLineDelta * SystemParameters.WheelScrollLines : _scrollLineDelta;
                                }
                            }
                        }
                    }
                    break;
            }

            if (!isItemScrolling || isMinorAxis)
            {
                CoerceOffsets(
                    isHorizontal,
                    isMinorAxis,
                    masterArrangeData,
                    ref svHorizontalOffset,
                    ref svVerticalOffset);
            }

            if (TraceUpdateOffsets)
            {
                Console.WriteLine("UpdateOffsets {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
                    svVerticalOffset, _itemsControl.Name,
                    isHorizontal,
                    isItemScrolling,
                    isKeyboardOperation,
                    isMouseWheelOperation,
                    isMinorAxis,
                    hackToIgnoreFocusedContainer,
                    key,
                    validateArrangeResult.FirstContainerIndexInMaster,
                    validateArrangeResult.LastContainerIndexInMaster,
                    validateArrangeResult.FocusedContainerIndexInMaster);
            }
        }

        private int FindFocusableContainerIndexInDirection(
            MasterArrangeData masterArrangeData,
            int startIndex,
            bool upwards,
            out bool foundFocusable)
        {
            int incr = upwards ? -1 : +1;
            int endCount = upwards ? -1 : masterArrangeData.MasterContainerArrangeDataList.Count;

            for (int i=startIndex; i!=endCount; i+=incr)
            {
                MasterContainerArrangeData masterContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[i];
                if (masterContainerArrangeData.IsFocusable)
                {
                    foundFocusable = true;
                    return i;
                }
            }

            foundFocusable = false;
            return endCount-incr;
        }

        private bool IsLastItemFullyVisibleInViewport(
            bool isHorizontal,
            MasterArrangeData masterArrangeData,
            ValidateArrangeResult validateArrangeResult,
            ref double svHorizontalOffset,
            ref double svVerticalOffset)
        {
            MasterContainerArrangeData lastContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[validateArrangeResult.LastContainerIndexInMaster];

            if (isHorizontal)
            {
                return DoubleUtil.LessThanOrClose(lastContainerArrangeData.ArrangeRect.Right, svHorizontalOffset + masterArrangeData.MasterViewportSize.Width);
            }
            else
            {
                return DoubleUtil.LessThanOrClose(lastContainerArrangeData.ArrangeRect.Bottom, svVerticalOffset + masterArrangeData.MasterViewportSize.Height);
            }
        }

        private void CoerceOffsets(
            bool isHorizontal,
            bool isMinorAxis,
            MasterArrangeData masterArrangeData,
            ref double svHorizontalOffset,
            ref double svVerticalOffset)
        {
            if (isMinorAxis == isHorizontal)
            {
                svVerticalOffset = Math.Max(Math.Min(
                    svVerticalOffset,
                    masterArrangeData.MasterExtentSize.Height - masterArrangeData.MasterViewportSize.Height), 0);
            }
            else
            {
                svHorizontalOffset = Math.Max(Math.Min(
                    svHorizontalOffset,
                    masterArrangeData.MasterExtentSize.Width - masterArrangeData.MasterViewportSize.Width), 0);
            }
        }

        private void AdjustOffsetsForItemScrolling(
            bool isHorizontal,
            MasterArrangeData masterArrangeData,
            ValidateArrangeResult validateArrangeResult,
            ref double svHorizontalOffset,
            ref double svVerticalOffset)
        {
            int masterContainerArrangeDataListIndex =
                FindFirstIndexInViewport(isHorizontal, svHorizontalOffset, svVerticalOffset, masterArrangeData);

            MasterContainerArrangeData masterContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[masterContainerArrangeDataListIndex];

            if (isHorizontal)
            {
                if (DoubleUtil.LessThan(masterContainerArrangeData.ArrangeRect.X, svHorizontalOffset))
                {
                    svHorizontalOffset = masterArrangeData.MasterContainerArrangeDataList[++masterContainerArrangeDataListIndex].ArrangeRect.Left;
                }
            }
            else
            {
                if (DoubleUtil.LessThan(masterContainerArrangeData.ArrangeRect.Y, svVerticalOffset))
                {
                    svVerticalOffset = masterArrangeData.MasterContainerArrangeDataList[++masterContainerArrangeDataListIndex].ArrangeRect.Top;
                }
            }
        }

        private bool IsFocusedContainerWithinViewport(
            bool isHorizontal,
            bool isItemScrolling,
            double svHorizontalOffset,
            double svVerticalOffset,
            MasterArrangeData masterArrangeData,
            ValidateArrangeResult validateArrangeResult,
            out bool isFirst,
            out bool isLast)
        {
            isFirst = false;
            isLast = false;

            int focusedContainerIndexInMaster = validateArrangeResult.FocusedContainerIndexInMaster;
            MasterContainerArrangeData focusedContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[focusedContainerIndexInMaster];
            int firstContainerIndexInMaster = validateArrangeResult.FirstContainerIndexInMaster;
            MasterContainerArrangeData firstContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[firstContainerIndexInMaster];
            int lastContainerIndexInMaster = validateArrangeResult.LastContainerIndexInMaster;
            MasterContainerArrangeData lastContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[lastContainerIndexInMaster];


            bool areOffsetsWithinViewport = focusedContainerIndexInMaster > validateArrangeResult.FirstContainerIndexInMaster &&
                                            focusedContainerIndexInMaster < validateArrangeResult.LastContainerIndexInMaster;

            if (!areOffsetsWithinViewport)
            {
                if (isHorizontal)
                {
                    if (focusedContainerIndexInMaster == validateArrangeResult.FirstContainerIndexInMaster)
                    {
                        areOffsetsWithinViewport = DoubleUtil.GreaterThanOrClose(focusedContainerArrangeData.ArrangeRect.Left, svHorizontalOffset);
                        isFirst = areOffsetsWithinViewport;
                    }
                    else if (focusedContainerIndexInMaster == validateArrangeResult.LastContainerIndexInMaster)
                    {
                        areOffsetsWithinViewport = DoubleUtil.LessThanOrClose(focusedContainerArrangeData.ArrangeRect.Right, svHorizontalOffset + masterArrangeData.MasterViewportSize.Width);
                        isLast = areOffsetsWithinViewport;
                    }
                }
                else
                {
                    if (focusedContainerIndexInMaster == validateArrangeResult.FirstContainerIndexInMaster)
                    {
                        areOffsetsWithinViewport = DoubleUtil.GreaterThanOrClose(focusedContainerArrangeData.ArrangeRect.Top, svVerticalOffset);
                        isFirst = areOffsetsWithinViewport;
                    }
                    else if (focusedContainerIndexInMaster == validateArrangeResult.LastContainerIndexInMaster)
                    {
                        areOffsetsWithinViewport = DoubleUtil.LessThanOrClose(focusedContainerArrangeData.ArrangeRect.Bottom, svVerticalOffset + masterArrangeData.MasterViewportSize.Height);
                        isLast = areOffsetsWithinViewport;
                    }
                }
            }
            else
            {
                if (isHorizontal)
                {
                    isFirst = (focusedContainerIndexInMaster == validateArrangeResult.FirstContainerIndexInMaster+1) &&
                               DoubleUtil.LessThan(firstContainerArrangeData.ArrangeRect.Left, svHorizontalOffset);
                    isLast = (focusedContainerIndexInMaster == validateArrangeResult.LastContainerIndexInMaster-1) &&
                              DoubleUtil.GreaterThan(lastContainerArrangeData.ArrangeRect.Right, svHorizontalOffset + masterArrangeData.MasterViewportSize.Width);
                }
                else
                {
                    isFirst = (focusedContainerIndexInMaster == validateArrangeResult.FirstContainerIndexInMaster+1) &&
                               DoubleUtil.LessThan(firstContainerArrangeData.ArrangeRect.Top, svVerticalOffset);
                    isLast = (focusedContainerIndexInMaster == validateArrangeResult.LastContainerIndexInMaster-1) &&
                              DoubleUtil.GreaterThan(lastContainerArrangeData.ArrangeRect.Bottom, svVerticalOffset + masterArrangeData.MasterViewportSize.Height);
                }
            }

            return areOffsetsWithinViewport;
        }

        private void ValidateItemsControl<FocusableType>(
            bool isHorizontal,
            bool isItemScrolling,
            double svHorizontalOffset,
            double svVerticalOffset,
            double cacheSizeBeforeViewport,
            double cacheSizeAfterViewport,
            VirtualizationCacheLengthUnit cacheUnit,
            MasterArrangeData masterArrangeData,
            ref ValidateArrangeResult validateArrangeResult) where FocusableType : FrameworkElement
        {
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            ValidateArrangeResult oldValidateArrangeResult = validateArrangeResult;
            validateArrangeResult = new ValidateArrangeResult();
            if (oldValidateArrangeResult != null)
            {
                validateArrangeResult.FocusedContainer = oldValidateArrangeResult.FocusedContainer;
                validateArrangeResult.FocusedContainerIndexInMaster = oldValidateArrangeResult.FocusedContainerIndexInMaster;
            }

            ItemsPresenter itemsPresenter = FindChild<ItemsPresenter>(_scrollContentPresenter);
            Panel itemsHost = VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;

            int firstContainerIndexInMaster = FindFirstIndexInViewport(
                isHorizontal,
                svHorizontalOffset,
                svVerticalOffset,
                masterArrangeData);
            int lastContainerIndexInMaster = FindLastIndexInViewport(
                isHorizontal,
                svHorizontalOffset,
                svVerticalOffset,
                masterArrangeData);

            int masterContainerArrangeDataListIndex;
            if (cacheUnit == VirtualizationCacheLengthUnit.Pixel)
            {
                if (isHorizontal)
                {
                    masterContainerArrangeDataListIndex = FindFirstIndexInViewport(
                        isHorizontal,
                        Math.Max(svHorizontalOffset - cacheSizeBeforeViewport, 0),
                        svVerticalOffset,
                        masterArrangeData);
                }
                else
                {
                    masterContainerArrangeDataListIndex = FindFirstIndexInViewport(
                        isHorizontal,
                        svHorizontalOffset,
                        Math.Max(svVerticalOffset - cacheSizeBeforeViewport, 0),
                        masterArrangeData);
                }
            }
            else
            {
                masterContainerArrangeDataListIndex = Math.Max(firstContainerIndexInMaster - (int)cacheSizeBeforeViewport, 0);
                int lastContainerIndexInExtendedViewport = Math.Min(lastContainerIndexInMaster + (int)cacheSizeAfterViewport, masterArrangeData.MasterContainerArrangeDataList.Count-1);

                if (isHorizontal)
                {
                    cacheSizeBeforeViewport = svHorizontalOffset - masterArrangeData.MasterContainerArrangeDataList[masterContainerArrangeDataListIndex].ArrangeRect.Left;
                    cacheSizeAfterViewport = masterArrangeData.MasterContainerArrangeDataList[lastContainerIndexInExtendedViewport].ArrangeRect.Right - svHorizontalOffset - masterArrangeData.MasterViewportSize.Width;
                }
                else
                {
                    cacheSizeBeforeViewport = svVerticalOffset - masterArrangeData.MasterContainerArrangeDataList[masterContainerArrangeDataListIndex].ArrangeRect.Top;
                    cacheSizeAfterViewport = masterArrangeData.MasterContainerArrangeDataList[lastContainerIndexInExtendedViewport].ArrangeRect.Bottom - svVerticalOffset - masterArrangeData.MasterViewportSize.Height;
                }
            }

            ValidateContainerArrangeRecursive<FocusableType>(
                _scrollContentPresenter,
                itemsHost,
                isHorizontal,
                isItemScrolling,
                svHorizontalOffset,
                svVerticalOffset,
                cacheSizeBeforeViewport,
                cacheSizeAfterViewport,
                masterArrangeData,
                validateArrangeResult,
                firstContainerIndexInMaster,
                lastContainerIndexInMaster,
                ref masterContainerArrangeDataListIndex);
        }

        private void ValidateContainerArrangeRecursive<FocusableType>(
            ScrollContentPresenter scrollContentPresenter,
            Panel itemsHost,
            bool isHorizontal,
            bool isItemScrolling,
            double svHorizontalOffset,
            double svVerticalOffset,
            double cacheSizeBeforeViewport,
            double cacheSizeAfterViewport,
            MasterArrangeData masterArrangeData,
            ValidateArrangeResult validateArrangeResult,
            int firstContainerIndexInMaster,
            int lastContainerIndexInMaster,
            ref int masterContainerArrangeDataListIndex) where FocusableType : FrameworkElement
        {
            IList children = itemsHost.Children;
            int count = children.Count;

            for (int i=0; i<count; i++)
            {
                FrameworkElement child = (FrameworkElement)children[i];
                if (child != null)
                {
                    IHierarchicalVirtualizationAndScrollInfo virtualizingChild = child as IHierarchicalVirtualizationAndScrollInfo;
                    if (virtualizingChild != null)
                    {
                        Control controlChild = (Control)virtualizingChild;
                        Expander expander = null;
                        FrameworkElement headerPresenter = controlChild.Template.FindName("PART_Header", controlChild) as FrameworkElement;
                        if (headerPresenter == null)
                        {
                            expander = FindChild<Expander>(child);
                            if (expander != null)
                            {
                                headerPresenter = expander.Template.FindName("HeaderSite", expander) as FrameworkElement;
                            }
                        }

                        if (headerPresenter != null)
                        {
                            Rect arrangeRect = headerPresenter.TransformToAncestor(scrollContentPresenter).TransformBounds(new Rect(0,0, headerPresenter.ActualWidth, headerPresenter.ActualHeight));

                            ValidateSingleContainerArrange(
                                headerPresenter,
                                controlChild,
                                arrangeRect,
                                isHorizontal,
                                isItemScrolling,
                                svHorizontalOffset,
                                svVerticalOffset,
                                cacheSizeBeforeViewport,
                                cacheSizeAfterViewport,
                                masterArrangeData,
                                validateArrangeResult,
                                firstContainerIndexInMaster,
                                lastContainerIndexInMaster,
                                ref masterContainerArrangeDataListIndex);
                        }

                        if (virtualizingChild.ItemsHost != null && virtualizingChild.ItemsHost.IsVisible)
                        {
                            ValidateContainerArrangeRecursive<FocusableType>(
                                scrollContentPresenter,
                                virtualizingChild.ItemsHost,
                                isHorizontal,
                                isItemScrolling,
                                svHorizontalOffset,
                                svVerticalOffset,
                                cacheSizeBeforeViewport,
                                cacheSizeAfterViewport,
                                masterArrangeData,
                                validateArrangeResult,
                                firstContainerIndexInMaster,
                                lastContainerIndexInMaster,
                                ref masterContainerArrangeDataListIndex);
                        }
                    }
                    else
                    {
                        Rect arrangeRect = child.TransformToAncestor(scrollContentPresenter).TransformBounds(new Rect(0,0, child.ActualWidth, child.ActualHeight));

                        ValidateSingleContainerArrange(
                            child,
                            child,
                            arrangeRect,
                            isHorizontal,
                            isItemScrolling,
                            svHorizontalOffset,
                            svVerticalOffset,
                            cacheSizeBeforeViewport,
                            cacheSizeAfterViewport,
                            masterArrangeData,
                            validateArrangeResult,
                            firstContainerIndexInMaster,
                            lastContainerIndexInMaster,
                            ref masterContainerArrangeDataListIndex);
                    }
                }
            }
        }

        private void ValidateSingleContainerArrange(
            FrameworkElement container,
            FrameworkElement alternateFocusedElement,
            Rect arrangeRect,
            bool isHorizontal,
            bool isItemScrolling,
            double svHorizontalOffset,
            double svVerticalOffset,
            double cacheSizeBeforeViewport,
            double cacheSizeAfterViewport,
            MasterArrangeData masterArrangeData,
            ValidateArrangeResult validateArrangeResult,
            int firstContainerIndexInMaster,
            int lastContainerIndexInMaster,
            ref int masterContainerArrangeDataListIndex)
        {
            MasterContainerArrangeData masterContainerArrangeData = null;
            bool isContainerInExtendedViewport = false;

            if (isHorizontal)
            {
                if (DoubleUtil.GreaterThan(arrangeRect.Right , -cacheSizeBeforeViewport) &&
                    DoubleUtil.LessThan(arrangeRect.Left, masterArrangeData.MasterViewportSize.Width + cacheSizeAfterViewport))
                {
                    isContainerInExtendedViewport = true;
                }
            }
            else
            {
                if (DoubleUtil.GreaterThan(arrangeRect.Bottom, -cacheSizeBeforeViewport) &&
                    DoubleUtil.LessThan(arrangeRect.Top, masterArrangeData.MasterViewportSize.Height + cacheSizeAfterViewport))
                {
                    isContainerInExtendedViewport = true;
                }
            }

            if (isContainerInExtendedViewport)
            {
                masterContainerArrangeData = masterArrangeData.MasterContainerArrangeDataList[masterContainerArrangeDataListIndex];

                arrangeRect.Offset(svHorizontalOffset, svVerticalOffset);
                String str = String.Format("{0} {1}", container, container.DataContext);

                DRT.Assert(masterContainerArrangeData.AreEqual(arrangeRect, str),
                           "Container {0} is arranged at incorrect position.",
                           container);

                // Is this the first container in the viewport
                if (masterContainerArrangeDataListIndex == firstContainerIndexInMaster)
                {
                    validateArrangeResult.FirstContainerInViewport = container;
                    validateArrangeResult.FirstContainerIndexInMaster = masterContainerArrangeDataListIndex;
                }

                // Is this the last container in the viewport
                if (masterContainerArrangeDataListIndex == lastContainerIndexInMaster)
                {
                    validateArrangeResult.LastContainerInViewport = container;
                    validateArrangeResult.LastContainerIndexInMaster = masterContainerArrangeDataListIndex;
                }

                masterContainerArrangeDataListIndex++;
            }

            if (validateArrangeResult.FocusedContainer != container &&
                (container.IsKeyboardFocusWithin || alternateFocusedElement.IsKeyboardFocused))
            {
                validateArrangeResult.FocusedContainer = container;
                validateArrangeResult.FocusedContainerIndexInMaster = masterContainerArrangeDataListIndex-1;
            }
        }
//#endif

        private void TestPerformance()
        {
            _itemsControl = (ItemsControl)(((TabItem)_tabControl.Items[SelectedTabIndex]).Content);
            _scrollViewer = FindChild<ScrollViewer>(_itemsControl);

            bool isHorizontal = false;
            TestPerformance(isHorizontal, IterationsPerfDataList);

            _perfLogWriter.Flush();
            _perfLogWriter.Close();
        }

        private void TestPerformance(
            bool isHorizontal,
            IList<int> iterationsDataList)
        {
            bool isDataGrid = _itemsControl is DataGrid;
            DrtControls controlsDRT = (DrtControls)DRT;

            Stopwatch stopWatch = Stopwatch.StartNew();

            // ScrollToEndAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToEndAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.ScrollToBottom();
                }
                else
                {
                    _scrollViewer.ScrollToRightEnd();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("ScrollToEndAlongMajorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.ScrollToEndAlongMajorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // ScrollToHomeAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToHomeAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.ScrollToTop();
                }
                else
                {
                    _scrollViewer.ScrollToLeftEnd();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("ScrollToHomeAlongMajorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.ScrollToHomeAlongMajorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // PageDownAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageDownAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.PageDown();
                }
                else
                {
                    _scrollViewer.PageRight();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("PageDownAlongMajorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.PageDownAlongMajorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // PageUpAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageUpAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.PageUp();
                }
                else
                {
                    _scrollViewer.PageLeft();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("PageUpAlongMajorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.PageUpAlongMajorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // LineDownAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineDownAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.LineDown();
                }
                else
                {
                    _scrollViewer.LineRight();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("LineDownAlongMajorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.LineDownAlongMajorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // LineUpAlongMajorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineUpAlongMajorAxis]; i++)
            {
                if (!isHorizontal)
                {
                    _scrollViewer.LineUp();
                }
                else
                {
                    _scrollViewer.LineLeft();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("LineUpAlongMajorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.LineUpAlongMajorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // ScrollToEndAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToEndAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.ScrollToBottom();
                }
                else
                {
                    _scrollViewer.ScrollToRightEnd();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("ScrollToEndAlongMinorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.ScrollToEndAlongMinorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // ScrollToHomeAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.ScrollToHomeAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.ScrollToTop();
                }
                else
                {
                    _scrollViewer.ScrollToLeftEnd();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("ScrollToHomeAlongMinorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.ScrollToHomeAlongMinorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // PageDownAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageDownAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.PageDown();
                }
                else
                {
                    _scrollViewer.PageRight();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("PageDownAlongMinorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.PageDownAlongMinorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // PageUpAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.PageUpAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.PageUp();
                }
                else
                {
                    _scrollViewer.PageLeft();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("PageUpAlongMinorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.PageUpAlongMinorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // LineDownAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineDownAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.LineDown();
                }
                else
                {
                    _scrollViewer.LineRight();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("LineDownAlongMinorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.LineDownAlongMinorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // LineUpAlongMinorAxis
            for (int i = 0; i < iterationsDataList[(int)TestIndex.LineUpAlongMinorAxis]; i++)
            {
                if (isHorizontal)
                {
                    _scrollViewer.LineUp();
                }
                else
                {
                    _scrollViewer.LineLeft();
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("LineUpAlongMinorAxis({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.LineUpAlongMinorAxis], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);

            // SetFocus to the ItemsControl in preparation for keyboard operations
            _itemsControl.Focus();
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            stopWatch = Stopwatch.StartNew();

            // KeyEnd
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyEnd]; i++)
            {
                if (isDataGrid)
                {
                    DRT.SendKeyboardInput(Key.LeftCtrl, true);
                }
                DRT.PressKey(Key.End);
                if (isDataGrid)
                {
                    DRT.SendKeyboardInput(Key.LeftCtrl, false);
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("KeyEnd({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.KeyEnd], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // KeyHome
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyHome]; i++)
            {
                if (isDataGrid)
                {
                    DRT.SendKeyboardInput(Key.LeftCtrl, true);
                }
                DRT.PressKey(Key.Home);
                if (isDataGrid)
                {
                    DRT.SendKeyboardInput(Key.LeftCtrl, false);
                }

                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("KeyHome({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.KeyHome], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // KeyPageDown
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyPageDown]; i++)
            {
                DRT.PressKey(Key.PageDown);
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("KeyPageDown({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.KeyPageDown], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // KeyPageUp
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyPageUp]; i++)
            {
                DRT.PressKey(Key.PageUp);
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("KeyPageUp({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.KeyPageUp], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // KeyLineDown
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyLineDown]; i++)
            {
                DRT.PressKey(Key.Down);
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("KeyLineDown({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.KeyLineDown], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // KeyLineUp
            for (int i = 0; i<iterationsDataList[(int)TestIndex.KeyLineUp]; i++)
            {
                DRT.PressKey(Key.Up);
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("KeyLineUp({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.KeyLineUp], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);

            // MoveMouse over the first container in viewport in preparation for mouse wheel operations
            _scrollViewer.ScrollToHome();
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            DRT.MoveMouse((UIElement)_itemsControl.ItemContainerGenerator.ContainerFromIndex(0), 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);

            stopWatch = Stopwatch.StartNew();

            // ScrollWheelDown
            for (int i = 0; i<iterationsDataList[(int)TestIndex.ScrollWheelDown]; i++)
            {
                DRT.MouseWheelDown();
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("ScrollWheelDown({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.ScrollWheelDown], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);
            stopWatch = Stopwatch.StartNew();

            // ScrollWheelUp
            for (int i = 0; i<iterationsDataList[(int)TestIndex.ScrollWheelUp]; i++)
            {
                DRT.MouseWheelUp();
                DrtBase.WaitForPriority(DispatcherPriority.ContextIdle);
            }

            stopWatch.Stop();
            _perfLogWriter.WriteLine("ScrollWheelUp({0}) Time for {1} in {2} mode: {3}", iterationsDataList[(int)TestIndex.ScrollWheelUp], controlsDRT.ControlNameForPerformanceTesting, controlsDRT.VirtualizationMode, stopWatch.ElapsedMilliseconds);

            _scrollViewer.ScrollToHome();
        }

        private static IList<int> IterationsDataList
        {
            get
            {
                if (s_iterationsDataList == null)
                {
                    s_iterationsDataList = new Collection<int>();
                    for (int i=0; i<(int)TestIndex.LastValue; i++) s_iterationsDataList.Add(0);

                    s_iterationsDataList[(int)TestIndex.InitialLoad] = 1;
                    s_iterationsDataList[(int)TestIndex.ScrollToEndAlongMajorAxis] = 1;
                    s_iterationsDataList[(int)TestIndex.ScrollToHomeAlongMajorAxis] = 1;
                    s_iterationsDataList[(int)TestIndex.PageDownAlongMajorAxis] = 3;
                    s_iterationsDataList[(int)TestIndex.PageUpAlongMajorAxis] = 2;
                    s_iterationsDataList[(int)TestIndex.LineDownAlongMajorAxis] = 15;
                    s_iterationsDataList[(int)TestIndex.LineUpAlongMajorAxis] = 20;
                    s_iterationsDataList[(int)TestIndex.ScrollToEndAlongMinorAxis] = 1;
                    s_iterationsDataList[(int)TestIndex.ScrollToHomeAlongMinorAxis] = 1;
                    s_iterationsDataList[(int)TestIndex.PageDownAlongMinorAxis] = 2;
                    s_iterationsDataList[(int)TestIndex.PageUpAlongMinorAxis] = 3;
                    s_iterationsDataList[(int)TestIndex.LineDownAlongMinorAxis] = 20;
                    s_iterationsDataList[(int)TestIndex.LineUpAlongMinorAxis] = 15;
                    s_iterationsDataList[(int)TestIndex.KeyEnd] = 1;
                    s_iterationsDataList[(int)TestIndex.KeyHome] = 1;
                    s_iterationsDataList[(int)TestIndex.KeyPageDown] = 2;
                    s_iterationsDataList[(int)TestIndex.KeyPageUp] = 4;
                    s_iterationsDataList[(int)TestIndex.KeyLineDown] = 35;
                    s_iterationsDataList[(int)TestIndex.KeyLineUp] = 40;
                    s_iterationsDataList[(int)TestIndex.ScrollWheelDown] = 5;
                    s_iterationsDataList[(int)TestIndex.ScrollWheelUp] = 15;
                }

                return s_iterationsDataList;
            }
        }

        private static IList<int> IterationsPerfDataList
        {
            get
            {
                if (s_iterationsPerfDataList == null)
                {
                    s_iterationsPerfDataList = new Collection<int>();
                    for (int i=0; i<(int)TestIndex.LastValue; i++) s_iterationsPerfDataList.Add(0);

                    s_iterationsPerfDataList[(int)TestIndex.InitialLoad] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.ScrollToEndAlongMajorAxis] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.ScrollToHomeAlongMajorAxis] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.PageDownAlongMajorAxis] = 10;
                    s_iterationsPerfDataList[(int)TestIndex.PageUpAlongMajorAxis] = 10;
                    s_iterationsPerfDataList[(int)TestIndex.LineDownAlongMajorAxis] = 40;
                    s_iterationsPerfDataList[(int)TestIndex.LineUpAlongMajorAxis] = 40;
                    s_iterationsPerfDataList[(int)TestIndex.ScrollToEndAlongMinorAxis] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.ScrollToHomeAlongMinorAxis] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.PageDownAlongMinorAxis] = 5;
                    s_iterationsPerfDataList[(int)TestIndex.PageUpAlongMinorAxis] = 5;
                    s_iterationsPerfDataList[(int)TestIndex.LineDownAlongMinorAxis] = 10;
                    s_iterationsPerfDataList[(int)TestIndex.LineUpAlongMinorAxis] = 10;
                    s_iterationsPerfDataList[(int)TestIndex.KeyEnd] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.KeyHome] = 1;
                    s_iterationsPerfDataList[(int)TestIndex.KeyPageDown] = 10;
                    s_iterationsPerfDataList[(int)TestIndex.KeyPageUp] = 10;
                    s_iterationsPerfDataList[(int)TestIndex.KeyLineDown] = 40;
                    s_iterationsPerfDataList[(int)TestIndex.KeyLineUp] = 40;
                    s_iterationsPerfDataList[(int)TestIndex.ScrollWheelDown] = 15;
                    s_iterationsPerfDataList[(int)TestIndex.ScrollWheelUp] = 15;
                }

                return s_iterationsPerfDataList;
            }
        }

        #endregion

        #region Data

        private bool _isPageLoaded;

        private WorkingPeople _peopleDataLB;
        private WorkingPeople _peopleDataLV;
        private WorkingPeople _peopleDataDG;
        private Folder _folderDataTV;

        private WorkingPeople _peoplePerfDataLB;
        private WorkingPeople _peoplePerfDataLV;
        private WorkingPeople _peoplePerfDataDG;
        private Folder _folderPerfDataTV;

        private ScrollViewer _scrollViewer;
        private ScrollContentPresenter _scrollContentPresenter;
        private ItemsControl _itemsControl;
        private TabControl _tabControl;
        private FrameworkElement _rootBorder;
        private TextBox _cacheLengthTextBox;
        private TextBox _cacheLengthUnitTextBox;
        private Button _refreshCacheSizesButton;

        private IList<MasterArrangeData> _masterArrangeDataListBox;
        private IList<MasterArrangeData> _masterArrangeDataListView;
        private IList<MasterArrangeData> _masterArrangeDataDataGrid;
        private IList<MasterArrangeData> _masterArrangeDataTreeView;

        private TextWriter _perfLogWriter;

        private static IList<int> s_iterationsDataList;
        private static IList<int> s_iterationsPerfDataList;

        private const double _scrollLineDelta = 16.0;   // Default physical amount to scroll with one Up/Down/Left/Right key
        private const double _mouseWheelDelta = 48.0;   // Default physical amount to scroll with one MouseWheel.

        #endregion

        #region Nested Classes

        public class WorkingPerson : EditablePerson
        {
            public WorkingPerson()
                : base()
            {
            }

            public WorkingPerson(string firstName, string lastName)
                : base(firstName, lastName)
            {
            }

            public string Occupation
            {
                get { return _occupation; }
                set
                {
                    if (_occupation != value)
                    {
                        _occupation = value;
                        OnPropertyChanged("Occupation");
                    }
                }
            }

            public string Class
            {
                get { return _class; }
                set
                {
                    if (_class != value)
                    {
                        _class = value;
                        OnPropertyChanged("Class");
                    }
                }
            }

            string _occupation = string.Empty;
            string _class = string.Empty;
        }

        public class WorkingPeople : ObservableCollection<WorkingPerson>
        {
            public WorkingPeople(int count)
            {
                if (count < 0)
                {
                    throw new ArgumentException("Invalid count");
                }

                int classAMin = count / 5;
                int classAMax = (3 * count) / 5;
                for (int i = 0; i < count; i++)
                {
                    WorkingPerson person = new WorkingPerson(i.ToString(), (count - i).ToString());
                    if (i % 2 == 0)
                    {
                        person.Occupation = "Doctor";
                    }
                    else
                    {
                        person.Occupation = "Teacher";
                    }

                    if (i > classAMin && i < classAMax)
                    {
                        person.Class = "Class A";
                    }
                    else
                    {
                        person.Class = "Class B";
                    }

                    this.Add(person);
                }
            }
        }

        public class Folder : INotifyPropertyChanged
        {
            public Folder(string name, Folder parentFolder, int numSubLevels, int numSubFolders, bool isExpanded)
            {
                ParentFolder = parentFolder;
                Level = ParentFolder != null ? ParentFolder.Level + 1 : 1;
                Name = name;
                IsExpanded = isExpanded;

                if (numSubLevels > 0)
                {
                    for (int i = 0; i < numSubFolders; i++)
                    {
                        SubFolders.Add(new Folder(Name + i, this, numSubLevels-1, numSubFolders, i%2 == 0));
                    }
                }
            }

            public int Level
            {
                get;
                private set;
            }

            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        NotifyPropertyChanged("Name");
                    }
                }
            }
            private string _name;

            public Folder ParentFolder
            {
                get;
                set;
            }

            public bool IsExpanded
            {
                get { return _isExpanded; }
                set
                {
                    if (_isExpanded != value)
                    {
                        _isExpanded = value;
                        NotifyPropertyChanged("IsExpanded");
                    }
                }
            }
            private bool _isExpanded;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                  PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public ObservableCollection<Folder> SubFolders
            {
                get
                {
                    if (_subFolders == null)
                        _subFolders = new ObservableCollection<Folder>();

                    return _subFolders;

                }
            }
            private ObservableCollection<Folder> _subFolders;

            public override string ToString()
            {
               return Name;
            }
        }

        private enum TestIndex : int
        {
            InitialLoad,
            ScrollToEndAlongMajorAxis,
            ScrollToHomeAlongMajorAxis,
            PageDownAlongMajorAxis,
            PageUpAlongMajorAxis,
            LineDownAlongMajorAxis,
            LineUpAlongMajorAxis,
            ScrollToEndAlongMinorAxis,
            ScrollToHomeAlongMinorAxis,
            PageDownAlongMinorAxis,
            PageUpAlongMinorAxis,
            LineDownAlongMinorAxis,
            LineUpAlongMinorAxis,
            KeyEnd,
            KeyHome,
            KeyPageDown,
            KeyPageUp,
            KeyLineDown,
            KeyLineUp,
            ScrollWheelDown,
            ScrollWheelUp,
            LastValue
        }

        private static class DoubleUtil
        {
            const double epsilon = 0.0000001;

            public static bool AreClose(double n1, double n2)
            {
                return (Math.Abs(n1 - n2) < epsilon);
            }

            public static bool GreaterThan(double n1, double n2)
            {
                return ((n1 - n2) > epsilon);
            }

            public static bool GreaterThanOrClose(double n1, double n2)
            {
                return ((n1 - n2) > epsilon) || AreClose(n1, n2);
            }

            public static bool LessThan(double n1, double n2)
            {
                return ((n2 - n1) > epsilon);
            }

            public static bool LessThanOrClose(double n1, double n2)
            {
                return ((n2 - n1) > epsilon) || AreClose(n1, n2);
            }
        }

        private class MasterArrangeData
        {
            public Size MasterViewportSize
            {
                get;
                set;
            }

            public Size MasterExtentSize
            {
                get;
                set;
            }

            public IList<MasterContainerArrangeData> MasterContainerArrangeDataList
            {
                get
                {
                    if (_masterContainerArrangeDataList == null)
                    {
                        _masterContainerArrangeDataList = new Collection<MasterContainerArrangeData>();
                    }

                    return _masterContainerArrangeDataList;
                }
            }
            private IList<MasterContainerArrangeData> _masterContainerArrangeDataList;
        }

        private class MasterContainerArrangeData
        {
            public MasterContainerArrangeData(Rect arrangeRect, string str, bool isFocusable, bool isHeader)
            {
                ArrangeRect = arrangeRect;
                Str = str;
                IsFocusable = isFocusable;
                IsHeader = isHeader;
            }

            public Rect ArrangeRect { get; set; }
            public string Str { get; set; }
            public bool IsFocusable { get; set; }
            public bool IsHeader { get; set; }

            public bool AreEqual(Rect arrangeRect, String str)
            {
                return (
                    DoubleUtil.AreClose(arrangeRect.X, ArrangeRect.X) &&
                    DoubleUtil.AreClose(arrangeRect.Y, ArrangeRect.Y) &&
                    DoubleUtil.AreClose(arrangeRect.Width, ArrangeRect.Width) &&
                    DoubleUtil.AreClose(arrangeRect.Height, ArrangeRect.Height) &&
                    str.Equals(Str));
            }

            public override string ToString()
            {
               return ArrangeRect.ToString() + " " + Str;
            }
        }

        private class ValidateArrangeResult
        {
            public FrameworkElement FirstContainerInViewport
            {
                get;
                set;
            }

            public int FirstContainerIndexInMaster
            {
                get;
                set;
            }

            public FrameworkElement LastContainerInViewport
            {
                get;
                set;
            }

            public int LastContainerIndexInMaster
            {
                get;
                set;
            }

            public FrameworkElement FocusedContainer
            {
                get;
                set;
            }

            public int FocusedContainerIndexInMaster
            {
                get;
                set;
            }
        }

        #endregion
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            bool x = (bool)o;
            return Enum.ToObject(type, x ? 1 : 0);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            int ret = (int)System.Convert.ChangeType(o, typeof(int));
            return ret == 0 ? false : true;
        }
    }

    public enum CollectionChangeOperation
    {
        None,
        Insert,
        Remove,
        Replace,
        Move,
        PropertyChange,
        Last
    }
}
