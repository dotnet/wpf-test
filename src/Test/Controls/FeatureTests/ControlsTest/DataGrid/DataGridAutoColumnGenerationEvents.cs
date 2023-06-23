using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// DataGrid auto-column generation event tests.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridAutoColumnGenerationEvents", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridAutoColumnGenerationEvents : DataGridTest
    {
        #region Private Fields

        private ListCollectionView cv;

        #endregion Private Fields

        #region Constructor

        public DataGridAutoColumnGenerationEvents()
            : base(@"DataGridAutoGenOffUserSet.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(TestAutoGeneratingColumnsEvent);
            RunSteps += new TestStep(TestCancellingColumnGeneration);
            RunSteps += new TestStep(TestModifyingTheColumnDuringGeneration);            
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

            Status("Setup specific for DataGridAutoColumnGenerationEvents");

            Assert.AssertTrue("AutoGenerateColumns should be false.", MyDataGrid.AutoGenerateColumns == false);

            SetupDataSource();

            Assert.AssertTrue("DataSource must be of type IList.", DataSource is IList);
            cv = new ListCollectionView(DataSource as IList);
            
            LogComment("Setup for DataGridAutoColumnGenerationEvents was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestAutoGeneratingColumnsEvent()
        {
            Status("TestAutoGeneratingColumnsEvent");

            Assert.AssertTrue("AutoGenerateColumns should be false.", MyDataGrid.AutoGenerateColumns == false);

            EventHandler<DataGridAutoGeneratingColumnEventArgs> handleEvent = null;

            try
            {
                // setup expected data
                int expectedNumEvents = TypeFromDataSource.GetProperties(BindingFlags.Public | BindingFlags.Instance).Length;
                IItemProperties itemProperties = cv as IItemProperties;

                int eventCounter = 0;
                handleEvent = (s, e) =>
                    {
                        //
                        // verify the arguments are correct
                        //

                        if (e.PropertyDescriptor != itemProperties.ItemProperties[eventCounter].Descriptor)
                        {
                            throw new TestValidationException(string.Format(
                                "PropertyDescriptors do not match.  Expect: {0}, Actual: {1}",
                                itemProperties.ItemProperties[eventCounter].Descriptor,
                                e.PropertyDescriptor));
                        }

                        if (e.PropertyName != itemProperties.ItemProperties[eventCounter].Name)
                        {
                            throw new TestValidationException(string.Format(
                                "PropertyNames do not match.  Expect: {0}, Actual: {1}",
                                itemProperties.ItemProperties[eventCounter].Name,
                                e.PropertyName));
                        }

                        if (e.PropertyType != itemProperties.ItemProperties[eventCounter].PropertyType)
                        {
                            throw new TestValidationException(string.Format(
                                "PropertyTypes do not match.  Expect: {0}, Actual: {1}",
                                itemProperties.ItemProperties[eventCounter].PropertyType,
                                e.PropertyType));
                        }                        

                        eventCounter++;
                    };
                MyDataGrid.AutoGeneratingColumn += handleEvent;

                MyDataGrid.AutoGenerateColumns = true;

                // verify the number of events fired
                if (eventCounter != expectedNumEvents)
                {
                    throw new TestValidationException(string.Format(
                        "The wrong number of AutoGeneratingColumnEvents fired.  Expect: {0}, Actual: {1}",
                        expectedNumEvents,
                        eventCounter));
                }
            }
            finally
            {
                if(handleEvent != null)
                    MyDataGrid.AutoGeneratingColumn -= handleEvent;

                MyDataGrid.AutoGenerateColumns = false;
            }

            LogComment("TestAutoGeneratingColumnsEvent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingColumnGeneration()
        {
            Status("TestCancellingColumnGeneration");

            Assert.AssertTrue("AutoGenerateColumns should be false.", MyDataGrid.AutoGenerateColumns == false);

            EventHandler<DataGridAutoGeneratingColumnEventArgs> handleEvent = null;

            try
            {
                DataGridColumn[] expectedColumns = new DataGridColumn[MyDataGrid.Columns.Count];
                MyDataGrid.Columns.CopyTo(expectedColumns, 0);

                handleEvent = (s, e) =>
                    {
                        e.Cancel = true;
                    };
                MyDataGrid.AutoGeneratingColumn += handleEvent;

                MyDataGrid.AutoGenerateColumns = true;

                // verify columns are not generated
                int i = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    if (column != expectedColumns[i])
                    {
                        throw new TestValidationException(string.Format(
                            "Mismatch in columns after cancelling generation.  Expect: {0}, Actual: {1}",
                            expectedColumns[i],
                            column));
                    }
                    i++;
                }
            }
            finally
            {
                if (handleEvent != null)
                    MyDataGrid.AutoGeneratingColumn -= handleEvent;

                MyDataGrid.AutoGenerateColumns = false;
            }

            LogComment("TestCancellingColumnGeneration was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestModifyingTheColumnDuringGeneration()
        {
            Status("TestModifyingTheColumnDuringGeneration");

            Assert.AssertTrue("AutoGenerateColumns should be false.", MyDataGrid.AutoGenerateColumns == false);

            EventHandler<DataGridAutoGeneratingColumnEventArgs> handleEvent = null;

            try
            {
                DataGridColumn[] expectedColumns = new DataGridColumn[MyDataGrid.Columns.Count];
                MyDataGrid.Columns.CopyTo(expectedColumns, 0);

                handleEvent = (s, e) =>
                {
                    // change every column to a template column
                    e.Column = new DataGridTemplateColumn();
                    e.Column.Header = "test";
                };
                MyDataGrid.AutoGeneratingColumn += handleEvent;

                MyDataGrid.AutoGenerateColumns = true;

                // verify generated columns are all template columns
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    if (column.IsAutoGenerated)
                    {
                        if (!(column is DataGridTemplateColumn) || column.Header.ToString() != "test")
                        {
                            throw new TestValidationException(string.Format(
                                "Column was not modified correctly from event.  Column type: {0}, Column header: {1}",
                                column.GetType().ToString(),
                                column.Header.ToString()));
                        }
                    }
                }
            }
            finally
            {
                if (handleEvent != null)
                    MyDataGrid.AutoGeneratingColumn -= handleEvent;

                MyDataGrid.AutoGenerateColumns = false;
            }

            LogComment("TestModifyingTheColumnDuringGeneration was successful");
            return TestResult.Pass;
        }        
        
        #endregion Test Steps 
    }
}
