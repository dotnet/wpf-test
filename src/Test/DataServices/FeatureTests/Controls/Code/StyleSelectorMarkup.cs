// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using System.Collections;
using System.Windows.Controls.Primitives;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Uses ItemTemplateSelector and ItemContainerStyleSelector in markup. (Bvts for these properties
	/// do everything in code)
	/// Verifies Style and UI Style are applied correctly. Then replaces an item in the data
	/// source (which is an ObservableCollection) and verifies it was styled correctly.
	/// </description>
	/// </summary>
    [Test(2, "Controls", "StyleSelectorMarkup")]
	public class StyleSelectorMarkup : XamlTest
	{
		ListBox _lb;
		ObjectDataProvider _dataSource;
		MyTemplateSelector _myItemStyleSelector;
		MyUIStyleSelector _myItemUIStyleSelector;

		public StyleSelectorMarkup() : base(@"StyleSelectorMarkup.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(VerifyUIStyle1);
			RunSteps += new TestStep(VerifyUIStyle2);
			RunSteps += new TestStep(VerifyStyle1);
			RunSteps += new TestStep(VerifyStyle2);
			RunSteps += new TestStep(ChangeSource);
		}

		#region Setup
        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_lb = (ListBox)Util.FindElement(RootElement, "lb");
			if (_lb == null)
			{
				LogComment("Fail - Unable to reference lb element (ListBox).");
				return TestResult.Fail;
			}

			_dataSource = RootElement.Resources["dso2"] as ObjectDataProvider;
			if (_dataSource == null)
			{
				LogComment("Fail - Unable to reference the ObjectDataProvider dso2.");
				return TestResult.Fail;
			}

			_myItemStyleSelector = RootElement.Resources["myItemTemplateSelector"] as MyTemplateSelector;
			if (_myItemStyleSelector == null)
			{
				LogComment("Fail - Unable to reference myItemStyleSelector");
				return TestResult.Fail;
			}

			_myItemUIStyleSelector = RootElement.Resources["myItemUIStyleSelector"] as MyUIStyleSelector;
			if (_myItemUIStyleSelector == null)
			{
				LogComment("Fail - Unable to reference myItemUIStyleSelector");
				return TestResult.Fail;
			}

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region VerifyUIStyle1
        private TestResult VerifyUIStyle1()
		{
			Status("VerifyUIStyle1");
			WaitForPriority(DispatcherPriority.Render);

			// item 1 is Redmond which has UIStyle 1 (does not start with S)
            TestResult res = VerifyUIStyle(1, Brushes.SkyBlue, Brushes.BlanchedAlmond, 3);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("VerifyUIStyle1 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region VerifyUIStyle2
        private TestResult VerifyUIStyle2()
		{
			Status("VerifyUIStyle2");
			WaitForPriority(DispatcherPriority.Render);

			// item 0 is Seattle which has UIStyle 2 (starts with S)
            TestResult res = VerifyUIStyle(0, Brushes.DodgerBlue, Brushes.SandyBrown, 4);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			// item 1 is Redmond which has UIStyle 1 (does not start with S)

			LogComment("VerifyUIStyle2 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region VerifyStyle1
        private TestResult VerifyStyle1()
		{
			Status("VerifyStyle1");
			WaitForPriority(DispatcherPriority.Render);

			// item 10 is Bellingham-WA which has Style 1 (does not start with S)
            TestResult res = VerifyStyle(10, "WA", "default", FontWeights.Normal);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("VerifyStyle1 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region VerifyStyle2
        private TestResult VerifyStyle2()
		{
			Status("VerifyStyle2");
			WaitForPriority(DispatcherPriority.Render);

			// item 9 is Santa Ana which has Style 2 (starts with S)
            TestResult res = VerifyStyle(9, "Santa Ana", "bold", FontWeights.Bold);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("VerifyStyle2 was successful");
			return TestResult.Pass;
		}
		#endregion

		#region ChangeSource
        private TestResult ChangeSource()
		{
			Status("ChangeSource");

            ObservableCollection<Place> aldc = _dataSource.Data as ObservableCollection<Place>;
            // was San Francisco (style 2) and now it's style 1
			aldc[5] = new Place("Eugene", "OR");

			WaitForPriority(DispatcherPriority.Render);

            TestResult resStyle = VerifyStyle(5, "OR", "default", FontWeights.Normal);
			if (resStyle != TestResult.Pass)
			{
				return TestResult.Fail;
			}
            TestResult resUIStyle = VerifyUIStyle(5, Brushes.SkyBlue, Brushes.BlanchedAlmond, 3);
			if (resUIStyle != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("ChangeSource was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AuxMethods
        private TestResult VerifyUIStyle(int index, Brush borderBrush, Brush background, int thickness)
		{
			Status("VerifyUIStyle");
			WaitForPriority(DispatcherPriority.Render);

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - ListBoxItem in index " + index + " is null");
				return TestResult.Fail;
			}

			int count = VisualTreeHelper.GetChildrenCount(li);

			if (count != 1)
			{
				LogComment("Fail - visualCollection.count of ListBoxItem in index " + index + " should be 1, instead it is " + count);
				return TestResult.Fail;
			}

			Border border = VisualTreeHelper.GetChild(li,0) as Border;
			if (border == null)
			{
				LogComment("Fail - ListBoxItem's Content in index " + index + " is null");
				return TestResult.Fail;
			}
			if (border.BorderBrush != borderBrush)
			{
				LogComment("Fail - BorderBrush in index " + index + " is not " + borderBrush + ", it is " + border.BorderBrush);
				return TestResult.Fail;
			}
			if (border.Background != background)
			{
				LogComment("Fail - Background in index " + index + " is not " + background + ", it is " + border.Background);
				return TestResult.Fail;
			}
			if (border.BorderThickness != new Thickness(thickness))
			{
				LogComment("Fail - BorderThickness in index " + index + " is not " + thickness + ", it is " + border.BorderThickness);
				return TestResult.Fail;
			}

			LogComment("VerifyUIStyle was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyStyle(int index, string textContent, string id, FontWeight expectedFontWeight)
		{
			Status("VerifyStyle");
			WaitForPriority(DispatcherPriority.Render);

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - ListBoxItem in index " + index + " is null");
				return TestResult.Fail;
			}

			int count = VisualTreeHelper.GetChildrenCount(li);

			if (count != 1)
			{
				LogComment("Fail - visualCollection.count of ListBoxItem in index " + index + " should be 1, instead it is " + count);
				return TestResult.Fail;
			}

			Border border = VisualTreeHelper.GetChild(li,0) as Border;
			if (border == null)
			{
				LogComment("Fail - ListBoxItem's Content in index " + index + " is null");
				return TestResult.Fail;
			}

			ContentPresenter cp = border.Child as ContentPresenter;
			if (cp == null)
			{
				LogComment("Fail - ContentPresenter is null in index " + index);
				return TestResult.Fail;
			}

			TextBlock text = VisualTreeHelper.GetChild(cp,0) as TextBlock;
			if (text == null)
			{
				LogComment("Fail - TextBlock is null in index " + index);
				return TestResult.Fail;
			}
			if (text.Text != textContent)
			{
				LogComment("Fail - TextBlock content is not as expected in index " + index + ". Actual:" + text.Text + ", Expected:" + textContent);
				return TestResult.Fail;
			}
			if (text.Name != id)
			{
				LogComment("Fail - TextBlock Name in index " + index + " is not as expected. Actual:" + text.Name + ", Expected:" + id);
				return TestResult.Fail;
			}
			if (text.FontWeight != expectedFontWeight)
			{
				LogComment("Fail - Text's FontWeight in index " + index + " is not as expected. Actual:" + text.FontWeight + ", Expected:" + expectedFontWeight);
				return TestResult.Fail;
			}

			LogComment("VerifyStyle was successful");
			return TestResult.Pass;
		}
		#endregion
	}

	#region StyleSelectors
	public class MyUIStyleSelector : StyleSelector
	{
		Style _style1;
		Style _style2;

		public MyUIStyleSelector()
		{
			_style1 = new Style(typeof(ListBoxItem));
			FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(Border));
			template1.SetValue(Border.BorderBrushProperty, Brushes.SkyBlue); // #87CEEB
			template1.SetValue(Border.BackgroundProperty, Brushes.BlanchedAlmond); // #FFEBCD
			template1.SetValue(Border.BorderThicknessProperty, new Thickness(3));
			FrameworkElementFactory template2 = new FrameworkElementFactory(typeof(ContentPresenter));
			template1.AppendChild(template2);
			ControlTemplate template = new ControlTemplate(typeof(ListBoxItem));
            template.VisualTree = template1;
            _style1.Setters.Add(new Setter(ListBoxItem.TemplateProperty, template));

			_style2 = new Style(typeof(ListBoxItem));
			FrameworkElementFactory template3 = new FrameworkElementFactory(typeof(Border));
			template3.SetValue(Border.BorderBrushProperty, Brushes.DodgerBlue); // #1E90FF
			template3.SetValue(Border.BackgroundProperty, Brushes.SandyBrown); // #F4A460
			template3.SetValue(Border.BorderThicknessProperty, new Thickness(4));
			FrameworkElementFactory template4 = new FrameworkElementFactory(typeof(ContentPresenter));
			template3.AppendChild(template4);
			template = new ControlTemplate(typeof(ListBoxItem));
            template.VisualTree = template3;
            _style2.Setters.Add(new Setter(ListBoxItem.TemplateProperty, template));
		}

		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is Place)
			{
				Place place = item as Place;

				if (place.Name.StartsWith("S"))
					return _style2;
				else
					return _style1;
			}

			return base.SelectStyle(item, container);
		}
	}

	public class MyTemplateSelector : DataTemplateSelector
	{
		DataTemplate _dataTemplate1;
		DataTemplate _dataTemplate2;

		public MyTemplateSelector()
		{
			_dataTemplate1 = new DataTemplate();
			FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(TextBlock));
            template1.SetBinding(TextBlock.TextProperty, new Binding("State"));
            template1.SetValue(TextBlock.NameProperty, "default");
            _dataTemplate1.VisualTree = template1;

			_dataTemplate2 = new DataTemplate();
			FrameworkElementFactory template2 = new FrameworkElementFactory(typeof(TextBlock));
            template2.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            template2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            template2.SetValue(TextBlock.NameProperty, "bold");
            _dataTemplate2.VisualTree = template2;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is Place)
			{
				Place place = item as Place;

				if (place.Name.StartsWith("S"))
					return _dataTemplate2;
				else
					return _dataTemplate1;
			}

			return base.SelectTemplate(item, container);
		}
	}
	#endregion
}

