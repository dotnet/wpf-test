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
	/// Verifies that ItemContainerStyle is correctly applied to different types of items in a ListBox: string,
	/// ListBoxItem, Button and a CLR object.
	/// It also verifies that a default ListBoxItem style gets applied to a generated wrapper in a ListBox.
	/// </description>
	/// <relatedBugs>


	/// </relatedBugs>
	/// </summary>




    [Test(1, "Controls", "ItemUIStyleDifferentItems")]
	public class ItemUIStyleDifferentItems : XamlTest
	{
		ListBox _lb;
		ListBox _lb1;

		public ItemUIStyleDifferentItems(): base(@"ItemUIStyleDifferentItems.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestCLRObject);
			RunSteps += new TestStep(TestString);
			RunSteps += new TestStep(TestListItem);
			RunSteps += new TestStep(TestButton);
			RunSteps += new TestStep(TestGeneratedWrapper);
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

			_lb1 = (ListBox)Util.FindElement(RootElement, "lb1");
			if (_lb1 == null)
			{
				LogComment("Fail - Unable to reference lb1 element (ListBox).");
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

			Place placeItem = _lb.Items[0] as Place;
			if (placeItem == null)
			{
				LogComment("Fail - Could not convert ListBox's first item into a Place");
				return TestResult.Fail;
			}

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromItem(placeItem) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - First ListBoxItem is null");
				return TestResult.Fail;
			}

			if (li.Background != Brushes.LightBlue)
			{
				LogComment("Fail - ItemContainerStyle was not applied to CLR object Place");
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

			string stringItem = _lb.Items[1] as string;
			if (stringItem == null)
			{
				LogComment("Fail - Could not convert ListBox's second item into a string");
				return TestResult.Fail;
			}

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromItem(stringItem) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - Second ListBoxItem is null");
				return TestResult.Fail;
			}

			if (li.Background != Brushes.LightBlue)
			{
				LogComment("Fail - ItemContainerStyle was not applied to string item");
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

			ListBoxItem listItemItem = _lb.Items[2] as ListBoxItem;
			if (listItemItem == null)
			{
				LogComment("Fail - Could not convert ListBox's third item into a ListBoxItem");
				return TestResult.Fail;
			}

			// notice that ListBoxItem is not wrapped with another ListBoxItem
			if (listItemItem.Background != Brushes.LightBlue)
			{
				LogComment("Fail - ItemContainerStyle was not applied to ListBoxItem item");
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

			Button buttonItem = _lb.Items[3] as Button;
			if (buttonItem == null)
			{
				LogComment("Fail - Could not convert ListBox's fourth item into a Button");
				return TestResult.Fail;
			}

			ListBoxItem li = _lb.ItemContainerGenerator.ContainerFromItem(buttonItem) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - Fourth ListBoxItem is null");
				return TestResult.Fail;
			}

			if (li.Background != Brushes.LightBlue)
			{
				LogComment("Fail - ItemContainerStyle was not applied to Button item");
				return TestResult.Fail;
			}

			LogComment("TestButton was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestGeneratedWrapper
	 
        private TestResult TestGeneratedWrapper()
		{
			Status("TestGeneratedWrapper");

			string stringItem = _lb1.Items[0] as string;
			if (stringItem == null)
			{
				LogComment("Fail - Could not convert ListBox's first item into a string (Generated Wrapper)");
				return TestResult.Fail;
			}

			ListBoxItem li = _lb1.ItemContainerGenerator.ContainerFromItem(stringItem) as ListBoxItem;
			if (li == null)
			{
				LogComment("Fail - First ListBoxItem is null (Generated Wrapper)");
				return TestResult.Fail;
			}
			if (li.Background != Brushes.PeachPuff)
			{
				LogComment("Fail - Default ListBoxItem was not applied to generated wrapper");
				return TestResult.Fail;
			}

			LogComment("TestGeneratedWrapper was successful");
			return TestResult.Pass;
		}
		#endregion
	}
}
