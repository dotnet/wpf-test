// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
	public class PropertyChangeInvalidatesAll : INotifyPropertyChanged
	{
        // No property changed notification
        private int _prop1;

        public int Prop1
        {
            get { return _prop1; }
            set 
            { 
                _prop1 = value;
            }
        }

        // String.Empty invalidates all properties in this class
        private string _prop2;

        public string Prop2
        {
            get { return _prop2; }
            set 
            { 
                _prop2 = value;
                OnPropertyChanged(String.Empty);
            }
        }

        // Null invalidates all properties in this class
        private double _prop3;

        public double Prop3
        {
            get { return _prop3; }
            set
            {
                _prop3 = value;
                OnPropertyChanged(null);
            }
        }

        public PropertyChangeInvalidatesAll()
        {
            this._prop1 = 10;
            this._prop2 = "Hello";
            this._prop3 = 1.1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }
	}
}
