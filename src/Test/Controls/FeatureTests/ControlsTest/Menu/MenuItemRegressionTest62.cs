using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Globalization;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Test.Controls.Helpers;

namespace Microsoft.Test.Controls
{
#if TESTBUILD_CLR40
    /// <summary>
    /// <description>
    /// Regression Test   Verify that MenuItem.IsChecked DP does not lose it's binding  
    /// </description>

    /// </summary>
    [Test(0, "MenuItem", "MenuItemRegressionTest62", SecurityLevel = TestCaseSecurityLevel.FullTrust, Disabled=true)]
    public class MenuItemRegressionTest62 : RegressionXamlTest
    {
        private TabControl tabControl;
        private Menu tabMenu;

        #region Constructor

        public MenuItemRegressionTest62()
            : base(@"MenuItemRegressionTest62.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestIsCheckedBinding);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for MenuItemRegressionTest62");

            tabControl = (TabControl)RootElement.FindName("testTabControl");
            Assert.AssertTrue("Unable to find testTabControl from the resources", tabControl != null);

            tabMenu = (Menu)RootElement.FindName("tabsMenu");
            Assert.AssertTrue("Unable to find tabsMenu from the resources", tabMenu != null);

            RootElement.DataContext = new TabItemsVM();

            QueueHelper.WaitTillQueueItemsProcessed();

            // Uncomment for ad-hoc debugging
            //base.Setup();

            LogComment("Setup for MenuItemRegressionTest62 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            tabControl = null;
            tabMenu = null;
            return base.CleanUp();
        }
        /// <summary>
        /// 1. Close tab0 => verify MenuItem0 IsChecked reflects changes (unchecked)
        /// 2. Click MenuItem0 => verify tab0 is visible again
        /// 3. Close tab0 again
        /// 
        /// Verify MenuItem0 IsChecked is unchecked again.
        /// </summary>
        private TestResult TestIsCheckedBinding()
        {
            Status("TestIsCheckedBinding");

            TabItem tabItem0 = (TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("tabItem at index 0 must exist for test to continue", tabItem0 != null);

            Button closeButton = DataGridHelper.FindVisualChild<Button>(tabItem0);
            Assert.AssertTrue("closeButton must exist for test to continue", closeButton != null);

            Assert.AssertTrue(
             string.Format("tabItem0.Visibility should be visible at this time.  Actual: {0}", tabItem0.Visibility),
             tabItem0.Visibility == Visibility.Visible);

            LogComment("1. close tab0");
            UserInput.MouseLeftClickCenter(closeButton);
            QueueHelper.WaitTillQueueItemsProcessed();

            Assert.AssertTrue(
                string.Format("tabItem0.Visibility should be collapsed at this time.  Actual: {0}", tabItem0.Visibility), 
                tabItem0.Visibility == Visibility.Collapsed);

            LogComment("1a. verify MenuItem0 IsChecked property is unchecked");
            MenuItem menuItem0 = OpenTabMenuAndGetSubMenuItem(0);
            Assert.AssertTrue("menuItem0.IsChecked should be false after closing tab0", !menuItem0.IsChecked);

            LogComment("2. click menuItem0");
            UserInput.MouseLeftClickCenter(menuItem0);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(0, DispatcherPriority.SystemIdle);
            QueueHelper.WaitTillQueueItemsProcessed();

            Assert.AssertTrue("menuItem0.IsChecked should be true at this time", menuItem0.IsChecked);
            
            LogComment("2a. verify tab0 is visible again");
            Assert.AssertTrue(
               string.Format("tabItem0.Visibility should be visible at this time.  Actual: {0}", tabItem0.Visibility),
               tabItem0.Visibility == Visibility.Visible);

            LogComment("3. close tabItem0 again");
            UserInput.MouseLeftClickCenter(closeButton);
            QueueHelper.WaitTillQueueItemsProcessed();

            Assert.AssertTrue(
                string.Format("tabItem0.Visibility should be collapsed at this time.  Actual: {0}", tabItem0.Visibility),
                tabItem0.Visibility == Visibility.Collapsed);

            LogComment("3a. verify menuItem0 IsChecked property is unchecked");
            menuItem0 = OpenTabMenuAndGetSubMenuItem(0);
            Assert.AssertTrue("menuItem0.IsChecked should be false after closing tab0", !menuItem0.IsChecked);

            LogComment("TestIsCheckedBinding was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

    #region Helpers        

        private MenuItem OpenTabMenuAndGetSubMenuItem(int index)
        {
            LogComment("open the tabMenu");
            UserInput.MouseLeftDown(tabMenu, 3, 3);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUp(tabMenu, 3, 3);
            QueueHelper.WaitTillQueueItemsProcessed();

            MenuItem menuItem = (MenuItem)tabMenu.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("menuItem must exist for test to continue", menuItem != null);

            MenuItem subMenuItem = (MenuItem)menuItem.ItemContainerGenerator.ContainerFromIndex(index);
            Assert.AssertTrue("subMenuItem must exist for test to continue", subMenuItem != null);

            MenuItem innerMenuItem = DataGridHelper.FindVisualChild<MenuItem>(subMenuItem);
            Assert.AssertTrue("innerMenuItem must exist for test to continue", innerMenuItem != null);

            BindingExpression be = innerMenuItem.GetBindingExpression(MenuItem.IsCheckedProperty);

            Assert.AssertTrue("be should not be null", be != null);
            LogComment(string.Format(
                "menuItem0 binding: path: {0}, converter: {1}, mode: {2} ",
                be.ParentBinding.Path.Path,
                be.ParentBinding.Converter.GetType().Name,
                be.ParentBinding.Mode.ToString()));

            return innerMenuItem;
        }

        #endregion Helpers
    }

    public class TabItemsVM : INotifyPropertyChanged
    {
        private ObservableCollection<TabItemVM> _tabList;
        private ICommand _closeTab;
        private ICommand _toggleTab;

        public TabItemsVM()
        {
            TabList = new ObservableCollection<TabItemVM>
            {
                new TabItemVM { Visibility = Visibility.Visible, Content = "one", IsSelected = true },
                new TabItemVM { Visibility = Visibility.Visible, Content = "two", IsSelected = false },
                new TabItemVM { Visibility = Visibility.Visible, Content = "three", IsSelected = false },
                new TabItemVM { Visibility = Visibility.Visible, Content = "four", IsSelected = false },
            };
        }

        public ObservableCollection<TabItemVM> TabList
        {
            get { return _tabList; }
            set
            {
                _tabList = value;
                OnPropertyChanged("TabList");
            }
        }

    #region Commands

        public ICommand CloseTab
        {
            get
            {
                if (_closeTab == null)
                    _closeTab = new RelayCommand<object>(
                        (param) =>
                        {
                            TabList[0].Visibility = Visibility.Collapsed;
                        });

                return _closeTab;
            }
        }

        public ICommand ToggleTab
        {
            get
            {
                if (_toggleTab == null)
                    _toggleTab = new RelayCommand<object>(
                        (param) =>
                        {

                            if ((bool)param)
                            {
                                TabList[0].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                TabList[0].Visibility = Visibility.Collapsed;
                            }
                        });

                return _toggleTab;
            }
        }

        #endregion Commands

    #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged

    #region TabItemVM

        public class TabItemVM : INotifyPropertyChanged
        {
            private Visibility _visible;
            public Visibility Visibility
            {
                get { return _visible; }
                set
                {
                    _visible = value;
                    OnPropertyChanged("Visibility");
                }
            }

            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }

            private object _content;
            public object Content
            {
                get { return _content; }
                set
                {
                    _content = value;
                    OnPropertyChanged("Content");
                }
            }

    #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion INotifyPropertyChanged
        }

        #endregion TabItemVM
    }
#endif
}
