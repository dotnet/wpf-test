// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Timers;
using System.Threading; using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{

    public class SlowDataItem : System.ComponentModel.INotifyPropertyChanged
    {

        #region Constructors

        public SlowDataItem()
        {
            _fastvalue = "This is fast!";
            _slowvalue = "This is slow!";
        }

        #endregion

        #region Public Properties
        public string SlowValue
        {
            get
            {
                Thread.Sleep(2000);
                return _slowvalue;
            }
            set
            {
                _slowvalue = value;
                RaisePropertyChangedEvent("SlowValue");
            }
        }

        public string FastValue
        {
            get { return _fastvalue; }
            set
            {
                _fastvalue = value;
                RaisePropertyChangedEvent("FastValue");
            }
        }

        public string SlowValueEndResult
        {
            get { return _slowvalue; }
        }

        #endregion

        #region INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }


        #endregion

        #region Private Data Members

        private string _slowvalue;
        private string _fastvalue;

        #endregion
    }
}
