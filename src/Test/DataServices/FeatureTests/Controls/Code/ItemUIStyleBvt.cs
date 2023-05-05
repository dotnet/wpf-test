// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
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
    /// Tests basic functionality of ItemContainerStyle
    /// </description>
    /// </summary>
    [Test(0, "Controls", "ItemUIStyleBvt")]
    public class ItemUIStyleBvt : WindowTest
    {
        ComboBox _comboBox;

        ObjectDataProvider _dso1;
        Library _library = new Library();

        public ItemUIStyleBvt()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ValidateItemUIStyle);
            RunSteps += new TestStep(ChangeCollection);
        }

        private TestResult Setup()
        {
            LogComment("Setup");
            WaitForPriority(DispatcherPriority.Render);

            DockPanel dockPanel = new DockPanel();
            Window.Content = dockPanel;
            _comboBox = new ComboBox();
            _comboBox.Height = 22;
            _comboBox.SelectedIndex = 0;
            _comboBox.MaxDropDownHeight = 100;
            dockPanel.Children.Add(_comboBox);

            _dso1 = new ObjectDataProvider();
            _dso1.ObjectType = typeof(Library);
            _dso1.ConstructorParameters.Add("100");

            Binding bindCombo = new Binding();
            bindCombo.Source = _dso1;
            _comboBox.SetBinding(ComboBox.ItemsSourceProperty, bindCombo);

            // ItemTemplate in the dockPanel's resources
            DataTemplate itemTemplate = new DataTemplate();
            FrameworkElementFactory fee = new FrameworkElementFactory(typeof(TextBlock));
            Binding bindItem = new Binding();
            bindItem.Path = new PropertyPath("Title");
            fee.SetBinding(TextBlock.TextProperty, bindItem);
            itemTemplate.VisualTree = fee;
            dockPanel.Resources.Add("itemTemplate", itemTemplate);

            // get itemTemplate from Resources and set comboBox.ItemTemplate to it
            DataTemplate st1 = dockPanel.Resources["itemTemplate"] as DataTemplate;
            if (st1 == null)
            {
                LogComment("Fail - DataTemplate is null");
                return TestResult.Fail;
            }
            _comboBox.ItemTemplate = st1;

            // ItemContainerStyle in the dockPanel's resources
            Style itemUIStyle = new Style(typeof(ComboBoxItem));
            itemUIStyle.Setters.Add(new Setter(ComboBoxItem.HeightProperty, 20d));
            dockPanel.Resources.Add("itemUIStyle", itemUIStyle);

            // get itemUIStyle from Resources and set comboBox.ItemContainerStyle to it
            Style st2 = dockPanel.Resources["itemUIStyle"] as Style;
            if (st2 == null)
            {
                LogComment("Fail - UI Style is null");
                return TestResult.Fail;
            }
            _comboBox.ItemContainerStyle = st2;

            return TestResult.Pass;
        }

        // Verifies that the ItemContainerStyle was applied for a particular item
        private TestResult VerifyComboBoxItem(int index)
        {
            Status("VerifyComboBoxItem - Index: " + index);
            ComboBoxItem comboBoxItem = _comboBox.ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItem;
            if (comboBoxItem == null)
            {
                LogComment("Fail - ComboBoxItem is null. Index: " + index);
                return TestResult.Fail;
            }
            if (comboBoxItem.Height != 20)
            {
                LogComment("Fail - Height is not as expected. Index: " + index + ", Actual: " + comboBoxItem.Height + ", Expected: 20px");
                return TestResult.Fail;
            }
            Status("Index " + index + ": ComboBoxItem is correct");
            return TestResult.Pass;
        }

        // Verifies the number of elements in ComboBox and that the ItemContainerStyle
        // was applied to the first and last elements and an element in the middle
        private TestResult ValidateItemUIStyle()
        {
            Status("ValidateItemUIStyle");
            WaitForPriority(DispatcherPriority.Render);

            CollectionView cv = _comboBox.Items as CollectionView;
            if (cv == null)
            {
                LogComment("Fail - CollectionView is null");
                return TestResult.Fail;
            }
            int count = cv.Count;
            if (count != 100)
            {
                LogComment("Fail - ComboBox should have 100 elements, instead it has " + count);
                return TestResult.Fail;
            }
            Status("Number of elements of ComboBox is correct (100)");

            // Pick the first element and verify that ItemContainerStyle was applied
            _comboBox.IsDropDownOpen = true;
            WaitForPriority(DispatcherPriority.Render);
            TestResult resVerify1 = VerifyComboBoxItem(0);
            if (resVerify1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            // Pick the last element
            // I selected 99 here because if the item I want to test is not shown when drop down
            // is opened, it is not generated (ComboBoxItem is null)
            _comboBox.SelectedIndex = 99;
            _comboBox.IsDropDownOpen = true;
            WaitForPriority(DispatcherPriority.Render);
            TestResult resVerify2 = VerifyComboBoxItem(99);
            if (resVerify2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            // Pick a nominal element
            _comboBox.SelectedIndex = 50;
            _comboBox.IsDropDownOpen = true;
            WaitForPriority(DispatcherPriority.Render);
            TestResult resVerify3 = VerifyComboBoxItem(50);
            if (resVerify3 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ChangeCollection()
        {
            Status("ChangeCollection");
            WaitForPriority(DispatcherPriority.Render);

            BindingExpression binding = _comboBox.GetBindingExpression(ComboBox.ItemsSourceProperty);
            ObjectDataProvider ods = binding.ParentBinding.Source as ObjectDataProvider;
            Library lib = ods.Data as Library;
            lib.Insert(0, new Book(1000)); // even - will be in index 1
            lib.Insert(0, new Book(1001)); // odd - will be in index 0

            _comboBox.SelectedIndex = 0;
            _comboBox.IsDropDownOpen = true;
            WaitForPriority(DispatcherPriority.Render);
            TestResult resVerify1 = VerifyComboBoxItem(0);
            if (resVerify1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            _comboBox.IsDropDownOpen = true;
            WaitForPriority(DispatcherPriority.Render);
            TestResult resVerify2 = VerifyComboBoxItem(1);
            if (resVerify2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
