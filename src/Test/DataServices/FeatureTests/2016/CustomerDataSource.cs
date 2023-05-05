// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    public class CustomerList : ObservableCollection<Customer>
    {
        public CustomerList()
        {
            Populate(null);
        }

        void Populate(Customer[] list)
        {
            if (list == null)
                list = s_defaultList;

            foreach (Customer customer in list)
            {
                Add(new Customer(customer));
            }
        }

        static readonly Customer[] s_defaultList = new Customer[]
        {
            new Customer("Harry", 24, "Washington"),
            new Customer("Sally", 28, "Oregon"),
            new Customer("Bill", 25, "California"),
            new Customer("Mary", 28, "California"),
            new Customer("Sandeep", 33, "Washington"),
            new Customer("Qian", 22, "Oregon"),
            new Customer("Josh", 39, "California"),
            new Customer("Sarah", 24, "Washington"),
            new Customer("Nathan", 31, "California"),
            new Customer("Tessa", 34, "Oregon"),
            new Customer("Sven", 28, "Washington"),
            new Customer("Nirmala", 24, "California"),
            new Customer("Richard", 33, "Oregon"),
            new Customer("Malika", 25, "Washington"),
            new Customer("Kwame", 31, "California"),
            new Customer("Rachel", 33, "Oregon"),
        };
    }

    public class Customer : INotifyPropertyChanged
    {
        #region Constructors

        public Customer(string name, int age, string state)
        {
            _name = name;
            _age = age;
            _state = state;
        }

        public Customer(Customer c)
        {
            _name = c._name;
            _age = c._age;
            _state = c._state;
        }

        #endregion Constructors

        #region Properties

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value;  OnPropertyChanged("Name"); }
        }

        int _age;
        public int Age
        {
            get { return _age; }
            set { _age = value;  OnPropertyChanged("Age"); }
        }

        string _state;
        public string State
        {
            get { return _state; }
            set { _state = value;  OnPropertyChanged("State"); }
        }

        #endregion Properties

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged
    }
}
