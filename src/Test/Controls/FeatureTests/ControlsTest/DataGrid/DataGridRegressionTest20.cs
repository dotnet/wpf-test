using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel;

using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;
using System.Windows.Threading;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test : ScrollIntoView fails, when called from SelectionChanged handler 
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest20", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest20 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest20()
            : base(@"DataGridRegressionTest20.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ChangeSelectedItem);
        }

        #endregion
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest20");

            this.SetupDataSource();
            this.MyDataGrid.SelectionChanged += DataGrid_SelectionChanged;

            LogComment("Setup for DataGridRegressionTest20 was successful");

            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            return base.CleanUp();
        }

        protected override void SetupDataSource()
        {
            this.Window.DataContext = new DataGridViewModel();
            this.WaitForPriority(DispatcherPriority.SystemIdle);
            this.Window.Focus();
        }

        private TestResult ChangeSelectedItem()
        {
            Status("ChangeSelectedItem");
            MyItems item = new MyItems();
            DataGridViewModel dataGridViewModel = this.Window.DataContext as DataGridViewModel;
            dataGridViewModel.Items.Add(item);
            dataGridViewModel.SelectedItem = item;
            try
            {
                QueueHelper.WaitTillQueueItemsProcessed();
                return TestResult.Pass;
            }
            catch (InvalidCastException e)
            {
                LogComment(string.Format("InvalidCastException occurs when ScrollIntoView fails: {0}", e.ToString()));
                return TestResult.Fail;
            }
        }

        /// <summary>
        /// When ScrollIntoView is called before layout has run, it calls BeginInvoke to do the work asynchronously.  
        /// This was double-wrapping the item in an ItemInfo - once on the calling end of BeginInvoke (e.g. DataGrid.ScrollIntoView) 
        /// and once on the receiving end (ItemsControl.OnBringItemIntoView).   
        /// Fixed at the receiving end by checking whether the arg is already an ItemInfo.
        /// This is a regression from 4.0.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = this.MyDataGrid.SelectedItem;
            this.MyDataGrid.ScrollIntoView(item);
        }
    }

    #region Helper Classes

    public class DataGridViewModel : INotifyPropertyChanged
    {
        public DataGridViewModel()
        {
            this.Items = new NotifyList<MyItems>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private NotifyList<MyItems> _items;
        public NotifyList<MyItems> Items
        {
            get { return this._items; }
            set
            {
                this._items = value;
                OnPropertyChanged("Items");
            }
        }

        private MyItems _item;
        public MyItems SelectedItem
        {
            get { return _item; }
            set
            {
                _item = value;
                OnPropertyChanged("SelectedItem");
            }
        }
    }

    public class NotifyList<T> : IList, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private ObservableCollection<T> items = new ObservableCollection<T>();

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public object SyncRoot
        {
            get
            {
                return (items as IList).SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Add(object value)
        {
            items.Add((T)value);
            return 0;
        }

        public bool Contains(object value)
        {
            return items.Contains((T)value);
        }

        public void Clear()
        {
            items.Clear();
        }

        public int IndexOf(object value)
        {
            return items.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            items.Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            items.Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = (T)value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { this.items.CollectionChanged += value; }
            remove { this.items.CollectionChanged -= value; }
        }
    }

    public class MyItems
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
    #endregion

}
