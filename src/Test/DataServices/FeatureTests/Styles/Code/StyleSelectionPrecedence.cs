// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
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
    /// Tests that:
    /// - It is not allowed to set ItemTemplate and ItemTemplateSelector at the same time
    /// - TemplateSelector has precedence over implicit template
    /// - Explicit template has precedence over implicit template
    /// - If there's no TemplateSelector or explicit template, then the implicit template in the resources 
    /// is applied
	/// </description>
	/// </summary>
    [Test(2, "Styles", "StyleSelectionPrecedence")]
	public class StyleSelectionPrecedence: WindowTest
	{
		ListBox _lb;
		FrameworkElementFactory _button1;
        DataTemplate _explicittemplate;

		/// <summary>
		/// Tests the StyleSelector functionality.
		/// </summary>
		public StyleSelectionPrecedence()
		{
			InitializeSteps += new TestStep(CreateTree);
            
			RunSteps += new TestStep(ApplyExplictStyleWithItemStyleSelector);
            RunSteps += new TestStep(ValidateExplictStyleHasPrecedence);
            RunSteps += new TestStep(RemoveExplicitStyle);
            RunSteps += new TestStep(ValidateStyleSelectorHasPrecedence);
            RunSteps += new TestStep(RemoveStyleSelector);
            RunSteps += new TestStep(ValidateResourceStyleHasPrecedence);
            RunSteps += new TestStep(ApplyExplictStyle);
            RunSteps += new TestStep(ValidateExplictStyleHasPrecedence);
            RunSteps += new TestStep(RemoveExplicitStyle);
            RunSteps += new TestStep(ValidateResourceStyleHasPrecedence);
		}


		TestResult CreateTree()
		{
			_lb = new ListBox();
			Window.Content = _lb;
			_lb.Items.Add(new Dwarf("Sleepy", "Brown", 2, 500, Colors.Purple, new Point(2, 5), true));
			_lb.Items.Add(new Dwarf("Dopey", "Green", 40, 300, Colors.Salmon, new Point(3, 7), false));
			_lb.Items.Add(new Dwarf("Happy", "Red", 30, 400, Colors.Purple, new Point(5, 1), true));
			_lb.Items.Add(new Dwarf("Grumpy", "Orange", 40, 275, Colors.Brown, new Point(7, 3), false));
			_lb.Items.Add(new Dwarf("Bashful", "Purple", 30, 600, Colors.Gray, new Point(1, 5), true));
			_lb.Items.Add(new Dwarf("Dopey", "Black", 40, 800, Colors.DeepPink, new Point(5, 2), false));
			_lb.Items.Add(new Dwarf("Doc", "Pink", 40, 800, Colors.DarkMagenta, new Point(2, 1), false));

			//resource template
			DataTemplate resourcetemplate = new DataTemplate();
			FrameworkElementFactory button = new FrameworkElementFactory(typeof (Button));

			resourcetemplate.VisualTree = button;
			button.SetBinding(Button.ContentProperty, new Binding("Name"));
			button.SetValue(Button.BackgroundProperty, Brushes.Purple);
			button.SetValue(Button.NameProperty, "rstyle");
			_lb.Resources = new ResourceDictionary();
			_lb.Resources[new DataTemplateKey(typeof(Dwarf))] = resourcetemplate;

            //explicittemplate
            _explicittemplate = new DataTemplate();
            _button1 = new FrameworkElementFactory(typeof(Button));
            _button1.SetBinding(Button.ContentProperty, new Binding("Name"));
            _button1.SetValue(Button.ForegroundProperty, Brushes.Red);
            _button1.SetValue(Button.NameProperty, "explicit");
            _explicittemplate.VisualTree = _button1;

			WaitForPriority(DispatcherPriority.Render);
			return TestResult.Pass;
		}

        // An exception used to be thrown for this scenario but this doesn't happen anymore.
        // Now ItemTemplate has precedence over StyleSelector (debug trace is printed in this case)
        private TestResult ApplyExplictStyleWithItemStyleSelector()
		{
			Status("Setting ItemTemplate with ItemTemplateSelector");
		    _lb.ItemTemplate = _explicittemplate;
            CoolDwarfSelectorTest ss = new CoolDwarfSelectorTest();
            _lb.ItemTemplateSelector = ss;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
		}

		private TestResult ValidateStyleSelectorHasPrecedence()
		{
			Status("Validating that Style Selector Style is applied.");

			FrameworkElement[] _visualCollection = Util.FindDataVisuals(_lb, _lb.Items.SourceCollection);
			TextBlock _txt = _visualCollection[1] as TextBlock;

			if (_txt == null)
			{
				LogComment("Visual Element is null");
				return TestResult.Fail;
			}
			if (_txt.FontWeight != FontWeights.Bold)
			{
				LogComment("Text Selector didn't apply correct FontWeights. Expected: " + FontWeights.Bold.ToString() + "  Actual: " + _txt.FontWeight.ToString());
				return TestResult.Fail;
			}
			return TestResult.Pass;
		}


		private TestResult RemoveStyleSelector()
		{
			_lb.ItemTemplateSelector = null;
			WaitForPriority(DispatcherPriority.Render);
			return TestResult.Pass;
		}

		private TestResult ApplyExplictStyle()
		{
			Status("Setting ItemTemplate with ItemTemplateSelector - Error Expected");
			_lb.ItemTemplate = _explicittemplate;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
		}

		private TestResult ValidateExplictStyleHasPrecedence()
		{
			Status("Validating that Explicit Style has Precedence");

            FrameworkElement[] _visualCollection = Util.FindDataVisuals(_lb, _lb.Items.SourceCollection);
            Button _btn = _visualCollection[0] as Button;
			if (_btn == null)
			{
				LogComment("Visual Elment is null");
				return TestResult.Fail;
			}
			if (_btn.Foreground != Brushes.Red)
			{
				LogComment("Button foreground was not the expected color. Expected: " + Brushes.Red.ToString() + " Actual: " + _btn.Foreground.ToString());
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}



		private TestResult RemoveExplicitStyle()
		{
			Status("Removing Explicite Style");
			_lb.ItemTemplate = null;
			WaitForPriority(DispatcherPriority.Render);
			return TestResult.Pass;
		}



		private TestResult ValidateResourceStyleHasPrecedence()
		{
			Status("Validating that ResourceStyle is applied.");
            WaitForPriority(DispatcherPriority.SystemIdle);

            FrameworkElement[] _visualCollection = Util.FindDataVisuals(_lb, _lb.Items.SourceCollection);
            Button _btn = _visualCollection[0] as Button;

			if (_btn == null)
			{
				LogComment("Visual Element is null");
				return TestResult.Fail;
			}

			if (_btn.Background != Brushes.Purple)
			{
				LogComment("Button Background was not the expected color. Expected: " + Brushes.Purple.ToString() + "  Actual: " + _btn.Background.ToString());
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

	}

	public class CoolDwarfSelectorTest : DataTemplateSelector
	{
		DataTemplate _dwarfTemplate;

		DataTemplate _cooldwarfTemplate;

		public CoolDwarfSelectorTest()
		{
			_dwarfTemplate = new DataTemplate();

			FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(TextBlock));

            template1.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            _dwarfTemplate.VisualTree = template1;
            template1.SetValue(TextBlock.NameProperty, "default");
            _cooldwarfTemplate = new DataTemplate();

			FrameworkElementFactory template = new FrameworkElementFactory(typeof(TextBlock));

            template.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            template.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            template.SetValue(TextBlock.NameProperty, "resource");
			_cooldwarfTemplate.VisualTree = template;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
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


}




