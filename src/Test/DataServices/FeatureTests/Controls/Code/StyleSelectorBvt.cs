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

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests basic functionality of StyleSelector.
	/// It creates a listbox, adds items to it and verifies that the correct style was applied.
	/// Then it adds, inserts and removes items to the list box (and of course verifies the style is
	/// correctly applied).
	/// </description>
	/// </summary>
    [Test(0, "Controls", "StyleSelectorBvt")]
	public class StyleSelectorBvt : WindowTest
	{
		ListBox _lb;
		FrameworkElement[] _elements;
		CoolDwarfSelector _dwarfSelector;

		public StyleSelectorBvt()
		{
			InitializeSteps += new TestStep(CreateTree);
			RunSteps += new TestStep(VerifyInitialList);
			RunSteps += new TestStep(AddVerifyList);
			RunSteps += new TestStep(RemoveVerifyList);
		}

		#region CreateTree
        TestResult CreateTree()
		{
			Status("CreateTree");
			_lb = new ListBox();
			Window.Content = _lb;

			_lb.Items.Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));
			_lb.Items.Add(new Dwarf("Dopey", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
			_lb.Items.Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));

			_dwarfSelector = new CoolDwarfSelector();
			_dwarfSelector.NumChanged = 3; // expect the style selector to select item styles for 3 items
			_dwarfSelector.ItemStyleFinishedChanging += new EventHandler(OnItemStyleFinishedChanging);
			_lb.ItemTemplateSelector = _dwarfSelector;

			WaitForPriority(DispatcherPriority.Background);

			LogComment("CreateTree was successful");
			return TestResult.Pass;
		}

		private void OnItemStyleFinishedChanging(object sender, EventArgs e)
		{
			Status("Event: finished selecting Styles");
			Signal("FinishedStyle", TestResult.Pass);
		}
		#endregion

		#region VerifyInitialList
		// Validates style gets applied to selected items.
        private TestResult VerifyInitialList()
		{
			Status("VerifyInitialList");
			// this solves previous timining issues
            TestResult resultWait = WaitForSignal("FinishedStyle");
			if (resultWait != TestResult.Pass)
			{
				LogComment("Fail - Failure while waiting for the event annoucing the end of the item style selection");
				return TestResult.Fail;
			}

			//applying the style when tree gets created - timing issue. - no more!
			// lb.Items: Bashful, Dopey, Doc
			// elements - Dopey, Doc - because only they start with D
			_elements = Util.FindElements(_lb, "bold");

            TestResult res1 = ValidateCount(2, 3);
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

            TestResult res2 = ValidateInitialList();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("VerifyInitialList was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AddVerifyList
        private TestResult AddVerifyList()
		{
			Status("AddVerifyList");
			//Add/Insert 3 items to items collection
			_dwarfSelector.NumChanged = 3; // expect the style selector to select item styles for 3 items
			Dwarf d1 = new Dwarf("David", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true);
			Dwarf d2 = new Dwarf("De Drake", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false);
			Dwarf d3 = new Dwarf("testing default", "Orange", 2, 500, Colors.Purple, new Point(2, 5), true);
			_lb.Items.Add(d1);
			_lb.Items.Insert(0, d2);
			_lb.Items.Add(d3);

			// If I don't scroll into view, inserting a new Dwarf in position 0 will add a scroller
			// to the ListBox and the item will not be visible
			_lb.ScrollIntoView(d1);
			WaitForPriority(DispatcherPriority.Render);
			_lb.ScrollIntoView(d2);
			WaitForPriority(DispatcherPriority.Render);
			_lb.ScrollIntoView(d3);
			WaitForPriority(DispatcherPriority.Render);

			// this solves previous timining issues
            TestResult resultWait = WaitForSignal("FinishedStyle");
			if (resultWait != TestResult.Pass)
			{
				LogComment("Fail - Failure while waiting for the event annoucing the end of the item style selection");
				return TestResult.Fail;
			}

			// lb.Items: De Drake, Bashful, Dopey, Doc, David, testing default
			// elements: De Drake, Dopey, Doc, David
			_elements = Util.FindElements(_lb, "bold");

            TestResult res1 = ValidateCount(4, 6);
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

            TestResult res2 = ValidateAddedElements();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			LogComment("AddVerifyList was successful");
			return TestResult.Pass;
		}
		#endregion

		#region RemoveVerifyList
        private TestResult RemoveVerifyList()
		{
			Status("RemoveVerifyList");

			_lb.Items.RemoveAt(3); // Doc gets removed
			WaitForPriority(DispatcherPriority.Render);

			// lb.Items: De Drake, Bashful, Dopey, David, testing default
			// elements: De Drake, Dopey, David
			_elements = Util.FindElements(_lb, "bold");

            TestResult res1 = ValidateCount(3, 5);
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

            TestResult res2 = ValidateRemovedElements();
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("RemoveVerifyList was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AuxMethods
        private TestResult ValidateInitialList()
		{
			Status("ValidateInitialList");
			// lb.Items: Bashful, Dopey, Doc
			// elements - Dopey, Doc - because only they start with D

			// Dopey
			TestResult res1 = CompareElementDwarf(0, 1);
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			// Doc
			TestResult res2 = CompareElementDwarf(1, 2);
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ValidateInitialList was successful");
			return TestResult.Pass;
		}

        private TestResult ValidateAddedElements()
		{
			Status("ValidateAddedElements");
			// lb.Items: De Drake, Bashful, Dopey, Doc, David, testing default
			// elements: De Drake, Dopey, Doc, David

			// De Drake
            TestResult res1 = CompareElementDwarf(0, 0);
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			// Dopey
            TestResult res2 = CompareElementDwarf(1, 2);
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			// Doc
            TestResult res3 = CompareElementDwarf(2, 3);
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			// David
            TestResult res4 = CompareElementDwarf(3, 4);
			if (res4 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ValidateAddedElements was successful");
			return TestResult.Pass;
		}

        private TestResult ValidateRemovedElements()
		{
			Status("ValidateRemovedElements");
			// lb.Items: De Drake, Bashful, Dopey, David, testing default
			// elements: De Drake, Dopey, David
			// De Drake
            TestResult res1 = CompareElementDwarf(0, 0);
			if (res1 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			// Dopey
            TestResult res2 = CompareElementDwarf(1, 2);
			if (res2 != TestResult.Pass)
			{
				return TestResult.Fail;
			}
			// Doc
            TestResult res3 = CompareElementDwarf(2, 3);
			if (res3 != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ValidateRemovedElements was successful");
			return TestResult.Pass;
		}

        private TestResult CompareElementDwarf(int indexElement, int indexDwarf)
		{
			Status("CompareElementDwarf");

			TextBlock text1 = _elements[indexElement] as TextBlock;
			Dwarf dwarf1 = _lb.Items[indexDwarf] as Dwarf;
			if (text1 == null)
			{
				LogComment("Fail - elements[" + indexElement + "] is null or could not be converted to Text");
				return TestResult.Fail;
			}
			if (dwarf1 == null)
			{
				LogComment("Fail - lb.Items[" + indexDwarf + "] is null or could not be converted to Text");
				return TestResult.Fail;
			}
			if (text1.Text != dwarf1.Name)
			{
				LogComment("Fail - The name of the dwarf (" + dwarf1.Name + ")and the content of " +
					"the TextBlock element (" + text1.Text + ") don't match");
				return TestResult.Fail;
			}

			LogComment("CompareElementDwarf was successful");
			return TestResult.Pass;
		}

        private TestResult ValidateCount(int expectedLength, int expectedCount)
		{
			Status("ValidateCount");
			if (_elements.Length != expectedLength)
			{
				LogComment("Fail - Array length is incorrect.  Expected:" +  expectedLength + ". Actual:  " + _elements.Length);
				return TestResult.Fail;
			}

			if (_lb.Items.Count != expectedCount)
			{
				LogComment("Fail - lb.Items has incorrect count. Expected:" + expectedCount + ". Actual: " + _lb.Items.Count);
				return TestResult.Fail;
			}
			LogComment("ValidateCount was successful");
			return TestResult.Pass;
		}
		#endregion
	}

	#region StyleSelector
	public class CoolDwarfSelector : DataTemplateSelector
	{
		DataTemplate _dwarfTemplate;
		DataTemplate _cooldwarfTemplate;
		// counts how many item styles were selected so far
		// reset when changing NumChanged
		private int _i;
		// counts how many item styles we want to select total
		private int _numChanged;
		public int NumChanged
		{
			get
			{
				return _numChanged;
			}
			set
			{
				_i = 0;
				_numChanged = value;
			}
		}
		public event EventHandler ItemStyleFinishedChanging;

		public CoolDwarfSelector()
		{
			_i = 0;
			_numChanged = 0;

			_dwarfTemplate = new DataTemplate();

			FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(TextBlock));

            template1.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            _dwarfTemplate.VisualTree = template1;
            template1.SetValue(TextBlock.NameProperty, "default");
            _cooldwarfTemplate = new DataTemplate();

			FrameworkElementFactory template2 = new FrameworkElementFactory(typeof(TextBlock));

            template2.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            template2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            template2.SetValue(TextBlock.NameProperty, "bold");
            _cooldwarfTemplate.VisualTree = template2;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			_i++;
			if (_i == NumChanged)
			{
				ItemStyleFinishedChanging(item, null);
			}

			if (item is Dwarf)
			{
				Dwarf dwarf = item as Dwarf;

				if (dwarf.Name.StartsWith("D"))
					return _cooldwarfTemplate;
				else
					return _dwarfTemplate;
			}

			return base.SelectTemplate(item, container);
		}
	}
	#endregion
}




