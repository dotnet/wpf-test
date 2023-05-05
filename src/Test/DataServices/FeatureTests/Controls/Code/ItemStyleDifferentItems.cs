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
	/// Verifies that ItemTemplate is correctly applied to strings and CLR objects but not to
	/// ListBoxItems and Buttons.
	/// </description>
	/// <relatedBugs>

	/// </relatedBugs>
	/// </summary>



    [Test(2, "Controls", "ItemStyleDifferentItems")]
	public class ItemStyleDifferentItems : XamlTest
	{
		ListBox _lb;

		public ItemStyleDifferentItems(): base(@"ItemStyleDifferentItems.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestCLRObject);
			RunSteps += new TestStep(TestString);
			
			RunSteps += new TestStep(TestListItem);
			RunSteps += new TestStep(TestButton);
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

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestCLRObject
        private TestResult TestCLRObject()
		{
			Status("TestCLRObject");
			WaitForPriority(DispatcherPriority.Render);

            TestResult res = TestItemStyleWasApplied(0);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("TestCLRObject was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestString
        private TestResult TestString()
		{
			Status("TestString");
			WaitForPriority(DispatcherPriority.Render);

            TestResult res = TestItemStyleWasApplied(1);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("TestString was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestListItem
		
        private TestResult TestListItem()
		{
			Status("TestListItem");
			WaitForPriority(DispatcherPriority.Render);

            TestResult res = TestItemStyleWasNotApplied(2);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("TestListItem was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestButton
        private TestResult TestButton()
		{
			Status("TestButton");
			WaitForPriority(DispatcherPriority.Render);

            TestResult res = TestItemStyleWasApplied(3);
			if (res != TestResult.Pass)
			{
				return TestResult.Fail;
			}

			LogComment("TestButton was successful");
			return TestResult.Pass;
		}
		#endregion

		#region AuxMethods
        private TestResult TestItemStyleWasApplied(int index)
		{
			Status("TestItemStyleWasApplied");
			WaitForPriority(DispatcherPriority.Render);

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - ListBox item is null - index: " + index);
				return TestResult.Fail;
			}

			// li's Content is a Place object
			// the TextBlock specified in the style is in li's visualTree
			// FindElement walks the visual tree (which is complex) looking for a
			// FrameworkElement with that Name
			TextBlock text = Util.FindElement(li, "inStyle") as TextBlock;
			if (text == null)
			{
				LogComment("Fail - ItemTemplate was not applied for item in index " + index);
				return TestResult.Fail;
			}

			string textContent = text.Text;
			if (textContent != "in style")
			{
				LogComment("Fail - Text's content not as expected - index: " + index);
				return TestResult.Fail;
			}

			LogComment("TestItemStyleWasApplied was successful");
			return TestResult.Pass;
		}

        private TestResult TestItemStyleWasNotApplied(int index)
		{
			Status("TestItemStyleWasNotApplied");
			WaitForPriority(DispatcherPriority.Render);

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - ListBox item is null - index: " + index);
				return TestResult.Fail;
			}

			TextBlock text = Util.FindElement(li, "inStyle") as TextBlock;
			if (text != null)
			{
				LogComment("Fail - ItemTemplate was applied for item in index " + index);
				return TestResult.Fail;
			}

			LogComment("TestItemStyleWasNotApplied was successful");
			return TestResult.Pass;
		}
		#endregion
	}
}
