using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// SelectorSelectionChangedTestBase
    /// </summary>
    public class SelectorSelectionChangedTest : SelectorEventTestBase
    {
        public SelectorSelectionChangedTest(Dictionary<string, string> variation, string testInfo)
            : base(variation, testInfo)
        {
        }

        public override TestResult RunTest()
        {
            string eventName = "SelectionChanged";
            int focusedIndex = Convert.ToInt32(variation["FocusedIndex"]);
            int toIndex = Convert.ToInt32(variation["ToIndex"]);
            bool shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);

            ContentControl removedItem = source.ItemContainerGenerator.ContainerFromIndex(focusedIndex) as ContentControl;
            if (removedItem == null)
            {
                throw new ArgumentNullException("Fail: the removedItem is null.");
            }

            List<object> removedItems = new List<object>();
            if (source.Items[0] is ContentControl)
            {
                removedItems.Add(removedItem);
            }
            else
            {
                removedItems.Add(removedItem.Content);
            }

            // In virtualization scenario, the container may be null if the index is off the viewport enough
            ContentControl addedItem = source.ItemContainerGenerator.ContainerFromIndex(toIndex) as ContentControl;
            if (addedItem == null)
            {
                throw new ArgumentNullException("Fail: the addedItem is null.");
            }

            List<object> addedItems = new List<object>();
            if (source.Items[0] is ContentControl)
            {
                addedItems.Add(addedItem);
            }
            else
            {
                addedItems.Add(addedItem.Content);
            }

            SelectionChangedEventArgs eventArgs = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, removedItems, addedItems);

            using (SelectorValidator validator = SelectorValidator.GetValidator)
            {
                switch (variation["InputType"])
                {
                    case "Keyboard":
                        validator.Validate<RoutedEventArgs>(source, focusedIndex, toIndex, eventName, eventArgs, shouldEventFire, (Key)Enum.Parse(typeof(Key), variation["Key"]));
                        break;
                    case "Mouse":
                        validator.Validate<RoutedEventArgs>(source, focusedIndex, toIndex, eventName, eventArgs, shouldEventFire, (MouseButton)Enum.Parse(typeof(MouseButton), variation["MouseButton"]));
                        break;
                }
            }

            return TestResult.Pass;
        }
    }
}


