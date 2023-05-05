// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Forms;


namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// Dwarfs datasource
	/// </summary>
	public class DwarfCollection : ObservableCollection<Dwarf>
	{
		public DwarfCollection()
		{
		}

		public Dwarf this[string name]
		{
			get
			{
				foreach (Dwarf dwarf in this)
				{
					if (dwarf.Name == name)
						return dwarf;
				}

				return null;
			}
		}

		public Dwarf this[string name, Color skinColor]
		{
			get
			{
				foreach (Dwarf dwarf in this)
				{
					if (dwarf.Name == name && dwarf.SkinColor == skinColor)
						return dwarf;
				}

				return null;
			}
		}
        public Dwarf this[string name, string eyeColor]
        {
            get
            {
                foreach (Dwarf dwarf in this)
                {
                    if (dwarf.Name == name && dwarf.EyeColor == eyeColor)
                        return dwarf;
                }

                return null;
            }
        }

	}

    public class DwarfBuddies : ObservableCollection<Dwarf>
    {
        public DwarfBuddies()
        {
            Dwarf dwarf;
            dwarf = new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);
            dwarf.Buddies.Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false));
            dwarf.Buddies.Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(5, 1), true));
            dwarf.Buddies.Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(7, 3), false));
            dwarf.Buddies.Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));
            dwarf.Buddies.Add(new Dwarf("Dopey", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
            dwarf.Buddies.Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));
            Add(dwarf);
        }
    }


	public class Dwarf : INotifyPropertyChanged
	{
		string _name,_eyecolor;
		int _height,_width;
		Color _skincolor;
		bool _angry = true;
		Dwarf _friend;
		Point _position = new Point(0, 0);
		DwarfCollection _buddies;

		//Default Constructor For Markup
		public Dwarf(){
		}

		public Dwarf(string Name, string EyeColor, int Height, int Width, Color SkinColor, Point Position, bool Angry)
		{
			this._name = Name;
			this._eyecolor = EyeColor;
			this._height = Height;
			this._width = Width;
			this._skincolor = SkinColor;
			this._position = Position;
			this._angry = Angry;
			_buddies = new DwarfCollection();
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

		public string EyeColor
		{
			get { return _eyecolor; }
			set
			{
				_eyecolor = value;
				RaisePropertyChangedEvent("EyeColor");
			}
		}

		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				RaisePropertyChangedEvent("Height");
			}
		}

		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				RaisePropertyChangedEvent("Width");
			}
		}

		public Color SkinColor
		{
			get { return _skincolor; }
			set
			{
				_skincolor = value;
				RaisePropertyChangedEvent("SkinColor");
			}
		}

		public Point Position
		{
			get { return _position; }
			set
			{
				_position = value;
				RaisePropertyChangedEvent("Position");
			}
		}

		public bool Angry
		{
			get { return _angry; }
			set
			{
				_angry = value;
				RaisePropertyChangedEvent("Angry");
			}
		}

		public Dwarf Friend
		{
			get { return _friend; }
			set
			{
				_friend = value;
				RaisePropertyChangedEvent("Friend");
			}
		}

		public DwarfCollection Buddies
		{
			get { return _buddies; }
		}

		//property changed
		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChangedEvent(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}


	public class Human
	{
		string _name;

		//Default Constructor For Markup
		public Human(){
		}


		public Human(string Name)
		{
			this._name = Name;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				if (NameChanged != null)
					NameChanged(this, new EventArgs());
			}
		}

		public event EventHandler NameChanged;

	}
}
