using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Collections;
using Microsoft.Test;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using System.Collections.Generic;

namespace Microsoft.Test.Controls
{
    [Test(0, "MultiSelector", "MultiSelectorTest")]
    public class MultiSelectorTest : StepsTest
    {
        #region default selection test data.
        static ConcreteMultiSelector concreteMultiSelector;
        static int singleSelectedItemValue = 1;
        static int itemsCount = 5;
        static int selectedItemCount = itemsCount / 2;
        static string selectionChangedEventName = "SelectionChanged";
        #endregion

        public MultiSelectorTest()
        {
            RunSteps += new TestStep(TestMultipleSelection);
            RunSteps += new TestStep(TestSingleSelection);
        }

        TestResult TestMultipleSelection()
        {
            // Multiple selection mode, we use SelectedItems to change selection.
            CreateConcreteMultiSelector(true);

            TestMultipleSelectionDirectlyAddItem();

            TestMultipleSelectionBeginAndEndPattern();

            TestMultipleSelectionSelectAllAndUnselectAll();

            TestMultipleSelectionExceptions();

            return TestResult.Pass;
        }

        TestResult TestSingleSelection()
        {
            // In single selection mode, we can only use SelectedItem to change selected item.
            CreateConcreteMultiSelector(false);

            TestSingleSelectionAssignItemToSelectedItem();

            TestSingleSelectionExceptions();

            return TestResult.Pass;
        }

        private void CreateConcreteMultiSelector(bool canSelectMultiple)
        {
            concreteMultiSelector = new ConcreteMultiSelector(canSelectMultiple);

            // add items to Items collection.
            for (int i = 0; i < itemsCount; i++)
            {
                concreteMultiSelector.Items.Add(i);
            }

            // default IsUpdatingSelectedItems should be false.
            if (concreteMultiSelector.IsUpdatingSelection)
            {
                throw new TestValidationException("IsUpdatingSelectedItems is false at initial state.");
            }
        }

        private SelectionChangedEventArgs CreateSelectionChangedEventArgs(MultiSelector concreteMultiSelector, SelectionMode selectionMode, 
            int removedItemsCount, int addedItemsCount)
        {
            List<int> removedItems = new List<int>();
            List<int> addedItems = new List<int>();
            switch(selectionMode)
            {
                case SelectionMode.Multiple:
                    for (int i = 0; i < removedItemsCount; i++)
                    {
                        removedItems.Add(i);
                    }

                    for (int i = 0; i < addedItemsCount; i++)
                    {
                        addedItems.Add(i);
                    }
                    break;
                case SelectionMode.Single:
                    if (removedItemsCount < 0 || removedItemsCount > 1)
                    {
                        throw new TestValidationException("RemovedItemsCount can only be 0 or 1 in single selection mode.");
                    }

                    if (addedItemsCount < 0 || addedItemsCount > 1)
                    {
                        throw new TestValidationException("AddedItemsCount can only be 0 or 1 in single selection mode.");
                    }

                    if (removedItemsCount.Equals(1))
                    {
                        removedItems.Add(singleSelectedItemValue);
                    }

                    if (addedItemsCount.Equals(1))
                    {
                        addedItems.Add(singleSelectedItemValue);
                    }
                    break;
                default:
                    throw new TestValidationException("Unsupported SelectionMode. " + selectionMode.ToString());
            }

            SelectionChangedEventArgs selectionChangedEventArgs = new SelectionChangedEventArgs(MultiSelector.SelectionChangedEvent,
                removedItems, addedItems);
            selectionChangedEventArgs.Source = concreteMultiSelector;

            return selectionChangedEventArgs;
        }

        private void TestMultipleSelectionDirectlyAddItem()
        {
            EventHelper.ExpectEvent<SelectionChangedEventArgs>(delegate() { concreteMultiSelector.SelectedItems.Add(singleSelectedItemValue); },
                concreteMultiSelector, selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Single, 0, 1), true);

            EventHelper.ExpectEvent<SelectionChangedEventArgs>(delegate() { concreteMultiSelector.SelectedItems.Clear(); },
                concreteMultiSelector, selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Single, 1, 0), true);
        }

        private void TestMultipleSelectionBeginAndEndPattern()
        {
            // Call BeginUpdateSelection();
            concreteMultiSelector.BeginUpdateSelection();

            // IsUpdatingSelectedItems should be true after BeginUpdateSelectedItems.
            if (!concreteMultiSelector.IsUpdatingSelection)
            {
                throw new TestValidationException("IsUpdatingSelectedItems is " +
                    concreteMultiSelector.IsUpdatingSelection.ToString() +
            " after BeginUpdateSelectedItems in MultipleSelection mode.");
            }

            EventHelper.ExpectEvent<SelectionChangedEventArgs>(
                delegate()
                {
                    // Adding selected items.
                    for (int i = 0; i < selectedItemCount; i++)
                    {
                        concreteMultiSelector.SelectedItems.Add(i);
                    }
                }, concreteMultiSelector,
                selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Multiple, 0, selectedItemCount), false);

            EventHelper.ExpectEvent<SelectionChangedEventArgs>(
                delegate()
                {
                    concreteMultiSelector.EndUpdateSelection();
                }, concreteMultiSelector,
                selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Multiple, 0, selectedItemCount), true);

            // IsUpdatingSelectedItems should be false after EndUpdateSelectedItems.
            if (concreteMultiSelector.IsUpdatingSelection)
            {
                throw new TestValidationException("IsUpdatingSelectedItems is " +
                    concreteMultiSelector.IsUpdatingSelection.ToString() +
            " after EndUpdateSelectedItems in MultipleSelection mode.");
            }

            // Validation
            if (!concreteMultiSelector.SelectedItems.Count.Equals(selectedItemCount))
            {
                throw new TestValidationException("Result: Fail SelectedItems Count is not " + 
                    (selectedItemCount).ToString() + ". It is " + concreteMultiSelector.SelectedItems.Count.ToString());
            }
            else
            {
                int expectedSelectedItemValue = 0;
                foreach (object obj in concreteMultiSelector.SelectedItems)
                {
                    if (!obj.Equals(expectedSelectedItemValue))
                    {
                        throw new TestValidationException("Expected selected item value is different than actual selected item value.");
                    }
                    expectedSelectedItemValue++;
                }
            }

            concreteMultiSelector.SelectedItems.Clear();
        }

        private void TestMultipleSelectionSelectAllAndUnselectAll()
        {
            EventHelper.ExpectEvent<SelectionChangedEventArgs>(
                delegate() { concreteMultiSelector.SelectAll(); }, concreteMultiSelector,
                selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Multiple, 0, itemsCount), true);

            EventHelper.ExpectEvent<SelectionChangedEventArgs>(
                delegate() { concreteMultiSelector.UnselectAll(); }, concreteMultiSelector,
                selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Multiple, itemsCount, 0), true);
        }

        private void TestMultipleSelectionExceptions()
        {
            string endUpdateSelectionWithoutBeginExceptionMessage = "Cannot end a selection when no selection is in progress.";
            string beginUpdateSelectionAfterBeginExceptionMessage = "Cannot begin a new selection while a selection is currently in progress.";

            ExceptionHelper.ExpectException(delegate()
            {
                concreteMultiSelector.EndUpdateSelection();
            },
                new InvalidOperationException(endUpdateSelectionWithoutBeginExceptionMessage, null));

            concreteMultiSelector.BeginUpdateSelection();

            ExceptionHelper.ExpectException(delegate()
            {
                concreteMultiSelector.BeginUpdateSelection();
            }, new InvalidOperationException(beginUpdateSelectionAfterBeginExceptionMessage, null));
        }

        private void TestSingleSelectionAssignItemToSelectedItem()
        {
            EventHelper.ExpectEvent<SelectionChangedEventArgs>(
                delegate() { concreteMultiSelector.SelectedItem = singleSelectedItemValue; }, concreteMultiSelector,
                selectionChangedEventName, CreateSelectionChangedEventArgs(concreteMultiSelector, SelectionMode.Single, 0, 1), true);

            // Validation
            // Fail if SelectedItems Count is not 1 in a single selection mode.
            if (!concreteMultiSelector.SelectedItems.Count.Equals(1))
            {
                throw new ApplicationException("Result: Fail SelectedItems Count is not 1. It is " + concreteMultiSelector.SelectedItems.Count.ToString());
            }
            else
            {
                if (!concreteMultiSelector.SelectedItems[0].Equals(singleSelectedItemValue))
                {
                    throw new ApplicationException("Result: Fail the first SelectedItems is not 1. It is " + concreteMultiSelector.SelectedItems[0].ToString());
                }
            }
        }

        private void TestSingleSelectionExceptions()
        {
            string singleSelectionModeExceptionMessage = "Can only change SelectedItems collection in multiple selection modes. Use SelectedItem in single select modes.";
            string singleSelectionModeSelectAllExceptionMessage = "Can only call SelectAll when CanSelectMultipleItems is true.";
            
            // Call SelectAll()
            ExceptionHelper.ExpectException(delegate() { concreteMultiSelector.SelectAll(); }, 
                new NotSupportedException(singleSelectionModeSelectAllExceptionMessage, null));

            // Use SelectedItems to add item.
            ExceptionHelper.ExpectException(delegate() { concreteMultiSelector.SelectedItems.Add(singleSelectedItemValue); }, 
                new InvalidOperationException(singleSelectionModeExceptionMessage, null));

            // Clean up the SelectedItem collection.
            ExceptionHelper.ExpectException(delegate() { concreteMultiSelector.SelectedItems.Clear(); }, 
                new InvalidOperationException(singleSelectionModeExceptionMessage, null));
        }
    }
}


