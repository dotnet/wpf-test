using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test : after adding grouping, ScrollIntoView fails to scroll the selected item into view.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest45", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5.1+")]
    public class DataGridRegressionTest45 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest45()
            : base(@"DataGridRegressionTest45.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestScrollIntoViewAfterAddingGrouping);
        }

        #endregion

        #region Test Steps

        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest45");

            //Replace base.DataSource and use more data items to enable datagrid scrollbar.
            TypeFromDataSource = typeof(Person);
            DataSource = new People(3);
            MyDataGrid.ItemsSource = DataSource;
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            LogComment("Setup for DataGridRegressionTest45 was successful");
            return TestResult.Pass;
        }

        private TestResult TestScrollIntoViewAfterAddingGrouping()
        {
            Status("TestScrollIntoViewAfterAddingGrouping");

            this.Window.Focus();

            //Choose the item which will not show in view by default after grouping
            int selectedItemIndex = 90;
            bool isGridRowInView;

            LogComment(string.Format("Step1: Select and MoveCurrentTo datagrid item: {0}", selectedItemIndex.ToString()));
            MyDataGrid.SelectedIndex = selectedItemIndex;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Step2: Add PropertyGroupDescription and GroupStyle");
            ICollectionView view = CollectionViewSource.GetDefaultView(MyDataGrid.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("FirstName"));
            view.GroupDescriptions.Add(new PropertyGroupDescription("LastName"));
            QueueHelper.WaitTillQueueItemsProcessed();
            GroupStyle groupStyle = RootElement.FindResource("GroupHeaderStyle") as GroupStyle;
            MyDataGrid.GroupStyle.Add(groupStyle);
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Step3: Check if the SelectedItem is NOT in view before ScrollIntoView");
            DataGridRow gridRow = (DataGridRow)MyDataGrid.ItemContainerGenerator.ContainerFromItem(MyDataGrid.SelectedItem);
            isGridRowInView = Helpers.DataGridHelper.IsRowInViewVirtual(gridRow);
            LogComment(string.Format("Is the SelectedItem in view before ScrollIntoView: {0}", isGridRowInView));
            Assert.AssertTrue("The SelectedItem is in view before ScrollIntoView, can't be used for this testing.", !isGridRowInView);

            LogComment(string.Format("Step4: ScrollIntoView the selected item into view and verify the result"));
            MyDataGrid.ScrollIntoView(MyDataGrid.SelectedItem);
            QueueHelper.WaitTillQueueItemsProcessed();
            gridRow = (DataGridRow)MyDataGrid.ItemContainerGenerator.ContainerFromItem(MyDataGrid.SelectedItem);
            isGridRowInView = Helpers.DataGridHelper.IsRowInViewVirtual(gridRow);
            Assert.AssertTrue("ScrollIntoView failed to scroll the selected item into view.", isGridRowInView);

            LogComment("TestScrollIntoViewAfterAddingGrouping was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

    }

}
