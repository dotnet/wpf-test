// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Xml;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests DataTriggers on a variety of different property types, verifying application of both Setters
    /// and Enter/ExitActions.
    /// 
    /// Types of Properties that are targeted by DataTriggers: Bool, DateTime, Double, Enum, String
    /// 
    /// A Book object is instantiated, and a DataTemplate has Data Triggers for properties matching each type
    /// of property to test. The condition is not satisfied initially. For each variation a property of the type
    /// to be tested is set to a value to satisfy the corresponding DataTrigger, and the application of a setter
    /// and triggering the EnterAction is verified. The value is then set back to a nonsatisfying value, and the nonapplication of
    /// the setter and triggering the ExitAction is verified.
    /// 
    /// Covered an additional DataTrigger for if ISBN is null. It shouldn't ever trigger, but
    /// if it does, it's setter and Enter/ExitActions have values that aren't used in the rest of the test,
    /// so subsequent validation in the testcase would fail.
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(2, "Styles", "DataTriggersEnterExitActions")]
    public class DataTriggersEnterExitActions : XamlTest
    {
        private PropertyType _propertyType;
        private enum PropertyType { Bool, DateTime, Double, Enum, String };

        [Variation("Bool")]
        [Variation("DateTime")]
        [Variation("Double")]
        [Variation("Enum")]
        [Variation("String")]
        public DataTriggersEnterExitActions(string pT)
            : base(@"DataTriggersEnterExitActions.xaml")
        {
            switch (pT)
            {
                case "Bool":
                    _propertyType = PropertyType.Bool;
                    break;
                case "DateTime":
                    _propertyType = PropertyType.DateTime;
                    break;
                case "Double":
                    _propertyType = PropertyType.Double;
                    break;
                case "Enum":
                    _propertyType = PropertyType.Enum;
                    break;
                case "String":
                    _propertyType = PropertyType.String;
                    break;
            }

            InitializeSteps += new TestStep(Setup);

            RunSteps += new TestStep(TestTriggers);
        }

        private TestResult Setup()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            return TestResult.Pass;
        }

        private TestResult TestTriggers()
        {
            Status("Begin Test");
            // We find the ListBox in our XAML page, whose first element is the Book to test.
            // We get the ListBoxItem for that object, and use the FindDataVisual helper to get
            // the border and textblock that will be affected by the DataTrigger.
            ListBox mediaItemsListBox = (ListBox)RootElement.FindName("mediaItemsListBox");
            Book myBook = (Book)mediaItemsListBox.Items[0];
            ListBoxItem BookListBoxItem = (ListBoxItem)mediaItemsListBox.ItemContainerGenerator.ContainerFromItem(myBook);
            Border myBorder = (Border)Util.FindDataVisual(BookListBoxItem, myBook);
            TextBlock myTextBlock = (TextBlock)myBorder.Child;

            // The Border's BorderBrush and TextBlocks's Foreground should be red initially. (DataTrigger not satisfied)
            if (myBorder.BorderBrush.ToString() != "#FFFF0000")
            {
                LogComment("Border's BorderBrush was expected to be red initially, was actually: " + myBorder.BorderBrush.ToString());
                return TestResult.Fail;
            }
            if (myTextBlock.Foreground.ToString() != "#FFFF0000")
            {
                LogComment("TextBlock's Foreground was expected to be red initially, was actually: " + myTextBlock.Foreground.ToString());
                return TestResult.Fail;
            }

            // Set a property of the type specified for this variation to a value that will satisfy the applicable DataTrigger.
            if (_propertyType == PropertyType.Bool)
            {
                myBook.IsSpecialEdition = false;
            }
            else if (_propertyType == PropertyType.DateTime)
            {
                myBook.ReleaseDate = new DateTime(1592, 3, 14); 
            }
            else if (_propertyType == PropertyType.Double)
            {
                myBook.Price = 42;
            }
            else if (_propertyType == PropertyType.Enum)
            {
                myBook.Genre = Book.BookGenre.Comic;
            }
            else if (_propertyType == PropertyType.String)
            {
                myBook.ISBN = "StringTriggerValue";
            }

            WaitForPriority(DispatcherPriority.SystemIdle);

            // After the trigger is satisfied, they should each be blue.
            if (myBorder.BorderBrush.ToString() != "#FF0000FF")
            {
                LogComment("Border's BorderBrush was expected to be blue when DataTrigger satisfied, was actually: " + myBorder.BorderBrush.ToString());
                return TestResult.Fail;
            }
            if (myTextBlock.Foreground.ToString() != "#FF0000FF")
            {
                LogComment("TextBlock's Foreground was expected to be blue when DataTrigger satisfied, was actually: " + myTextBlock.Foreground.ToString());
                return TestResult.Fail;
            }

            // Set the property back to a non-satisfying value.
            if (_propertyType == PropertyType.Bool)
            {
                myBook.IsSpecialEdition = true;
            }
            else if (_propertyType == PropertyType.DateTime)
            {
                myBook.ReleaseDate = new DateTime(4567, 1, 23);  
            }
            else if (_propertyType == PropertyType.Double)
            {
                myBook.Price = 7;
            }
            else if (_propertyType == PropertyType.Enum)
            {
                myBook.Genre = Book.BookGenre.SciFi;
            }
            else if (_propertyType == PropertyType.String)
            {
                myBook.ISBN = "StringNoTriggerValue";
            }

            WaitForPriority(DispatcherPriority.SystemIdle);

            // They should each be red again.
            if (myBorder.BorderBrush.ToString() != "#FFFF0000")
            {
                LogComment("Border's BorderBrush was expected to be red when DataTrigger no longer satisfied, was actually: " + myBorder.BorderBrush.ToString());
                return TestResult.Fail;
            }
            if (myTextBlock.Foreground.ToString() != "#FFFF0000")
            {
                LogComment("TextBlock's Foreground was expected to be red when DataTrigger no longer satisfied, was actually: " + myTextBlock.Foreground.ToString());
                return TestResult.Fail;
            }

            Status("End Test");
            return TestResult.Pass;
        }
    }
}
