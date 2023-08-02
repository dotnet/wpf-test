using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// SelectorSelectionChangedTestBase
    /// </summary>
    public class TreeViewSelectedItemChangedEventTest : TreeViewEventTestBase
    {
        public TreeViewSelectedItemChangedEventTest(Dictionary<string, string> variation, string testInfo)
            : base(variation, testInfo)
        {
        }

        public override TestResult RunTest()
        {
            string eventName = "SelectedItemChanged";
            int fromIndex = Convert.ToInt32(variation["FromIndex"]);
            int toIndex = Convert.ToInt32(variation["ToIndex"]);
            bool shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);

            TreeViewItem fromItem = TreeViewHelper.GetContainer(source, fromIndex);
            fromItem.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            RoutedPropertyChangedEventArgs<object> eventArgs = new RoutedPropertyChangedEventArgs<object>(source.Items[fromIndex], source.Items[toIndex]);

            using (TreeViewValidator validator = TreeViewValidator.GetValidator)
            {
                switch (variation["InputType"])
                {
                    case "Keyboard":
                        validator.Validate<RoutedPropertyChangedEventArgs<object>>((TreeView)source, fromIndex, toIndex, eventName, eventArgs, shouldEventFire, (Key)Enum.Parse(typeof(Key), variation["Key"]));
                        break;
                    case "Mouse":
                        validator.Validate<RoutedPropertyChangedEventArgs<object>>((TreeView)source, fromIndex, toIndex, eventName, eventArgs, shouldEventFire, (MouseButton)Enum.Parse(typeof(MouseButton), variation["MouseButton"]));
                        break;
                }
            }

            return TestResult.Pass;
        }
    }
}


