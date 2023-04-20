// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// XML CollectionViewSource in templates
	/// </description>
	/// </summary>
    [Test(1, "Views", "CvsTemplateTest")]
	public class CvsTemplateTest : XamlTest
	{
        ListBox _customersLB;
        ListBox [] _ordersLB;
        ListBox [] _itemsLB;

        public CvsTemplateTest()
            : base(@"CVSTemplateBasicTest.xaml")
		{
			InitializeSteps += new TestStep (init);
            RunSteps += new TestStep(VerifyInitial);
            RunSteps += new TestStep(ChangeOrderSort);
            RunSteps += new TestStep(VerifyOrderSort);
		}

		private TestResult init ()
		{
			Status("init");

            WaitForPriority(DispatcherPriority.Background);
            WaitFor(2000);

            FrameworkElement[] array;
            int i = 0;

            Status("Find CustomersLB");
            _customersLB = (ListBox)Util.FindElement(RootElement, "CustomersLB");
            if (_customersLB == null)
            {
                LogComment("Could not reference customersLB");
                return TestResult.Fail;
            }

            Status("Find and copy Orders LB");
            array = Util.FindElements(RootElement, "OrdersLB");
            if (array == null)
            {
                LogComment("Could not reference OrdersLB");
                return TestResult.Fail;
            }
            _ordersLB = new ListBox[array.Length];
            
            foreach (ListBox lb in array)
            {
                _ordersLB[i] = lb;
                i++;
            }
            if (_ordersLB == null || _ordersLB.Length < 1)
            {
                LogComment("Could not find OrdersLB ListBoxes to copy");
                return TestResult.Fail;
            }

            Status("Find and copy Items LB");
            array = null;
            i = 0;
            array = Util.FindElements(RootElement, "ItemsLB");
            if (array == null)
            {
                LogComment("Could not reference ItemsLB");
                return TestResult.Fail;
            }
            _itemsLB = new ListBox[array.Length];
            foreach (ListBox lb in array)
            {
                _itemsLB[i] = lb;
                i++;
            }
            if (_itemsLB == null || _itemsLB.Length < 1)
            {
                LogComment("Could not find ItemsLB ListBoxes to copy");
                return TestResult.Fail;
            }

            LogComment("OrdersLB " + _ordersLB.Length.ToString());
            LogComment("ItemsLB " + _itemsLB.Length.ToString());
            
			LogComment("Initialize was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyInitial()
        {
            Status("Verifying Initial values");
            string[] expected = new string[] 
                        {"New CD",
                        "My Book",
                        "DVD 45",
                        "DVD 32",
                        "DVD 23",
                        "DVD 20",
                        "Power to Help (Book)",
                        "DVD 23",
                        "Book of Ages",
                        "Music Composition",
                        "Market your Music!",
                        "Book of sound 23",
                        "Dictionary",
                        "CD 3"};
            return Verify(expected, "Initial Values");
        }
        
        private TestResult ChangeOrderSort()
        {
            CollectionViewSource cvs = (CollectionViewSource)_ordersLB[1].DataContext;
            if (cvs == null)
            {
                LogComment("Could not reference CollectionViewSource");
                return TestResult.Fail;
            }

            SortDescription sd = new SortDescription("@Number", ListSortDirection.Descending);
            
            cvs.SortDescriptions.Clear();
            cvs.SortDescriptions.Add(sd);

            return TestResult.Pass;
        }

        private TestResult VerifyOrderSort()
        {
            Status("Verifying OrderSort");
            string[] expected = new string[] 
                        {"New CD",
                        "My Book",
                        "DVD 45",
                        "DVD 32",
                        "DVD 23",
                        "DVD 20",
                        "Music Composition",
                        "Market your Music!",
                        "Book of sound 23",
                        "Power to Help (Book)",
                        "DVD 23",
                        "Book of Ages",
                        "Dictionary",
                        "CD 3"};
            return Verify(expected, "Order Sort");
        }

        private TestResult Verify(string[] expectedValues, string stepName)
        {
            WaitForPriority(DispatcherPriority.Background);

            Status("Find and copy itemTitles");
            FrameworkElement[] array = null;
            Collection<TextBlock> items = new Collection<TextBlock>();

            int i = 0;
            array = Util.FindElements(RootElement, "ItemTitle");
            if (array == null)
            {
                LogComment("Could not reference ItemTitle");
                return TestResult.Fail;
            }
            
            foreach (TextBlock txt in array)
            {
                items.Add(txt);
            }
            if (items.Count != expectedValues.Length)
            {
                LogComment("Expected " + expectedValues.Length.ToString() + " items, but Actual was " + items.Count.ToString());
                return TestResult.Fail;
            }

            bool isCorrect = true;
            for (i = 0; i < items.Count; i++)
            {
                if (items[i].Text != expectedValues[i])
                {
                    LogComment("Expected " + expectedValues[i] + "  Actual " + items[i].Text);
                    isCorrect = false;
                }
            }
            if (isCorrect)
            {
                LogComment( "Step " + stepName + " - Text values matched expected values");
                return TestResult.Pass;
            }
            else
                return TestResult.Fail;
        }

	}
}

