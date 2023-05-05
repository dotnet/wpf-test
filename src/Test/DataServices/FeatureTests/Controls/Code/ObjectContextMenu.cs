// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
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
	/// A context menu items control of a listbox is populated with a bound collection that has
	/// a CLR object as the data source.
	/// This test verifies the count of items in the listbox, and also the object type, data context and
	/// content of the visible item.
	/// </description>
	/// </summary>
    [Test(3, "Controls", TestCaseSecurityLevel.FullTrust, "ObjectContextMenu")]
	public class ObjectContextMenu : WindowTest
	{
		private FrameworkElement[] _fes;
		private ListBox _lb1;
		private ContextMenu _cm1;

		public ObjectContextMenu()
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestElementCount);
			RunSteps += new TestStep(TestObjectType);
			RunSteps += new TestStep(TestDataContext);
			RunSteps += new TestStep(TestContent);
		}

		#region Setup
        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			// init ListBox
			_lb1 = new ListBox();
			Window.Content = _lb1;

			// init ContextMenu
            _lb1.ContextMenu = new ContextMenu();
			_cm1 = _lb1.ContextMenu;

			// populate ListBox
			_lb1.Items.Add("item1");
			_lb1.Items.Add("item2");
			_lb1.Items.Add("item3");

			// populate ContextMenu
			_lb1.ContextMenu.ItemsSource = new Places();

			// style ContextMenu
			DataTemplate datatemplate = new DataTemplate();
			FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
			Binding bind = new Binding();
			bind.Path = new PropertyPath("Name");
            fef.SetBinding(TextBlock.TextProperty, bind);
            fef.SetValue(TextBlock.NameProperty, "xmldatastyle");
            datatemplate.VisualTree = fef;
			_lb1.ContextMenu.ItemTemplate = datatemplate;

			// open ContextMenu
			_lb1.ContextMenu.IsOpen = true;

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestElementCount
        private TestResult TestElementCount()
		{
			Status("TestElementCount");
			WaitForPriority(DispatcherPriority.Render);

			_fes = Util.FindElements(_lb1.ContextMenu, "xmldatastyle");

			int expectedCount = 11;
			int actualCount = _fes.Length;

			if (expectedCount != actualCount)
			{
				LogComment("Fail - Expected count:" + expectedCount + ". Actual:" + actualCount);
				return TestResult.Fail;
			}

			LogComment("TestElementCount was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestObjectType
        public TestResult TestObjectType()
		{
			Status("TestObjectType");
			WaitForPriority(DispatcherPriority.Render);

			Type expectedType = typeof(TextBlock);
			for (int i = 0; i < 11; i++)
			{
				Type actualType = _fes[i].GetType();
				if (expectedType != actualType)
				{
					LogComment("Fail - Expected type:" + expectedType + ". Actual:" + actualType + ". Index:" + i);
					return TestResult.Fail;
				}
			}

			LogComment("TestObjectType was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestDataContext
        private TestResult TestDataContext()
		{
			Status("TestDataContext");
			WaitForPriority(DispatcherPriority.Render);

			string expectedDataContext = "Microsoft.Test.DataServices.Place";
			for (int i = 0; i < 11; i++)
			{
				string actualDataContext = _fes[i].DataContext.ToString();
				if (expectedDataContext != actualDataContext)
				{
					LogComment("Fail - Expected DataContext:" + expectedDataContext + ". Actual:" + actualDataContext + ". Index:" + i);
					return TestResult.Fail;
				}
			}

			LogComment("TestDataContext was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestContent
        private TestResult TestContent()
		{
			Status("TestContent");
			WaitForPriority(DispatcherPriority.Render);

			string[] expectedContent = {"Seattle", "Redmond", "Bellevue", "Kirkland", "Portland",
			"San Francisco", "Los Angeles", "San Diego", "San Jose", "Santa Ana", "Bellingham" };

			for (int i = 0; i < 11; i++)
			{
				if (((TextBlock)_fes[i]).Text != expectedContent[i])
				{
					LogComment("Fail - Expected content:" + expectedContent[i] + ". Actual:" + ((TextBlock)_fes[i]).Text + ". Index:" + i);
					return TestResult.Fail;
				}
			}

			LogComment("TestContent was successful");
			return TestResult.Pass;
		}
		#endregion
	}
}
