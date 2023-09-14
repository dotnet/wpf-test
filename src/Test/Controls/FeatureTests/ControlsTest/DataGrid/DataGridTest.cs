using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Base test class for all DataGrid tests using XamlTest
    /// </description>

    /// </summary>
    public abstract class DataGridTest : XamlTest
    {
        #region Constructor

        public DataGridTest(string filename)
            : base(filename)
        {
            CleanUpSteps += CleanUp;
        }

        #endregion

        #region Public and Protected Members

        protected readonly string dataGridPartName = "DataGrid_Standard";

        /// <summary>
        /// DataGrid reference
        /// </summary>
        protected DataGrid MyDataGrid { get; set; }

        /// <summary>
        /// The data source that will be used on the DataGrid
        /// </summary>
        protected IEnumerable DataSource { get; set; }

        /// <summary>
        /// string name of the data source type.  used to construct
        /// the DataSource
        /// </summary>
        protected string DataSourceTypeName { get; set; }

        /// <summary>
        /// The type that the data source collection uses
        /// </summary>
        protected Type TypeFromDataSource { get; set; }

        /// <summary>
        /// string name of the type that the data source colleciton uses.  used to
        /// construct the TypeFromDataSource
        /// </summary>
        protected string TypeNameFromDataSource { get; set; }

        /// <summary>
        /// state for the current row
        /// </summary>
        protected int CurRow { get; set; }

        /// <summary>
        /// state for the current column
        /// </summary>
        protected int CurCol { get; set; }

        protected bool UseDisplayIndexOrder { get; set; }

        #region Setup

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public virtual TestResult Setup()
        {
            Status("Setup");

            // find the datagrid
            MyDataGrid = (DataGrid)RootElement.FindName(dataGridPartName);
            if (MyDataGrid == null)
            {
                LogComment("Can not find a DataGrid in xaml file");
                return TestResult.Fail;
            }

            UseDisplayIndexOrder = false;            

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        public virtual TestResult CleanUp()
        {
            MyDataGrid = null;   
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            return TestResult.Pass;
        }

        #endregion Setup

        #endregion Public and Protected Members

        #region Helpers

        protected virtual void CreateDataSource()
        {
            CreateDataSource(1);
        }

        protected virtual void CreateDataSource(int dataCountMultipler)
        {
            if (DataSourceTypeName != null && TypeNameFromDataSource != null)
            {
                TypeFromDataSource = Type.GetType(TypeNameFromDataSource, true);

                // check special case
                if (DataSourceTypeName.Contains("ObservableCollection"))
                {
                    //
                    Type baseListType = typeof(ObservableCollection<>);
                    Type dataSourceType = baseListType.MakeGenericType(TypeFromDataSource);
                    DataSource = (IEnumerable)Activator.CreateInstance(dataSourceType);
                }
                else
                {
                    // setup DataSource and TypeFromDataSource
                    Type dataSourceType = Type.GetType(DataSourceTypeName, true);
                    DataSource = (IEnumerable)Activator.CreateInstance(dataSourceType, new object[] { dataCountMultipler });
                }
            }
        }

        protected virtual void SetupDataSource()
        {
            // if DataSource is provided use it
            if (DataSource != null)
            {
                MyDataGrid.ItemsSource = DataSource;
            }
            // otherwise use People as the default
            else
            {
                DataSource = new People();
                TypeFromDataSource = typeof(Person);
                MyDataGrid.ItemsSource = DataSource;
            }

            this.WaitForPriority(DispatcherPriority.SystemIdle);
            this.Window.Focus();
        }

        /// <summary>
        /// Creates a constructed type generic method based on TypeFromDataSource
        /// </summary>
        /// <param name="targetType">the type that implements the generic method</param>
        /// <param name="methodName">the generic method name</param>
        /// <returns></returns>
        public TestStep CreateTestStepFromGeneric(Type targetType, string methodName)
        {
            Assert.AssertTrue("TypeFromDataSource must be set to create the generic TestStep.", TypeFromDataSource != null);

            MethodInfo step = targetType
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(new Type[] { TypeFromDataSource });
            return (TestStep)(Delegate.CreateDelegate(typeof(TestStep), this, step, true));
        }

        public int IncrementColumnBy(int value, int toAdd)
        {
            if (UseDisplayIndexOrder)
            {
                DataGridColumn column = MyDataGrid.ColumnFromDisplayIndex((MyDataGrid.Columns[value].DisplayIndex + toAdd) % MyDataGrid.Columns.Count);
                return MyDataGrid.Columns.IndexOf(column);
            }
            else
            {
                return value + toAdd;
            }
        }

        public int DecrementColumnBy(int value, int toSub)
        {
            if (UseDisplayIndexOrder)
            {
                int tempVal = MyDataGrid.Columns[value].DisplayIndex - toSub;
                if (tempVal < 0)
                    tempVal = 0;
                DataGridColumn column = MyDataGrid.ColumnFromDisplayIndex(tempVal);
                return MyDataGrid.Columns.IndexOf(column);
            }
            else
            {
                return value - toSub;
            }
        }

        public int ColumnEquivalent(int value)
        {
            if (UseDisplayIndexOrder)
            {
                DataGridColumn column = MyDataGrid.ColumnFromDisplayIndex(value);
                return MyDataGrid.Columns.IndexOf(column);
            }
            else
            {
                return value;
            }
        }

        public bool ColumnLessThan(int col1, int col2)
        {
            if (UseDisplayIndexOrder)
            {
                return MyDataGrid.Columns[col1].DisplayIndex < MyDataGrid.Columns[col2].DisplayIndex;
            }
            else
            {
                return col1 < col2;
            }
        }

        public bool ColumnEqual(int col1, int col2)
        {
            if (UseDisplayIndexOrder)
            {
                return MyDataGrid.Columns[col1].DisplayIndex == MyDataGrid.Columns[col2].DisplayIndex;
            }
            else
            {
                return col1 == col2;
            }
        }

        #endregion Helpers
    }
}
