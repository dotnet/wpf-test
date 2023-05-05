// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    public class Order : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private int _number;
        [ComparableByValue()]
        public int Number
        {
            get
            {
                return this._number;
            }
            set
            {
                if (value != this._number)
                {
                    this._number = value;
                    NotifyPropertyChanged("Number");
                }
            }
        }
        private DateTime _time;
        [ComparableByValue()]
        public DateTime Time
        {
            get
            {
                return this._time;
            }
            set
            {
                if (value != this._time)
                {
                    this._time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }
    }

}

