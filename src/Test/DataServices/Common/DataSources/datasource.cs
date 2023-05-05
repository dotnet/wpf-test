// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using System.Collections;
using System.Reflection;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
namespace Microsoft.Test.DataServices
{
    public class CLRBook : INotifyPropertyChanged
	{
		public CLRBook(string title, string author, int year, double price, BookType type)
		{
			_title = title;
			_author = author;
			_year = year;
			_price = price;
			_type = type;
		}

		public string Title
		{
			get { return _title; }
            set { _title = value; RaisePropertyChangedEvent("Title"); }
		}

        public string Author
        {
            get { return _author; }
            set { _author = value; RaisePropertyChangedEvent("Author"); }
        }
		public int Year
		{
			get { return _year; }
            set { _year = value; RaisePropertyChangedEvent("Year"); }
		}

		public double Price
		{
			get { return _price; }
            set { _price = value; RaisePropertyChangedEvent("Price"); }
		}

		public BookType BookType
		{
			get { return _type; }
            set { _type = value; RaisePropertyChangedEvent("BookType"); }
        }

		private string      _title;
		private string      _author;
		private int         _year;
		private double      _price;
		private BookType    _type;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyname));
            }
        }

    
    }
	
	public enum BookType
	{
		Novel,
		Lyrics,
		Fiction,
		Reference
	}


	#region Filter Routine
	public class MyFilter
	{
		public MyFilter(BookType typeToFilter)
		{
			_typeToFilter = typeToFilter;
		}

        // delegate DataListFilterCallback
        public bool Contains(object item)
        {
			CLRBook book = (CLRBook) item;
			return (book.BookType == _typeToFilter);
		}


		BookType _typeToFilter;
	}
	#endregion Filter Routine

	
	public class SuperSort : IComparer
	{
		int _direction = 1;

		public SuperSort (ListSortDirection direction)
		{
			if (direction == ListSortDirection.Descending)
				this._direction = -1;
		}

		public int Compare (object o1, object o2)
		{
			return _direction * Comparer.Default.Compare (o1, o2);
		}
	}

	public class SuperFilter
	{
		public SuperFilter (string compareType, object FilterValue)
		{
			_compareType = compareType;
			_FilterValue = FilterValue;
		}

        // delegate DataListFilterCallback
        public bool Contains(object item)
        {
			if (_compareType == "=")
				return Comparer.Default.Compare (item, _FilterValue) == 0;
			else if (_compareType == ">")
				return Comparer.Default.Compare (item, _FilterValue) > 0;
			else if (_compareType == "<")
				return Comparer.Default.Compare (item, _FilterValue) < 0;
			else
				return false;
		}

		object _FilterValue;
		string _compareType;
	}

		#region SortRoutine
	public class MySort : IComparer
	{
		public MySort(Type type, string propertyName)
		{
			
			_pi = type.GetProperty(propertyName);
		}

		public int Compare(object o1, object o2)
		{
			object v1 = _pi.GetValue(o1, null);
			object v2 = _pi.GetValue(o2, null);
			int result = Comparer.Default.Compare(v1,v2);
			return result;
		}



		private PropertyInfo _pi;
	}
	#endregion Sort Routine


}
