// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
using System.Globalization;
using System.Collections;
using System.Windows.Controls;


namespace Microsoft.Test.DataServices
{
    #region DataSource


    public class SortDataItems : ObservableCollection<SortItem>
    {
        public SortDataItems()
        {
			Add(new SortItem("al:tinda", 100));
			Add(new SortItem("ch:aque", 50));
			Add(new SortItem("Cz:ech", 150));
			Add(new SortItem("co:te", 25));
			Add(new SortItem("hi:zli", 200));
			Add(new SortItem("i:erigiyle", 10));
        }
    }


    public class SortItem
    {
        public SortItem() : this("bindvalue", 0)
        {
        }

        public SortItem(string name) : this(name, 0) { }

        public SortItem(string name, double top)
        {
            _name = name;
            _top = top;
        }

        public string Name { get { return _name; } set { _name = value; } }

        public double Top { get { return _top; } set { _top = value; } }

        string _name;

        double _top = 0;
    }

    public class SortDataItems1 : ObservableCollection<SortItem>
    {
        public SortDataItems1()
        {
            Add(new SortItem("test1", 1));
            Add(new SortItem("test2", 2));
            Add(new SortItem("test3", 3));
            Add(new SortItem("test4", 4));
            Add(new SortItem("test5", 5));
        }
    }



    #endregion DataSource
}

