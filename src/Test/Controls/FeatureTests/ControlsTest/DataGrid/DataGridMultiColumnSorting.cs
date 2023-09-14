using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Multi-column sorting Behavioral tests for DataGrid.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridMultiColumnSorting", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridMultiColumnSorting : DataGridTest
    {
        #region Private Fields

        private List<SortDescription> expectedSortDescriptions = new List<SortDescription>();

        #endregion Private Fields

        #region Constructor

        public DataGridMultiColumnSorting()
            : base(@"DataGridMultiColumnSorting.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestBasicMultiColumnSorting);
            RunSteps += new TestStep(TestSortingAllColumns);
            RunSteps += new TestStep(TestSortingWithRepeatedColumns);
            RunSteps += new TestStep(TestMultiSortingThenSingleSorting);
            RunSteps += new TestStep(TestClearingSortDescription);
            RunSteps += new TestStep(TestOrderOfSelectedColumnsToSort);
            RunSteps += new TestStep(TestSortingDescriptionsAfterRemovingItemsSource);
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

            Status("Setup specific for DataGridAutoSorting");

            this.SetupDataSource();

            LogComment("Setup for DataGridAutoSorting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify sorting three valid columns together
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestBasicMultiColumnSorting()
        {
            Status("TestBasicMultiColumnSorting");

            InitStep();            

            // Click on column 0
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, false);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(0), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            LogComment("TestBasicMultiColumnSorting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify sorting all columns together
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingAllColumns()
        {
            Status("TestSortingAllColumns");

            InitStep();

            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, i, true);
                expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(i), ListSortDirection.Ascending));
                i++;
            }

            this.VerifySortDescriptions();

            LogComment("TestSortingAllColumns was successful");
            return TestResult.Pass;
        }        

        /// <summary>
        /// Verify multi-sorting with several repeated sorted columns
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithRepeatedColumns()
        {
            Status("TestSortingWithRepeatedColumns");

            InitStep();            

            // Click on column 0
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, false);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(0), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 5
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 5, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(5), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.RemoveAt(1);
            expectedSortDescriptions.Insert(1, new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Descending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.RemoveAt(2);
            expectedSortDescriptions.Insert(2, new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Descending));

            this.VerifySortDescriptions();

            LogComment("TestSortingWithRepeatedColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Do multi-column sorting then select a single column to be sorted
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestMultiSortingThenSingleSorting()
        {
            Status("TestMultiSortingThenSingleSorting");

            InitStep();            

            // Click on column 0
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, false);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(0), ListSortDirection.Ascending));

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Ascending));

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // Click on column 2
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 2, false);
            expectedSortDescriptions.Clear();
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(2), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // repeat and click on an existing sorted column========
            InitStep();                        

            // Click on column 0
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, false);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(0), ListSortDirection.Ascending));

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Ascending));

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, false);
            expectedSortDescriptions.Clear();
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Descending));

            this.VerifySortDescriptions();

            LogComment("TestMultiSortingThenSingleSorting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Do multi-column sorting then clearing the SortDescriptions
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestClearingSortDescription()
        {
            Status("TestClearingSortDescription");

            InitStep();            

            // Click on column 0
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, false);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(0), ListSortDirection.Ascending));

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Ascending));

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            MyDataGrid.Items.SortDescriptions.Clear();
            expectedSortDescriptions.Clear();

            this.VerifySortDescriptions();

            LogComment("TestClearingSortDescription was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify multi-column sorting with different order of selected columns to sort
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestOrderOfSelectedColumnsToSort()
        {
            Status("TestOrderOfSelectedColumnsToSort");

            InitStep();

            // Click on column 4
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 4, false);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(4), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 1
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(1), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 3
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 3, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(3), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 0
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(0), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 5
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 5, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(5), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            // SHIFT + Click on column 2
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 2, true);
            expectedSortDescriptions.Add(new SortDescription(this.GetSortPropertyName(2), ListSortDirection.Ascending));

            this.VerifySortDescriptions();

            LogComment("TestOrderOfSelectedColumnsToSort was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingDescriptionsAfterRemovingItemsSource()
        {
            Status("TestSortingDescriptionsAfterRemovingItemsSource");

            LogComment("initially sort a few columns");
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0, false);
            QueueHelper.WaitTillQueueItemsProcessed();
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 1, true);
            QueueHelper.WaitTillQueueItemsProcessed();
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 2, true);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (MyDataGrid.Items.SortDescriptions.Count <= 0)
            {
                throw new TestValidationException("SortDescription count should be greater than zero.");
            }

            LogComment("now change the ItemsSource");
            MyDataGrid.ItemsSource = new People();
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify the SortDescriptions are cleared.");
            if (MyDataGrid.Items.SortDescriptions.Count != 0)
            {
                throw new TestValidationException("SortDescription count should be zero after ItemsSource change.");
            }

            LogComment("TestSortingDescriptionsAfterRemovingItemsSource was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void InitStep()
        {
            expectedSortDescriptions.Clear();

            MyDataGrid.Items.SortDescriptions.Clear();
            MyDataGrid.Items.Refresh();
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                column.SortDirection = null;
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private string GetSortPropertyName(int colIndex)
        {
            string sortPropertyName = null;

            DataGridColumn column = MyDataGrid.Columns[colIndex];

            if (column != null)
            {
                sortPropertyName = column.SortMemberPath;
                if (string.IsNullOrEmpty(sortPropertyName))
                {
                    DataGridBoundColumn boundColumn = column as DataGridBoundColumn;
                    if (boundColumn != null)
                    {
                        Binding binding = boundColumn.Binding as Binding;
                        if (binding != null)
                        {
                            if (!string.IsNullOrEmpty(binding.XPath))
                            {
                                sortPropertyName = binding.XPath;
                            }
                            else if (binding.Path != null)
                            {
                                sortPropertyName = binding.Path.Path;
                            }
                        }
                    }
                }
            }

            return sortPropertyName;
        }

        public void VerifySortDescriptions()
        {
            if (expectedSortDescriptions.Count != MyDataGrid.Items.SortDescriptions.Count)
            {
                throw new TestValidationException(string.Format(
                    "Mismatch in SortDescpription count. Expected: {0}, Actual: {1}",
                    expectedSortDescriptions.Count,
                    MyDataGrid.Items.SortDescriptions.Count));
            }

            int i = 0;
            foreach (SortDescription sortDescription in expectedSortDescriptions)
            {
                if (sortDescription.Direction != MyDataGrid.Items.SortDescriptions[i].Direction)
                {
                    throw new TestValidationException(string.Format(
                        "Mismatch in SortDescpription's Direction. Expected: {0}, Actual: {1}",
                        sortDescription.Direction,
                        MyDataGrid.Items.SortDescriptions[i].Direction));
                }
                if (sortDescription.PropertyName != MyDataGrid.Items.SortDescriptions[i].PropertyName)
                {
                    throw new TestValidationException(string.Format(
                        "Mismatch in SortDescpription's PropertyName. Expected: {0}, Actual: {1}",
                        sortDescription.PropertyName,
                        MyDataGrid.Items.SortDescriptions[i].PropertyName));
                }
                i++;
            }
        }

        #endregion Helpers
    }
}
