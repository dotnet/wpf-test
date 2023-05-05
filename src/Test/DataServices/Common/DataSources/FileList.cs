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
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;

namespace Microsoft.Test.DataServices
{
	public class FileItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		public FileItem(string name, int size, string modTime)
		{
			int k = name.LastIndexOf(".");

			if (k < 0)
			{
				_name = name;
				_extension = String.Empty;
			}
			else
			{
				_name = name.Substring(0, k);
				_extension = name.Substring(k + 1);
			}

			_size = size;
			_dateModified = DateTime.Parse(modTime, new CultureInfo("en-us"));
		}

		string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; RaisePropertyChanged("Name"); }
		}

		string _extension;

		public string Extension
		{
			get { return _extension; }
			set { _extension = value; RaisePropertyChanged("Extension"); }
		}

		int _size;

		public int Size
		{
			get { return _size; }
			set { _size = value; RaisePropertyChanged("Size"); }
		}

		DateTime _dateModified;

		public DateTime DateModified
		{
			get { return _dateModified; }
			set { _dateModified = value; RaisePropertyChanged("DateModified"); }
		}
	}


	public class FileList : ObservableCollection<FileItem>
	{
		public FileList() : base()
		{
			Add(new FileItem("file1.cs", 2172, "12/8/03"));
			Add(new FileItem("file2.cs", 2172, "12/6/03"));
			Add(new FileItem("file3.cs", 2172, "12/7/03"));
			Add(new FileItem("file5.hxx", 1711, "12/9/03"));
			Add(new FileItem("file5.xml", 1711, "12/4/03"));
			Add(new FileItem("file6.xml", 1711, "12/4/03"));
			Add(new FileItem("file5.ixx", 1711, "12/4/03"));
			Add(new FileItem("file6.ixx", 1711, "12/3/03")); 
			
		}
	}
}
