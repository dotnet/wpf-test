// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.Data;
using Microsoft.Test;

namespace Microsoft.Test.DataServices
{
	using System.ComponentModel;
	public class aString: INotifyPropertyChanged
	{
		string _name;

		public aString(string Name)
		{
			this._name = Name;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChangedEvent("Name");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChangedEvent(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}

	public class myStrings: ObservableCollection<aString>
	{
		public myStrings()
		{
			for (int x = 0; x < 5; x++)
			{
				Add(new aString("hello " + x));
			}
		}
	}

	public class Dwarves: ObservableCollection<Dwarf>
	{
		public Dwarves()
		{
			Add(new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true));
			Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false));
			Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(5, 1), true));
			Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(7, 3), false));
			Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));
			Add(new Dwarf("Sneezy", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
			Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));
		}
	}

	public class GreekKing : Dwarf
	{
		public GreekKing()
		{
			this.Name = "Oddysseus";
		}
	}


	public class MyStar
	{
		private string _firstName;

		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		private string _lastName;

		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		private int _age;
		public int Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public MyStar(string firstName, string lastName, int age)
		{
			this._firstName = firstName;
			this._lastName = lastName;
			this._age = age;
		}

		public MyStar()
		{
			this._firstName = "";
			this._lastName = "";
			this._age = 0;
		}
	}


    #region Static Property

    //Static Property used for DataContext case
    public class StaticProperty 
    {
        public StaticProperty()
        {
            s_stringProperty = "Howdy?";
        }

        private static string s_stringProperty;
        
        public static string StringProperty
        {
            get { 
                return (s_stringProperty != null) ? s_stringProperty : "Howdy!"; }
            set { s_stringProperty = value; }
        }

    }

    #endregion

}
