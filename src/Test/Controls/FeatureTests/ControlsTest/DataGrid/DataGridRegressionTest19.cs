using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test : Keyboard navigation in DataGrid moves focus to the wrong element
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest19", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5+,4.5Client+")]
    public class DataGridRegressionTest19 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest19()
            : base(@"DataGridRegressionTest19.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(KeyboardNavigationToDataGridHeader);
        }

        #endregion

        #region Test Steps

        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest19");
            this.SetupDataSource();

            LogComment("Setup for DataGridRegressionTest19 was successful");
            return TestResult.Pass;
        }

        protected override void SetupDataSource()
        {
            MyList myModel = new MyList();
            myModel.List.Add(new MyItem { MyURL = new Uri("http://microsoft.com") });

            this.Window.DataContext = myModel;
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            this.MyDataGrid.Focus();
        }

        private TestResult KeyboardNavigationToDataGridHeader()
        {
            Status("KeyboardNavigationToDataGridHeader");

            Assert.AssertTrue("The datagrid should be focused.", this.MyDataGrid.IsFocused);

            MyList model = this.Window.DataContext as MyList;
            Button header = this.RootElement.FindName("btn") as Button;
            Assert.AssertTrue("Unable to find header button from the resources", header != null);
            Assert.AssertTrue("The header button should not be focused before test start.", !header.IsFocused);

            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            ContentPresenter cp = cell.Content as ContentPresenter;
            TextBox textbox = DataGridHelper.FindVisualChild<TextBox>(cp);
            Assert.AssertTrue("Unable to find textbox from the resources", textbox != null);

            LogComment("focus starts on last DataGrid row.");
            for (int i = 0; i < MyDataGrid.Items.Count + 1; i++)
            {
                LogComment("press key up...");
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Up);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }
            Assert.AssertTrue("Keyboard navigation in DataGrid moves focus to the wrong element , the header button should get focused .", header.IsFocused);

            LogComment("press key down...");
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Down);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("press key Left...");
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Assert.AssertTrue("Keyboard navigation in DataGrid moves focus to the wrong element , the first DataGridCell should get focused , Actual TextBox get focused .", (!textbox.IsFocused && cell.IsFocused));

            QueueHelper.WaitTillQueueItemsProcessed();
            return TestResult.Pass;
        }

        #endregion
    }

    #region Helper Classes

    public class MyItem
    {
        public Uri MyURL { get; set; }
    }

    public class MyList
    {
        ObservableCollection<MyItem> _list = new ObservableCollection<MyItem>();
        public ObservableCollection<MyItem> List
        {
            get { return _list; }
        }
    }

    #endregion
}
