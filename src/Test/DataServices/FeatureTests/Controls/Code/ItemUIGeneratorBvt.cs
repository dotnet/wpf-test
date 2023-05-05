// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests basic functionality of ItemContainerGenerator property of ListBox.
	/// </description>
	/// </summary>
    [Test(0, "Controls", "ItemUIGeneratorBvt")]
	public class ItemUIGeneratorBvt : WindowTest
	{
		ObjectDataProvider _dso1;
		ComboBox _comboBox;

		public ItemUIGeneratorBvt()
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestStatusChanged);
			RunSteps += new TestStep(ValidateItemUIGenerator);
			RunSteps += new TestStep(TestItemsChanged);
		}

		#region Setup
        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_comboBox = new ComboBox();
			_comboBox.Height = 20;
			_comboBox.SelectedIndex = 0;
			_comboBox.MaxDropDownHeight = 100;

			Window.Content = _comboBox;

			_dso1 = new ObjectDataProvider();
			_dso1.ObjectType = typeof(Library);
            _dso1.ConstructorParameters.Add("100");

			Binding bindCombo = new Binding();
			bindCombo.Source = _dso1;
			_comboBox.SetBinding(ComboBox.ItemsSourceProperty, bindCombo);

			// set ItemTemplate
			DataTemplate itemTemplate = new DataTemplate();
			FrameworkElementFactory fee = new FrameworkElementFactory(typeof(TextBlock));
			Binding bindItem = new Binding();
			bindItem.Path = new PropertyPath("Title");
            fee.SetBinding(TextBlock.TextProperty, bindItem);
            itemTemplate.VisualTree = fee;
			_comboBox.ItemTemplate = itemTemplate;

			// set ItemContainerStyle
			Style itemUIStyle = new Style(typeof(ComboBoxItem));
			itemUIStyle.Setters.Add (new Setter(ComboBoxItem.BackgroundProperty, Brushes.AntiqueWhite));
			_comboBox.ItemContainerStyle = itemUIStyle;

			LogComment("Setup was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestStatusChanged
		// Tests the StatusChanged eventhandler
        private TestResult TestStatusChanged()
		{
			Status("TestStatusChanged");
			WaitForPriority(DispatcherPriority.Render);

			GeneratorStatus gs = _comboBox.ItemContainerGenerator.Status;
			_comboBox.ItemContainerGenerator.StatusChanged += new EventHandler(MyStatusChangedEventHandler);
			_comboBox.IsDropDownOpen = true;
			TestResult resGeneratingContent = WaitForSignal("GeneratingContainers");
			if (resGeneratingContent != TestResult.Pass)
			{
				LogComment("Fail - Failure waiting for the GeneratingContainers signal when Status changes");
				return TestResult.Fail;
			}
            TestResult resContentReady = WaitForSignal("ContainersGenerated");
			if (resContentReady != TestResult.Pass)
			{
				LogComment("Fail - Failure waiting for the ContainersGenerated signal when Status changes");
				return TestResult.Fail;
			}

			_comboBox.ItemContainerGenerator.StatusChanged -= new EventHandler(MyStatusChangedEventHandler);

			LogComment("TestStatusChanged was successful");
			return TestResult.Pass;
		}

		private void MyStatusChangedEventHandler(object sender, EventArgs args)
		{
			ItemContainerGenerator iug = sender as ItemContainerGenerator;
			Status("MyStatusChangedEventHandler - Status: " + iug.Status);
			if (iug.Status == GeneratorStatus.GeneratingContainers)
			{
				Signal("GeneratingContainers", TestResult.Pass);
			}
			else if (iug.Status == GeneratorStatus.ContainersGenerated)
			{
				Signal("ContainersGenerated", TestResult.Pass);
			}
		}
		#endregion

		#region ValidateItemUIGenerator
		// tests ContainerFromIndex, ContainerFromItem, IndexFromContainer and ItemFromContainer
        private TestResult ValidateItemUIGenerator()
		{
			Status("ValidateItemUIGenerator");
			_comboBox.IsDropDownOpen = true;
			WaitForPriority(DispatcherPriority.Render);

			// test ContainerFromIndex
			int indexUIFromIndex = 2;
			ComboBoxItem cbi1 = _comboBox.ItemContainerGenerator.ContainerFromIndex(indexUIFromIndex) as ComboBoxItem;
			if (cbi1 == null)
			{
				LogComment("Fail - ComboBoxItem is null (testing ContainerFromIndex)");
				return TestResult.Fail;
			}
			if (cbi1.Background != Brushes.AntiqueWhite)
			{
				LogComment("Fail - Background is not as expected. Actual: " + cbi1.Background + ", Expected: #FFFAEBD7");
				return TestResult.Fail;
			}
			Status("ContainerFromIndex works as expected");

			// test ContainerFromItem
            TestResult resUIFromItem = TestUIFromItem();
			if (resUIFromItem != TestResult.Pass)
			{
				LogComment("Fail - Failed in TestUIFromItem");
				return TestResult.Fail;
			}
			Status("ContainerFromItem works as expected");

			// test IndexFromContainer
			int indexRetrieved = _comboBox.ItemContainerGenerator.IndexFromContainer(cbi1);
			if (indexUIFromIndex != indexRetrieved)
			{
				LogComment("Fail - IndexFromContainer did not return the correct index. Actual: " + indexRetrieved + ", Expected: " + indexUIFromIndex);
				return TestResult.Fail;
			}
			Status("IndexFromContainer works as expected");

			// test ItemFromContainer
            TestResult resItemFromUI = TestItemFromUI(cbi1, indexUIFromIndex);
			if (resItemFromUI != TestResult.Pass)
			{
				LogComment("Fail - Failed in TestItemFromUI");
				return TestResult.Fail;
			}
			Status("ItemFromContainer works as expected");

			LogComment("ValidateItemUIGenerator was successful");
			return TestResult.Pass;
		}

        private TestResult TestUIFromItem()
		{
			Status("TestUIFromItem");
			int indexUIFromItem = 2;
			Library lib = _dso1.Data as Library;
			if (lib == null)
			{
				LogComment("Fail - Library is null");
				return TestResult.Fail;
			}
			Book book1 = lib[indexUIFromItem] as Book;
			if (book1 == null)
			{
				LogComment("Fail - Book retrieved from Library is null");
				return TestResult.Fail;
			}
			ComboBoxItem cbi2 = _comboBox.ItemContainerGenerator.ContainerFromItem(book1) as ComboBoxItem;
			if (cbi2 == null)
			{
				LogComment("Fail - ComboBoxItem is null (testing ContainerFromIndex)");
				return TestResult.Fail;
			}
			if (cbi2.Background != Brushes.AntiqueWhite)
			{
				LogComment("Fail - Background is not as expected. Actual: " + cbi2.Background + ", Expected: #FFFAEBD7");
				return TestResult.Fail;
			}
			Book book2 = cbi2.Content as Book;
			if (book2 == null)
			{
				LogComment("Fail - Book retrieved from ComboBoxItem is null");
				return TestResult.Fail;
			}
			if (book1 != book2)
			{
				LogComment("Fail - Book retrieved from ComboBoxItem does not match book retrieved from Library object");
				return TestResult.Fail;
			}
			LogComment("TestUIFromItem was successful");
			return TestResult.Pass;
		}

        private TestResult TestItemFromUI(ComboBoxItem cbi1, int indexUIFromIndex)
		{
			Status("TestItemFromUI");
			Book book3 = _comboBox.ItemContainerGenerator.ItemFromContainer(cbi1) as Book;
			if (book3 == null)
			{
				LogComment("Fail - Book retrieved from ItemFromContainer is null");
				return TestResult.Fail;
			}
			Library lib = _dso1.Data as Library;
			if (lib == null)
			{
				LogComment("Fail - Library is null");
				return TestResult.Fail;
			}
			Book book4 = lib[indexUIFromIndex] as Book;
			if (book4 == null)
			{
				LogComment("Fail - Book retrieved from Library is null");
				return TestResult.Fail;
			}
			if (book3 != book4)
			{
				LogComment("Fail - Book retrieved from Library does not match the book retrieved using ItemFromContainer");
				return TestResult.Fail;
			}
			LogComment("TestItemFromUI was successful");
			return TestResult.Pass;
		}
		#endregion

		#region TestItemsChanged
		// Make sure the ItemsChanged event handler gets executed when items change
        private TestResult TestItemsChanged()
		{
			Status("TestItemsChanged");
			WaitForPriority(DispatcherPriority.Render);

			_comboBox.ItemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(MyItemsChangedEventHandler);
			Library lib = _dso1.Data as Library;
			lib.Insert(0, new Book(1000));
            TestResult itemsChangedRes = WaitForSignal("ItemsChanged");
			if (itemsChangedRes != TestResult.Pass)
			{
				LogComment("Fail - Failed waiting for signal ItemsChanged");
				return TestResult.Fail;
			}

			_comboBox.ItemContainerGenerator.ItemsChanged -= new ItemsChangedEventHandler(MyItemsChangedEventHandler);
			LogComment("TestItemsChanged was successful");
			return TestResult.Pass;
		}

		private void MyItemsChangedEventHandler(object sender, ItemsChangedEventArgs args)
		{
			Signal("ItemsChanged", TestResult.Pass);
		}
		#endregion
	}
}
