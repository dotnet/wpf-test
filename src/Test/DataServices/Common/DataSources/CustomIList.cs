// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;
using System.Collections;

namespace Microsoft.Test.DataServices
{
    // This data can be used as the source of a TreeView.
    public class HierarchicalCustomIList : CustomIList
    {
        public HierarchicalCustomIList()
        {
            CustomObject co1 = new CustomObject();
            co1.Name = "item1";
            co1.CustomList.Add("item11");
            co1.CustomList.Add("item12");
            CustomObject co2 = new CustomObject();
            co2.Name = "item2";
            co2.CustomList.Add("item21");
            co2.CustomList.Add("item22");
            CustomObject co3 = new CustomObject();
            co3.Name = "item3";
            co3.CustomList.Add("item31");
            co3.CustomList.Add("item32");
            CustomObject co4 = new CustomObject();
            co4.Name = "item4";
            co4.CustomList.Add("item41");
            co4.CustomList.Add("item42");

            this.Add(co1);
            this.Add(co2);
            this.Add(co3);
            this.Add(co4);
        }
    }

    // This is flat data. It can be used as the source of a ListBox.
    public class NonHierarchicalCustomIList : CustomIList
    {
        public NonHierarchicalCustomIList()
        {
            this.Add("item1");
            this.Add("item2");
            this.Add("item3");
            this.Add("item4");
        }
    }

    // Used to help get a hierarchy of data for the TreeView scenario.
    public class CustomObject
    {
        private CustomIList _customList;

        public CustomIList CustomList
        {
            get { return _customList; }
            set { _customList = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public CustomObject()
        {
            _customList = new CustomIList();
        }
    }

    // Counts the number of times Count, Contains and IndexOf are called. Used to test 
    // that these are not called unnecessarily to provide coverage for perf improvements.
    public class CustomIList : IList
    {
        private ArrayList _list;

        public CustomIList()
        {
            _list = new ArrayList();
        }

        // Number of times Count is accessed
        private int _numCount;

        public int NumCount
        {
            get { return _numCount; }
            set { _numCount = value; }
        }

        // Number of times Contains is accessed
        private int _numContains;

        public int NumContains
        {
            get { return _numContains; }
            set { _numContains = value; }
        }

        // Number of times IndexOf is accesssed
        private int _numIndexOf;

        public int NumIndexOf
        {
            get { return _numIndexOf; }
            set { _numIndexOf = value; }
        }

        #region IList Members

        public int Add(object value)
        {
            return _list.Add(value);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(object value)
        {
            this._numContains++;
            return _list.Contains(value);
        }

        public int IndexOf(object value)
        {
            this._numIndexOf++;
            return _list.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            _list.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return _list.IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        public void Remove(object value)
        {
            _list.Remove(value);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            _list.CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                this._numCount++;
                return _list.Count;
            }
        }

        public bool IsSynchronized
        {
            get { return _list.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _list.SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
