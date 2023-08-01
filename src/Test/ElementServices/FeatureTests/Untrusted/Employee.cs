// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Used for testing of databinding scenarios.
 * 
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Base employee data type.
    /// </summary>
    public class Employee : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies listeners of property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Calls listeners of the PropertyChanged event if there are any.
        /// </summary>
        /// <param name="name"></param>
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
            
        /// <summary>
        /// Constructor.
        /// </summary>
        public Employee(string name, string phone, string team)
        {
            _name = name;
            _team = team;
            _phone = phone;
        }

        /// <summary>
        /// Name of the employee.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }
        string _name = String.Empty;

        /// <summary>
        /// Name of the employee's team.
        /// </summary>
        public string Team
        {
            get { return _team; }
            set { _team = value; RaisePropertyChanged("Team"); }
        }
        string _team = String.Empty;

        /// <summary>
        /// Phone number of the employee.
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; RaisePropertyChanged("Phone"); }
        }
        string _phone = String.Empty;

        /// <summary>
        /// Title of the employee.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged("Title"); }
        }
        string _title = "Employee";
    }

    /// <summary>
    /// Data collection of Employee types.
    /// </summary>
    public class EmployeeCollection : ObservableCollection<Employee>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Contains some number of Employee instances by default.
        /// </remarks>
        public EmployeeCollection()
            : base()
        {
            Add(new Employee("Sundaram Ramani", "64342", "Controls"));
        }
    }

    /// <summary>
    /// Data converter for employee data types.
    /// </summary>
    public class EmployeeToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts an Employee to a Brush.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!typeof(Employee).Equals(value))
            {
                throw new ArgumentException("The value type '" + value.GetType().Name + "' is not supported.", "value");
            }

            if (targetType != typeof(Brush))
            {
                throw new ArgumentException("Employee cannot be converted to '" + targetType.Name + "'.", "targetType");
            }

            return Brushes.Salmon;
        }

        /// <summary>
        /// Converts a type to an Employee.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

