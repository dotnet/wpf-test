// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// A menu items control is populated with 2 items: a string and a bound collection that has a
	/// CLR object as the data source.
	/// This test verifies the count of the data visuals, their object type, data context and
	/// content.
	/// </description>
	/// </summary>

    [Test(3, "Controls", "ObjectMenu")]
	public class ObjectMenu : WindowTest
	{
		private Menu _menu;
		private FrameworkElement[] _fes;

		public ObjectMenu()
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestElementCount);
			RunSteps += new TestStep(TestObjectType);
			RunSteps += new TestStep(TestDataContext);
			RunSteps += new TestStep(TestContent);
		}

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_menu = new Menu();
			Window.Content = _menu;

            CompositeCollection composite = new CompositeCollection();
            _menu.ItemsSource = composite;
            composite.Add("individual item");

			CollectionContainer cc = new CollectionContainer();
			cc.Collection = new myStrings();
			composite.Add(cc);

			FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
			Binding bind = new Binding();
			bind.Path = new PropertyPath("Name");
                        fef.SetValue(TextBlock.TextProperty, bind);
                        Style style = new Style();
                        ControlTemplate template = new ControlTemplate();
			template.VisualTree = fef;
                        style.Setters.Add(new Setter(Control.TemplateProperty, template));

			_menu.ItemTemplateSelector = new StylesTest();

			LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult TestElementCount()
		{
			Status("TestElementCount");
			WaitForPriority(DispatcherPriority.Render);

			_fes = Util.FindDataVisuals(_menu, _menu.Items);
			if (_fes == null)
			{
				LogComment("Fail - There are no data visuals in the items control");
				return TestResult.Fail;
			}
			int actualCount = _fes.Length;
			int expectedCount = 6;
			if (actualCount != expectedCount)
			{
				LogComment("Fail - Expected number of data visuals in items control: " + expectedCount + ". Actual:" + actualCount);
				return TestResult.Fail;
			}

			LogComment("TestElementCount was successful");
			return TestResult.Pass;
		}

        private TestResult TestObjectType()
		{
			Status("TestObjectType");
			WaitForPriority(DispatcherPriority.Render);

			Type actualType0 = _fes[0].GetType();
			Type expectedType0 = typeof(TextBlock);
			if (actualType0 != expectedType0)
			{
				LogComment("Fail - Expected type of individual data visual:" + expectedType0 + ". Actual:" + actualType0);
				return TestResult.Fail;
			}

			Type expectedTypeCol = typeof(Button);
			for (int i = 1; i < 6; i++)
			{
				Type actualTypeCol = _fes[i].GetType();
				if (actualTypeCol != expectedTypeCol)
				{
					LogComment("Fail - Expected type of data visual in collection:" + expectedTypeCol + ". Actual:" + actualTypeCol);
					return TestResult.Fail;
				}
			}

			LogComment("TestObjectType was successful");
			return TestResult.Pass;
		}

        private TestResult TestDataContext()
		{
			Status("TestDataContext");
			WaitForPriority(DispatcherPriority.Render);

			string actualDataContext0 = _fes[0].DataContext.ToString();
			string expectedDataContext0 = "individual item";
			if (actualDataContext0 != expectedDataContext0)
			{
				LogComment("Fail - DataContext of individual item is not as expected. Expected:" + expectedDataContext0 + ". Actual:" + actualDataContext0);
				return TestResult.Fail;
			}

			string expectedDataContextCol = "aString";
			for (int i = 1; i < 6; i++)
			{
				string actualDataContextCol = _fes[i].DataContext.ToString();
				if (actualDataContext0 != expectedDataContext0)
				{
					LogComment("Fail - DataContext of item in collection is not as expected. Expected:" + expectedDataContextCol + ". Actual:" + actualDataContextCol);
					return TestResult.Fail;
				}
			}

			LogComment("TestDataContext was successful");
			return TestResult.Pass;
		}

        private TestResult TestContent()
		{
			Status("TestContent");
			WaitForPriority(DispatcherPriority.Render);

			string actualContent0 = ((TextBlock)_fes[0]).Text;
			string expectedContent0 = "individual item";
			if (actualContent0 != expectedContent0)
			{
				LogComment("Fail - Expected content of individual item:" + expectedContent0 + ". Actual:" + actualContent0);
				return TestResult.Fail;
			}

			for (int i = 1; i < 6; i++)
			{
				string actualContentCol = ((Button)_fes[i]).Content.ToString();
				string expectedContentCol = "hello " + (i-1);
				if(actualContentCol != expectedContentCol)
				{
					LogComment("Fail - Expected content of item in collection:" + expectedContentCol + ". Actual:" + actualContentCol);
					return TestResult.Fail;
				}
			}

			LogComment("TestContent was successful");
			return TestResult.Pass;
		}
	}

    public class StylesTest : DataTemplateSelector
    {
        DataTemplate _stringTemplate;

        public StylesTest()
        {
            _stringTemplate = new DataTemplate();

            FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(Button));

            template1.SetBinding(Button.ContentProperty, new Binding("Name"));
            _stringTemplate.VisualTree = template1;
            template1.SetValue(Button.FontWeightProperty, FontWeights.Bold);
            template1.SetValue(Button.NameProperty, "UseMyStyle");
            _stringTemplate.VisualTree = template1;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is aString)
            {
                aString test = item as aString;

                return _stringTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
