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
	/// Tests setting item style of an ItemsControl explicitly with ItemTemplate property and by defining
	/// a data style in the resources. It makes sure these styles are still applied after a collection
	/// change and that setting ItemTemplate to null removes the items style.
	/// </description>
	/// <relatedBugs>

	/// </relatedBugs>
	/// </summary>



    [Test(0, "Controls", "ItemStyleBvt")]
	public class ItemStyleBvt : WindowTest
	{
		ListBox _lb1,_lb2;
		FrameworkElement[] _elements;
		DataTemplate _lb2Template;
		DataTemplate _dwarfTemplate;
		FrameworkElementFactory _button;
		FrameworkElementFactory _simpletext;
		DockPanel _dp;

		public ItemStyleBvt()
		{
			InitializeSteps += new TestStep(CreateTree);
			RunSteps += new TestStep(ExplicitStyle);
			RunSteps += new TestStep(ResourceStyle);
			RunSteps += new TestStep(ChangeCollection);
			
			RunSteps += new TestStep(RemoveStyle);
		}

		#region CreateTree
        private TestResult CreateTree()
		{
			Status("CreateTree");
			_dp = new DockPanel();
			_lb1 = new ListBox();
			_lb2 = new ListBox();

			_dp.Children.Add(_lb1);
			_dp.Children.Add(_lb2);
			Window.Content = _dp;

			// lb1 - explicit style
			_lb1.Items.Add(new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true));
			_lb1.Items.Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false));

			_dwarfTemplate = new DataTemplate();
			_simpletext = new FrameworkElementFactory(typeof (TextBlock));
            _simpletext.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            _simpletext.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            _simpletext.SetValue(TextBlock.NameProperty, "lb1");
            _dwarfTemplate.VisualTree = _simpletext;

			_lb1.ItemTemplate = _dwarfTemplate;

			// lb2 - resource style
			_lb2.Items.Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(5, 1), true));
			_lb2.Items.Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(7, 3), false));

			_lb2Template = new DataTemplate();
			_button = new FrameworkElementFactory(typeof (Button));
			_button.SetBinding(Button.ContentProperty, new Binding("Name"));
			_button.SetValue(Button.NameProperty, "lb2");
			_lb2Template.VisualTree = _button;

			_lb2.Resources = new ResourceDictionary();
			_lb2.Resources[new DataTemplateKey(typeof(Dwarf))] = _lb2Template;

			WaitForPriority(DispatcherPriority.Render);

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region ExplicitStyle
		// Validates ExplicitStyle selection
        private TestResult ExplicitStyle()
		{
			Status("ExplicitStyle");
			//due to timing issues, style is set when tree gets created.
			Status("ExplicitStyle");
			if (!ValidateListBox1(_lb1))
				return TestResult.Fail;

			LogComment("ExplicitStyle was successful");
			return TestResult.Pass;
		}
		#endregion

		#region ResourceStyle
		// Validates ResourceStyle selection
        private TestResult ResourceStyle()
		{
			Status("ResourceStyle");
			//due to timing issues, style is set when tree gets created.
			WaitForPriority(DispatcherPriority.Render);
			if(!(ValidateListBox2(_lb2)))
				return TestResult.Fail;

			LogComment("ResourceStyle was successful");
			return TestResult.Pass;
		}
		#endregion

		#region ChangeCollection
		// Validates styles don't get corrupted when changing collection.
        private TestResult ChangeCollection()
		{
			Status("ChangeCollection");
			_lb1.Items.Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));

			_lb2.Items.Add(new Dwarf("Dopey", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
			_lb2.Items.Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));

			WaitForPriority(DispatcherPriority.Render);
			if (!ValidateListBox1(_lb1))
				return TestResult.Fail;

			if(!ValidateListBox2(_lb2))
				return TestResult.Fail;


			Status("Remove dwarfs from collection.");
			_lb1.Items.RemoveAt(1);
			WaitForPriority(DispatcherPriority.Render);
			if (!ValidateListBox1(_lb1))
				return TestResult.Fail;

			Status("Remove dwarfs from collection.");
			_lb2.Items.RemoveAt(0);
			WaitForPriority(DispatcherPriority.Render);
			if (!ValidateListBox2(_lb2))
				return TestResult.Fail;

			LogComment("ChangeCollection was successful");
			return TestResult.Pass;
		}
		#endregion

		#region RemoveStyle
		// Validates that itemscollection return to default style.
		
        private TestResult RemoveStyle()
		{
			Status("RemoveStyle");
			_lb1.ItemTemplate = null;
			WaitForPriority(DispatcherPriority.Render);

			_elements = Util.FindElements(_lb1, "lb1");

			if (_elements.Length != 0)
			{
				LogComment("Elements array should be empty.  Actual:  " + _elements.Length);
				return TestResult.Fail;
			}

			LogComment("RemoveStyle was successful");
			return TestResult.Pass;
		}
		#endregion

		#region auxMethods
		// Validates correct element(s) is placed on VisualTree for lb1 and its properties contain the expected value.
		private bool ValidateListBox1(ListBox lb)
		{
			_elements = Util.FindElements(lb, "lb1");

			if (_elements.Length != lb.Items.Count)
			{
				LogComment("Array count does not match the item collection count.  No element was found with specified Name.");
				return false;
			}

			for (int i = 0; i < _elements.Length; i++)
			{
				if (!(_elements[i] is TextBlock))
				{
					LogComment("Element on VisualTree is not of correct type.  Expected:  Text" + " Actual: " + _elements[i].GetType().Name);
					return false;
				}

				TextBlock txt = _elements[i] as TextBlock;
				Dwarf dwarf = lb.Items[i] as Dwarf;

				if (txt.Text != dwarf.Name)
				{
					LogComment("Text text was not the correct bound value.  Expected: " + dwarf.Name + " Actual: " + txt.Text);
					return false;
				}

				if (txt.FontWeight != FontWeights.Bold)
				{
					LogComment("Text fontweight value is incorrect.  Expected:  Bold.  Actual:  " + txt.FontWeight.ToString());
					return false;
				}
			}

			return true;
		}

		// Validates correct element(s) is placed on VisualTree for lb1 and its properties contain the expected value.
		private bool ValidateListBox2(ListBox lb)
		{
			_elements = Util.FindElements(lb, "lb2");

			if (_elements.Length != lb.Items.Count)
			{
				LogComment("Array count does not match the item collection count.  No element was found with specified Name.");
				return false;
			}

			for (int i = 0; i < _elements.Length; i++)
			{
				if (!(_elements[i] is Button))
				{
					LogComment("Element on VisualTree is not of correct type.  Expected:  Button" + " Actual: " + _elements[i].GetType().Name);
					return false;
				}

				Button btn = _elements[i] as Button;
				Dwarf dwarf = lb.Items[i] as Dwarf;

				if (btn.Content.ToString() != dwarf.Name)
				{
					LogComment("Text text was not the correct bound value.  Expected: " + dwarf.Name + " Actual: " + btn.Content);
					return false;
				}
			}

			return true;
		}
		#endregion
	}
}




