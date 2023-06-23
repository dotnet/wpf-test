using System;
using System.Collections;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Test the CanUserAddRows and CanUserDeleteRows properties
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridNewDeleteRowEnableDisable", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridNewDeleteRowEnableDisable : DataGridEditing
    {
        #region Constructor

        public DataGridNewDeleteRowEnableDisable()
            //: this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon", 1)
            //: this("System.Collections.ObjectModel.ObservableCollection`1", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon", -1)
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon", -1)
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon", 1)]
        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon", 1)]
        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon", -1)]
        [Variation("System.Collections.ObjectModel.ObservableCollection`1", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon", -1)]
        public DataGridNewDeleteRowEnableDisable(string dataSourceName, string dataTypeName, int dataCountMultiplier)
            : base(@"DataGridEditing.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource(dataCountMultiplier);

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestCanUserAddRows);
            RunSteps += new TestStep(TestCanUserDeleteRows);
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

            Status("Setup specific for DataGridNewDeleteRowEnableDisable");



            LogComment("Setup for DataGridNewDeleteRowEnableDisable was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify if able to add a new row under these factors:
        /// 
        /// CanUserAddRows: {true, false}
        /// IsReadOnly: {true, false} 
        /// IsEnabled: {true, false}
        /// DataSource: {of type ObservableCollection<T>, non-generic type, }
        /// Quantity items to begin with: {0, 1, x, n}                
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCanUserAddRows()
        {
            Status("TestCanUserAddRows");

            foreach (bool? canUserAddRows in new bool?[] { null, true, false })
            {
                foreach (bool? isReadOnly in new bool?[] { null, true, false })
                {
                    foreach (bool? isEnabled in new bool?[] { null, true, false })
                    {
                        DataGridPropertyInfo pi = new DataGridPropertyInfo
                            {
                                CanUserAddRows = canUserAddRows,
                                IsEnabled = isEnabled,
                                IsReadOnly = isReadOnly,
                                ItemsSource = DataSource
                            };

                        SetPropertiesOnDataGrid(pi);

                        // verify state
                        VerifyDataGridAddRowState(pi);
                    }
                }
            }

            LogComment("TestCanUserAddRows was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify if able to add a new row under these factors:
        /// 
        /// CanUserDeleteRows: {true, false}
        /// IsReadOnly: {true, false} 
        /// IsEnabled: {true, false}
        /// DataSource: {of type ObservableCollection<T>, non-generic type, }
        /// Quantity items to begin with: {0, 1, x, n}                
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCanUserDeleteRows()
        {
            Status("TestCanUserDeleteRows");

            foreach (bool? canUserDeleteRows in new bool?[] { null, true, false })
            {
                LogComment(string.Format("Begin testing with canUserDeleteRows: {0}", canUserDeleteRows));
                foreach (bool? isReadOnly in new bool?[] { null, true, false })
                {
                    LogComment(string.Format("Begin testing with isReadOnly: {0}", isReadOnly));                    

                    foreach (bool? isEnabled in new bool?[] { null, true, false })
                    {
                        LogComment(string.Format("Begin testing with isEnabled: {0}", isEnabled));

                        DataGridPropertyInfo pi = new DataGridPropertyInfo
                        {
                            CanUserDeleteRows = canUserDeleteRows,
                            IsEnabled = isEnabled,
                            IsReadOnly = isReadOnly,
                            ItemsSource = DataSource
                        };

                        SetPropertiesOnDataGrid(pi);

                        // verify state
                        VerifyDataGridDeleteRowState(pi);

                        LogComment(string.Format("End testing with isEnabled: {0}", isEnabled));
                    }

                    LogComment(string.Format("End testing with isReadOnly: {0}", isReadOnly));
                }

                LogComment(string.Format("End testing with canUserDeleteRows: {0}", canUserDeleteRows));
            }

            LogComment("TestCanUserDeleteRows was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Data

        public struct DataGridPropertyInfo
        {
            public bool? IsReadOnly;
            public bool? IsEnabled;
            public bool? CanUserAddRows;
            public bool? CanUserDeleteRows;
            public IEnumerable ItemsSource;
        }

        #endregion Data

        #region Helpers

        private void SetPropertiesOnDataGrid(DataGridPropertyInfo dataGridPropertyInfo)
        {
            if (dataGridPropertyInfo.IsEnabled.HasValue)
            {
                MyDataGrid.IsEnabled = dataGridPropertyInfo.IsEnabled.Value;
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            if (dataGridPropertyInfo.IsReadOnly.HasValue)
            {
            }
            if (dataGridPropertyInfo.CanUserAddRows.HasValue)
            {
                MyDataGrid.CanUserAddRows = dataGridPropertyInfo.CanUserAddRows.Value;
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            if (dataGridPropertyInfo.CanUserDeleteRows.HasValue)
            {
                MyDataGrid.CanUserDeleteRows = dataGridPropertyInfo.CanUserDeleteRows.Value;
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            MyDataGrid.ItemsSource = dataGridPropertyInfo.ItemsSource;
            this.WaitForPriority(DispatcherPriority.SystemIdle);
            this.Window.Focus();
        }

        private void VerifyDataGridAddRowState(DataGridPropertyInfo dataGridPropertyInfo)
        {            
            if (dataGridPropertyInfo.CanUserAddRows.HasValue && !dataGridPropertyInfo.CanUserAddRows.Value)
            {
                // no coercion should have took place since it was already set to false
                if (MyDataGrid.CanUserAddRows)
                {
                    throw new TestValidationException(string.Format("CanUserAddRows should be false"));
                }

                // verify NewItemPlaceholder is not present
                if (MyDataGrid.Items.Count > 0 && MyDataGrid.Items[MyDataGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder)
                {
                    throw new TestValidationException(string.Format(
                        "Expects the last item to not be the NewItemPlaceholder.  Actual: {0}",
                        MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
                }
            }
            else if (MyDataGrid.IsReadOnly || !MyDataGrid.IsEnabled)
            {
                // CanUserAddRows should be coerced to false
                if (MyDataGrid.CanUserAddRows)
                {
                    throw new TestValidationException(string.Format("CanUserAddRows should have been coerced to false"));
                }

                // verify NewItemPlaceholder is not present
                if (MyDataGrid.Items.Count > 0 && MyDataGrid.Items[MyDataGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder)
                {
                    throw new TestValidationException(string.Format(
                        "Expects the last item to not be the NewItemPlaceholder.  Actual: {0}",
                        MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
                }
            }
            else
            {
                // check IEditableCollectionView.CanAddNew
                Type type = dataGridPropertyInfo.ItemsSource.GetType();
                if (!type.IsGenericType && MyDataGrid.Items.Count <= 1)
                {
                    // as of 4.0, IECV.CanAddNew has been updated to support this scenario of a non-generic with a collection
                    // count <= 1
                    if (!MyDataGrid.CanUserAddRows)
                    {
                        throw new TestValidationException(string.Format("CanUserAddRows should be true"));
                    }

                    // verify NewItemPlaceholder is present
                    if (MyDataGrid.Items.Count > 0 && MyDataGrid.Items[MyDataGrid.Items.Count - 1] != CollectionView.NewItemPlaceholder)
                    {
                        throw new TestValidationException(string.Format(
                            "Expects the last item to be the NewItemPlaceholder.  Actual: {0}",
                            MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
                    }
                }
                else
                {
                    // CanUserAddRows should be true
                    if (!MyDataGrid.CanUserAddRows)
                    {
                        throw new TestValidationException(string.Format("CanUserAddRows should be true"));
                    }

                    // verify NewItemPlaceholder is present
                    if (MyDataGrid.Items.Count > 0 && MyDataGrid.Items[MyDataGrid.Items.Count - 1] != CollectionView.NewItemPlaceholder)
                    {
                        throw new TestValidationException(string.Format(
                            "Expects the last item to be the NewItemPlaceholder.  Actual: {0}",
                            MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
                    }
                }
            }
        }

        private void VerifyDataGridDeleteRowState(DataGridPropertyInfo dataGridPropertyInfo)
        {
            if (dataGridPropertyInfo.CanUserDeleteRows.HasValue && !dataGridPropertyInfo.CanUserDeleteRows.Value)
            {
                // no coercion should have took place since it was already set to false
                if (MyDataGrid.CanUserDeleteRows)
                {
                    throw new TestValidationException(string.Format("CanUserAddRows should be false"));
                }

                // try to do a delete and verify
                int prevItemCount = MyDataGrid.Items.Count;
                if (prevItemCount > 0)
                {
                    DataGridActionHelper.DeleteRow(MyDataGrid, 0);
                    if (MyDataGrid.Items.Count != prevItemCount)
                    {
                        throw new TestValidationException(string.Format(
                            "The row was deleted unexpectedly.  Expected count: {0}, Actual count: {1}",
                            prevItemCount,
                            MyDataGrid.Items.Count));
                    }
                }
            }
            else if (MyDataGrid.IsReadOnly || !MyDataGrid.IsEnabled)
            {
                // CanUserAddRows should be coerced to false
                if (MyDataGrid.CanUserDeleteRows)
                {
                    throw new TestValidationException(string.Format("CanUserAddRows should have been coerced to false"));
                }

                // try to do a delete and verify
                int prevItemCount = MyDataGrid.Items.Count;
                if (prevItemCount > 0)
                {
                    DataGridActionHelper.DeleteRow(MyDataGrid, 0);
                    if (MyDataGrid.Items.Count != prevItemCount)
                    {
                        throw new TestValidationException(string.Format(
                            "The row was deleted unexpectedly.  Expected count: {0}, Actual count: {1}",
                            prevItemCount,
                            MyDataGrid.Items.Count));
                    }
                }
            }
            else
            {
                // CanUserAddRows should be true
                if (!MyDataGrid.CanUserDeleteRows)
                {
                    throw new TestValidationException(string.Format("CanUserAddRows should be true"));
                }

                // do a delete and verify
                int prevItemCount = MyDataGrid.Items.Count;
                if (prevItemCount > 0)
                {
                    DataGridActionHelper.DeleteRow(MyDataGrid, 0);
                    if (MyDataGrid.Items.Count != prevItemCount - 1)
                    {
                        throw new TestValidationException(string.Format(
                            "The row was not deleted.  Expected count: {0}, Actual count: {1}",
                            prevItemCount - 1,
                            MyDataGrid.Items.Count));
                    }
                }
            }
        }

        #endregion Helpers
    }
}
