// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
 	#region DataSource 
	
	public class ListLeagueList : List<League> {
		public ListLeagueList()
		{
			League l;
			Division d;

			Add(l = new League("American League"));
			l.Divisions.Add((d = new Division("East")));
			d.Teams.Add(new Team("Baltimore"));
			d.Teams.Add(new Team("Boston"));
			d.Teams.Add(new Team("New York"));
			d.Teams.Add(new Team("Tampa Bay"));
			d.Teams.Add(new Team("Toronto"));
			l.Divisions.Add((d = new Division("Central")));
			d.Teams.Add(new Team("Chicago"));
			d.Teams.Add(new Team("Cleveland"));
			d.Teams.Add(new Team("Detroit"));
			d.Teams.Add(new Team("Kansas City"));
			d.Teams.Add(new Team("Minnesota"));
			l.Divisions.Add((d = new Division("West")));
			d.Teams.Add(new Team("Anaheim"));
			d.Teams.Add(new Team("Oakland"));
			d.Teams.Add(new Team("Seattle"));
			d.Teams.Add(new Team("Texas"));
			Add(l = new League("National League"));
			l.Divisions.Add((d = new Division("East")));
			d.Teams.Add(new Team("Atlanta"));
			d.Teams.Add(new Team("Florida"));
			d.Teams.Add(new Team("Montreal"));
			d.Teams.Add(new Team("New York"));
			d.Teams.Add(new Team("Philadelphia"));
			l.Divisions.Add((d = new Division("Central")));
			d.Teams.Add(new Team("Chicago"));
			d.Teams.Add(new Team("Cincinnati"));
			d.Teams.Add(new Team("Houston"));
			d.Teams.Add(new Team("Milwaukee"));
			d.Teams.Add(new Team("Pittsburgh"));
			d.Teams.Add(new Team("St. Louis"));
			l.Divisions.Add((d = new Division("West")));
			d.Teams.Add(new Team("Arizona"));
			d.Teams.Add(new Team("Colorado"));
			d.Teams.Add(new Team("Los Angeles"));
			d.Teams.Add(new Team("San Diego"));
			d.Teams.Add(new Team("San Francisco"));
		}

		public League this[string name]
		{
			get
			{
				foreach (League l in this)
					if (l.Name == name)
						return l;

				return null;
			}
		}


	}
	
	public class LeagueList2 : ObservableCollection<League>
	{
		public LeagueList2() : base()
		{

			League l;
			Division d;

			Add(l = new League("American League"));
			l.Divisions.Add((d = new Division("East")));
			d.Teams.Add(new Team("Baltimore"));
			d.Teams.Add(new Team("Boston"));
			d.Teams.Add(new Team("New York"));
			d.Teams.Add(new Team("Tampa Bay"));
			d.Teams.Add(new Team("Toronto"));
			l.Divisions.Add((d = new Division("Central")));
			d.Teams.Add(new Team("Chicago"));
			d.Teams.Add(new Team("Cleveland"));
			d.Teams.Add(new Team("Detroit"));
			d.Teams.Add(new Team("Kansas City"));
			d.Teams.Add(new Team("Minnesota"));
			l.Divisions.Add((d = new Division("West")));
			d.Teams.Add(new Team("Anaheim"));
			d.Teams.Add(new Team("Oakland"));
			d.Teams.Add(new Team("Seattle"));
			d.Teams.Add(new Team("Texas"));
			Add(l = new League("National League"));
			l.Divisions.Add((d = new Division("East")));
			d.Teams.Add(new Team("Atlanta"));
			d.Teams.Add(new Team("Florida"));
			d.Teams.Add(new Team("Montreal"));
			d.Teams.Add(new Team("New York"));
			d.Teams.Add(new Team("Philadelphia"));
			l.Divisions.Add((d = new Division("Central")));
			d.Teams.Add(new Team("Chicago"));
			d.Teams.Add(new Team("Cincinnati"));
			d.Teams.Add(new Team("Houston"));
			d.Teams.Add(new Team("Milwaukee"));
			d.Teams.Add(new Team("Pittsburgh"));
			d.Teams.Add(new Team("St. Louis"));
			l.Divisions.Add((d = new Division("West")));
			d.Teams.Add(new Team("Arizona"));
			d.Teams.Add(new Team("Colorado"));
			d.Teams.Add(new Team("Los Angeles"));
			d.Teams.Add(new Team("San Diego"));
			d.Teams.Add(new Team("San Francisco"));
		}
		

		public new League this[int index] {
			get { return (League)base[index]; }
			set { base[index] = value; }
		}
	}

    public class League : INotifyPropertyChanged
	{
		public League(string name)
		{
			_name = name;
			_divisions = new List<Division>();
		}


		string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("Name");
            }
        }

		List<Division> _divisions;
		public List<Division> Divisions { get { return _divisions; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
	}
    public class Division : INotifyPropertyChanged
	{
		public Division(string name)
		{
			_name = name;
			_teams = new List<Team>();

		}

		string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("Name");
            }
        }

		List<Team> _teams;

		public List<Team> Teams { get { return _teams; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
	}

	public class Team
	{
		public Team(string name)
		{
			_name = name;
		}

		string _name;

		public string Name { get { return _name; } }
	}
	#endregion DataSource


}

