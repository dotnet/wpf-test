// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where BindingGroup.CancelEdit does not clear error status on the owning element.
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingGroupCancelEdit")]
    public class BindingGroupCancelEdit : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListBox _myListBox;        
        private TextBox _tbSalary;
        private BindingGroup _bindingGroup;
        
        #endregion

        #region Constructors

        public BindingGroupCancelEdit()
            : base(@"BindingGroupCancelEdit.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListBox = (ListBox)RootElement.FindName("myListBox");            
            _tbSalary = (TextBox)RootElement.FindName("tbSalary");
            _bindingGroup = _myStackPanel.BindingGroup;

            if (_myStackPanel == null || _myListBox == null || _tbSalary == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);

            // Grab an item in the listbox.
            _myListBox.SelectedItem = _myListBox.Items[2];
            _tbSalary.Focus();

            // Edit object, enter invalid value, and then submit.
            _bindingGroup.BeginEdit();
            _tbSalary.Text = "100000";
            _myStackPanel.Focus();
            _bindingGroup.CommitEdit();

            WaitForPriority(DispatcherPriority.Render);  

            // We should see a validation error
            StackPanel parentStackPanel = (StackPanel)VisualTreeHelper.GetParent(_myStackPanel);
            UniformGrid uniformGrid = (UniformGrid)VisualTreeHelper.GetParent(parentStackPanel);
            ContentPresenter contentPresenter = (ContentPresenter)VisualTreeHelper.GetParent(uniformGrid);
            AdornerDecorator adornerDecorator = (AdornerDecorator)VisualTreeHelper.GetParent(contentPresenter);
            AdornerLayer adornerLayer = (AdornerLayer)VisualTreeHelper.GetChild(adornerDecorator, 1);

            if (VisualTreeHelper.GetChildrenCount(adornerLayer) != 1)
            {
                LogComment("Templated Adorner is NOT present. Validation Errors were expected.");
                return TestResult.Fail;
            }

            // Now hit cancel to remove error.
            _bindingGroup.CancelEdit();
            WaitForPriority(DispatcherPriority.Render);            

            // Verify if the validation error is gone.
            if (VisualTreeHelper.GetChildrenCount(adornerLayer) != 0)
            {
                LogComment("Templated Adorner is still present. Validation Errors did not go away.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public enum Title
    {
        Director,
        Concertmaster,
        Principal,
        Section
    }

    public class Musician : INotifyPropertyChanged, IEditableObject
    {
        public Musician(string name, Title title, double salary)
        {
            _internalName = name;
            _internalTitle = title;
            _internalSalary = salary;
        }

        string _internalName;
        public string Name
        {
            get { return _internalName; }
            set { _internalName = value; OnPropertyChanged("Name"); }
        }

        Title _internalTitle;
        public Title Title
        {
            get { return _internalTitle; }
            set { _internalTitle = value; OnPropertyChanged("Title"); }
        }

        double _internalSalary;
        public double Salary
        {
            get { return _internalSalary; }
            set { _internalSalary = value; OnPropertyChanged("Salary"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void IEditableObject.BeginEdit()
        {
            _savedValue = new Musician(_internalName, _internalTitle, _internalSalary);
        }

        void IEditableObject.EndEdit()
        {
            _savedValue = null;
        }

        void IEditableObject.CancelEdit()
        {
            if (_savedValue != null)
            {
                _internalName = _savedValue.Name;
                _internalTitle = _savedValue.Title;
                _internalSalary = _savedValue.Salary;

                _savedValue = null;
            }
        }

        Musician _savedValue;
    }

    public class Orchestra : ObservableCollection<Musician>
    {
        public Orchestra()
        {
            Add(new Musician("Dave Waltman", Title.Director, 83212.32));
            Add(new Musician("Dick Poppe", Title.Section, 25342.11));
            Add(new Musician("Sam Bent", Title.Principal, 62821.27));
            Add(new Musician("Susan Reynolds", Title.Section, 38231.21));
            Add(new Musician("Ilkka Talvi", Title.Concertmaster, 120211.24));
            Add(new Musician("Tom Walworth", Title.Principal, 53321.90));
        }
    }

    public class SalaryValidationRule : ValidationRule
    {
        static SalaryValidationRule()
        {
            s_Limit = new Dictionary<Title, double>();
            s_Limit[Title.Director] = 90000.0;
            s_Limit[Title.Concertmaster] = 120000.0;
            s_Limit[Title.Principal] = 80000.0;
            s_Limit[Title.Section] = 50000.0;
        }

        public SalaryValidationRule()
            : base(ValidationStep.ConvertedProposedValue, false)
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;
            BindingGroup bindingGroup = (BindingGroup)value;
            Musician musician = (Musician)bindingGroup.Items[0];
            double salary = (double)bindingGroup.GetValue(musician, "Salary");
            Title title = (Title)bindingGroup.GetValue(musician, "Title");
            double limit = s_Limit[title];
            if (salary > limit)
                result = new ValidationResult(false, String.Format("Salary for {0} cannot exceed {1}", title, limit));
            return result;
        }

        static Dictionary<Title, double> s_Limit;
    }

    #endregion
}
