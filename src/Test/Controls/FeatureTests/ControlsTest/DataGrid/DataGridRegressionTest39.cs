using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify Exception is not thrown when resetting the ItemsSource of
    /// a DataGrid with Grouping turned on.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest39", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest39 : DataGridTest
    {
        private Button debugButton;
        private Button switchButton;

        #region Constructor

        public DataGridRegressionTest39()
            : base(@"DataGridRegressionTest39.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDataGrid);
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

            Status("Setup specific for DataGridRegressionTest39");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            switchButton = (Button)RootElement.FindName("btn_Switch");
            Assert.AssertTrue("Unable to find switchButton from the resources", switchButton != null);

            switchButton.Click += SwitchCollection;
            RootElement.DataContext = new ObjectWithCollection(new CollectionItem[] { new CollectionItem(1, "Text") });

            //


            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for DataGridRegressionTest39 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;
            switchButton.Click -= SwitchCollection;
            switchButton = null;

            return base.CleanUp();
        }
        /// <summary>
        /// 1. Switch the collection of the DataGrid
        /// 
        /// Verify: no exception is throw
        /// </summary>
        private TestResult TestDataGrid()
        {
            Status("TestDataGrid");

            LogComment("Swith the collection of the DataGrid");
            SwitchCollection(null, null);

            //
            // if got to this point without any exception thrown, test passes
            //                        

            LogComment("TestDataGrid was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        private void SwitchCollection(object sender, EventArgs e)
        {
            RootElement.DataContext = new ObjectWithCollection(new CollectionItem[] { });
            QueueHelper.WaitTillQueueItemsProcessed();
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
    }

    public class ObjectWithCollection
    {
        private ObservableCollection<CollectionItem> _items;

        public ObjectWithCollection(IEnumerable<CollectionItem> items)
        {
            _items = new ObservableCollection<CollectionItem>(items);
        }

        public ObservableCollection<CollectionItem> Items
        {
            get
            {
                return _items;
            }
        }
    }

    public class CollectionItem
    {
        private int _groupID;
        private string _sortData;

        public CollectionItem(int groupID, string sortData)
        {
            _groupID = groupID;
            _sortData = sortData;
        }

        public int GroupID
        {
            get
            {
                return _groupID;
            }
        }

        public string SortData
        {
            get
            {
                return _sortData;
            }
            set
            {
                _sortData = value;
            }
        }
    }
}
