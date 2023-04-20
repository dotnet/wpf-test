// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Test.DataServices
{
    #region Sample Record

    public class SampleRecord : System.ComponentModel.INotifyPropertyChanged
    {
        public SampleRecord(int id)
        {
            _id = id;
            _name = "Record " + id.ToString();
        }

        public SampleRecord() : this(0) { }

        public SampleRecord(string id) : this(Int32.Parse(id)) { }

        private int _id;

        public int IntField
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChangedEvent("IntField");
            }
        }

        private string _name;

        public string StringField
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("StringField");
            }
        }

        // INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }
    }

    #endregion

    #region SampleDataSet - Not ADO DataSet

    /// <summary>
    /// This is not an ADO DataSet
    /// </summary>
    public class SampleDataSet : ObservableCollection<SampleRecord>
    {
        public SampleDataSet()
        {
            for (int i = 0; i < 5; ++i)
            {
                Add(new SampleRecord(i));
            }
        }
    }

    #endregion

    #region SlowRecord

    /// <summary>
    /// Thread.Sleep for 1000ms in constructor
    /// </summary>
    public class SlowRecord : System.ComponentModel.INotifyPropertyChanged
    {
        public SlowRecord(int id)
        {
            System.Threading.Thread.Sleep(1000);
            _id = id;
            _name = "Record " + id.ToString();
        }

        public SlowRecord() : this(0) { }

        public SlowRecord(string id) : this(Int32.Parse(id)) { }

        private int _id;

        public int IntField
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChangedEvent("IntField");
            }
        }

        private string _name;

        public string StringField
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("StringField");
            }
        }

        // INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

    }

    #endregion

}
