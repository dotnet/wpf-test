using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Testing around editing and tab navigation
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingStateAndNavigation", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridEditingStateAndNavigation : DataGridEditing
    {
        #region Constructor

        public DataGridEditingStateAndNavigation()
            : this("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")]
        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridEditingStateAndNavigation(string dataSourceName, string dataTypeName)
            : base(@"DataGridEditing.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingStateAndNavigation), "TestEditingAndTabNavigation");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingStateAndNavigation), "TestEditingAndTabNavigationAndReadOnly");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingStateAndNavigation), "TestEditingAndTabNavigationAndReorderingColumns");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingStateAndNavigation), "TestEditingAndScrollingOffScreen");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingStateAndNavigation), "TestEditingAndNoEditingElement");
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

            Status("Setup specific for DataGridEditingStateAndNavigation");



            LogComment("Setup for DataGridEditingStateAndNavigation was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1.  Open {cell(0,0), cell(last,0)} for edit and begin edit.
        /// 2.  Tab to each cell and verify still in edit mode.
        ///         A. Tab from the last cell and verify not in edit mode anymore.           
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingAndTabNavigation<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingAndTabNavigation");

            foreach (KeyValuePair<int, int> kvp in new Dictionary<int, int> { { 0, 0 }, { MyDataGrid.Items.Count - 2, 0 } })
            {
                // set a row to begin with
                int testRow = kvp.Key;
                int testCol = ColumnEquivalent(kvp.Value);
                
                List<EditingStepInfo<DT>> steps = new List<EditingStepInfo<DT>>();

                // begin here
                steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "open first cell for edit",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoBeginEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterBegin<DT>,
                        beginAction = DataGridCommandHelper.BeginEditAction.F2
                    });
                steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "begin editing",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoEditCell<DT>
                    });

                // tab each cell in the row
                for (int i = 0; i < MyDataGrid.Columns.Count - 1; i++)
                {
                    steps.Add(new EditingStepInfo<DT>
                        {
                            debugComments = "tab to the next cell",
                            row = testRow,
                            col = testCol,
                            DoAction = this.DoCommitEditing<DT>,
                            VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                            commitAction = DataGridCommandHelper.CommitEditAction.Tab
                        });
                    steps.Add(new EditingStepInfo<DT>
                        {
                            debugComments = "begin editing",
                            row = testRow,
                            col = (testCol = IncrementColumnBy(testCol, 1)),
                            DoAction = this.DoEditCell<DT>
                        });
                }

                // tab to the next row
                steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next row",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });

                this.DoEditingSteps<DT>(steps.ToArray());
            }

            LogComment("TestEditingAndTabNavigation was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1.  Open cell(mid,last) for edit and begin edit.
        /// 2.  SHIFT + Tab to each cell and verify still in edit mode.
        ///         A. SHIFT + Tab from the first cell and verify not in edit mode anymore.         
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingAndTabNavigationBackwards<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingAndTabNavigationBackwards");

            foreach (KeyValuePair<int, int> kvp in new Dictionary<int, int> { { 0, MyDataGrid.Columns.Count - 1 }, { MyDataGrid.Items.Count - 2, MyDataGrid.Columns.Count - 1 } })
            {
                // set a row to begin with
                int testRow = kvp.Key;
                int testCol = kvp.Value;

                List<EditingStepInfo<DT>> steps = new List<EditingStepInfo<DT>>();

                // begin here
                steps.Add(new EditingStepInfo<DT>
                {
                    debugComments = "open cell for edit",
                    row = testRow,
                    col = testCol,
                    DoAction = this.DoBeginEditing<DT>,
                    VerifyAfterAction = this.VerifyAfterBegin<DT>,
                    beginAction = DataGridCommandHelper.BeginEditAction.F2
                });
                steps.Add(new EditingStepInfo<DT>
                {
                    debugComments = "begin editing",
                    row = testRow,
                    col = testCol,
                    DoAction = this.DoEditCell<DT>
                });

                // tab each cell in the row
                for (int i = 0; i < MyDataGrid.Columns.Count - 1; i++)
                {
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.ShiftTab
                    });
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "begin editing",
                        row = testRow,
                        col = (testCol = DecrementColumnBy(testCol, 1)),
                        DoAction = this.DoEditCell<DT>
                    });
                }

                // tab to the previous row
                steps.Add(new EditingStepInfo<DT>
                {
                    debugComments = "tab to the previous row",
                    row = testRow,
                    col = testCol,
                    DoAction = this.DoCommitEditing<DT>,
                    VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                    commitAction = DataGridCommandHelper.CommitEditAction.ShiftTab
                });

                this.DoEditingSteps<DT>(steps.ToArray());
            }

            LogComment("TestEditingAndTabNavigationBackwards was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1.  Set cell(0,2) to read only
        /// 2.  Open cell(0,0) for edit and begin edit.
        /// 3.  Tab to each cell and verify still in edit mode. Verify cell(0,2) does not go to edit mode.         
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingAndTabNavigationAndReadOnly<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingAndTabNavigationAndReadOnly");            

            // set the column to readonly
            int indexToSet = 2;
            DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, indexToSet);

            column.IsReadOnly = true;
            QueueHelper.WaitTillQueueItemsProcessed();

            // set a row to begin with
            int testRow = 0;
            int testCol = 0;

            List<EditingStepInfo<DT>> steps = new List<EditingStepInfo<DT>>();

            // begin here
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "open first cell for edit",
                row = testRow,
                col = testCol,
                DoAction = this.DoBeginEditing<DT>,
                VerifyAfterAction = this.VerifyAfterBegin<DT>,
                beginAction = DataGridCommandHelper.BeginEditAction.F2
            });
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "begin editing",
                row = testRow,
                col = testCol,
                DoAction = this.DoEditCell<DT>
            });

            // tab each cell in the row
            for (int i = 0; i < MyDataGrid.Columns.Count - 1; i++)
            {
                // special case here
                if (i == indexToSet - 1)
                {
                    // verify new cell does not open for edit
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell which is readonly",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });

                    // tab again, new cell should still not be editable
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell after the readonly cell",
                        row = testRow,
                        col = (testCol = IncrementColumnBy(testCol, 1)),
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyCellStateAfterNormalTab<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });

                    // break out of the loop
                    break;
                }
                else
                {
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "begin editing",
                        row = testRow,
                        col = (testCol = IncrementColumnBy(testCol, 1)),
                        DoAction = this.DoEditCell<DT>
                    });
                }
            }

            // go to the next row, verify the previous cells are still committed
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "go to the next row",
                row = testRow,
                col = (testCol = IncrementColumnBy(testCol, 1)),
                DoAction = this.DoCommitEditing<DT>,
                VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                commitAction = DataGridCommandHelper.CommitEditAction.Enter
            });

            this.DoEditingSteps<DT>(steps.ToArray());

            // clean up ReadOnly column
            column.IsReadOnly = false;            

            LogComment("TestEditingAndTabNavigationAndReadOnly was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Reorder columns then do editing through the row with tab navigation           
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingAndTabNavigationAndReorderingColumns<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingAndTabNavigationAndReorderingColumns");

            try
            {
                // reorder the columns
                this.UseDisplayIndexOrder = true;
                MyDataGrid.Columns[0].DisplayIndex = MyDataGrid.Columns.Count / 2;
                MyDataGrid.Columns[1].DisplayIndex = MyDataGrid.Columns.Count - 1;
                MyDataGrid.Columns[MyDataGrid.Columns.Count / 2].DisplayIndex = 0;
                QueueHelper.WaitTillQueueItemsProcessed();

                // run the same test for editing and tabing
                if (TestResult.Pass != this.TestEditingAndTabNavigation<DT>())
                {
                    LogComment("TestEditingAndTabNavigation failed with column reordering");
                    return TestResult.Fail;
                }

            }
            finally
            {
                // set columns back to the same order
                for (int i = 0; i < MyDataGrid.Columns.Count; i++)
                {
                    MyDataGrid.Columns[i].DisplayIndex = i;
                }
                this.UseDisplayIndexOrder = false;
            }

            LogComment("TestEditingAndTabNavigationAndReorderingColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Edit a cell then scroll offscreen.  Verify when scrolling back, state is not modified.       
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingAndScrollingOffScreen<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingAndScrollingOffScreen");

            int testRow = 0;
            int testCol = 0;

            List<EditingStepInfo<DT>> steps = new List<EditingStepInfo<DT>>();

            // begin here
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "open cell for edit",
                row = testRow,
                col = testCol,
                DoAction = this.DoBeginEditing<DT>,
                VerifyAfterAction = this.VerifyAfterBegin<DT>,
                beginAction = DataGridCommandHelper.BeginEditAction.F2
            });
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "begin editing",
                row = testRow,
                col = testCol,
                DoAction = this.DoEditCell<DT>
            });            

            // scroll down to the bottom of the DataGrid
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "scroll down to the bottom of the DataGrid",
                row = MyDataGrid.Items.Count - 1,
                col = testCol,
                DoAction = this.DoScrollIntoView<DT>
            });

            // scroll back to the top of the DataGrid
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "scroll back to the top of the DataGrid",
                row = testRow,
                col = testCol,
                DoAction = this.DoScrollIntoView<DT>
            });

            // verify the data
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "verify data is in tact and still in edit mode",
                row = testRow,
                col = testCol,                
                VerifyAfterAction = this.VerifyAfterBegin<DT>,
                beginAction = DataGridCommandHelper.BeginEditAction.F2
            });

            // commit the row
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "go to the next row",
                row = testRow,
                col = testCol,
                DoAction = this.DoCommitEditing<DT>,
                VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                commitAction = DataGridCommandHelper.CommitEditAction.Enter
            });

            this.DoEditingSteps<DT>(steps.ToArray());

            LogComment("TestEditingAndScrollingOffScreen was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1.  Add a new template column with no CellEditingTemplate
        /// 2.  Open cell(0,0) for edit and begin edit.
        /// 3.  Tab to each cell and verify still in edit mode. Verify new template does not go to edit mode.         
        /// 4.  Commit the row, verify row is committed.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingAndNoEditingElement<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingAndNoEditingElement");

            // add new template column
            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();

            FrameworkElementFactory cellTemplateFactory = new FrameworkElementFactory(typeof(TextBlock));
            Binding binding = new Binding("FirstName");
            cellTemplateFactory.SetBinding(TextBlock.TextProperty, binding);
            DataTemplate defaultCellTemplate = new DataTemplate();
            defaultCellTemplate.VisualTree = cellTemplateFactory;
            defaultCellTemplate.Seal();

            templateColumn.CellTemplate = defaultCellTemplate;
            templateColumn.DisplayIndex = 2;
            MyDataGrid.Columns.Insert(2, templateColumn);
            QueueHelper.WaitTillQueueItemsProcessed();

            // set a row to begin with
            int testRow = 0;
            int testCol = 0;

            List<EditingStepInfo<DT>> steps = new List<EditingStepInfo<DT>>();

            // begin here
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "open first cell for edit",
                row = testRow,
                col = testCol,
                DoAction = this.DoBeginEditing<DT>,
                VerifyAfterAction = this.VerifyAfterBegin<DT>,
                beginAction = DataGridCommandHelper.BeginEditAction.F2
            });
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "begin editing",
                row = testRow,
                col = testCol,
                DoAction = this.DoEditCell<DT>
            });

            // tab each cell in the row
            for (int i = 0; i < MyDataGrid.Columns.Count - 1; i++)
            {
                // special case here
                if (i == templateColumn.DisplayIndex - 1)
                {
                    // verify new cell does not open for edit
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell which is has no EditingElement",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });

                    // tab again, new cell should still not be editable
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell after the no EditingElement cell",
                        row = testRow,
                        col = (testCol = IncrementColumnBy(testCol, 1)),
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = null,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });

                    // break out of the loop
                    break;
                }
                else
                {
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "tab to the next cell",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab
                    });
                    steps.Add(new EditingStepInfo<DT>
                    {
                        debugComments = "begin editing",
                        row = testRow,
                        col = (testCol = IncrementColumnBy(testCol, 1)),
                        DoAction = this.DoEditCell<DT>
                    });
                }
            }

            // go to the next row, verify the previous cells are still committed
            steps.Add(new EditingStepInfo<DT>
            {
                debugComments = "go to the next row",
                row = testRow,
                col = (testCol = IncrementColumnBy(testCol, 1)),
                DoAction = this.DoCommitEditing<DT>,
                VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                commitAction = DataGridCommandHelper.CommitEditAction.Enter
            });

            this.DoEditingSteps<DT>(steps.ToArray());

            // clean up
            MyDataGrid.Columns.Remove(templateColumn);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("TestEditingAndNoEditingElement was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
