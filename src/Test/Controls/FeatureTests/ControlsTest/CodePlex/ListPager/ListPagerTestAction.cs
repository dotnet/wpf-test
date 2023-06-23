using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    using PropertyName = ListPagerPropertyName;
    using Validation = ListPagerTestValidationCommon;

    public static class ListPagerTestAction
    {
        /// <summary>
        /// Public method to compare instance's actual to expected  simple-property values. This method delegates to 
        /// private method InternalValidateBasicState where the actual comparisons are done. This outer method cannot
        /// process an ItemsSource property for comparison but checks all the other properties specified
        /// in ExpectedValues' collection. 
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="ExpectedValues">An ItemsControl containg a DictionaryEntry for each property to be checked</param>
        /// <returns></returns>
        public static bool ValidatePropertyState(ContentControl TargetElement, ItemsControl ExpectedValues)
        {
            Dictionary<PropertyName, int> propertyValues = new Dictionary<PropertyName,int>();
            foreach (var entry in ExpectedValues.Items)
            {
                DictionaryEntry ent = (DictionaryEntry)entry;

                propertyValues.Add((PropertyName)Enum.Parse(typeof(PropertyName), (string)(ent.Key)),
                                                  int.Parse((string)ent.Value));
            }

            // Extract itemsSource to send it back into validation -- this argument will always succeed, of course.
            // A reference itemsSource is not passed in from the xtc because the actual itemsSource instance is 
            // not available there. The itemsSource was populated-from, but is not equal-to the ItemsControl.Items.
            // constructed in the xtc.
            UIListPager thePager = (UIListPager)TargetElement.Content;
            IList itemsSource = thePager.ItemsSource;

            return Validation.InternalValidateBasicState(thePager, itemsSource, propertyValues);
        }

        /// <summary>
        /// Public method to compare instance's actual to expected command-property values. This is a wrapper
        /// method that repackages the input parameters and delegates to private method InternalValidateCommandsState
        /// where the actual comparisons are done.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="Expected">An ItemsControl</param>
        /// <returns></returns>
        public static bool ValidateCommandsState(ContentControl TargetElement, ItemsControl Expected)
        {
            UIListPager thePager = (UIListPager)TargetElement.Content;
            IList expected = Expected.Items;

            bool[] expectedExecuteValues = new bool[expected.Count];

            expected.CopyTo(expectedExecuteValues, 0);

            return Validation.InternalValidateCommandsState(thePager, expectedExecuteValues);
        }

        /// <summary>
        /// Public method to validate expected simple-property and command-property values of
        /// a freshly instantiated ListPager control.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <returns></returns>
        public static bool ValidateInitialState(ContentControl TargetElement)
        {
                UIListPager thePager = (UIListPager)TargetElement.Content;

                Dictionary< PropertyName, int> expected = new Dictionary< PropertyName, int>();
                expected[PropertyName.currentPageCount] = 0;
                expected[PropertyName.currentPageIndex] = 0;
                expected[PropertyName.maxPageIndex] = 0;
                // Newly instantiated ListPager has null ItemsSource but implicit invariant (pageCount == maxPageIndex + 1) is false.
                // If ListPager is assigned a null ItemsSource after instantiation will it conflict with current state? Apparently not.
                expected[PropertyName.pageCount] = 0;
                expected[PropertyName.pageSize] = 10;

                return  Validation.InternalValidateBasicState(thePager, null, expected) &&
                        Validation.InternalValidateCommandsState(thePager, new bool[Validation.builtInCmdCount]);
        }

        /// <summary>
        /// Method that takes an instantiated ListPager control, puts it into a known state,
        /// and then verifies correctness & consistency of all properties.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="Items">ItemsControl containing objects that will be copied into a newly constructed object array
        /// and assigned as the ItemsSource property of the ListPager</param>
        /// <param name="PageSize">PageSize value to be assigned</param>
        /// <param name="PageIndex">CurrentPageIndex to be assigned</param>
        /// <returns></returns>
        public static bool PopulateAndValidateState(ContentControl TargetElement, ItemsControl Items, int PageSize, int PageIndex )
        {
            if (PageSize <= 0)
            {
                throw new ArgumentException("Bad XTC parameter -- \"PageSize\" param must be non-negative.");
            }
            
            if (Items == null)
            {
                throw new ArgumentException("Bad XTC parameter -- \"Items\" param can't be null.");
            }

            UIListPager thePager = (UIListPager)TargetElement.Content;

            int itemsCount = Items.Items.Count;
            int expectedNbrOfPages = (itemsCount == 0) ? 1
                : ((itemsCount % PageSize > 0) ? itemsCount / PageSize + 1 : itemsCount / PageSize);

            int expectedMaxPageIndex = expectedNbrOfPages - 1;

            if (PageIndex < 0 || PageIndex > expectedMaxPageIndex)
            {
                throw new ArgumentException("Bad XTC parameter -- \"PageIndex\" param out of range: " +
                                            "Condition \"(PageIndex < 0 || PageIndex > expectedMaxPageIndex)\"");
            }

            int expectedItemsOnCurrentPage =
                (PageIndex < expectedMaxPageIndex || (itemsCount % PageSize == 0) ) ? PageSize : itemsCount % PageSize;
            
            // Have to copy elements into a collection that does not expose INotifyCollectionChanged
            // ListPager explicitly tests for that, and throws an exception in that case.
            object[] itemsArray= new object[itemsCount];
            (Items.Items).CopyTo(itemsArray, 0);

            thePager.ItemsSource = (IList)itemsArray;
            thePager.PageSize = PageSize;
            thePager.CurrentPageIndex = PageIndex;

            Dictionary<PropertyName, int> expected = new Dictionary<PropertyName, int>();

            expected[PropertyName.currentPageCount] = expectedItemsOnCurrentPage;
            expected[PropertyName.currentPageIndex] = PageIndex;
            expected[PropertyName.maxPageIndex] = expectedMaxPageIndex;
            expected[PropertyName.pageCount] = expectedNbrOfPages;
            expected[PropertyName.pageSize] = PageSize;

            bool[] expectedCommandsCanExecute = new bool[Validation.builtInCmdCount + expectedNbrOfPages];
            /*first*/ expectedCommandsCanExecute[0] = (expected[PropertyName.currentPageIndex] != 0);
            /*last*/  expectedCommandsCanExecute[1] = (expected[PropertyName.currentPageIndex] != expected[PropertyName.maxPageIndex]);
            /*next*/  expectedCommandsCanExecute[2] = (expected[PropertyName.currentPageIndex] < expected[PropertyName.maxPageIndex]);
            /*prev*/ expectedCommandsCanExecute[3]  = (expected[PropertyName.currentPageIndex] > 0);

            for (int j = 0; j < expectedNbrOfPages; j++)
            {
                expectedCommandsCanExecute[j + Validation.builtInCmdCount] = (expected[PropertyName.currentPageIndex] != j);
            }

            return Validation.InternalValidateBasicState(thePager, itemsArray, expected) &&
                   Validation.InternalValidateCommandsState(thePager, expectedCommandsCanExecute);
        }
    }
}
