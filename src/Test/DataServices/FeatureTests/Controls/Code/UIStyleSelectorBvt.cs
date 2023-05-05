// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests basic functionality of StyleSelector when used to select a UI style
    /// </description>
    /// </summary>
    [Test(0, "Controls", "UIStyleSelectorBvt")]
    public class UIStyleSelectorBvt : WindowTest
    {
        ListBox _lb;
        ObjectDataProvider _dso1;
        Library _library = new Library();
        BookUIStyleSelector _buiss;

        public UIStyleSelectorBvt()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ValidateUIStyle);
            RunSteps += new TestStep(ChangeCollection);
        }

        private TestResult Setup()
        {
            LogComment("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _lb = new ListBox();
            Window.Content = _lb;

            _dso1 = new ObjectDataProvider();
            _dso1.ObjectType = typeof(Library);
            _dso1.ConstructorParameters.Add("100");

            Binding bindList = new Binding();
            bindList.Source = _dso1;
            _lb.SetBinding(ListBox.ItemsSourceProperty, bindList);
            _lb.Height = 300;

            // itemTemplate
            DataTemplate itemTemplate = new DataTemplate();
            FrameworkElementFactory fee = new FrameworkElementFactory(typeof(TextBlock));
            Binding bindItem = new Binding();
            bindItem.Path = new PropertyPath("Title");
            fee.SetBinding(TextBlock.TextProperty, bindItem);
            itemTemplate.VisualTree = fee;
            _lb.ItemTemplate = itemTemplate;

            _buiss = new BookUIStyleSelector();
            _buiss.NumChanged = 15; // expect the style selector to select item UI styles for 15 items (only 15 items will be visible)
            // notifies when the ItemContainerStyle selection is over - avoids the timing issues
            _buiss.ItemUIStyleFinishedChanging += new EventHandler(OnItemUIStyleFinishedChanging);
            _lb.ItemContainerStyleSelector = _buiss;

            return TestResult.Pass;
        }

        private void OnItemUIStyleFinishedChanging(object sender, EventArgs e)
        {
            Status("Event: finished selecting UIStyles");
            Signal("FinishedUIStyle", TestResult.Pass);
        }

        // checks for the number of even/odd items and verifies that the correct
        // style was applied to one odd and one even items
        private TestResult ValidateUIStyle()
        {
            Status("ValidateUIStyle");
            TestResult resultWait = WaitForSignal("FinishedUIStyle");
            if (resultWait != TestResult.Pass)
            {
                LogComment("Fail - Failure while waiting for the event annoucing the end of the item UI style selection");
                return TestResult.Fail;
            }

            TestResult resItems;

            if ((SystemInformation.WpfVersion == WpfVersions.Wpf30) || (SystemInformation.WpfVersion == WpfVersions.Wpf35) || (SystemInformation.WpfVersion == WpfVersions.Wpf40))
            {
                resItems = ValidateListItems(8, 8);
            }
            else
            {
                resItems = ValidateListItems(8, 7);
            }

            if (resItems != TestResult.Pass)
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // add 2 items to the data source and verify that the correct UI style is
        // applied to them correctly
        private TestResult ChangeCollection()
        {
            Status("ChangeCollection");
            WaitForPriority(DispatcherPriority.Render);

            BindingExpression binding = _lb.GetBindingExpression(ListBox.ItemsSourceProperty);
            ObjectDataProvider ods = binding.ParentBinding.Source as ObjectDataProvider;
            Library lib = ods.Data as Library;
            _buiss.NumChanged = 2; // expect the style selector to select item UI styles for 2 items
            Book b1 = new Book(1000);
            Book b2 = new Book(1001);
            lib.Insert(0, b1); // even - will be in index 1
            lib.Insert(0, b2); // odd - will be in index 0

            _lb.ScrollIntoView(b2); // so that the 2 new books inserted become visible
            WaitForPriority(DispatcherPriority.Render);

            TestResult resultWait = WaitForSignal("FinishedUIStyle");
            if (resultWait != TestResult.Pass)
            {
                LogComment("Fail - Failure while waiting for the event annoucing the end of the item UI style selection");
                return TestResult.Fail;
            }

            TestResult resItems;

            if ((SystemInformation.WpfVersion == WpfVersions.Wpf30) || (SystemInformation.WpfVersion == WpfVersions.Wpf35) || (SystemInformation.WpfVersion == WpfVersions.Wpf40))
            {
                resItems = ValidateListItems(9, 9);
            }
            else
            {
                resItems = ValidateListItems(9, 8);
            }

            if (resItems != TestResult.Pass)
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // params: number of even and odd elements expected in the list
        // validates the number of even/odd elements and checks whether the
        // correct style was applied to the first even and first odd elements
        private TestResult ValidateListItems(int num1, int num2)
        {
            ListBoxItem[] fe1 = FindListBoxItemWithBackground(_lb, Brushes.AntiqueWhite);
            ListBoxItem[] fe2 = FindListBoxItemWithBackground(_lb, Brushes.LightBlue);

            TestResult resCount = VerifyCount(fe1, fe2, num1, num2);
            if (resCount != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            // verify that the even list item added picked up the correct style
            ListBoxItem evenLI = fe1[0] as ListBoxItem;
            if (evenLI == null)
            {
                LogComment("Fail - ListBoxItem is null");
                return TestResult.Fail;
            }
            TestResult resEven = VerifyEvenLI(evenLI);
            if (resEven != TestResult.Pass)
            {
                return TestResult.Fail;
            }
            Status("Added even ListBoxItem has the correct style");

            // verify that the odd list item inserted picked up the correct style
            ListBoxItem oddLI = fe2[0] as ListBoxItem;
            if (oddLI == null)
            {
                LogComment("Fail - ListBoxItem is null");
                return TestResult.Fail;
            }
            TestResult resOdd = VerifyOddLI(oddLI);
            if (resOdd != TestResult.Pass)
            {
                return TestResult.Fail;
            }
            Status("Added odd ListBoxItem has the correct style");

            return TestResult.Pass;
        }

        // verify that the # of elements with each style is correct
        TestResult VerifyCount(FrameworkElement[] fe1, FrameworkElement[] fe2, int num1, int num2)
        {
            if (fe1.Length != num1)
            {
                LogComment("Fail - There should be " + num1 + " element(s) with color1Style, instead there are " + fe1.Length);
                return TestResult.Fail;
            }
            if (fe2.Length != num2)
            {
                LogComment("Fail - There should be " + num2 + " element(s) with color2Style, instead there are " + fe2.Length);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // verify that the ListBoxItem got the correct style (even rows)
        TestResult VerifyEvenLI(ListBoxItem evenLI)
        {
            if (evenLI.Background != Brushes.AntiqueWhite)
            {
                LogComment("Fail - ListBoxItem does not have the expected background. Actual background: " + evenLI.Background + " Expected background: #FAEBD7");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // verify that the ListBoxItem got the correct style (odd rows)
        TestResult VerifyOddLI(ListBoxItem oddLI)
        {
            if (oddLI.Background != Brushes.LightBlue)
            {
                LogComment("Fail - ListBoxItem does not have the expected background. Actual background: " + oddLI.Background + " Expected background: #ADD8E6");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        ListBoxItem[] FindListBoxItemWithBackground(FrameworkElement element, Brush background)
        {
            ArrayList list = new ArrayList();
            FindListBoxItemWithBackground(element, background, list);
            return (ListBoxItem[])(list.ToArray(typeof(ListBoxItem)));
        }

        void FindListBoxItemWithBackground(FrameworkElement element, Brush background, ArrayList list)
        {
            ListBoxItem lbi = element as ListBoxItem;
            if ((lbi != null) && (lbi.Background.Equals(background)))
            {
                list.Add(element);
            }

            //make sure that the visual tree is available
            element.ApplyTemplate();

            //search all the children
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                {
                    FindListBoxItemWithBackground((FrameworkElement)child, background, list);
                }
            }
        }

    }

    public class BookUIStyleSelector : StyleSelector
    {
        Style _color1Style;
        Style _color2Style;
        // counts how many item UI styles were selected so far
        // reset when changing NumChanged
        private int _i;
        // counts how many item UI styles we want to select total
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
        public event EventHandler ItemUIStyleFinishedChanging;

        public BookUIStyleSelector()
        {
            _i = 0;
            _numChanged = 0;
            _color1Style = new Style(typeof(ListBoxItem));
            _color1Style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.AntiqueWhite));
            _color1Style.Setters.Add(new Setter(ListBoxItem.HeightProperty, 20d));

            _color2Style = new Style(typeof(ListBoxItem));
            _color2Style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.LightBlue));
            _color2Style.Setters.Add(new Setter(ListBoxItem.HeightProperty, 20d));
        }

        /// <summary>
        /// Even rows are white, odd rows are light blue
        /// </summary>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            _i++;
            if (_i == NumChanged)
            {
                ItemUIStyleFinishedChanging(item, null);
            }
            int isbn = Int32.Parse(((Book)item).ISBN);
            if (isbn % 2 == 0)
            {
                return _color1Style;
            }
            else
            {
                return _color2Style;
            }
        }
    }
}

