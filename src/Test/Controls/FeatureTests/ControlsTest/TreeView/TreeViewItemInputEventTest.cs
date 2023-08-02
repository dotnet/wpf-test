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
    public class TreeViewItemInputEventTest : TreeViewEventTestBase
    {
        public TreeViewItemInputEventTest(Dictionary<string, string> variation, string testInfo)
            : base(variation, testInfo)
        {
        }

        public override TestResult RunTest()
        {
            string eventName = variation["EventName"];
            int fromIndex = Convert.ToInt32(variation["FromIndex"]);
            int toIndex = Convert.ToInt32(variation["ToIndex"]);
            bool shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);

            TreeViewItem fromItem = TreeViewHelper.GetContainer(source, fromIndex);
            fromItem.Focus();

            RoutedEventArgs eventArgs;
            switch (eventName)
            {
                case "Collapsed":
                    eventArgs = new RoutedEventArgs(TreeViewItem.CollapsedEvent);
                    fromItem.IsExpanded = true;
                    break;
                case "Expanded":
                    eventArgs = new RoutedEventArgs(TreeViewItem.ExpandedEvent);
                    break;
                case "Selected":
                    eventArgs = new RoutedEventArgs(TreeViewItem.SelectedEvent);
                    break;
                case "Unselected":
                    eventArgs = new RoutedEventArgs(TreeViewItem.UnselectedEvent);
                    break;
                default:
                    throw new NotSupportedException("Fail: unsupported event name " + eventName);
            }

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            using (TreeViewItemValidator validator = TreeViewItemValidator.GetValidator)
            {
                switch (variation["InputType"])
                {
                    case "Keyboard":
                        validator.Validate<RoutedEventArgs>((TreeView)source, fromIndex, toIndex, eventName, eventArgs, shouldEventFire, (Key)Enum.Parse(typeof(Key), variation["Key"]));
                        break;
                    case "Mouse":
                        validator.Validate<RoutedEventArgs>((TreeView)source, fromIndex, toIndex, eventName, eventArgs, shouldEventFire, (System.Windows.Input.MouseButton)Enum.Parse(typeof(System.Windows.Input.MouseButton), variation["MouseButton"]));
                        break;
                }
            }

            return TestResult.Pass;
        }
    }
}


