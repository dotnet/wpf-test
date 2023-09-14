using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for displayIndex when displayIndex is changed programmatically.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnReorderingDisplayIndexReorder", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")] 
    public class DataGridColumnReorderingDisplayIndexReorder : DataGridTest
    {
        #region Private members

        private int[] columnLengths = { 1, 2, 5, 6, 15 };
        private List<StartEndTuple> startEndTable;

        public struct StartEndTuple
        {
            public int start;
            public int end;
        };

        #endregion Private members

        #region Constructor

        public DataGridColumnReorderingDisplayIndexReorder()
            : base(@"DataGridEditing.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(TestDisplayIndexReordering);             
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

            Status("Setup specific for DataGridColumnReorderingDisplayIndexReorder");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnReorderingDisplayIndexReorder was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDisplayIndexReordering()
        {
            Status("TestDisplayIndexReordering");

            foreach (int columnLength in columnLengths)
            {                
                startEndTable = new List<StartEndTuple>();
                startEndTable.Add(new StartEndTuple { start = 0, end = columnLength / 2 });
                startEndTable.Add(new StartEndTuple { start = 0, end = columnLength - 1 });
                startEndTable.Add(new StartEndTuple { start = columnLength / 2, end = 0 });
                startEndTable.Add(new StartEndTuple { start = columnLength / 2, end = columnLength - 1 });
                startEndTable.Add(new StartEndTuple { start = columnLength - 1, end = 0 });
                startEndTable.Add(new StartEndTuple { start = columnLength - 1, end = columnLength / 2 });
                
                this.SetupDataGridColumns(columnLength);

                foreach (StartEndTuple startEndTuple in startEndTable)
                {
                    ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
                    int startIndex = MyDataGrid.Columns[startEndTuple.start].DisplayIndex;
                    MyDataGrid.Columns[startEndTuple.start].DisplayIndex = startEndTuple.end;                    

                    this.VerifyReorder(prevColumns, startIndex, startEndTuple.end);

                    LogComment(string.Format("Reordering with {0} columns from {1} to {2} passed.", columnLength, startEndTuple.start, startEndTuple.end));
                }

                LogComment(string.Format("Reordering with {0} columns passed.", columnLength));
            }

            LogComment("TestDisplayIndexReordering was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void SetupDataGridColumns(int numColumns)
        {
            MyDataGrid.Columns.Clear();

            for (int i = 0; i < numColumns; i++)
            {
                DataGridColumn newColumn = new DataGridTextColumn();
                newColumn.Width = DataGridLength.Auto;
                newColumn.Header = "Column" + i;
                MyDataGrid.Columns.Add(newColumn);
            }
        }

        private void VerifyReorder(ObservableCollection<DataGridColumn> prevColumns, int startIndex, int endIndex)
        {
            prevColumns.Move(startIndex, endIndex);

            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);
        }

        #endregion Helpers
    }
}
