using System;
using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// CalendarSelectionAutomationValidator
    /// </summary>
    public class CalendarSelectionAutomationValidator : AutomationValidator
    {
        public CalendarSelectionAutomationValidator(string args, string targetElementName)
            : base(args, targetElementName)
        {
        }

        public override void Run()
        {
            AutomationElement startAfterItem = null;
            AutomationElement newSelectedItem = null;
            object pattern;

            // Get the startAfterItem and newSelectedItem from SelectionPattern
            SelectionPattern selectionPattern = null;
            if (targetElement != null && targetElement.TryGetCurrentPattern(SelectionPattern.Pattern, out pattern))
            {
                selectionPattern = pattern as SelectionPattern;
                AutomationElement[] selection = selectionPattern.Current.GetSelection();
                if (selection != null)
                {
                    startAfterItem = selection[0];
                    newSelectedItem = selection[2];
                }
            }

            // Select a new item
            InvokePattern invokePattern = null;
            if (newSelectedItem != null && newSelectedItem.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                invokePattern = pattern as InvokePattern;
                invokePattern.Invoke();
            }

            // Validate the newSelectedItem is selected
            SelectionItemPattern selectionItemPattern = null;
            if (newSelectedItem != null && newSelectedItem.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
            {
                selectionItemPattern = pattern as SelectionItemPattern;
                if (!selectionItemPattern.Current.IsSelected)
                {
                    throw new TestValidationException("Fail: the invoked item is not selected.");
                }
            }
        }
    }
}
