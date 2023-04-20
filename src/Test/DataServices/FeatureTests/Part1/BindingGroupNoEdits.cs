// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Test.Input;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where BindingGroup should run item-level validation rules even when no edits are pending
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingGroupNoEdits", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class BindingGroupNoEdits : XamlTest
    {
        #region Private Data
        
        private StackPanel _myStackPanel;
        private DataGrid _myDataGrid;

        private ObservableCollection<Entity> _properties =
            new ObservableCollection<Entity>
        {
            new Entity("", "AA", "AAA")
        };

        public ObservableCollection<Entity> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
        
        #endregion

        #region Constructors

        public BindingGroupNoEdits()
            : base(@"BindingGroupNoEdits.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myDataGrid = (DataGrid)RootElement.FindName("myDataGrid");

            if (_myStackPanel == null || _myDataGrid == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            _myStackPanel.DataContext = this;

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Click on the first row.
            _myDataGrid.SelectedItem = _myDataGrid.Items[0];
            
            // Verify if there is an validation error on the edited item.
            Entity item = _myDataGrid.SelectedItem as Entity;

            DataGridRow row = _myDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

            // Double Click the row, to bring it into editing mode.
            UserInput.MouseLeftDown(row, 40, 5);
            WaitForPriority(DispatcherPriority.Input);         
            UserInput.MouseLeftUp(row, 40, 5);
            WaitForPriority(DispatcherPriority.Input);
            UserInput.MouseLeftDown(row, 40, 5);
            WaitForPriority(DispatcherPriority.Input);         
            UserInput.MouseLeftUp(row, 40, 5);
            WaitForPriority(DispatcherPriority.Input);         
                        
            // Verify the validation error
            if (Validation.GetHasError(row) == false)
            {
                LogComment("Failed to get expected error.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class Entity : IDataErrorInfo
    {
        public Entity()
        {
        }
        public Entity(string _name, string _type, string _value)
        {
            this._name = _name;
            this._type = _type;
            this._value = _value;
        }

        private string _name, _type, _value;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Error
        {
            get
            {
                return this["Name"] ?? this["Type"] ?? this["Value"];
            }
        }

        public string this[string columnName]
        {
            get
            {
                PropertyInfo info = typeof(Entity).GetProperty(columnName);

                if (info != null)
                {
                    if (string.IsNullOrEmpty(info.GetValue(this, null) as string))
                    {
                        return "Empty Field!";
                    }
                }

                return null;
            }
        }
    }

    #endregion
}
