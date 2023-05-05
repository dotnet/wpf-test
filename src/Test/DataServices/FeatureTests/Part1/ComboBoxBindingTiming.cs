// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Binding both ItemsSource and SelectedItem - sensitive dependence on declaration order
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ComboBoxBindingTiming")]
    public class ComboBoxBindingTiming : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ComboBox _myComboBox;                    
        private TestObject _object1;
        private TestObject _object2;
        
        #endregion

        #region Constructors

        public ComboBoxBindingTiming()
            : base(@"ComboBoxBindingTiming.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myComboBox = (ComboBox)RootElement.FindName("myComboBox");
            
            if (_myStackPanel == null || _myComboBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            _object1 = new TestObject();
            _object1.Name = "Object1";
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("A");
            values.Add("B");
            values.Add("C");
            _object1.AvailableValues = values;

            _object2 = new TestObject();
            _object2.Name = "Object2";
            values = new ObservableCollection<string>();
            values.Add("C");
            values.Add("D");
            values.Add("E");
            _object2.AvailableValues = values;

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Bind to object1            
            _myComboBox.DataContext = _object1;            
           
            // Select value "A" from combobox
            _myComboBox.SelectedIndex = 0;            

            //Bind to object2
            _myComboBox.DataContext = _object2;            

            // Verify value of object1 (should remain "A")           
            if (_object1.Value.ToString() != "A")
            {
                LogComment("Binding with original object was lost when datacontext was switched.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion        
    }
    
    #region Helper Classes

    public class TestObject : INotifyPropertyChanged
    {
        private string _internalValue;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private ObservableCollection<string> _availableValues;

        public string Value
        {
            get { return _internalValue; }
            set
            {
                _internalValue = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public ObservableCollection<string> AvailableValues
        {
            get { return _availableValues; }
            set
            {
                _availableValues = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("AvailableValues"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
    
    #endregion
}
