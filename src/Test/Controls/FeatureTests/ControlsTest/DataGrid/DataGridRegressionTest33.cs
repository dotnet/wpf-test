using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Linq;

namespace Microsoft.Test.Controls
{
#if TESTBUILD_CLR40
    /// <summary>
    /// <description>
    /// Regression Test   Verify that a FormatException is not thrown when
    /// binding to the NewItemPlaceholder.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest33", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest33 : DataGridTest
    {
        private Button debugButton;

        #region Constructor

        public DataGridRegressionTest33()
            : base(@"DataGridRegressionTest33.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestNewItemPlaceholderSelection);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest33");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            this.SetupDataSource();

            //Debug();

            LogComment("Setup for DataGridRegressionTest33 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;
            return base.CleanUp();
        }
        /// <summary>
        /// 1. Set selection to the NewItemPlaceholder
        /// 
        /// Verify no exception is thrown even if there is a binding set on that row.
        /// </summary>
        private TestResult TestNewItemPlaceholderSelection()
        {
            Status("TestNewItemPlaceholderSelection");

            LogComment("Set selection to the NewItemPlaceholder by clicking");
            int lastRow = MyDataGrid.Items.Count - 1;
            DataGridActionHelper.ClickOnCell(MyDataGrid, lastRow, 0);

            Assert.AssertTrue(string.Format("DataGrid.SelectedItems count is not greater than zero. Actual count: {0}", MyDataGrid.SelectedItems.Count), MyDataGrid.SelectedItems.Count > 0);

            LogComment("If got this far without exception, test was successful");            

            LogComment("TestNewItemPlaceholderSelection was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps    

        #region Helpers

        protected override void SetupDataSource()
        {
            this.Window.DataContext = new DataGridViewModel();

            this.WaitForPriority(DispatcherPriority.SystemIdle);
            this.Window.Focus();
        }

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }        


        #endregion Helpers

        #region Helper Classes

        public class DataGridViewModel : INotifyPropertyChanged
        {
            #region Fields

            private ObservableCollection<ItemViewModel> _items;
            private ItemViewModel _currentItem;

            #endregion Fields

            public DataGridViewModel()
            {
                Items = new ObservableCollection<ItemViewModel>((from person in new EditablePeople()
                                                                 select new ItemViewModel
                                                                 {
                                                                     Item = person
                                                                 }).ToList());
            }

            #region Properties

            public ObservableCollection<ItemViewModel> Items
            {
                get { return _items; }
                set
                {
                    _items = value;
                    OnPropertyChanged("Items");
                }
            }

            public ItemViewModel CurrentItem
            {
                get { return _currentItem; }
                set
                {
                    _currentItem = value;
                    OnPropertyChanged("CurrentItem");
                }
            }

            #endregion Properties

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Raises this object's PropertyChanged event.
            /// </summary>
            /// <param name="propertyName">The property that has a new value.</param>
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    handler(this, e);
                }
            }

            #endregion INotifyPropertyChanged
        }

        /// <summary>
        /// ViewModel for a DataGridRow
        /// </summary>
        public class ItemViewModel : INotifyPropertyChanged
        {
            #region Fields

            private EditablePerson _item;
            private bool _isSelected;

            #endregion Fields

            /// <summary>
            /// Item Data
            /// </summary>
            public EditablePerson Item
            {
                get { return _item; }
                set
                {
                    _item = value;
                    OnPropertyChanged("Item");
                }
            }

            /// <summary>
            /// Models selection of the item
            /// </summary>
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Raises this object's PropertyChanged event.
            /// </summary>
            /// <param name="propertyName">The property that has a new value.</param>
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    handler(this, e);
                }
            }

            #endregion INotifyPropertyChanged
        }

        #endregion Helper Classes
    }
#endif
}
