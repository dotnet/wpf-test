// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Master detail scenario
	/// </description>
	/// </summary>
    [Test(0, "Views", "MasterDetail")]
	public class MasterDetail : XamlTest
	{
		ListBox _leagueLB;
		ListBox _divisionLB;
		ListBox _teamLB;
		
		public MasterDetail() : base(@"MasterDetail.xaml")
		{
			InitializeSteps += new TestStep (init);
			RunSteps += new TestStep(DefaultSelectionCheck);
			RunSteps += new TestStep(initRetainSelectionCheck);
			RunSteps += new TestStep(RetainSelectionCheck);
		}

		private TestResult init ()
		{
			Status("init");


			WaitForPriority (DispatcherPriority.Background);
			DockPanel dp = ((DockPanel)((StackPanel)RootElement).Children[0]);
			_leagueLB = LogicalTreeHelper.FindLogicalNode (dp, "leagueLB") as ListBox;
			_divisionLB = LogicalTreeHelper.FindLogicalNode (dp, "divisionLB") as ListBox;
			_teamLB = LogicalTreeHelper.FindLogicalNode (dp, "teamLB") as ListBox;
			WaitForPriority (DispatcherPriority.Render);

			LogComment("init was successful");
			return TestResult.Pass;
		}

		private TestResult DefaultSelectionCheck()
		{
			Status("DefaultSelectionCheck");

			//Move select and render
			_leagueLB.SelectedIndex = 1;
			WaitForPriority (DispatcherPriority.Background);

			//get current selection on sub item
			Team t = _teamLB.SelectedItem as Team;
			//validate
			if (t.Name != "Atlanta")
			{
				LogComment ("Team selection was incorrect, Should be 'Atlanta'");
				return TestResult.Fail;
			}

			LogComment("DefaultSelectionCheck was successful");
			return TestResult.Pass;
		}
		private TestResult initRetainSelectionCheck()
		{
			Status("initRetainSelectionCheck");

			//select last team
			_teamLB.SelectedIndex = 4;

			//Changing leagues
			_leagueLB.SelectedIndex = 0;
			WaitForPriority (DispatcherPriority.Background);

			//making sure teams really changed
			Team t = _teamLB.SelectedItem as Team;
			if (t.Name != "Baltimore")
			{
				LogComment ("Team selection was incorrect, Should be 'Baltimore'");
				return TestResult.Fail;
			}

			LogComment("initRetainSelectionCheck was successful");
			return TestResult.Pass;
		}


		private TestResult RetainSelectionCheck()
		{
			Status("RetainSelectionCheck");

			_leagueLB.SelectedIndex = 1;
			WaitForPriority (DispatcherPriority.Background);
			Team t = _teamLB.SelectedItem as Team;

			if (t.Name != "Philadelphia")
			{
				LogComment ("Team selection was incorrect, Should be 'Philadelphia'");
				return TestResult.Fail;
			}

			WaitFor (2000);
			LogComment("RetainSelectionCheck was successful");
			return TestResult.Pass;
		}

	}

    #region DataSource
    public class LeagueHierarchicalList : ObservableCollection<LeagueHierarchical>
    {
        public LeagueHierarchicalList()
            : base()
        {
            LeagueHierarchical l;
            DivisionHierarchical d;

            Add(l = new LeagueHierarchical("American League"));
            l.Divisions.Add((d = new DivisionHierarchical("East")));
            d.Teams.Add(new Team("Baltimore"));
            d.Teams.Add(new Team("Boston"));
            d.Teams.Add(new Team("New York"));
            d.Teams.Add(new Team("Tampa Bay"));
            d.Teams.Add(new Team("Toronto"));
            l.Divisions.Add((d = new DivisionHierarchical("Central")));
            d.Teams.Add(new Team("Chicago"));
            d.Teams.Add(new Team("Cleveland"));
            d.Teams.Add(new Team("Detroit"));
            d.Teams.Add(new Team("Kansas City"));
            d.Teams.Add(new Team("Minnesota"));
            l.Divisions.Add((d = new DivisionHierarchical("West")));
            d.Teams.Add(new Team("Anaheim"));
            d.Teams.Add(new Team("Oakland"));
            d.Teams.Add(new Team("Seattle"));
            d.Teams.Add(new Team("Texas"));
            Add(l = new LeagueHierarchical("National League"));
            l.Divisions.Add((d = new DivisionHierarchical("East")));
            d.Teams.Add(new Team("Atlanta"));
            d.Teams.Add(new Team("Florida"));
            d.Teams.Add(new Team("Montreal"));
            d.Teams.Add(new Team("New York"));
            d.Teams.Add(new Team("Philadelphia"));
            l.Divisions.Add((d = new DivisionHierarchical("Central")));
            d.Teams.Add(new Team("Chicago"));
            d.Teams.Add(new Team("Cincinnati"));
            d.Teams.Add(new Team("Houston"));
            d.Teams.Add(new Team("Milwaukee"));
            d.Teams.Add(new Team("Pittsburgh"));
            d.Teams.Add(new Team("St. Louis"));
            l.Divisions.Add((d = new DivisionHierarchical("West")));
            d.Teams.Add(new Team("Arizona"));
            d.Teams.Add(new Team("Colorado"));
            d.Teams.Add(new Team("Los Angeles"));
            d.Teams.Add(new Team("San Diego"));
            d.Teams.Add(new Team("San Francisco"));
        }
    }

    public class LeagueHierarchical
    {
        public LeagueHierarchical(string name)
        {
            _name = name;
            _divisions = new DivisionList();
        }

        string _name;

        public string Name { get { return _name; } }

        DivisionList _divisions;

        public DivisionList Divisions { get { return _divisions; } }
    }

    public class DivisionList : ObservableCollection<DivisionHierarchical>
    {
        public DivisionList()
            : base()
        {
        }
    }

    public class DivisionHierarchical
    {
        public DivisionHierarchical(string name)
        {
            _name = name;
            _teams = new TeamList();
        }

        string _name;

        public string Name { get { return _name; } }

        TeamList _teams;

        public TeamList Teams { get { return _teams; } }
    }

    public class TeamList : ObservableCollection<Team>
    {
        public TeamList()
            : base()
        {
        }
    }

     //Team is defined in mlb.cs, in common
    #endregion DataSource
}

