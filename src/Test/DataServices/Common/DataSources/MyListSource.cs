// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;


namespace Microsoft.Test.DataServices
{
	public class MyListSource :IListSource
	{
		private ArrayList _myList;
		public ArrayList MyList
		{
			get { return _myList; }
			set { _myList = value; }
		}

		private bool _containsListCollection;
		public bool ContainsListCollection
		{
			get { return _containsListCollection; }
			set { _containsListCollection = value; }
		}

		public MyListSource()
		{
			MyList = new ArrayList();
			MyList.Add(1);
			MyList.Add(2);
			MyList.Add(3);
			MyList.Add(4);
			MyList.Add(5);
			MyList.Add(6);
			MyList.Add(7);
			_containsListCollection = false;
		}

		public IList GetList()
		{
			return MyList;
		}
	}
}
