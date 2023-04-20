// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading; using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests the scenario where there's a ListBox inside the ItemTemplate of another ListBox.
	/// Allows to display a collection of collections.
	/// </description>
	/// </summary>

    [Test(3, "Controls", "ListBoxInsideListBox")]
	public class ListBoxInsideListBox : XamlTest
	{
		private ListBox _lb1;
		private ObjectDataProvider _ods;
		private ICollectionView _cv;

		public ListBoxInsideListBox(): base(@"ListBoxInsideListBox.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(VerifyMainCollection);
			RunSteps += new TestStep(VerifySubCollections);
			RunSteps += new TestStep(VerifyItems);
		}

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_lb1 = Util.FindElement(RootElement, "lb1") as ListBox;
			if (_lb1 == null)
			{
				LogComment("Fail - Unable to reference lb1 element (ListBox)");
				return TestResult.Fail;
			}
			_ods = RootElement.Resources["ods"] as ObjectDataProvider;
			if (_ods == null)
			{
				LogComment("Fail - Unable to reference ObjectDataProvider ods");
				return TestResult.Fail;
			}
			LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyMainCollection()
		{
			Status("VerifyMainCollection");

			if (_lb1.ItemsSource == null || _lb1.ItemsSource == DependencyProperty.UnsetValue)
			{
				LogComment("Fail - main collection");
				return TestResult.Fail;
			}

			LogComment("VerifyMainCollection was successful");
			return TestResult.Pass;
		}

        private TestResult VerifySubCollections()
		{
			Status("VerifySubCollections");

            _cv = _lb1.Items;
            if (_cv == null)
			{
				LogComment("Fail - ItemCollection CollectionView is null");
				return TestResult.Fail;
			}
			// 4 sub-collections
			int expectedCount = 4;
			int actualCount = ((CollectionView)_cv).Count;
			if (expectedCount != actualCount)
			{
				LogComment("Fail - Expected count:" + expectedCount + " Actual:" + actualCount + " - subcollections");
				return TestResult.Fail;
			}

			LogComment("VerifySubCollections was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyItems()
		{
			Status("VerifyItems");

            ObservableCollection<ListBoxPlaces> aldc = _ods.Data as ObservableCollection<ListBoxPlaces>;
            ObservableCollection<ListBoxPlace> sourceCollection = aldc[0] as ObservableCollection<ListBoxPlace>;

            IEnumerable ie2 = ((IEnumerable)_cv);
			// 11 items in each sub-collection
			foreach (ListBoxPlaces places in ie2)
			{
				int expectedCountItems = 11;
				int actualCountItems = places.Count;
				if (expectedCountItems != actualCountItems)
				{
					LogComment("Fail - Expected count:" + expectedCountItems + " Actual:" + actualCountItems + " - items");
					return TestResult.Fail;
				}
				for (int i = 0; i < sourceCollection.Count; i++)
				{
					string expectedPlaceName = ((ListBoxPlace)sourceCollection[i]).Name;
					string actualPlaceName = ((ListBoxPlace)places[i]).Name;
					if (expectedPlaceName != actualPlaceName)
					{
						LogComment("Fail - Expected place:" + expectedPlaceName + " Actual:" + actualPlaceName);
						return TestResult.Fail;
					}
				}
			}

			LogComment("VerifyItems was successful");
			return TestResult.Pass;
		}
	}

	#region DataSource
    public class ListOfPlaces : ObservableCollection<ListBoxPlaces>
    {
		public ListOfPlaces()
		{
			Add(new ListBoxPlaces());
			Add(new ListBoxPlaces());
			Add(new ListBoxPlaces());
			Add(new ListBoxPlaces());
		}
	}

    public class ListBoxPlaces : ObservableCollection<ListBoxPlace>
    {
		public ListBoxPlaces()
		{
			Add(new ListBoxPlace("Seattle", "WA"));
			Add(new ListBoxPlace("Redmond", "WA"));
			Add(new ListBoxPlace("Bellevue", "WA"));
			Add(new ListBoxPlace("Kirkland", "WA"));
			Add(new ListBoxPlace("Portland", "OR"));
			Add(new ListBoxPlace("San Francisco", "CA"));
			Add(new ListBoxPlace("Los Angeles", "CA"));
			Add(new ListBoxPlace("San Diego", "CA"));
			Add(new ListBoxPlace("San Jose", "CA"));
			Add(new ListBoxPlace("Santa Ana", "CA"));
			Add(new ListBoxPlace("Bellingham", "WA"));
		}
	}

	public class ListBoxPlace
	{
		private string _name;

		private string _state;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		public ListBoxPlace()
		{
			this._name = "";
			this._state = "";
		}

		public ListBoxPlace(string name, string state)
		{
			this._name = name;
			this._state = state;
		}
	}
	#endregion
}
