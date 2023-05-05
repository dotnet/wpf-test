// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where CollectionView sorting is silently discarded
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "CollectionViewSort")]
    public class CollectionViewSort : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListView _myListView;        
        
        #endregion

        #region Constructors

        public CollectionViewSort()
            : base(@"CollectionViewSort.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListView = (ListView)RootElement.FindName("myListView");

            if (_myStackPanel == null || _myListView == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Set the DataContext for the list and sort by sport
            List<Person1> people = Person1.GetPeople(10);
            _myListView.DataContext = people;
            CollectionViewSource.GetDefaultView(people).SortDescriptions.Add(new SortDescription("Sport", ListSortDirection.Ascending));
            
            Person1 first = (Person1)_myListView.Items[0];
            Person1 last = (Person1)_myListView.Items[9];
            
            // Verify 
            if (first.Sport[0] > last.Sport[0])
            {
                LogComment("The ListView was not sorted correctly by sport");                
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class Person1
    {
        public override string ToString()
        {
            return String.Format("Name={0}, Favourite Color={1}, Sport={2}, Birth Year={3}, Id={4}.",
                this.Name, this._favoriteColor, this.Sport, this.BirthYear, this.Id);
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _favoriteColor;

        public string FavoriteColor
        {
            get { return _favoriteColor; }
            set { _favoriteColor = value; }
        }
        private string _sport;

        public string Sport
        {
            get { return _sport; }
            set { _sport = value; }
        }
        private int _birthYear;

        public int BirthYear
        {
            get { return _birthYear; }
            set { _birthYear = value; }
        }

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }


        public Person1(string name, string favoriteColor, string sport, int birthYear, int id)
        {
            this._name = name;
            this._favoriteColor = favoriteColor;
            this._sport = sport;
            this._birthYear = birthYear;
            this._id = id;
        }

        private static string[] s_names = new string[] { "John", "Mark", "Judy", "Carl", "Steve", "Mary", "Gabriel", "Yohan", "Esteban", "Gary" };
        private static string[] s_colors = new string[] { "Red", "Green" };//, "Black", "Yellow", "Brown", "Purple", "White", "Orange" };
        private static string[] s_sports = new string[] { "Baseball", "Soccer" };//, "Basketball", "Swimming", "Running", "Tennis", "Cycling" };
        private static int[] s_years = new int[] { 1971, 1999 };//, 2006, 1912, 1897, 1911, 1983, 1964, 1937 };
        private static Random s_rand = new Random(314159);

        public static List<Person1> GetPeople(int number)
        {
            List<Person1> returnValue = new List<Person1>();
            for (int i = 1; i <= number; i++)
            {
                returnValue.Add(
                    new Person1(
                        s_names[s_rand.Next(s_names.Length)],
                        s_colors[s_rand.Next(s_colors.Length)],
                        s_sports[s_rand.Next(s_sports.Length)],
                        s_years[s_rand.Next(s_years.Length)],
                        i));
            }
            return returnValue;
        }
    }

    #endregion
}
