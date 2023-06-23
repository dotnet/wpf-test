using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Controls.DataSources;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Test committing edits to normalizing properties.
    /// </description>
    /// <remarks>
    /// A "normalizing" property is one whose setter stores a value different
    /// from its argument.   After committing an edit to such a property, the
    /// DataGrid should display the normalized value. We discovered
    /// circumstances where the DataGrid continues to display the value the
    /// user entered while editing.
    /// </remarks>

    /// </summary>
    [Test(1, "DataGrid", "DataGridEditingNormalizingProperties", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.7.1+")]
    public class DataGridEditingNormalizingProperties : DataGridEditing
    {
        #region Constructor

        public DataGridEditingNormalizingProperties()
            : base(@"DataGridEditingNormalizingProperties.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestEditingNormalizingProperty);
            RunSteps += new TestStep(TestEditingNormalizingPropertyWithLazyNotification);
            RunSteps += new TestStep(TestEditingNormalizingDependencyProperty);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for DataGridEditingNormalizingProperties");

            // setup the data source
            DataSource = new NormalizingList();
            TypeFromDataSource = typeof(NormalizingItem);

            // sets up the data source
            base.Setup();

            // Edit the first row
            CurRow = 0;

            LogComment("Setup for DataGridEditingNormalizingProperties was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify edit and commit of a simple normalizing property.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingNormalizingProperty()
        {
            Status("TestEditingNormalizingProperty");

            CurCol = 0;     // Property1
            TestProperty();

            LogComment("TestEditingNormalizingProperty was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify edit and commit of a simple normalizing property.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingNormalizingPropertyWithLazyNotification()
        {
            Status("TestEditingNormalizingPropertyWithLazyNotification");

            CurCol = 1;     // Property2
            TestProperty();

            LogComment("TestEditingNormalizingPropertyWithLazyNotification was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify edit and commit of a simple normalizing property.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditingNormalizingDependencyProperty()
        {
            Status("TestEditingNormalizingDependencyProperty");

            CurCol = 2;     // Property3
            TestProperty();

            LogComment("TestEditingNormalizingDependencyProperty was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region worker methods

        void TestProperty()
        {
            // change the value to one with a different normalization (this always worked)
            Status("Changing property to a new value with a different normalization");
            EditAndCommit("3", "1");

            // change the value to one with the same normalization (this didn't work)
            Status("Changing property to a new value with the same normalization");
            EditAndCommit("4", "1");
        }

        void EditAndCommit(string editedValue, string normalizedValue)
        {
            // turn editing mode on
            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // verify cell is editable
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                true    /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                CurRow  /* the new current row */,
                CurCol  /* the new current col */);

            // edit the cell
            DataGridActionHelper.EditCellCustomInput(MyDataGrid, CurRow, CurCol, editedValue);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // verify edit
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource,
                TypeFromDataSource,
                CurRow,
                CurCol,
                editedValue,
                true,
                false,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);

            // commit the changes
            DataGridCommandHelper.CommitEdit(MyDataGrid, CurRow, CurCol);

            // verify commit
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource,
                TypeFromDataSource,
                CurRow,
                CurCol,
                normalizedValue,
                false,
                true,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);
        }

        #endregion worker methods
    }


    // a class with several different kinds of normalizing properties
    public class NormalizingItem : DependencyObject, INotifyPropertyChanged
    {
        // a simple normalizing property - setter normalizes the value
        int _property1 = 0;
        public int Property1
        {
            get { return _property1; }
            set { _property1 = Math.Sign(value);  OnPropertyChanged("Property1"); }
        }

        // a lazy normalizing property - setter normalizes the value, but
        // only notfies if the value changed
        int _property2 = 0;
        public int Property2
        {
            get { return _property2; }
            set
            {
                int normalizedValue = Math.Sign(value);
                if (normalizedValue != _property2)
                {
                    _property2 = normalizedValue;
                    OnPropertyChanged("Property2");
                }
            }
        }

        // a normalizing DependencyProperty.  Normalization happens in CoerceValue
        public static readonly DependencyProperty Property3Property =
            DependencyProperty.Register("Property3", typeof(int), typeof(NormalizingItem),
                                        new PropertyMetadata(0, null, (d, v) => Math.Sign((int)v)));

        public int Property3
        {
            get { return (int)GetValue(Property3Property); }
            set { SetValue(Property3Property, value); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public class NormalizingList : ObservableCollection<NormalizingItem>
    {
        public NormalizingList()
        {
            Add(new NormalizingItem());
        }
    }

}

