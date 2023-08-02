// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

#endregion

namespace Microsoft.Test.Layout
{
    public class MyDataSource : INotifyPropertyChanged
    {
        private string _myStringProperty;

        public string StringProperty
        {
            get { return _myStringProperty; }
            set { _myStringProperty = value; RaisePropertyChangedEvent("StringProperty"); }
        }

        public MyDataSource()
        {
            StringProperty = "Default BindingExpression String";            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
