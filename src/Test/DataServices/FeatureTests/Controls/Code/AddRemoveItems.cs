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
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using System.Reflection;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This test exercises ItemsControl by adding, removing, inserting and altering its items. It 
	/// also includes code to alter the data source and swap elements.
	/// The xaml file for this test has 3 listboxes: one bound to a collection using 
	/// ItemsSource, another bound to a collection using compound-property syntax and
	/// another one with CLR objects explicitly added to it.
	/// </description>
	/// </summary>



    [Test(0, "Controls", "AddRemoveItems", Keywords = "MicroSuite")]
	public class AddRemoveItems : XamlTest
	{
		ListBox _lb1;
		ListBox _lb2;
		ListBox _lb3;

        public AddRemoveItems()
            : base(@"AddRemoveItems.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(AddElements);
            RunSteps += new TestStep(RemoveElements);
            RunSteps += new TestStep(AlterSource);
            RunSteps += new TestStep(InsertElements);
            RunSteps += new TestStep(AlterElements);
			RunSteps += new TestStep(ReorderElements);
		}

		#region Setup
        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_lb1 = Util.FindElement(RootElement, "lb1") as ListBox;
			if (_lb1 == null)
			{
				LogComment("Fail - Unable to reference lb1 element (ListBox).");
				return TestResult.Fail;
			}

			_lb2 = Util.FindElement(RootElement, "lb2") as ListBox;
			if (_lb2 == null)
			{
				LogComment("Fail - Unable to reference lb2 element (ListBox).");
				return TestResult.Fail;
			}

			_lb3 = Util.FindElement(RootElement, "lb3") as ListBox;
			if (_lb3 == null)
			{
				LogComment("Fail - Unable to reference lb3 element (ListBox).");
				return TestResult.Fail;
			}

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AddElements
        private TestResult AddElements()
		{
			Status("AddElements");

			TestResult res1 = AddElementsLb1();
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

            TestResult res2 = AddElementsLb2();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

            TestResult res3 = AddElementsLb3();
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AddElements was successful");
			return TestResult.Pass;
		}

        private TestResult AddElementsLb1()
		{
			Status("AddElementsLb1");

			(_lb1.ItemsSource as IList).Add(new Person("Peter", "dutch"));

			int expectedCount = 11;
			if (expectedCount != _lb1.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb1.Items.Count + " - AddElements in lb1");
				return TestResult.Fail;
			}

			int index = 10;
			string expectedName = "Peter";
			string expectedNationality = "dutch";
            TestResult res = VerifyPerson(index, expectedName, expectedNationality, _lb1, "AddElements in lb1");
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AddElementsLb1 was successful");
			return TestResult.Pass;
		}

        private TestResult AddElementsLb2()
		{
			Status("AddElementsLb2");

			(_lb2.ItemsSource as IList).Add(new Place("Hood River", "OR"));

			int expectedCount = 12;
			if (expectedCount != _lb2.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb2.Items.Count + " - AddElements in lb2");
				return TestResult.Fail;
			}

			int index = 11;
			string expectedName = "Hood River";
			string expectedState = "OR";
            TestResult res = VerifyPlace(index, expectedName, expectedState, _lb2, "AddElements in lb2");
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AddElementsLb2 was successful");
			return TestResult.Pass;
		}

        private TestResult AddElementsLb3()
		{
			Status("AddElementsLb3");

			_lb3.Items.Add(new Place("Hood River", "OR"));

			int expectedCount = 3;
			if (expectedCount != _lb3.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb3.Items.Count + " - AddElements in lb3");
				return TestResult.Fail;
			}

			int index = 2;
			string expectedName = "Hood River";
			string expectedState = "OR";
            TestResult res = VerifyPlace(index, expectedName, expectedState, _lb3, "AddElements in lb3");
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AddElementsLb3 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region RemoveElements
        private TestResult RemoveElements()
		{
			Status("RemoveElements");

            TestResult res1 = RemoveElementsLb1();
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			TestResult res2 = RemoveElementsLb2();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			TestResult res3 = RemoveElementsLb3();
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("RemoveElements was successful");
			return TestResult.Pass;
        }

        private TestResult RemoveElementsLb1()
		{
			Status("RemoveElementsLb1");

			(_lb1.ItemsSource as IList).RemoveAt(0);

			int expectedCount = 10;
			if (expectedCount != _lb1.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb1.Items.Count + " - RemoveElements in lb1");
				return TestResult.Fail;
			}

			int index = 0;
			string expectedName = "Radu";
			string expectedNationality = "romanian";
            TestResult res = VerifyPerson(index, expectedName, expectedNationality, _lb1, "RemoveElements in lb1");
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("RemoveElementsLb1 was successful");
			return TestResult.Pass;
		}

        private TestResult RemoveElementsLb2()
		{
			Status("RemoveElementsLb2");

			CollectionContainer cc = (_lb2.ItemsSource as CompositeCollection)[0] as CollectionContainer;
			if (cc == null)
			{
				LogComment("Fail - CollectionContainer is null");
				return TestResult.Fail;
			}
            ObservableCollection<Place> aldc = cc.Collection as ObservableCollection<Place>;
            if (aldc == null)
			{
                LogComment("Fail - ObservableCollection<Places> is null");
                return TestResult.Fail;
			}
			aldc.RemoveAt(0);

			int expectedCount = 11;
			if (expectedCount != _lb2.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb2.Items.Count + " - RemoveElements in lb2");
				return TestResult.Fail;
			}

			int index = 0;
			string expectedName = "Redmond";
			string expectedState = "WA";
            TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb2, "RemoveElement in lb2");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("RemoveElementsLb2 was successful");
			return TestResult.Pass;
		}

        private TestResult RemoveElementsLb3()
		{
			Status("RemoveElementsLb3");

			_lb3.Items.RemoveAt(1);

			int expectedCount = 2;
			if (expectedCount != _lb3.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb3.Items.Count + " - RemoveElements in lb3");
				return TestResult.Fail;
			}

			int index = 1;
			string expectedName = "Hood River";
			string expectedState = "OR";
            TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb3, "RemoveElements in lb3");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("RemoveElementsLb3 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AlterSource
        private TestResult AlterSource()
		{
			Status("AlterSource");

            TestResult res1 = AlterSourceLb1();
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
            }

			TestResult res2 = AlterSourceLb2();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			//AlterSourceLb3 - lb3 does not use databinding

			LogComment("AlterSource was successful");
			return TestResult.Pass;
        }

        private TestResult AlterSourceLb1()
		{
			Status("AlterSourceLb1");

            ObservableCollection<Person> aldc = _lb1.ItemsSource as ObservableCollection<Person>;
            if (aldc == null)
			{
                LogComment("Fail - ObservableCollection<People> is null");
                return TestResult.Fail;
			}
			aldc[0] = new Person("Peter", "dutch");

			int expectedCount = 10;
			if (expectedCount != _lb1.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb1.Items.Count + " - AlterSource in lb1");
				return TestResult.Fail;
			}

			int index = 0;
			string expectedName = "Peter";
			string expectedNationality = "dutch";
            TestResult res2 = VerifyPerson(index, expectedName, expectedNationality, _lb1, "AlterElements in lb1");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AlterSourceLb1 was successful");
			return TestResult.Pass;
		}

        private TestResult AlterSourceLb2()
		{
			Status("AlterSourceLb2");

			CollectionContainer cc = (_lb2.ItemsSource as CompositeCollection)[0] as CollectionContainer;
			if (cc == null)
			{
				LogComment("Fail - CollectionContainer is null");
				return TestResult.Fail;
			}
            ObservableCollection<Place> aldc = cc.Collection as ObservableCollection<Place>;
            if (aldc == null)
			{
                LogComment("Fail - ObservableCollection<Places> is null");
                return TestResult.Fail;
			}
			aldc[1] = new Place("Eugene", "OR");

			int expectedCount = 11;
			if (expectedCount != _lb2.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb2.Items.Count + " - AlterSource in lb2");
				return TestResult.Fail;
			}

			int index = 1;
			string expectedName = "Eugene";
			string expectedState = "OR";
            TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb2, "AlterElements in lb2");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AlterSourceLb2 was successful");
			return TestResult.Pass;
		}

		#endregion

		#region InsertElements
        private TestResult InsertElements()
		{
			Status("InsertElements");

            TestResult res1 = InsertElementsLb1();
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
            TestResult res2 = InsertElementsLb2();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
            TestResult res3 = InsertElementsLb3();
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("InsertElements was successful");
			return TestResult.Pass;
		}

        private TestResult InsertElementsLb1()
		{
			Status("InsertElementsLb1");

			(_lb1.ItemsSource as IList).Insert(0, new Person("Manu", "french"));

			int expectedCount = 11;
			if (expectedCount != _lb1.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb1.Items.Count + " - InsertElements in lb1");
				return TestResult.Fail;
			}

			int index = 0;
			string expectedName = "Manu";
			string expectedNationality = "french";
            TestResult res = VerifyPerson(index, expectedName, expectedNationality, _lb1, "InsertElements in lb1");
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("InsertElementsLb1 was successful");
			return TestResult.Pass;
		}

        private TestResult InsertElementsLb2()
		{
			Status("InsertElementsLb2");

			(_lb2.ItemsSource as IList).Insert(1, new Place("Spokane", "WA"));

			int expectedCount = 12;
			if (expectedCount != _lb2.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb2.Items.Count + " - InsertElements in lb2");
				return TestResult.Fail;
			}

			int index = 10;
			string expectedName = "Spokane";
			string expectedState = "WA";
            TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb2, "InsertElements in lb2");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("InsertElementsLb2 was successful");
			return TestResult.Pass;
		}

        private TestResult InsertElementsLb3()
		{
			Status("InsertElementsLb3");

			_lb3.Items.Insert(1, new Place("Spokane", "WA"));

			int expectedCount = 3;
			if (expectedCount != _lb3.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb3.Items.Count + " - InsertElements in lb3");
				return TestResult.Fail;
			}

			int index = 1;
			string expectedName = "Spokane";
			string expectedState = "WA";
            TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb3, "InsertElements in lb3");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("InsertElementsLb3 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AlterElements
        private TestResult AlterElements()
		{
			Status("AlterElements");

            TestResult res1 = AlterElementsLb1();
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
            TestResult res2 = AlterElementsLb2();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
            TestResult res3 = AlterElementsLb3();
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AlterElements was successful");
			return TestResult.Pass;
		}

        private TestResult AlterElementsLb1()
		{
			Status("AlterElementsLb1");

			(_lb1.ItemsSource as IList)[0] = new Person("Manu", "french");

			int expectedCount = 11;
			if (expectedCount != _lb1.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb1.Items.Count + " - AlterElements in lb1");
				return TestResult.Fail;
			}

			int index = 0;
			string expectedName = "Manu";
			string expectedNationality = "french";
            TestResult res2 = VerifyPerson(index, expectedName, expectedNationality, _lb1, "AlterElements in lb1");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AlterElementsLb1 was successful");
			return TestResult.Pass;
		}

        private TestResult AlterElementsLb2()
		{
			Status("AlterElementsLb2");

			(_lb2.ItemsSource as IList)[2] = new Place("Seattle", "WA");

			int expectedCount = 12;
			if (expectedCount != _lb2.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb2.Items.Count + " - AlterElements in lb2");
				return TestResult.Fail;
			}

			int index = 11;
			string expectedName = "Seattle";
			string expectedState = "WA";
			TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb2, "AlterElements in lb2");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AlterElementsLb2 was successful");
			return TestResult.Pass;
		}

        private TestResult AlterElementsLb3()
		{
			Status("AlterElementsLb3");

			_lb3.Items[0] = new Place("Eugene", "OR");

			int expectedCount = 3;
			if (expectedCount != _lb3.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb3.Items.Count + " - AlterElements in lb3");
				return TestResult.Fail;
			}

			int index = 0;
			string expectedName = "Eugene";
			string expectedState = "OR";
			TestResult res2 = VerifyPlace(index, expectedName, expectedState, _lb3, "AlterElements in lb3");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("AlterElementsLb3 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region ReorderElements
 
        private TestResult ReorderElements()
		{
			Status("ReorderElements");

            TestResult res1 = ReorderElementsLb1();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }
            TestResult res2 = ReorderElementsLb2();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }
			TestResult res3 = ReorderElementsLb3();
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ReorderElements was successful");
			return TestResult.Pass;
		}

		// swap element in index 0 with element in index 1
        private TestResult ReorderElementsLb1()
		{
            Person tempPerson = ((IList)(_lb1.ItemsSource))[0] as Person;
            ((IList)(_lb1.ItemsSource))[0] = ((IList)(_lb1.ItemsSource))[1];
            ((IList)(_lb1.ItemsSource))[1] = tempPerson;

            int expectedCount = 11;
            if (expectedCount != _lb1.Items.Count)
            {
                LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb1.Items.Count + " - InsertElements in lb1");
                return TestResult.Fail;
            }

            int index1 = 0;
            string expectedName1 = "Peter";
            string expectedNationality1 = "dutch";
			TestResult res1 = VerifyPerson(index1, expectedName1, expectedNationality1, _lb1, "ReorderElements in lb1");
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			int index2 = 1;
			string expectedName2 = "Manu";
			string expectedNationality2 = "french";
			TestResult res2 = VerifyPerson(index2, expectedName2, expectedNationality2, _lb1, "ReorderElements in lb1");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ReorderElementsLb1 was successful");
			return TestResult.Pass;
		}

		// swaps element in index 10 with element in index 11
		// notice that lb.ItemsSource is in use (not lb.Items), so we cannot use lb.Items to do the swapping
        private TestResult ReorderElementsLb2()
		{
			Place tempPlace = (_lb2.ItemsSource as CompositeCollection)[1] as Place;
			(_lb2.ItemsSource as CompositeCollection)[1] = (_lb2.ItemsSource as CompositeCollection)[2];
			(_lb2.ItemsSource as CompositeCollection)[2] = tempPlace;

			int expectedCount = 12;
			if (expectedCount != _lb2.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb2.Items.Count + " - ReorderElements in lb2");
				return TestResult.Fail;
			}

			int index1 = 10;
			string expectedName1 = "Seattle";
			string expectedState1 = "WA";
			TestResult res2 = VerifyPlace(index1, expectedName1, expectedState1, _lb2, "ReorderElements in lb2");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			int index2 = 11;
			string expectedName2 = "Spokane";
			string expectedState2 = "WA";
			TestResult res3 = VerifyPlace(index2, expectedName2, expectedState2, _lb2, "ReorderElements in lb2");
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ReorderElementsLb2 was successful");
			return TestResult.Pass;
		}

		// swaps element in index 0 with element in index 1
        private TestResult ReorderElementsLb3()
		{
			Status("ReorderElementsLb3");

			Place tempPlace = _lb3.Items[0] as Place;
			_lb3.Items[0] = _lb3.Items[1];
			_lb3.Items[1] = tempPlace;

			int expectedCount = 3;
			if (expectedCount != _lb3.Items.Count)
			{
				LogComment("Fail - Count of items in lb should be " + expectedCount + ", instead it is " + _lb3.Items.Count + " - ReorderElements in lb3");
				return TestResult.Fail;
			}

			int index1 = 0;
			string expectedName1 = "Spokane";
			string expectedState1 = "WA";
			TestResult res2 = VerifyPlace(index1, expectedName1, expectedState1, _lb3, "ReorderElements in lb3");
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			int index2 = 1;
			string expectedName2 = "Eugene";
			string expectedState2 = "OR";
			TestResult res3 = VerifyPlace(index2, expectedName2, expectedState2, _lb3, "ReorderElements in lb3");
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ReorderElementsLb3 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AuxMethods
        private TestResult VerifyPlace(int index, string expectedName, string expectedState, ListBox lb, string step)
		{
			Status("VerifyPlace");
			WaitForPriority(DispatcherPriority.Render);

			Place placeAdded = lb.Items[index] as Place;
			if (placeAdded == null)
			{
				LogComment("Fail - Not able to convert item in index " + index + " to Place - " + step);
				return TestResult.Fail;
			}
			if (placeAdded.Name != expectedName)
			{
				LogComment("Fail - Item in index " + index + " should have name " + expectedName + ", instead it has name " + placeAdded.Name + " - " + step);
				return TestResult.Fail;
			}
			if (placeAdded.State != expectedState)
			{
				LogComment("Fail - Item in index " + index + " should have state " + expectedState + ", instead it has state " + placeAdded.State + " - " + step);
				return TestResult.Fail;
			}

			LogComment("VerifyPlace was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyPerson(int index, string expectedName, string expectedNationality, ListBox lb, string step)
		{
			Status("VerifyPerson");
			WaitForPriority(DispatcherPriority.Render);

			Person personAdded = lb.Items[index] as Person;
			if (personAdded == null)
			{
				LogComment("Fail - Not able to convert item in index " + index + " to Person - " + step);
				return TestResult.Fail;
			}
			if (personAdded.Name != expectedName)
			{
				LogComment("Fail - Item in index " + index + " should have name " + expectedName + ", instead it has name " + personAdded.Name + " - " + step);
				return TestResult.Fail;
			}
			if (personAdded.Nationality != expectedNationality)
			{
				LogComment("Fail - Item in index " + index + " should have nationality " + expectedNationality + ", instead it has nationality " + personAdded.Nationality + " - " + step);
				return TestResult.Fail;
			}

			LogComment("VerifyPerson was successful");
			return TestResult.Pass;
		}
		#endregion
	}
}
