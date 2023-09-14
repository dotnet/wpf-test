using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Controls.DataSources;
using System.Windows.Data;
using System.ComponentModel;
using System;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Collections;
using System.Xml;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Auto-sorting Behavioral tests for DataGrid.
    /// </description>

    /// </summary>
    // [ DISABLED_WHILE_PORTING ]
    [Test(0, "DataGrid", "DataGridAutoSorting", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite", Disabled=true)]
    public class DataGridAutoSorting : DataGridTest
    {
        #region Private Fields

        private People personDataSource;
        private CollectionViewSource cvs;
        private ListSortDirection? currentSortDirection;
        private DoSortOnColumn SortUnsortableColumn;

        #endregion Private Fields

        #region Constructor

        public DataGridAutoSorting()
            : base(@"DataGridAutoSorting.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestSortingWithTextColumns);
            RunSteps += new TestStep(TestSortingWithCheckBoxColumns);
            RunSteps += new TestStep(TestSortingWithComboBoxColumns);
            RunSteps += new TestStep(TestSortingWithHyperlinkColumns);
            RunSteps += new TestStep(TestSortingWithTemplateColumns);
            RunSteps += new TestStep(TestSortingInvalidInput);
            RunSteps += new TestStep(TestSortingWithXPathData);
            RunSteps += new TestStep(TestSortingEvent);

            // as of 4.5, auto-sorting an unsortable column doesn't throw
            SortUnsortableColumn =
                #if TESTBUILD_NET_ATLEAST_45
                    this.DoSort;
                #else
                    this.DoSortAndExpectException;
                #endif
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

            cvs = (CollectionViewSource)this.RootElement.FindResource("ViewSource");
            personDataSource = new People();

            this.WaitForPriority(DispatcherPriority.SystemIdle);
            this.Window.Focus();

            LogComment("Setup for DataGridAutoSorting was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            cvs = null;

            return base.CleanUp();
        }
        /// <summary>
        /// Sorting on a text column
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithTextColumns()
        {
            Status("TestSortingWithTextColumns");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTextColumn },
                canSortList = new bool?[] { null, false },
                sortDirectionList = new ListSortDirection?[] { null, ListSortDirection.Ascending, ListSortDirection.Descending },
                sortMemberPathList = new string[] { null, "LastName", "DOB" },
                displayMemberPathList = new string[] { null, "LastName" },
                isCustomHandled = false,
                DoSortOnColumn = this.DoSort,
                VerifySortOnColumn = this.VerifySort<Person>,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config);

            LogComment("TestSortingWithTextColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Sorting on a checkbox column
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithCheckBoxColumns()
        {
            Status("TestSortingWithCheckBoxColumns");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridCheckBoxColumn },
                canSortList = new bool?[] { true, false },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { null, "LikesCake" },
                displayMemberPathList = new string[] { "LikesCake", "DOB" },
                isCustomHandled = false,
                DoSortOnColumn = this.DoSort,
                VerifySortOnColumn = this.VerifySort<Person>,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config);

            LogComment("TestSortingWithCheckBoxColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Sorting on a combobox column
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithComboBoxColumns()
        {
            Status("TestSortingWithComboBoxColumns");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridComboBoxColumn },
                canSortList = new bool?[] { true },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { null, "Cake" },
                displayMemberPathList = new string[] { "LastName" },
                isCustomHandled = false,
                DoSortOnColumn = this.DoSort,
                VerifySortOnColumn = this.VerifySort<Person>,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config);

            LogComment("TestSortingWithComboBoxColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Sorting with a hyperlink column
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithHyperlinkColumns()
        {
            Status("TestSortingWithHyperlinkColumns");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridHyperlinkColumn },
                canSortList = new bool?[] { true },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { null, "Homepage" },
                displayMemberPathList = new string[] { "Homepage" },
                isCustomHandled = false,
                DoSortOnColumn = this.SortUnsortableColumn,
                VerifySortOnColumn = null,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config);

            // settings for valid property with a hyperlink column
            ConfigurationMatrix<Person> config2 = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridHyperlinkColumn },
                canSortList = new bool?[] { true },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { "FirstName" },
                displayMemberPathList = new string[] { "Homepage" },
                isCustomHandled = false,
                DoSortOnColumn = this.DoSort,
                VerifySortOnColumn = this.VerifySort<Person>,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config2);

            LogComment("TestSortingWithHyperlinkColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Sorting with a template column
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithTemplateColumns()
        {
            Status("TestSortingWithTemplateColumns");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTemplateColumn },
                canSortList = new bool?[] { true, false },
                sortDirectionList = new ListSortDirection?[] { null, ListSortDirection.Ascending, ListSortDirection.Descending },
                sortMemberPathList = new string[] { null, "LastName", "DOB" },
                displayMemberPathList = new string[] { null },
                isCustomHandled = false,
                DoSortOnColumn = this.DoSort,
                VerifySortOnColumn = this.VerifySort<Person>,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config);

            // configuration and expect exception
            ConfigurationMatrix<Person> config2 = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTemplateColumn },
                canSortList = new bool?[] { true },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { "Homepage" },
                displayMemberPathList = new string[] { null },
                isCustomHandled = false,
                DoSortOnColumn = this.SortUnsortableColumn,
                VerifySortOnColumn = null,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config2);

            LogComment("TestSortingWithTemplateColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Set the sortmemberpath and DataFieldBinding to a type not of
        /// IComparable.  Note that setting a property not in the collection
        /// does not throw an exception.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingInvalidInput()
        {
            Status("TestSortingInvalidInput");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTextColumn },
                canSortList = new bool?[] { true },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { null, "Homepage" /*, "NotInCollection"*/ },
                displayMemberPathList = new string[] { "Homepage" /*, "NotInCollection"*/ },
                isCustomHandled = false,
                DoSortOnColumn = this.SortUnsortableColumn,
                VerifySortOnColumn = null,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            this.TestAutoSortingMatrix<Person>(config);

            LogComment("TestSortingInvalidInput was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify sorting functionality when XPath binding is used
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingWithXPathData()
        {
            Status("TestSortingWithXPathData");

            ConfigurationMatrix<XmlElement> config = new ConfigurationMatrix<XmlElement>
            {
                dataSource = cvs.View.SourceCollection,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTextColumn },
                canSortList = new bool?[] { null, true, false },
                sortDirectionList = new ListSortDirection?[] { null, ListSortDirection.Ascending, ListSortDirection.Descending },
                sortMemberPathList = new string[] { null, "@FirstName" },
                displayMemberPathList = new string[] { "@LastName" },
                isCustomHandled = false,
                DoSortOnColumn = this.DoSort,
                VerifySortOnColumn = this.VerifySort<XmlElement>,
                GetPropertyValue = this.GetPropertyValueFromXmlElement<XmlElement>
            };

            this.TestAutoSortingMatrix<XmlElement>(config);

            LogComment("TestSortingWithXPathData was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify setting handled to true does not trigger the default sorting event.
        /// Verify setting handled to false does trigger the default sorting event
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingEvent()
        {
            Status("TestSortingEvent");

            ConfigurationMatrix<Person> config = new ConfigurationMatrix<Person>
             {
                 dataSource = personDataSource,
                 columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTextColumn },
                 canSortList = new bool?[] { true },
                 sortDirectionList = new ListSortDirection?[] { null },
                 sortMemberPathList = new string[] { "LastName" },
                 displayMemberPathList = new string[] { null },
                 isCustomHandled = true,
                 DoSortOnColumn = this.DoCustomSortAndExpectEvent,
                 VerifySortOnColumn = this.VerifySort<Person>,
                 GetPropertyValue = this.GetPropertyValueFromObject<Person>
             };

            this.TestAutoSortingMatrix<Person>(config);

            // listen for event to do not handle
            ConfigurationMatrix<Person> config2 = new ConfigurationMatrix<Person>
            {
                dataSource = personDataSource,
                columnTypes = new DataGridHelper.ColumnTypes[] { DataGridHelper.ColumnTypes.DataGridTextColumn },
                canSortList = new bool?[] { true },
                sortDirectionList = new ListSortDirection?[] { null },
                sortMemberPathList = new string[] { "LastName" },
                displayMemberPathList = new string[] { null },
                isCustomHandled = false,
                DoSortOnColumn = this.DoCustomSortAndExpectEvent,
                VerifySortOnColumn = this.VerifySort<Person>,
                GetPropertyValue = this.GetPropertyValueFromObject<Person>
            };

            // verify default sort was used
            this.TestAutoSortingMatrix<Person>(config2);

            LogComment("TestSortingEvent was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        #region Configuration Struct Helper

        public struct ConfigurationMatrix<T>
        {
            /// <summary>
            /// data source that will be set on the DataGrid
            /// </summary>
            public IEnumerable dataSource;

            /// <summary>
            /// array of column types to exercise
            /// </summary>
            public DataGridHelper.ColumnTypes[] columnTypes;

            /// <summary>
            /// array of CanSort values to exercise
            /// </summary>
            public bool?[] canSortList;

            /// <summary>
            /// array of SortDirection values to exercise
            /// </summary>
            public ListSortDirection?[] sortDirectionList;

            /// <summary>
            /// array of SortMemberPath values to exercise
            /// </summary>
            public string[] sortMemberPathList;

            /// <summary>
            /// array of DataFieldBinding values to exercise
            /// </summary>
            public string[] displayMemberPathList;

            /// <summary>
            /// Will be passed to DoSortOnColumn where DoSortOnColumn
            /// can attach to the Sorting event and use this as the Handled property
            /// </summary>
            public bool isCustomHandled;

            /// <summary>
            /// Delegate for the actual sorting action in the simulation
            /// </summary>
            public DoSortOnColumn DoSortOnColumn;

            /// <summary>
            /// Delegate for the verification in the simulation
            /// </summary>
            public VerifySortOnColumn<T> VerifySortOnColumn;

            /// <summary>
            /// Delegate for how the dataitem property value is retrieved
            /// </summary>
            public GetPropertyValue<T> GetPropertyValue;
        }

        public struct VerifyConfiguration<T>
        {
            public DataGridHelper.ColumnTypes columnType;
            public bool? canSort;
            public ListSortDirection? sortDirection;
            public string sortMemberPath;
            public string displayMemberPath;
            public T prevDataItem0;
            public T prevDataItem1;
            public GetPropertyValue<T> GetPropertyValue;
            public bool isCustomHandled;

            /// <summary>
            /// Logging info
            /// </summary>
            /// <returns></returns>
            public StringBuilder GetDebugInfo()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Configuration:" + Environment.NewLine);
                sb.Append("CanSort: " + canSort.ToString() + Environment.NewLine);
                sb.Append("SortDirection: " + sortDirection.ToString() + Environment.NewLine);
                sb.Append("SortMemberPath: " + sortMemberPath + Environment.NewLine);
                sb.Append("DisplayMemberPath: " + displayMemberPath + Environment.NewLine);
                sb.Append("prevDataItem0: " + prevDataItem0.ToString() + Environment.NewLine);
                sb.Append("prevDataItem1: " + prevDataItem1.ToString() + Environment.NewLine);
                return sb;
            }
        }

        #endregion Configuration Struct Helper

        #region TestAutoSortingMatrix

        /// <summary>
        /// Delegate the sorting action which is called by TestAutoSortingMatrix
        /// </summary>
        /// <param name="index">the column index to sort</param>
        /// <param name="isCustomHandled">if true, will do custom handling, otherwise DefaultSort will be called</param>
        public delegate void DoSortOnColumn(int index, bool isCustomHandled);

        /// <summary>
        /// Delegate the verification of the sort.  Called by TestAutoSortingMatrix
        /// </summary>
        /// <typeparam name="T">the data item type of the datasource</typeparam>
        /// <param name="verifyConfig">config items necessary to do the verification</param>
        public delegate void VerifySortOnColumn<T>(VerifyConfiguration<T> verifyConfig);

        /// <summary>
        /// Simulation of the factors involved with sorting
        /// </summary>
        /// <typeparam name="T">the data item type of the datasource</typeparam>
        /// <param name="configuration">matrix configuration for how to run the simulation</param>
        public void TestAutoSortingMatrix<T>(ConfigurationMatrix<T> configuration)
        {
            if (configuration.DoSortOnColumn == null)
            {
                throw new NullReferenceException("DoSortOnColumn delegate must be set.");
            }

            // setup the itemssource
            MyDataGrid.ItemsSource = configuration.dataSource;
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            //

            foreach (DataGridHelper.ColumnTypes columnType in configuration.columnTypes)
            {
                LogComment(string.Format("Begin testing with ColumnType: {0}", columnType.ToString()));

                // get the column to sort
                int columnIndex = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, columnType);
                DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, columnIndex);

                foreach (bool? canSort in configuration.canSortList)
                {
                    LogComment(string.Format("Begin testing with canSort: {0}", canSort.ToString()));

                    if (canSort.HasValue)
                    {
                        column.CanUserSort = canSort.Value;
                    }

                    foreach (ListSortDirection? sortDirection in configuration.sortDirectionList)
                    {
                        LogComment(string.Format("Begin testing with sortDirection: {0}", sortDirection));

                        if (sortDirection.HasValue)
                        {
                            column.SortDirection = sortDirection.Value;
                            currentSortDirection = sortDirection;
                        }
                        else
                        {
                            column.SortDirection = null;
                            currentSortDirection = null;
                        }

                        foreach (string sortMemberPath in configuration.sortMemberPathList)
                        {
                            LogComment(string.Format("Begin testing with sortMemberPath: {0}", sortMemberPath));

                            if (sortMemberPath != null)
                            {
                                column.SortMemberPath = sortMemberPath;
                            }
                            else
                            {
                                column.SortMemberPath = null;
                            }

                            foreach (string displayMemberPath in configuration.displayMemberPathList)
                            {
                                LogComment(string.Format("Begin testing with displayMemberPath: {0}", displayMemberPath));

                                DataGridBoundColumn boundColumn = column as DataGridBoundColumn;
                                if (boundColumn != null)
                                {
                                    boundColumn.Binding = GetBindingForDisplayMemberPath(displayMemberPath);
                                }

                                DataGridComboBoxColumn comboBoxColumn = column as DataGridComboBoxColumn;
                                if (comboBoxColumn != null)
                                {
                                    comboBoxColumn.TextBinding = GetBindingForDisplayMemberPath(displayMemberPath);
                                }

                                this.WaitForPriority(DispatcherPriority.SystemIdle);

                                // get previous index values
                                T curDataItem0 = (T)MyDataGrid.Items[0];
                                T curDataItem1 = (T)MyDataGrid.Items[1];

                                LogComment("do sorting actions");
                                configuration.DoSortOnColumn(columnIndex, configuration.isCustomHandled);

                                // update the sort direction state
                                if (!string.IsNullOrEmpty(sortMemberPath) || !string.IsNullOrEmpty(displayMemberPath))
                                {
                                    if (!currentSortDirection.HasValue)
                                        currentSortDirection = ListSortDirection.Ascending;
                                    else if (currentSortDirection.Value == ListSortDirection.Ascending)
                                        currentSortDirection = ListSortDirection.Descending;
                                    else
                                        currentSortDirection = ListSortDirection.Ascending;
                                }

                                // verify the sort on the column
                                if (configuration.VerifySortOnColumn != null)
                                {
                                    LogComment("do sorting verification");
                                    VerifyConfiguration<T> verifyConfig = new VerifyConfiguration<T>();
                                    verifyConfig.canSort = canSort;
                                    verifyConfig.sortDirection = currentSortDirection;
                                    verifyConfig.sortMemberPath = sortMemberPath;
                                    verifyConfig.displayMemberPath = displayMemberPath;
                                    verifyConfig.prevDataItem0 = curDataItem0;
                                    verifyConfig.prevDataItem1 = curDataItem1;
                                    verifyConfig.GetPropertyValue = configuration.GetPropertyValue;
                                    verifyConfig.isCustomHandled = configuration.isCustomHandled;
                                    configuration.VerifySortOnColumn(verifyConfig);
                                }

                                LogComment(string.Format("End testing with displayMemberPath: {0}", displayMemberPath));
                            }

                            LogComment(string.Format("End testing with sortMemberPath: {0}", sortMemberPath));
                        }

                        LogComment(string.Format("End testing with sortDirection: {0}", sortDirection.ToString()));
                    }

                    LogComment(string.Format("End testing with canSort: {0}", canSort.ToString()));
                }

                LogComment(string.Format("End testing with ColumnType: {0}{1}", columnType.ToString(), Environment.NewLine));
            }
        }

        Binding GetBindingForDisplayMemberPath(string displayMemberPath)
        {
            Binding binding;
            if (displayMemberPath != null)
            {
                binding = new Binding();

                if (displayMemberPath.Contains("@"))
                {
                    binding.XPath = displayMemberPath;
                }
                else
                {
                    binding.Path = new PropertyPath(displayMemberPath);
                }
            }
            else
            {
                binding = null;
            }

            return binding;
        }

        #endregion TestAutoSortingMatrix

        #region Sorting Actions

        public void DoSort(int columnIndex, bool isCustomHandled)
        {
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, columnIndex);
        }

        public void DoSortAndExpectException(int columnIndex, bool isCustomHandled)
        {
            ExceptionHelper.ExpectException(
                () =>
                {
                    DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, columnIndex);
                },
                new InvalidOperationException());
        }

        public void DoCustomSortAndExpectEvent(int columnIndex, bool isCustomHandled)
        {
            DataGridSortingEventArgs expectedArgs = null;
            DataGridSortingEventArgs actualArgs = null;

            DataGridSortingEventHandler OnSorting = (s, e) =>
                {
                    e.Handled = isCustomHandled;
                    actualArgs = e;
                };

            try
            {
                // setup event
                MyDataGrid.Sorting += OnSorting;

                // expected args
                DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, columnIndex);
                expectedArgs = new DataGridSortingEventArgs(column);
                expectedArgs.Handled = isCustomHandled;

                // do action
                DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, columnIndex);
            }
            finally
            {
                MyDataGrid.Sorting -= OnSorting;
            }

            // verify the event fired
            if (expectedArgs.Column != actualArgs.Column)
            {
                throw new TestValidationException(string.Format(
                    "Sorting event args do not match. Expected column: {0}, Actual column: {1}",
                    expectedArgs.Column,
                    actualArgs.Column));
            }
        }

        #endregion Sorting Actions

        #region Verification Helpers

        /// <summary>
        /// Delegate for retrieving the property value of the data item
        /// </summary>
        /// <typeparam name="T">the data item type of the datasource</typeparam>
        /// <param name="dataItem">the data item</param>
        /// <param name="propertyName">the property to retrieve</param>
        public delegate object GetPropertyValue<T>(T dataItem, string propertyName);

        public void VerifySort<T>(VerifyConfiguration<T> verifyConfig)
        {
            // get the current data
            T curDataItem0 = (T)MyDataGrid.Items[0];
            T curDataItem1 = (T)MyDataGrid.Items[1];

            // debug info
            StringBuilder sb = verifyConfig.GetDebugInfo();
            sb.Append("curDataItem0: " + curDataItem0.ToString() + Environment.NewLine);
            sb.Append("curDataItem1: " + curDataItem1.ToString() + Environment.NewLine);

            // true by default
            // Note: if isCustomHandled is true, this function will verify that sort did not occur.
            if ((!verifyConfig.canSort.HasValue || verifyConfig.canSort.Value) && !verifyConfig.isCustomHandled)
            {
                object property0 = null;
                object property1 = null;
                string memberPath = null;

                // get the properties to compare
                if (!string.IsNullOrEmpty(verifyConfig.sortMemberPath))
                {
                    memberPath = verifyConfig.sortMemberPath;
                }
                else if (!string.IsNullOrEmpty(verifyConfig.displayMemberPath))
                {
                    memberPath = verifyConfig.displayMemberPath;
                }

                if (memberPath != null)
                {
                    property0 = verifyConfig.GetPropertyValue(curDataItem0, memberPath);
                    property1 = verifyConfig.GetPropertyValue(curDataItem1, memberPath);
                }

                if (property0 != null && property1 != null)
                {
                    if (!verifyConfig.sortDirection.HasValue || verifyConfig.sortDirection.Value == ListSortDirection.Ascending)
                    {
                        // click will change sort direction to ascending
                        if (string.Compare(property0.ToString(), property1.ToString()) > 0)
                        {
                            sb.Append(string.Format(
                                "property0: {0}, should be less than property1: {1}",
                                property0.ToString(),
                                property1.ToString()));
                            throw new TestValidationException(string.Format(
                                "Sort order is incorrect. Should be ascending. Details:{0}{1}",
                                sb.ToString(),
                                Environment.NewLine));
                        }
                    }
                    else
                    {
                        // click will change sort direction to descending
                        if (string.Compare(property0.ToString(), property1.ToString()) < 0)
                        {
                            sb.Append(string.Format(
                                "property0: {0}, should be greater than property1: {1}",
                                property0.ToString(),
                                property1.ToString()));
                            throw new TestValidationException(string.Format(
                                "Sort order is incorrect. Should be descending. Details:{0}{1}",
                                sb.ToString(),
                                Environment.NewLine));
                        }
                    }
                }
            }
            else
            {
                // should be the same values
                if (!verifyConfig.prevDataItem0.Equals(curDataItem0) || !verifyConfig.prevDataItem1.Equals(curDataItem1))
                {
                    throw new TestValidationException(string.Format(
                        "Order should be the same when it isn't.  Details:{0}{1}",
                        sb.ToString(),
                        Environment.NewLine));
                }
            }
        }

        #endregion Verification Helpers

        #region GetPropertyValue custom implementations

        public object GetPropertyValueFromObject<T>(T dataItem, string propertyName)
        {
            Type type = typeof(T);
            return type.InvokeMember(propertyName,
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                            null, dataItem, null);
        }

        public object GetPropertyValueFromXmlElement<T>(T dataItem, string propertyName)
        {
            XmlElement xmlElement = dataItem as XmlElement;
            foreach (XmlAttribute attribute in xmlElement.Attributes)
            {
                if (propertyName.Contains(attribute.Name))
                {
                    return attribute.Value;
                }
            }

            return null;
        }

        #endregion GetPropertyValue custom implementations

        #endregion Helpers
    }
}
