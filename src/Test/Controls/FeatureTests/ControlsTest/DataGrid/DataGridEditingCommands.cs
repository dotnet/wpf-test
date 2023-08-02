using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Controls.DataSources;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for the possible commands that can be done for editing.  This 
    /// currently tests cell editing.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingCommands", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridEditingCommands : DataGridEditing
    {
        #region Constructor

        public DataGridEditingCommands()
            //: this("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")]
        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridEditingCommands(string dataSourceName, string dataTypeName)
            : base(@"DataGridEditing.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestBeginEditingCommands);
            RunSteps += new TestStep(TestCancelEditingCommands);
            RunSteps += new TestStep(TestCommitEditingCommands);
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

            Status("Setup specific for DataGridEditingCommands");            

            // set the current row
            CurRow = 0;

            LogComment("Setup for DataGridEditingCommands was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify each way cell can be put into editing mode
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestBeginEditingCommands()
        {
            Status("TestBeginEditingCommands");

            foreach (DataGridHelper.ColumnTypes columnType in columnTypes)
            {
                LogComment(string.Format("Begin testing with ColumnType: {0}", columnType.ToString()));

                //
                if (columnType == DataGridHelper.ColumnTypes.DataGridTemplateColumn)
                {
                    continue;
                }

                CurCol = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, columnType);

                this.TestCommandMatrix(
                    CurRow                 /* cur row to be edited */,
                    CurCol                 /* cur col to be edited */,
                    false                   /* do not edit any data */,
                    beginActions           /* all the begin actions to execute */,
                    null                    /* all the commit actions to execute */,
                    null                    /* all the cancel actions to execute */);

                LogComment(string.Format("End testing with ColumnType: {0}\n\n", columnType.ToString()));
            }

            LogComment("TestBeginEditingCommands was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify each way a cell can be cancelled from editing mode.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancelEditingCommands()
        {
            Status("TestCancelEditingCommands");

            foreach (DataGridHelper.ColumnTypes columnType in columnTypes)
            {
                LogComment(string.Format("Begin testing with ColumnType: {0}", columnType.ToString()));

                //
                if (columnType == DataGridHelper.ColumnTypes.DataGridTemplateColumn)
                {
                    continue;
                }

                CurCol = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, columnType);

                this.TestCommandMatrix(
                   CurRow                 /* cur row to be edited */,
                   CurCol                 /* cur col to be edited */,
                   true                    /* do not edit any data */,
                   defaultBeginAction     /* all the begin actions to execute */, 
                   null                    /* all the commit actions to execute */,
                   cancelActions          /* all the cancel actions to execute */);

                LogComment(string.Format("End testing with ColumnType: {0}\n\n", columnType.ToString()));
            }

            LogComment("TestCancelEditingCommands was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify each way a cell can commit data from editing mode.  Note: this only tests for single cell editing scenarios.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCommitEditingCommands()
        {
            Status("TestCommitEditingCommands");

            foreach (DataGridHelper.ColumnTypes columnType in columnTypes)
            {
                LogComment(string.Format("Begin testing with ColumnType: {0}", columnType.ToString()));

                //
                if (columnType == DataGridHelper.ColumnTypes.DataGridTemplateColumn)
                {
                    continue;
                }

                CurCol = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, columnType);

                this.TestCommandMatrix(
                   CurRow                 /* cur row to be edited */,
                   CurCol                 /* cur col to be edited */,
                   true                    /* do not edit any data */,
                   defaultBeginAction     /* all the begin actions to execute */,
                   commitActions          /* all the commit actions to execute */,
                   null                    /* all the cancel actions to execute */);

                LogComment(string.Format("End testing with ColumnType: {0}\n\n", columnType.ToString()));
            }

            LogComment("TestCommitEditingCommands was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
