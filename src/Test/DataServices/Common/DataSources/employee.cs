// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    public class Employee : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Employee(string name, string phone, string team)
        {
            _name = name;
            _team = team;
            _phone = phone;
        }

        string _name = String.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        string _team = String.Empty;
        public string Team
        {
            get { return _team; }
            set { _team = value; RaisePropertyChanged("Team"); }
        }

        string _phone = String.Empty;
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; RaisePropertyChanged("Phone"); }
        }

        public virtual string Title
        {
            get { return "Employee"; }
        }
    }


    public class Dev : Employee
    {
        public Dev(string name, string phone, string team, int bugsFixed) : base(name, phone, team)
        {
            _bugsFixed = bugsFixed;
        }

        int _bugsFixed = 0;
        public int BugsFixed
        {
            get { return _bugsFixed; }
            set { _bugsFixed = value; RaisePropertyChanged("BugsFixed"); }
        }

        public override string Title
        {
            get { return "Dev"; }
        }
    }

    public class PM : Employee
    {
        public PM(string name, string phone, string team, int specsWritten) : base(name, phone, team)
        {
            _specsWritten = specsWritten;
        }

        int _specsWritten = 0;
        public int SpecsWritten
        {
            get { return _specsWritten; }
            set { _specsWritten = value; RaisePropertyChanged("SpecsWritten"); }
        }

        public override string Title
        {
            get { return "PM"; }
        }
    }

    public class Tester : Employee
    {
        public Tester(string name, string phone, string team, int bugsOpened) : base(name, phone, team)
        {
            _bugsOpened = bugsOpened;
        }

        int _bugsOpened = 0;
        public int BugsOpened
        {
            get { return _bugsOpened; }
            set { _bugsOpened = value; RaisePropertyChanged("BugsOpened"); }
        }

        public override string Title
        {
            get { return "Tester"; }
        }
    }

    public class EmployeeCollection : ObservableCollection<Employee>
    {
        public EmployeeCollection() : base()
        {
            Add(new Dev("Dev1", "64342", "Controls", 25));
            Add(new Dev("Dev2", "26740", "Controls", 25));
            Add(new Tester("Tester1", "64871", "Controls", 48));
            Add(new PM("PM1", "75569", "Controls", 4));
            Add(new Dev("Dev3", "66461", "Data", 25));
            Add(new Dev("Dev4", "54023", "Data", 12));
            Add(new Tester("Tester2", "62172", "Data", 48));
            Add(new Tester("Tester3", "58856", "Data", 17));
            Add(new PM("PM2", "75569", "Data", 7));
        }
    }

}

